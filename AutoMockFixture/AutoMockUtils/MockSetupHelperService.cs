using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.HelperRequests.AutoMock;
using AutoMockFixture.FixtureUtils.Requests.HelperRequests.NonAutoMock;
using SequelPay.DotNetPowerExtensions.Reflection;
using System.Linq;
using static AutoMockFixture.AutoMockUtils.CannotSetupMethodException;

namespace AutoMockFixture.AutoMockUtils;

internal class MockSetupHelperService
{
    private static readonly DelegateSpecification delegateSpecification = new DelegateSpecification();

    private readonly IAutoMock mock;
    private readonly ISpecimenContext context;
    private readonly SetupServiceFactoryBase setupServiceFactory;
    private readonly IAutoMockHelpers autoMockHelpers;
    private readonly Type mockedType;
    private readonly ITracker? tracker;
    private readonly bool noMockDependencies;

    public MockSetupHelperService(IAutoMock mock, ISpecimenContext context, SetupServiceFactoryBase setupServiceFactory, IAutoMockHelpers autoMockHelpers)
    {
        this.mock = mock;
        this.context = context;
        this.setupServiceFactory = setupServiceFactory;
        this.autoMockHelpers = autoMockHelpers;
        // Don't do mock.GetMocked().GetType() as it has additional properties etc.
        mockedType = mock.GetInnerType();
        tracker = mock.Tracker;
        noMockDependencies = mock.Tracker?.StartTracker.MockDependencies ?? false;
    }

    protected virtual void Setup<T>(MemberDetail<T> member, Func<ISetupService> setupFunc) where T : MemberInfo
    {
        var prop = member as PropertyDetail;
        var method = member as MethodDetail ?? prop!.GetMethod ?? prop.SetMethod!;

        var trackingPath = prop?.GetTrackingPath() ?? method!.GetTrackingPath();

        if (mock.MethodsSetup.ContainsKey(trackingPath)) return; // Already setup, needed for IEnumerable setup when mock and callBase

        if (!autoMockHelpers.CanMock(method.ExplicitInterface ?? method.ReflectionInfo.DeclaringType!))
        {
            HandleCannotSetup(trackingPath, CannotSetupReason.TypeNotPublic);
            return;
        }

        if (mock.CallBase && !method.ReflectionInfo.IsAbstract) // Cannot check by interface as an interface can have a default implementation
        { // It is callBase and has an implementation so let's ignore it
            HandleCannotSetup(trackingPath, CannotSetupReason.CallBaseNoAbstract);
            return;
        }

        if (!method.IsExplicit) // Explicit is always private and non virtual but is anyway configurable
        {
            var configureInfo = CanBeConfigured(method.ReflectionInfo);
            if (!configureInfo.CanConfigure)
            {
                HandleCannotSetup(trackingPath, configureInfo.Reason!.Value);
                return;
            }
        }

        try
        {
            setupFunc().Setup();
            mock.MethodsSetup.Add(trackingPath, member.ReflectionInfo);
        }
        catch (Exception ex)
        {
            mock.MethodsNotSetup.Add(trackingPath, new CannotSetupMethodException(CannotSetupReason.Exception, ex));
        }
    }

    internal void SetupMethod(MethodDetail method)
        => Setup(method, () => setupServiceFactory.GetMethodSetup(mock, method, context));

    internal void SetupSingleMethodProperty(PropertyDetail prop)
        => Setup(prop, () => setupServiceFactory.GetSingleMethodPropertySetup(mock, prop, context));

    internal void SetupReadWriteProperty(PropertyDetail prop)
    {
        Setup(prop, () =>
        {
            var request = noMockDependencies
                                    ? new PropertyRequest(mockedType, prop.ReflectionInfo, tracker)
                                    : new AutoMockPropertyRequest(mockedType, prop.ReflectionInfo, tracker);
            var propValueGenerator = () => context.Resolve(request);
            return setupServiceFactory.GetReadWritePropertySetup(mockedType, prop.ReflectionInfo.PropertyType, mock, prop.ReflectionInfo, propValueGenerator);
        });
    }

    private void HandleCannotSetup(string trackingPath, CannotSetupReason reason)
        => mock.MethodsNotSetup.Add(trackingPath, new CannotSetupMethodException(reason));

    protected virtual (bool CanConfigure, CannotSetupReason? Reason) CanBeConfigured(MethodInfo method)
    {
        if (!mockedType.IsInterface && !method.IsOverridable()) return (false, CannotSetupReason.NonVirtual);

        if (method.IsPrivate) return (false, CannotSetupReason.Private);

        if (!method.IsPublicOrInternal()) return (false, CannotSetupReason.Protected); //TODO... maybe we should set it up in case someone calls callBase on a method?

        return (true, null);
    }
}
