using SequelPay.DotNetPowerExtensions.Reflection;

namespace AutoMockFixture.AutoMockUtils;

internal abstract class SetupServiceFactoryBase
{
    protected readonly Func<MethodSetupTypes> setupTypeFunc;

    public SetupServiceFactoryBase(Func<MethodSetupTypes> setupTypeFunc)
    {
        this.setupTypeFunc = setupTypeFunc;
    }

    public ISetupService GetMethodSetup(IAutoMock mock, MethodDetail method, ISpecimenContext context)
    {
        return GetService(setupTypeFunc(), mock, method, context, method.GetTrackingPath());
    }

    public ISetupService GetSingleMethodPropertySetup(IAutoMock mock, PropertyDetail prop, ISpecimenContext context)
    {
        var setupType = setupTypeFunc();
        // For properties we always use same
        if (setupType == MethodSetupTypes.LazyDifferent) setupType = MethodSetupTypes.LazySame;

        var method = prop.GetMethod is not null && (!prop.GetMethod.ReflectionInfo.IsPrivate || prop.IsExplicit)
                                                                                    ? prop.GetMethod : prop.SetMethod;

        return GetService(setupType, mock, method!, context, prop.GetTrackingPath());
    }

    public abstract ISetupService GetReadWritePropertySetup(Type mockedType, Type propertyType, IAutoMock mock, PropertyInfo prop, Func<object?> propValueGenerator);

    protected abstract ISetupService GetService(MethodSetupTypes setupType,
        IAutoMock mock, MethodDetail method, ISpecimenContext context, string trackingPath);
}
