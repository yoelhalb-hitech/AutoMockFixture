
using AutoMoqExtensions.FixtureUtils.Requests;
using AutoMoqExtensions.FixtureUtils.Requests.HelperRequests.AutoMock;
using AutoMoqExtensions.FixtureUtils.Requests.HelperRequests.NonAutoMock;
using AutoMoqExtensions.FixtureUtils.Requests.MainRequests;
using Moq;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoMoqExtensions.MockUtils;

internal class ExpressionGenerator
{
    private static readonly DelegateSpecification delegateSpecification = new DelegateSpecification();
    private readonly Type mockedType;
    private readonly MethodInfo method;
    private readonly ISpecimenContext context;
    private readonly ITracker? tracker;
    private readonly bool noMockDependencies;

    public ExpressionGenerator(Type mockedType, MethodInfo method,
                                                  ISpecimenContext context, ITracker? tracker)
    {
        this.mockedType = mockedType;
        this.method = method;
        this.context = context;
        this.tracker = tracker;
        this.noMockDependencies = tracker?.StartTracker.MockDependencies == false;
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

            Logger.LogInfo("Out underlying: " + underlyingType.ToString());
            // resolve the "out" param from the context
            var request = noMockDependencies
                            ? new OutParameterRequest(mockedType, method, parameter, underlyingType, tracker)
                            : new AutoMockOutParameterRequest(mockedType, method, parameter, underlyingType, tracker);
            
            // TODO... change this to to delay generate it when called first time
            object variable = context.Resolve(request);
            Logger.LogInfo("Out underlying result: " + variable.ToString());
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
