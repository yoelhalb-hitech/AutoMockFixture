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
                    var methodInvocationLambda = MakeMethodInvocationLambda(mockedType, method, context, tracker);

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
                            var returnValue = context.Resolve(new AutoMockReturnRequest(mockedType, method, method.ReturnType, tracker));                                
                            SetupHelpers.SetupMethodWithResult(mockedType, returnType, mock, methodInvocationLambda, returnValue);
                        }
                        catch { }
                    }
                    else
                    {
                        try
                        {
                        }
                        catch { }
                    }

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
                   !method.IsGenericMethod &&
                   !method.HasRefParameters() &&                    
                    (!method.IsVoid() || method.HasOutParameters()); // No point in setting up a void method...

        private static Expression? MakeMethodInvocationLambda(Type mockedType, MethodInfo method,
                                                      ISpecimenContext context, ITracker? tracker)
        {
            var lambdaParam = Expression.Parameter(mockedType, "x");

            var methodCallParams = method.GetParameters()
                            .Select(param => MakeParameterExpression(mockedType, method,  param, context, tracker))
                            .ToList();

            if (methodCallParams.Any(exp => exp == null))
                return null;

            Expression methodCall;
            if (DelegateSpecification.IsSatisfiedBy(mockedType))
            {
                // e.g. "x(It.IsAny<string>(), out parameter)"
                methodCall = Expression.Invoke(lambdaParam, methodCallParams);
            }
            else
            {
                // e.g. "x.Method(It.IsAny<string>(), out parameter)"
                methodCall = Expression.Call(lambdaParam, method, methodCallParams);
            }

            // e.g. "x => x.Method(It.IsAny<string>(), out parameter)"
            // or "x => x(It.IsAny<string>(), out parameter)"
            return Expression.Lambda(methodCall, lambdaParam);
        }

        // TODO... add generic support using IsAnyType?
        private static Expression? MakeParameterExpression(Type mockedType, MethodInfo method, ParameterInfo parameter, ISpecimenContext context, ITracker? tracker)
        {
            // check if parameter is an "out" parameter
            if (parameter.IsOut)
            {
                // gets the type corresponding to this "byref" type
                // e.g., the underlying type of "System.String&" is "System.String"
                var underlyingType = parameter.ParameterType.GetElementType();

                // resolve the "out" param from the context
                var request = new AutoMockOutParameterRequest(mockedType, method, parameter, underlyingType, tracker);
                object variable = context.Resolve(request);
                if (variable is OmitSpecimen)
                    return null;

                return Expression.Constant(variable, underlyingType);
            }
            else
            {
                // for any non-out parameter, invoke "It.IsAny<T>()"
                var isAnyMethod = typeof(It).GetMethod(nameof(It.IsAny)).MakeGenericMethod(parameter.ParameterType);

                return Expression.Call(isAnyMethod);
            }
        }

    }
}
