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
        var singleMethodProperties = allProperties.Where(p => p.SetMethod is null || p.SetMethod.ReflectionInfo.IsPrivate);
        foreach (var prop in singleMethodProperties)
        {
            SetupSingleMethodProperty(prop);
        }

        if (mock.CallBase || delegateSpecification.IsSatisfiedBy(mockedType)) return; // Explicit interface implementation must have an implementation so only if !callbase

        var detailType = mockedType.GetTypeDetailInfo();

        var explicitProperties = detailType.ExplicitPropertyDetails.ToArray();
        foreach (var prop in explicitProperties.Where(p => p.SetMethod is null || p.SetMethod.ReflectionInfo.IsPrivate))
        {
            SetupSingleMethodProperty(prop);
        }

        var explicitMethods = detailType.ExplicitMethodDetails.ToArray();
        foreach (var method in explicitMethods)
        {
            SetupMethod(method);
        }

    }

    private void Setup<T>(MemberDetail<T> member, Func<ISetupService> setupFunc) where T : MemberInfo
    {
        var prop = member as PropertyDetail;
        var method = member as MethodDetail ?? prop!.GetMethod ?? prop.SetMethod!;

        var trackingPath = prop?.GetTrackingPath() ?? method!.GetTrackingPath();

        if (!autoMockHelpers.CanMock(method.ExplicitInterface ?? method.ReflectionInfo.DeclaringType!))
        {
            HandleCannotSetup(trackingPath, CannotSetupReason.TypeNotPublic);
            return;
        }

        if (mock.CallBase && !method.ReflectionInfo.IsAbstract) // Cannot check by interface as an interface can have a default implementation
        { // It is callbase and has an implementation so let's ignore it
            HandleCannotSetup(trackingPath, CannotSetupReason.CallBaseNoAbstract);
            return;
        }

        if(!method.IsExplicit) // Explicit is always private and non virtual but is anyway configurable
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

    private void SetupMethod(MethodDetail method) 
        => Setup(method, () => setupServiceFactory.GetMethodSetup(mock, method, context));

    private void SetupSingleMethodProperty(PropertyDetail prop)
        => Setup(prop, () => setupServiceFactory.GetSingleMethodPropertySetup(mock, prop, context));

    private void SetupAutoProperty(PropertyDetail prop)
    {
        Setup(prop, () =>
        {
            var request = noMockDependencies
                                    ? new PropertyRequest(mockedType, prop.ReflectionInfo, tracker)
                                    : new AutoMockPropertyRequest(mockedType, prop.ReflectionInfo, tracker);
            var propValue = context.Resolve(request);
            return setupServiceFactory.GetAutoPropertySetup(mockedType, prop.ReflectionInfo.PropertyType, mock, prop.ReflectionInfo, propValue);
        });
    }

    private IEnumerable<PropertyDetail> GetProperties(bool includeNotOverridableCurrent, bool includeNotOverridableBase)
    {
        // If "type" is a delegate, return "Invoke" method only and skip the rest of the methods.
        if (delegateSpecification.IsSatisfiedBy(mockedType)) return new PropertyDetail[] {};

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

        return propDetails;
    }

    private IEnumerable<MethodDetail> GetMethods(bool includeNotOverridableCurrent, bool includeNotOverridableBase)
    {
        // If "type" is a delegate, return "Invoke" method only and skip the rest of the methods.
        if (delegateSpecification.IsSatisfiedBy(mockedType)) return new[] { mockedType.GetTypeInfo().GetTypeDetailInfo().MethodDetails.FirstOrDefault(md => md.Name == "Invoke") };

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

        return methodDetails; // Remember that the property methods and explicit methods will get filtered out by the TypeDetaiInfo
    }

    private void HandleCannotSetup(string trackingPath, CannotSetupReason reason)
        => mock.MethodsNotSetup.Add(trackingPath, new CannotSetupMethodException(reason));

    private (bool CanConfigure, CannotSetupReason? Reason) CanBeConfigured(MethodInfo method)
    {
        if (!mockedType.IsInterface && !method.IsOverridable()) return (false, CannotSetupReason.NonVirtual);

        if (method.IsPrivate) return (false, CannotSetupReason.Private);

        if (!method.IsPublicOrInternal()) return (false, CannotSetupReason.Protected); //TODO... maybe we should set it up in case someone calls callbase on a method?

        return (true, null);
    }
}
