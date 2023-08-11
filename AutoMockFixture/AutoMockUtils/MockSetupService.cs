using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.HelperRequests.AutoMock;
using AutoMockFixture.FixtureUtils.Requests.HelperRequests.NonAutoMock;
using DotNetPowerExtensions.Reflection;
using DotNetPowerExtensions.Reflection.Models;
using System.Linq;
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
        // CAUTION: I am doing the decision here and not in the `GetMethod()` in case we want to change the logic it should be in one place
        // If it's not overridable it won't be in the proxy but we still want to send it to the setup method so to be able to save a CannotSetupReason
        var includeNotOverridableCurrent = true;
        var includeNotOverridableBase = false; // But private in the base class or shadowed we don't need

        foreach (var method in GetMethods(includeNotOverridableCurrent, includeNotOverridableBase))
        {
            SetupMethod(method);
        }

        if (delegateSpecification.IsSatisfiedBy(mockedType)) return;

        var allProperties = GetProperties(includeNotOverridableCurrent, includeNotOverridableBase);

        // Properties with set will be handled in the command for it
        // TODO... for virtual methods we can do it here and use a custom invocation func so to delay the generation of the objects, but maybe this might cause it to stop having property behavier
        // Remeber that `private` setters in the base will have no setter in the proxy
        var singleMethodProperties = allProperties.Where(p => p.SetMethod is null || p.SetMethod.IsPrivate == true);
        foreach (var prop in singleMethodProperties)
        {
            SetupSingleMethodProperty(prop);
        }

        if (mock.CallBase || delegateSpecification.IsSatisfiedBy(mockedType)) return; // Explicit interface implementation must have an implementation so only if !callbase

        var detailType = mockedType.GetTypeDetailInfo();

        var explicitProperties = detailType.ExplicitPropertyDetails.ToArray();
        foreach (var prop in explicitProperties.Where(p => p.SetMethod is null || p.ReflectionInfo.SetMethod.IsPrivate == true))
        {
            SetupExplicitProperty(prop, mock);
        }

        var explicitMethods = detailType.ExplicitMethodDetails.ToArray();
        foreach (var method in explicitMethods)
        {
            SetupExplicitMethod(method, mock);
        }

    }

    private void SetupExplicitProperty(PropertyDetail propInfo, IAutoMock mock)
    {
        var trackingPath = ":" + propInfo.ExplicitInterface!.FullName + "." + propInfo.Name;

        var method = propInfo.ReflectionInfo.GetMethod;
        var underlying = propInfo.ExplicitInterface.GetProperty(propInfo.Name, BindingFlagsExtensions.AllBindings).GetMethod;

        Func<ISetupService> setupFunc = ()
                                => setupServiceFactory.GetPropertySetup(mock, method, context, trackingPath, propInfo.ExplicitInterface, underlying);

        SetupExplicitMember(method, mock, propInfo.ReflectionInfo, trackingPath, setupFunc);
    }

    private void SetupExplicitMethod(MethodDetail methodInfo, IAutoMock mock)
    {
        var trackingPath = ":" + methodInfo.ExplicitInterface!.FullName + "." + methodInfo.Name; // Do not use DeclaringType as it might be an override

        var method = methodInfo.ReflectionInfo;
        var underlying = methodInfo.ExplicitInterface.GetMethod(methodInfo.Name, BindingFlagsExtensions.AllBindings);

        Func<ISetupService> setupFunc = ()
                                => setupServiceFactory.GetMethodSetup(mock, method, context, trackingPath, methodInfo.ExplicitInterface, underlying);

        SetupExplicitMember(method, mock, method, trackingPath, setupFunc);
    }

    private void SetupExplicitMember(MethodInfo method, IAutoMock mock, MemberInfo member,
        string trackingPath, Func<ISetupService> setupFunc)
    {
        try
        {
            if (!autoMockHelpers.CanMock(method.DeclaringType))
            {
                HandleCannotSetup(trackingPath, CannotSetupReason.TypeNotPublic);
                return;
            }

            setupFunc().Setup();

            mock.MethodsSetup.Add(trackingPath, member);
        }
        catch (Exception ex)
        {
            mock.MethodsNotSetup.Add(trackingPath, new CannotSetupMethodException(CannotSetupReason.Exception, ex));
        }
    }

    private void Setup(MemberInfo member, Action action, string? trackingPath = null)
    {
        var prop = member as PropertyInfo;
        var method = member as MethodInfo ?? prop!.GetMethod;

        trackingPath ??= prop?.GetTrackingPath() ?? method!.GetTrackingPath();

        if (mock.CallBase && !method.IsAbstract) // Cannot check by interface as an interface can have a default implementation
        { // It is callbase and has an implementation so let's ignore it
            HandleCannotSetup(trackingPath, CannotSetupReason.CallBaseNoAbstract);
            return;
        }

        var configureInfo = CanBeConfigured(method);
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
        var method = prop.GetAllMethods().First();

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

    private IEnumerable<PropertyInfo> GetProperties(bool includeNotOverridableCurrent, bool includeNotOverridableBase)
    {
        // If "type" is a delegate, return "Invoke" method only and skip the rest of the methods.
        if (delegateSpecification.IsSatisfiedBy(mockedType)) return new PropertyInfo[] {};

        var detailType = mockedType.GetTypeDetailInfo();

        // Private method can't be overriden...
        // TODO... maybe change it in the IsOverridable logic (true that explicit can still be overriden by reimlementing it but in this case IsOverridable would also be wrong)
        var propDetails = detailType.PropertyDetails.Where(d => includeNotOverridableCurrent || d.ReflectionInfo.IsOverridable());

        if (includeNotOverridableBase)
        {
            propDetails = propDetails
                                .Concat(detailType.BasePrivatePropertyDetails)
                                .Concat(detailType.ShadowedPropertyDetails); // Cannot override a shadowed method...
        }

        return propDetails.Select(d => d.ReflectionInfo);
    }

    private IEnumerable<MethodInfo> GetMethods(bool includeNotOverridableCurrent, bool includeNotOverridableBase)
    {
        // If "type" is a delegate, return "Invoke" method only and skip the rest of the methods.
        if (delegateSpecification.IsSatisfiedBy(mockedType)) return new[] { mockedType.GetTypeInfo().GetMethod("Invoke") };

        var detailType = mockedType.GetTypeDetailInfo();

        // Private method can't be overriden...
        // TODO... maybe change it in the IsOverridable logic (true that explicit can still be overriden by reimlementing it but in this case IsOverridable would also be wrong)
        var methodDetails = detailType.MethodDetails.Where(d => includeNotOverridableCurrent || d.ReflectionInfo.IsOverridable());

        if(includeNotOverridableBase)
        {
            methodDetails = methodDetails
                                .Concat(detailType.BasePrivateMethodDetails)
                                .Concat(detailType.ShadowedMethodDetails); // Cannot override a shadowed method...
        }

        return methodDetails.Select(d => d.ReflectionInfo); // Remember that the property methods and explicit methods will get filtered out by the TypeDetaiInfo
    }

    private void HandleCannotSetup(string trackingPath, CannotSetupReason reason)
        => mock.MethodsNotSetup.Add(trackingPath, new CannotSetupMethodException(reason));

    private (bool CanConfigure, CannotSetupReason? Reason) CanBeConfigured(MethodInfo methods)
    {
        if (!mockedType.IsInterface && !methods.IsOverridable()) return (false, CannotSetupReason.NonVirtual);

        if (methods.IsPrivate) return (false, CannotSetupReason.Private);

        if (!methods.IsPublicOrInternal()) return (false, CannotSetupReason.Protected); //TODO... maybe we should set it up in case someone calls callbase on a method?

        return (true, null);
    }
}
