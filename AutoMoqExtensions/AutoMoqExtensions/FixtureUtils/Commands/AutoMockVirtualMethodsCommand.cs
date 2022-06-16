using AutoFixture.AutoMoq;
using AutoFixture.Kernel;
using AutoMoqExtensions.AutoMockUtils;
using AutoMoqExtensions.Extensions;
using AutoMoqExtensions.FixtureUtils.Requests;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Commands
{
    // TODO... we need to handle internal methods
    internal class AutoMockVirtualMethodsCommand : ISpecimenCommand
    {
        private static readonly DelegateSpecification DelegateSpecification = new DelegateSpecification();

        public void Execute(object specimen, ISpecimenContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            try
            {
                var mock = AutoMockHelpers.GetFromObj(specimen);
                if (mock is null) return;

                var mockedType = mock.GetInnerType();
                var methods = GetConfigurableMethods(mockedType);

                var tracker = mock.Tracker;

                foreach (var method in methods)
                {
                    var returnType = method.ReturnType;
                    var methodInvocationLambda = new ExpressionGenerator(mockedType, method, context, tracker)
                                                                .MakeMethodInvocationLambda();

                    if (methodInvocationLambda == null) continue;

                    if(method.DeclaringType == typeof(object)) // Overriding .Equals etc. can cause problems
                    {
                        try
                        {
                            SetupHelpers.SetupCallbaseMethod(mockedType, returnType, mock, methodInvocationLambda);
                        }
                        catch { }
                    }
                    else if (method.IsVoid())
                    {
                        try
                        {
                            SetupHelpers.SetupVoidMethod(mockedType, mock, methodInvocationLambda);
                        }
                        catch { }
                    }
                    else if(!method.ReturnType.ContainsGenericParameters)
                    {
                        try
                        {
                            var returnValue = context.Resolve(new AutoMockReturnRequest(mockedType, method, method.ReturnType, tracker));                                
                            SetupHelpers.SetupMethodWithResult(mockedType, returnType, mock, methodInvocationLambda, returnValue);
                        }
                        catch { }
                    }
                    else
                    {
                        try
                        {
                            Dictionary<string, object?> resultDict = new Dictionary<string, object?>();                            
                            Console.WriteLine("\t\t\tBefore return: " + method.ReturnType.Name);
                            var invocationFunc = new InvocationFunc((Func<IInvocation, object?>)(invocation =>
                                {
                                    var tag = invocation.Method.GetGenericArguments().GetTagForTypes();
                                    if (resultDict.ContainsKey(tag)) return resultDict[tag];

                                    lock (resultDict)
                                    {
                                        if (resultDict.ContainsKey(tag)) return resultDict[tag];
                                        var result = GenerateResult(invocation, method);
                                        resultDict[tag] = result;

                                        Console.WriteLine("Resolved type: " + (result?.GetType().FullName ?? "null"));
                                        return result;
                                    }
                                }));
                            
                            var genericArgs = returnType.GetGenericArguments().Select(a => MatcherGenerator.GetGenericMatcher(a)).ToArray();
                            var newReturnType = returnType.IsGenericParameter ? MatcherGenerator.GetGenericMatcher(returnType) : returnType.GetGenericTypeDefinition().MakeGenericType(genericArgs);
                            
                            SetupHelpers.SetupMethodWithGenericResult(mockedType, newReturnType, mock, methodInvocationLambda, invocationFunc);
                        }
                        catch { }
                    }

                }

                object? GenerateResult(IInvocation invocation, MethodInfo method)
                {
                    var typeGenerics = method.GetGenericArguments().ToList();
                    var returnGenerics = method.ReturnType.GetGenericArguments();
                    var returnGenericsToTypeGenerics = returnGenerics.Select(rg => typeGenerics.First(tg => rg == tg)).ToList();
                    var returnTypes = returnGenericsToTypeGenerics
                        .Select(g => typeGenerics.IndexOf(g))
                        .Select(i => invocation.Method.GetGenericArguments()[i]).ToArray();

                    var actualReturnType = method.ReturnType.IsGenericParameter
                            ? invocation.Method.GetGenericArguments().First()
                            : method.ReturnType.GetGenericTypeDefinition().MakeGenericType(returnTypes);
                    Console.WriteLine("\t\tResolving return: " + actualReturnType.FullName);

                    var request = new AutoMockReturnRequest(mockedType, method, actualReturnType, tracker);
                    Console.WriteLine("\t\tResolving return for containing path: " + request.Path);

                    var result = context.Resolve(request);
                    return result;
                }
            }
            catch { }
        }

        private static IEnumerable<MethodInfo> GetConfigurableMethods(Type type)
        {
            // If "type" is a delegate, return "Invoke" method only and skip the rest of the methods.
            var methods = DelegateSpecification.IsSatisfiedBy(type)
                ? new[] { type.GetTypeInfo().GetMethod("Invoke") }
                : type.GetAllMethods().Where(m => m.IsPublicOrInternal());

            // Skip properties that have both getters and setters to not interfere
            // with other post-processors in the chain that initialize them later.
            methods = SkipWritablePropertyGetters(type, methods);

            return methods.Where(m => CanBeConfigured(type, m));
        }

        private static IEnumerable<MethodInfo> SkipWritablePropertyGetters(Type type, IEnumerable<MethodInfo> methods)
        {
            var getterMethods = type.GetAllProperties()
                .Where(p => p.GetMethod != null && p.SetMethod != null)
                .Select(p => p.GetMethod);

            return methods.Except(getterMethods);
        }


        private static bool CanBeConfigured(Type type, MethodInfo method)
            => (type.IsInterface || method.IsOverridable()) &&
                   //!method.IsGenericMethod && TODO...  we have to setup args and out for generic etc.
                   !method.HasRefParameters() &&           
                    (!method.IsVoid() || method.HasOutParameters()); // No point in setting up a void method...

    }
}
