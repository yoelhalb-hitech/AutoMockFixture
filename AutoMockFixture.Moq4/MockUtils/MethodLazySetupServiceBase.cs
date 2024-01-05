using DotNetPowerExtensions.Reflection.Models;

namespace AutoMockFixture.Moq4.MockUtils;

internal abstract class MethodLazySetupServiceBase : MethodSetupServiceBase
{
    protected MethodLazySetupServiceBase(IAutoMock mock, MethodDetail method, ISpecimenContext context, string trackingPath)
            : base(mock, method, context, trackingPath)
    {
    }

    protected override void HandleNonGenericFunction(Expression methodInvocationLambda, Type returnType)
    {
        SetupWithInvocationFunc(methodInvocationLambda, returnType);
    }
}
