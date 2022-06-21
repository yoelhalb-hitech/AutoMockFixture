using AutoFixture.Kernel;
using AutoMoqExtensions.FixtureUtils;
using AutoMoqExtensions.FixtureUtils.Requests;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoMoqExtensions.MockUtils
{
    internal class ExpressionGenerator
    {
        private static readonly DelegateSpecification delegateSpecification = new DelegateSpecification();
        private readonly Type mockedType;
        private readonly MethodInfo method;
        private readonly ISpecimenContext context;
        private readonly ITracker? tracker;

        public ExpressionGenerator(Type mockedType, MethodInfo method,
                                                      ISpecimenContext context, ITracker? tracker)
        {
            this.mockedType = mockedType;
            this.method = method;
            this.context = context;
            this.tracker = tracker;
        }

        public Expression? MakeMethodInvocationLambda()
        {
            var lambdaParam = Expression.Parameter(mockedType, "x");

            var methodCallParams = method.GetParameters()
                            .Select(param => MakeParameterExpression(param))
                            .ToList();

            if (methodCallParams.Any(exp => exp == null))
                return null;

            Expression methodCall;
            if (delegateSpecification.IsSatisfiedBy(mockedType))
            {
                // e.g. "x(It.IsAny<string>(), out parameter)"
                methodCall = Expression.Invoke(lambdaParam, methodCallParams);
            }
            else if (method.ContainsGenericParameters)
            {
                var genericMethod = method.MakeGenericMethod(
                    method.GetGenericArguments()
                            .Select(a => MatcherGenerator.GetGenericMatcher(a)).ToArray());
                // e.g. "x.Method(It.IsAny<string>(), out parameter)"
                methodCall = Expression.Call(lambdaParam, genericMethod, methodCallParams.ToArray());
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

        // TODO... add generic support for out
        // TODO... out doesn't always work
        // TODO... add support for ref
        private Expression? MakeParameterExpression(ParameterInfo parameter)
        {
            // check if parameter is an "out" parameter
            if (parameter.IsOut)
            {
                // gets the type corresponding to this "byref" type
                // e.g., the underlying type of "System.String&" is "System.String"
                var underlyingType = parameter.ParameterType.GetElementType();

                Console.WriteLine("Out underlying: " + underlyingType.ToString());
                // resolve the "out" param from the context
                var request = new AutoMockOutParameterRequest(mockedType, method, parameter, underlyingType, tracker);
                object variable = context.Resolve(request);
                Console.WriteLine("Out underlying result: " + variable.ToString());
                if (variable is OmitSpecimen)
                    return null;

                return Expression.Constant(variable, underlyingType);
            }
            else // Appears to work perfectly for ref
            {
                // for any non-out parameter, invoke "It.IsAny<T>()"
                var type = MatcherGenerator.GetGenericMatcher(parameter);
                var isAnyMethod = typeof(It).GetMethod(nameof(It.IsAny)).MakeGenericMethod(type);

                return Expression.Call(isAnyMethod);
            }
        }
    }
}
