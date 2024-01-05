using DotNetPowerExtensions.Reflection.Models;

namespace AutoMockFixture.Moq4.MockUtils;

internal class SetupServiceFactory : SetupServiceFactoryBase
{
    public SetupServiceFactory(Func<MethodSetupTypes> setupTypeFunc) : base(setupTypeFunc) { }

    public override ISetupService GetAutoPropertySetup(Type mockedType, Type propertyType, IAutoMock mock, PropertyInfo prop, object? propValue)
        => new AutoPropertySetupService(mockedType, propertyType, mock, prop, propValue);

    protected override ISetupService GetService(MethodSetupTypes setupType,
        IAutoMock mock, MethodDetail method, ISpecimenContext context, string trackingPath)
    {
        switch (setupType)
        {
            case MethodSetupTypes.Eager:
                return new MethodEagerSetupService(mock, method, context, trackingPath);
            case MethodSetupTypes.LazySame:
                return new MethodSetupServiceWithSameResult(mock, method, context, trackingPath);
            case MethodSetupTypes.LazyDifferent:
                return new MethodSetupServiceWithDifferentResult(mock, method, context, trackingPath);
            default:
                throw new InvalidEnumArgumentException();
}
    }
}
