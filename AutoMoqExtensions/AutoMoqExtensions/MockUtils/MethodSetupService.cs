using AutoFixture.Kernel;
using AutoMoqExtensions.Extensions;
using AutoMoqExtensions.FixtureUtils.Requests;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoMoqExtensions.MockUtils
{
    internal class MethodSetupService
    {
        private readonly IAutoMock mock;
        private readonly Type mockedType;
        private readonly MethodInfo method;
        private readonly ISpecimenContext context;
        private readonly ITracker? tracker;

        public MethodSetupService(IAutoMock mock, Type mockedType, MethodInfo method, ISpecimenContext context)
        {
            this.mock = mock;
            this.mockedType = mockedType;
            this.method = method;
            this.context = context;
            this.tracker = mock.Tracker;
        }

        public void Setup()
        {
            var returnType = method.ReturnType;
            var methodInvocationLambda = new ExpressionGenerator(mockedType, method, context, tracker)
                                                        .MakeMethodInvocationLambda();

            if (methodInvocationLambda is null) return;

            // TODO... this is all based on the assumption that the method can return the same object everytime
            if (method.DeclaringType == typeof(object)) // Overriding .Equals etc. can cause problems
            {
                SetupHelpers.SetupCallbaseMethod(mockedType, returnType, mock, methodInvocationLambda);              
            }
            else if (method.IsVoid())
            {
                SetupHelpers.SetupVoidMethod(mockedType, mock, methodInvocationLambda);                
            }
            else if (!method.ReturnType.ContainsGenericParameters)
            {              
                Console.WriteLine("\t\t\tBefore return: " + method.ReturnType.Name);
                var returnValue = context.Resolve(new AutoMockReturnRequest(mockedType, method, method.ReturnType, tracker));
                
                Console.WriteLine("\t\t\tResolved return: " + returnValue.GetType().Name);                
                SetupHelpers.SetupMethodWithResult(mockedType, returnType, mock, methodInvocationLambda, returnValue);            
            }
            else
            { 
                Console.WriteLine("\t\t\tBefore return: " + method.ReturnType.Name);
                var invocationFunc = new InvocationFunc(HandleInvocationFunc);                

                var genericArgs = returnType.GetGenericArguments().Select(a => MatcherGenerator.GetGenericMatcher(a)).ToArray();
                var newReturnType = returnType.IsGenericParameter ? MatcherGenerator.GetGenericMatcher(returnType) : returnType.GetGenericTypeDefinition().MakeGenericType(genericArgs);

                SetupHelpers.SetupMethodWithGenericResult(mockedType, newReturnType, mock, methodInvocationLambda, invocationFunc);
            }
        }

        private Dictionary<string, object?> resultDict = new Dictionary<string, object?>();
        private object lockObject = new object(); // Not static as it is only local to the object

        private object? HandleInvocationFunc(IInvocation invocation)
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
        }

        private object? GenerateResult(IInvocation invocation, MethodInfo method)
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
}
