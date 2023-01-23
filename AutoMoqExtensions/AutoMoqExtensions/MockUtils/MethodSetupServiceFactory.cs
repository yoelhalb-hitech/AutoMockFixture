using System.ComponentModel;
using System.Reflection;

namespace AutoMoqExtensions.MockUtils;

internal class MethodSetupServiceFactory
{
    private readonly Func<MethodSetupTypes> setupTypeFunc;

    public MethodSetupServiceFactory(Func<MethodSetupTypes> setupTypeFunc)
    {
        this.setupTypeFunc = setupTypeFunc;
    }

    public MethodSetupServiceBase GetMethodSetup(IAutoMock mock, MethodInfo method,
        ISpecimenContext context, string? customTrackingPath = null, Type? mockType = null)
    {
        return GetService(setupTypeFunc(), mock, method, context, customTrackingPath, mockType);           
    }

    public MethodSetupServiceBase GetPropertySetup(IAutoMock mock, MethodInfo method,
                                                    ISpecimenContext context, string? customTrackingPath = null, Type? mockType = null)
    {
        var setupType = setupTypeFunc();
        // For properties we always use same
        if (setupType == MethodSetupTypes.LazyDifferent) setupType = MethodSetupTypes.LazySame;

        return GetService(setupType, mock, method, context, customTrackingPath, mockType);
    }

    private MethodSetupServiceBase GetService(MethodSetupTypes setupType,
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
