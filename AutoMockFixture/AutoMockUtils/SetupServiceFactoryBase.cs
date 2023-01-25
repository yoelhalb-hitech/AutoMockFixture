using Moq;
using System.ComponentModel;
using System.Reflection;

namespace AutoMockFixture.AutoMockUtils;

internal abstract class SetupServiceFactoryBase
{
    private readonly Func<MethodSetupTypes> setupTypeFunc;

    public SetupServiceFactoryBase(Func<MethodSetupTypes> setupTypeFunc)
    {
        this.setupTypeFunc = setupTypeFunc;
    }

    public ISetupService GetMethodSetup(IAutoMock mock, MethodInfo method,
        ISpecimenContext context, string? customTrackingPath = null, Type? mockType = null)
    {
        return GetService(setupTypeFunc(), mock, method, context, customTrackingPath, mockType);
    }

    public ISetupService GetPropertySetup(IAutoMock mock, MethodInfo method,
                                                    ISpecimenContext context, string? customTrackingPath = null, Type? mockType = null)
    {
        var setupType = setupTypeFunc();
        // For properties we always use same
        if (setupType == MethodSetupTypes.LazyDifferent) setupType = MethodSetupTypes.LazySame;

        return GetService(setupType, mock, method, context, customTrackingPath, mockType);
    }

    public abstract ISetupService GetAutoPropertySetup(Type mockedType, Type propertyType, IAutoMock mock, PropertyInfo prop, object? propValue);

    protected abstract ISetupService GetService(MethodSetupTypes setupType,
        IAutoMock mock, MethodInfo method, ISpecimenContext context, string? customTrackingPath, Type? mockType = null);    
}
