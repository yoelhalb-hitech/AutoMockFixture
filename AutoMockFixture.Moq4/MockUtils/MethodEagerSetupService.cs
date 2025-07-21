using SequelPay.DotNetPowerExtensions.Reflection;

namespace AutoMockFixture.Moq4.MockUtils;

internal class MethodEagerSetupService : MethodSetupServiceBase
{
    public MethodEagerSetupService(IAutoMock mock, MethodDetail method, ISpecimenContext context, string trackingPath)
            : base(mock, method, context, trackingPath)
    {
    }

    protected override object? HandleInvocationFunc(IInvocation invocation)
        => HandleInvocationFuncWithSameResult(invocation);

    protected override void HandleNonGenericFunction(Expression methodInvocationLambda, Type returnType)
    {
        var result = GenerateResult(this.method.ReflectionInfo);
        SetupHelpers.SetupMethodWithResult(mockedType, returnType, mock, methodInvocationLambda, result);
    }
}
