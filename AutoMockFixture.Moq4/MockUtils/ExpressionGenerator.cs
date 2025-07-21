using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.HelperRequests.AutoMock;
using AutoMockFixture.FixtureUtils.Requests.HelperRequests.NonAutoMock;
using SequelPay.DotNetPowerExtensions.Reflection;

namespace AutoMockFixture.Moq4.MockUtils;

internal class ExpressionGenerator
{
    private static readonly DelegateSpecification delegateSpecification = new DelegateSpecification();
    private readonly Type mockedType;
    private readonly MethodDetail method;
    private readonly ISpecimenContext context;
    private readonly ITracker? tracker;
    private readonly bool noMockDependencies;

    public ExpressionGenerator(Type mockedType, MethodDetail method,
                                                  ISpecimenContext context, ITracker? tracker)
    {
        this.mockedType = mockedType;
        this.method = method;
        this.context = context;
        this.tracker = tracker;
        noMockDependencies = tracker?.StartTracker.MockDependencies == false;
    }

    public Expression? MakeMethodInvocationLambda()
    {
        var lambdaParam = Expression.Parameter(mockedType, "x");

        var methodCallParams = method.ReflectionInfo.GetParameters()
                        .Select(param => MakeParameterExpression(param))
                        .ToList();

        if (methodCallParams.Any(exp => exp is null))
            return null;

        var parameters = methodCallParams.OfType<Expression>().ToList();

        var methodToUse = method.ExplicitInterfaceReflectionInfo ?? method.ReflectionInfo;

        Expression methodCall;
        if (delegateSpecification.IsSatisfiedBy(mockedType))
        {
            // e.g. "x(It.IsAny<string>(), out parameter)"
            methodCall = Expression.Invoke(lambdaParam, parameters);
        }
        else if (methodToUse.ContainsGenericParameters)
        {
            var genericMethod = methodToUse.MakeGenericMethod(
                methodToUse.GetGenericArguments()
                        .Select(a => MatcherGenerator.GetGenericMatcher(a)).ToArray());
            // e.g. "x.Method(It.IsAny<string>(), out parameter)"
            methodCall = Expression.Call(lambdaParam, genericMethod, parameters.ToArray());
        }
        else
        {
            // e.g. "x.Method(It.IsAny<string>(), out parameter)"
            methodCall = Expression.Call(lambdaParam, methodToUse, parameters);
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
            var underlyingType = parameter.ParameterType.GetElementType()!;

            Logger.LogInfo("Out underlying: " + underlyingType!.ToString());
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
            var isAnyMethod = typeof(It).GetMethod(nameof(It.IsAny))!.MakeGenericMethod(type)!;

            return Expression.Call(isAnyMethod);
        }
    }
}
