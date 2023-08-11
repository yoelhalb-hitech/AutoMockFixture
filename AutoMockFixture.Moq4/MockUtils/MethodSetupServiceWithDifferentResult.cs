
namespace AutoMockFixture.Moq4.MockUtils;

internal class MethodSetupServiceWithDifferentResult : MethodLazySetupServiceBase
{
    public MethodSetupServiceWithDifferentResult(IAutoMock mock, MethodInfo method, ISpecimenContext context,
        string? customTrackingPath = null, Type? mockType = null, MethodInfo? underlying = null)
            : base(mock, method, context, customTrackingPath, mockType, underlying)
    {
    }

    protected override object? HandleInvocationFunc(IInvocation invocation)
    {
        var result = GenerateResult(invocation.Method);

        Logger.LogInfo("Resolved type: " + (result?.GetType().FullName ?? "null"));
        return result;
    }
}
