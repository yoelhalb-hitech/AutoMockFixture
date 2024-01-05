using DotNetPowerExtensions.Reflection.Models;

namespace AutoMockFixture.Moq4.MockUtils;

internal class MethodSetupServiceWithSameResult : MethodLazySetupServiceBase
{
    public MethodSetupServiceWithSameResult(IAutoMock mock, MethodDetail method, ISpecimenContext context, string trackingPath)
            : base(mock, method, context, trackingPath)
    {
    }

    protected override object? HandleInvocationFunc(IInvocation invocation)
        => HandleInvocationFuncWithSameResult(invocation);
}
