using AutoFixture.Kernel;
using Moq;
using System.Reflection;

namespace AutoMockFixture.Moq4.MockUtils;

internal class MethodSetupServiceWithSameResult : MethodLazySetupServiceBase
{
    public MethodSetupServiceWithSameResult(IAutoMock mock, MethodInfo method, ISpecimenContext context,
        string? customTrackingPath = null, Type? mockType = null)
            : base(mock, method, context, customTrackingPath, mockType)
    {
    }

    protected override object? HandleInvocationFunc(IInvocation invocation)
        => HandleInvocationFuncWithSameResult(invocation);
}
