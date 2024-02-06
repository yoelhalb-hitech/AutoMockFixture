using DotNetPowerExtensions.Reflection.Models;

namespace AutoMockFixture.Moq4.MockUtils;

internal class SetupServiceFactory : SetupServiceFactoryBase
{
    class AutoPropertyEagerService : ISetupService
    {
        public void Setup() { throw new Exception("Not setting up read-write properties when `MethodSetupTypes.Eager`"); }
    }

    public SetupServiceFactory(Func<MethodSetupTypes> setupTypeFunc) : base(setupTypeFunc) { }

    public override ISetupService GetReadWritePropertySetup(Type mockedType, Type propertyType, IAutoMock mock, PropertyInfo prop, Func<object?> propValueGenerator)
        => setupTypeFunc() == MethodSetupTypes.Eager ? new AutoPropertyEagerService() :
                                new ReadWritePropertyLazySetupService(mockedType, propertyType, mock, prop, propValueGenerator);

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
