
using Moq;
using System.Reflection;

namespace AutoMoqExtensions.MockUtils;

internal class MethodSetupServiceWithSameResult : MethodSetupServiceBase
{
    public MethodSetupServiceWithSameResult(IAutoMock mock, Type mockedType, MethodInfo method, ISpecimenContext context) : base(mock, mockedType, method, context)
    {
    }

    private Dictionary<MethodInfo, object?> resultDict = new Dictionary<MethodInfo, object?>();
    private object lockObject = new object(); // Not static as it is only local to the object
    
    
    protected override object? HandleInvocationFunc(IInvocation invocation)
    {
        if (resultDict.ContainsKey(invocation.Method)) return resultDict[invocation.Method];

        lock (resultDict)
        {
            if (resultDict.ContainsKey(invocation.Method)) return resultDict[invocation.Method];
            var result = GenerateResult(invocation.Method);
            resultDict[invocation.Method] = result;

            Logger.LogInfo("Resolved type: " + (result?.GetType().FullName ?? "null"));
            return result;
        }
    }
}
