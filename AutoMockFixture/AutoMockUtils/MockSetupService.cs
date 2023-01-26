using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.HelperRequests.AutoMock;
using AutoMockFixture.FixtureUtils.Requests.HelperRequests.NonAutoMock;
using System.Reflection;
using static AutoMockFixture.AutoMockUtils.CannotSetupMethodException;

namespace AutoMockFixture.AutoMockUtils;

internal class MockSetupService
{
    private static readonly DelegateSpecification delegateSpecification = new DelegateSpecification();

    private readonly IAutoMock mock;
    private readonly ISpecimenContext context;
    private readonly SetupServiceFactoryBase setupServiceFactory;
    private readonly IAutoMockHelpers autoMockHelpers;
    private readonly Type mockedType;
    private readonly ITracker? tracker;
    private readonly bool noMockDependencies;

    public MockSetupService(IAutoMock mock, ISpecimenContext context, SetupServiceFactoryBase setupServiceFactory, IAutoMockHelpers autoMockHelpers)
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

    public void Setup()
    {
        // If it's private it won't be in the proxy, CAUTION: I am doing it here and not in the `GetMethod()` in case we want to change the logic it should be in one place
        var methods = GetMethods().Where(m => !m.IsPrivate); // Remember that explicit interface implementations are private so they will be filtered out...
        foreach (var method in methods)
        {
            SetupMethod(method);
        }

        if (delegateSpecification.IsSatisfiedBy(mockedType)) return;

        var allProperties = mockedType.GetAllProperties().Where(p => p.GetMethod?.IsPublicOrInternal() == true); // This will also filter out all explicit implementation as they are always private

        // Properties with both get and public/internal set will be handled in the command for it
        // TODO... for virtual methods we can do it here and use a custom invocation func so to delay the generation of the objects
        // Remeber that `private` setters in the base will have no setter in the proxy
        // TODO... we changed that properties where one of the bases has a private setter it should be done in the auto command
        //      So maybe we have to check it here as well so it shouldn't be killing it to setup the get
        //      But maybe it isn't an issue because we shouldn't setup for Callbase and for non callbase it shouldn't really matter
        var singleMethodProperties = allProperties.Where(p => !p.HasGetAndSet(false) || p.SetMethod?.IsPrivate == true);
        foreach (var prop in singleMethodProperties)
        {
            SetupSingleMethodProperty(prop);
        }

        if (mock.CallBase || delegateSpecification.IsSatisfiedBy(mockedType)) return; // Explicit interface implementation must have an implementation so only if !callbase

        var explicitProperties = mockedType.GetExplicitInterfaceProperties().ToArray();
        foreach (var prop in explicitProperties.Where(p => !p.HasGetAndSet(false)))
        {
            SetupExplicitProperty(prop, mockedType, mock);
        }

        var explicitMethods = mockedType.GetExplicitInterfaceMethods().Except(explicitProperties.SelectMany(p => p.GetMethods()));
        foreach (var method in explicitMethods)
        {
            SetupExplicitMethod(method, mockedType, mock);
        }

    }

    private void SetupExplicitProperty(PropertyInfo prop, Type mockedType, IAutoMock mock)
    {
        var trackingPath = prop.GetTrackingPath();

        Func<MethodInfo, ISetupService> setupFunc = ifaceMethod
                                => setupServiceFactory.GetPropertySetup(mock, ifaceMethod, context, trackingPath, ifaceMethod.DeclaringType);

        SetupExplicitMember(prop.GetMethods().First(), mockedType, mock, prop, trackingPath, setupFunc);
    }

    private void SetupExplicitMethod(MethodInfo method, Type mockedType, IAutoMock mock)
    {
        var trackingPath = method.GetTrackingPath();

        Func<MethodInfo, ISetupService> setupFunc = ifaceMethod
                                => setupServiceFactory.GetMethodSetup(mock, ifaceMethod, context, trackingPath, ifaceMethod.DeclaringType);

        SetupExplicitMember(method, mockedType, mock, method, trackingPath, setupFunc);
    }

    private void SetupExplicitMember(MethodInfo method, Type mockedType, IAutoMock mock, MemberInfo member,
        string trackingPath, Func<MethodInfo, ISetupService> setupService)
    {
        try
        {
            var ifaceMethod = method.GetExplicitInterfaceMethod();
            if (ifaceMethod is null) // Should not happen...
            {
                HandleCannotSetup(trackingPath, CannotSetupReason.InterfaceMethodNotFound);
                return;
            }

            if (!autoMockHelpers.CanMock(ifaceMethod.DeclaringType))
            {
                HandleCannotSetup(trackingPath, CannotSetupReason.TypeNotPublic);
                return;
            }

            setupServiceFactory.GetPropertySetup(mock, ifaceMethod, context, trackingPath, ifaceMethod.DeclaringType).Setup();

            mock.MethodsSetup.Add(trackingPath, member);
        }
        catch (Exception ex)
        {
            mock.MethodsNotSetup.Add(trackingPath, new CannotSetupMethodException(CannotSetupReason.Exception, ex));
        }
    }

    private void Setup(MemberInfo member, Action action, string? trackingPath = null)
    {
        var method = member as MethodInfo;
        var prop = member as PropertyInfo;

        trackingPath ??= method?.GetTrackingPath() ?? prop!.GetTrackingPath();
        var methods = prop?.GetMethods() ?? new[] { method! };

        if (mock.CallBase && !mock.GetInnerType().IsInterface && !methods.Any(m => m.IsAbstract))
        { // It is callbase and has an implementation so let's ignore it
            HandleCannotSetup(trackingPath, CannotSetupReason.CallBaseNoAbstract);
            return;
        }

        var configureInfo = CanBeConfigured(methods);
        if (!configureInfo.CanConfigure)
        {
            HandleCannotSetup(trackingPath, configureInfo.Reason!.Value);
            return;
        }

        try
        {
            action();
            mock.MethodsSetup.Add(trackingPath, member);
        }
        catch (Exception ex)
        {
            mock.MethodsNotSetup.Add(trackingPath, new CannotSetupMethodException(CannotSetupReason.Exception, ex));
        }
    }

    private void SetupMethod(MethodInfo method)
    {
        Setup(method, () => setupServiceFactory.GetMethodSetup(mock, method, context).Setup());
    }

    private void SetupSingleMethodProperty(PropertyInfo prop)
    {
        var method = prop.GetMethods().First();

        var trackingPath = prop.GetTrackingPath();
        Setup(method, () => setupServiceFactory.GetPropertySetup(mock, method, context, trackingPath).Setup(), trackingPath);
    }

    private void SetupAutoProperty(PropertyInfo prop)
    {
        Setup(prop, () =>
        {
            var request = noMockDependencies
                                    ? new PropertyRequest(mockedType, prop, tracker)
                                    : new AutoMockPropertyRequest(mockedType, prop, tracker);
            var propValue = context.Resolve(request);
            setupServiceFactory.GetAutoPropertySetup(mockedType, prop.PropertyType, mock, prop, propValue).Setup();
        });
    }

    private IEnumerable<MethodInfo> GetMethods()
    {
        // If "type" is a delegate, return "Invoke" method only and skip the rest of the methods.
        var methods = delegateSpecification.IsSatisfiedBy(mockedType)
            ? new[] { mockedType.GetTypeInfo().GetMethod("Invoke") }
            : mockedType.GetAllMethods();

        var propMethods = mockedType.GetAllProperties().SelectMany(p => p.GetMethods());
        return methods.Except(propMethods).Where(m => m.IsPublicOrInternal());
    }

    private void HandleCannotSetup(string trackingPath, CannotSetupReason reason)
        => mock.MethodsNotSetup.Add(trackingPath, new CannotSetupMethodException(reason));

    private (bool CanConfigure, CannotSetupReason? Reason) CanBeConfigured(MethodInfo[] methods)
    {
        if (!mockedType.IsInterface && methods.Any(m => !m.IsOverridable())) return (false, CannotSetupReason.NonVirtual);

        if (methods.Any(m => m.IsPrivate)) return (false, CannotSetupReason.Private);

        if (methods.Any(m => !m.IsPublicOrInternal())) return (false, CannotSetupReason.Protected);

        return (true, null);
    }
}
