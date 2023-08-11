
namespace AutoMockFixture.Moq4.MockUtils;

internal class SetupServiceFactory : SetupServiceFactoryBase
{
    public SetupServiceFactory(Func<MethodSetupTypes> setupTypeFunc) : base(setupTypeFunc) { }

    public override ISetupService GetAutoPropertySetup(Type mockedType, Type propertyType, IAutoMock mock, PropertyInfo prop, object? propValue)
        => new AutoPropertySetupService(mockedType, propertyType, mock, prop, propValue);

    protected override ISetupService GetService(MethodSetupTypes setupType,
        IAutoMock mock, MethodInfo method, ISpecimenContext context, string? customTrackingPath, Type? mockType = null, MethodInfo? underlying = null)
    {
        switch (setupType)
        {
            case MethodSetupTypes.Eager:
                return new MethodEagerSetupService(mock, method, context, customTrackingPath, mockType, underlying);
            case MethodSetupTypes.LazySame:
                return new MethodSetupServiceWithSameResult(mock, method, context, customTrackingPath, mockType, underlying);
            case MethodSetupTypes.LazyDifferent:
                return new MethodSetupServiceWithDifferentResult(mock, method, context, customTrackingPath, mockType, underlying);
            default:
                throw new InvalidEnumArgumentException();
}
    }
}
