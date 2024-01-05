using DotNetPowerExtensions.Reflection.Models;

namespace AutoMockFixture.Moq4.MockUtils;

internal class MethodSetupServiceWithDifferentResult : MethodLazySetupServiceBase
{
    public MethodSetupServiceWithDifferentResult(IAutoMock mock, MethodDetail method, ISpecimenContext context,
                                                                                                    string trackingPath)
            : base(mock, method, context, trackingPath)
    {
    }

    protected override object? HandleInvocationFunc(IInvocation invocation)
    {
        var result = GenerateResult(invocation.Method);

        Logger.LogInfo("Resolved type: " + (result?.GetType().FullName ?? "null"));
        return result;
    }
}
