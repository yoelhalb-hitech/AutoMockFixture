using System.Linq.Expressions;
using System.Reflection;

namespace AutoMoqExtensions.MockUtils;

internal abstract class MethodLazySetupServiceBase : MethodSetupServiceBase
{
    protected MethodLazySetupServiceBase(IAutoMock mock, MethodInfo method,ISpecimenContext context,
        string? customTrackingPath = null, Type? mockType = null) 
            : base(mock, method, context, customTrackingPath, mockType)
    {
    }

    protected override void HandleNonGenericFunction(Expression methodInvocationLambda, Type returnType)
    {
        SetupWithInvocationFunc(methodInvocationLambda, returnType);
    }
}
