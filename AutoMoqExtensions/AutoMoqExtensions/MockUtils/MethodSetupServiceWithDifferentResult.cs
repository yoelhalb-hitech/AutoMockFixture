
using Moq;
using System.Reflection;

namespace AutoMoqExtensions.MockUtils;

internal class MethodSetupServiceWithDifferentResult : MethodSetupServiceBase
{
    public MethodSetupServiceWithDifferentResult(IAutoMock mock, Type mockedType, MethodInfo method, ISpecimenContext context) : base(mock, mockedType, method, context)
    {
    }

    protected override object? HandleInvocationFunc(IInvocation invocation)
    {
        var result = GenerateResult(invocation.Method);

        Logger.LogInfo("Resolved type: " + (result?.GetType().FullName ?? "null"));
        return result;        
    }
}
