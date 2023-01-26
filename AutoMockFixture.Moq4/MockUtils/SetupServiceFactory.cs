using AutoFixture.Kernel;
using AutoMockFixture.AutoMockUtils;
using Moq;
using System.ComponentModel;
using System.Reflection;

namespace AutoMockFixture.Moq4.MockUtils;

internal class SetupServiceFactory : SetupServiceFactoryBase
{
    public SetupServiceFactory(Func<MethodSetupTypes> setupTypeFunc) : base(setupTypeFunc) { }

    public override ISetupService GetAutoPropertySetup(Type mockedType, Type propertyType, IAutoMock mock, PropertyInfo prop, object? propValue)
        => new AutoPropertySetupService(mockedType, propertyType, mock, prop, propValue);

    protected override ISetupService GetService(MethodSetupTypes setupType,
        IAutoMock mock, MethodInfo method, ISpecimenContext context, string? customTrackingPath, Type? mockType = null)
    {
        switch (setupType)
        {
            case MethodSetupTypes.Eager:
                return new MethodEagerSetupService(mock, method, context, customTrackingPath, mockType);
            case MethodSetupTypes.LazySame:
                return new MethodSetupServiceWithSameResult(mock, method, context, customTrackingPath, mockType);
            case MethodSetupTypes.LazyDifferent:
                return new MethodSetupServiceWithDifferentResult(mock, method, context, customTrackingPath, mockType);
            default:
                throw new InvalidEnumArgumentException();
}
    }
}
