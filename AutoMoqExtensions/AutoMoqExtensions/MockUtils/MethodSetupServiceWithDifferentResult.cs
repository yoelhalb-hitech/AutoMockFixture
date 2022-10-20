
using Moq;
using System.Reflection;

namespace AutoMoqExtensions.MockUtils;

internal class MethodSetupServiceWithDifferentResult : MethodSetupServiceBase
{
    public MethodSetupServiceWithDifferentResult(IAutoMock mock, MethodInfo method, ISpecimenContext context,
        string? customTrackingPath = null) 
            : base(mock, method, context, customTrackingPath)
    {
    }

    protected override object? HandleInvocationFunc(IInvocation invocation)
    {
        var result = GenerateResult(invocation.Method);

        Logger.LogInfo("Resolved type: " + (result?.GetType().FullName ?? "null"));
        return result;        
    }
}
