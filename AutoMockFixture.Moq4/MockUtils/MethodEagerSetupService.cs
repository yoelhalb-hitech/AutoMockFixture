
namespace AutoMockFixture.Moq4.MockUtils;

internal class MethodEagerSetupService : MethodSetupServiceBase
{
    public MethodEagerSetupService(IAutoMock mock, MethodInfo method, ISpecimenContext context,
        string? customTrackingPath = null, Type? mockType = null, MethodInfo? underlying = null)
            : base(mock, method, context, customTrackingPath, mockType, underlying)
    {
    }

    protected override object? HandleInvocationFunc(IInvocation invocation)
        => HandleInvocationFuncWithSameResult(invocation);

    protected override void HandleNonGenericFunction(Expression methodInvocationLambda, Type returnType)
    {
        var result = GenerateResult(this.method);
        SetupHelpers.SetupMethodWithResult(mockedType, returnType, mock, methodInvocationLambda, result);
    }
}
