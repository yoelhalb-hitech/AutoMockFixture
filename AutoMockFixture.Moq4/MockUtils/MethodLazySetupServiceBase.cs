
namespace AutoMockFixture.Moq4.MockUtils;

internal abstract class MethodLazySetupServiceBase : MethodSetupServiceBase
{
    protected MethodLazySetupServiceBase(IAutoMock mock, MethodInfo method,ISpecimenContext context,
        string? customTrackingPath = null, Type? mockType = null, MethodInfo? underlying = null)
            : base(mock, method, context, customTrackingPath, mockType, underlying)
    {
    }

    protected override void HandleNonGenericFunction(Expression methodInvocationLambda, Type returnType)
    {
        SetupWithInvocationFunc(methodInvocationLambda, returnType);
    }
}
