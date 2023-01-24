using Moq;
using System.Reflection;

namespace AutoMockFixture.MockUtils;

internal class MethodSetupServiceWithDifferentResult : MethodLazySetupServiceBase
{
    public MethodSetupServiceWithDifferentResult(IAutoMock mock, MethodInfo method, ISpecimenContext context,
        string? customTrackingPath = null, Type? mockType = null) 
            : base(mock, method, context, customTrackingPath, mockType)
    {
    }

    protected override object? HandleInvocationFunc(IInvocation invocation)
    {
        var result = GenerateResult(invocation.Method);

        Logger.LogInfo("Resolved type: " + (result?.GetType().FullName ?? "null"));
        return result;        
    }
}
