
namespace AutoMockFixture.Moq4.MockUtils;

internal class MethodSetupServiceWithSameResult : MethodLazySetupServiceBase
{
    public MethodSetupServiceWithSameResult(IAutoMock mock, MethodInfo method, ISpecimenContext context,
        string? customTrackingPath = null, Type? mockType = null, MethodInfo? underlying = null)
            : base(mock, method, context, customTrackingPath, mockType, underlying)
    {
    }

    protected override object? HandleInvocationFunc(IInvocation invocation)
        => HandleInvocationFuncWithSameResult(invocation);
}
