using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.HelperRequests.AutoMock;
using AutoMockFixture.FixtureUtils.Requests.HelperRequests.NonAutoMock;
using SequelPay.DotNetPowerExtensions.Reflection;
using System.Linq;
using static AutoMockFixture.AutoMockUtils.CannotSetupMethodException;

namespace AutoMockFixture.AutoMockUtils;

internal class MockSetupService
{
    private static readonly DelegateSpecification delegateSpecification = new DelegateSpecification();

    private readonly IAutoMock mock;
    private readonly MockSetupHelperService setupHelperService;
    private readonly Type mockedType;

    public MockSetupService(IAutoMock mock, MockSetupHelperService setupHelperService)
    {
        this.mock = mock;
        this.setupHelperService = setupHelperService;
        // Don't do mock.GetMocked().GetType() as it has additional properties etc.
        mockedType = mock.GetInnerType();
    }

    public void Setup()
    {
        // CAUTION: I am doing the decision here and not in the `GetMethod()` in case we want to change the logic it should be in one place
        // If it's not overridable it won't be in the proxy but we still want to send it to the setup method so to be able to save a CannotSetupReason
        var includeNotOverridableCurrent = true;
        var includeNotOverridableBase = false; // But private in the base class or shadowed we don't need

        foreach (var method in GetMethods(includeNotOverridableCurrent, includeNotOverridableBase))
        {
            setupHelperService.SetupMethod(method);
        }

        if (delegateSpecification.IsSatisfiedBy(mockedType)) return;

        var allProperties = GetProperties(includeNotOverridableCurrent, includeNotOverridableBase);

         // TODO... for virtual methods we can do it here and use a custom invocation func so to delay the generation of the objects, but maybe this might cause it to stop having property behavier
        // Remeber that `private` setters in the base will have no setter in the proxy
        var singleMethodProperties = allProperties.Where(p => p.SetMethod is null || p.SetMethod.ReflectionInfo.IsPrivate);
        foreach (var prop in singleMethodProperties)
        {
            setupHelperService.SetupSingleMethodProperty(prop);
        }

        var autoProperties = allProperties.Where(p => p.SetMethod is not null && p.GetMethod is not null && !p.SetMethod.ReflectionInfo.IsPrivate);
        foreach (var prop in autoProperties)
        {
            setupHelperService.SetupReadWriteProperty(prop);
        }

        if (mock.CallBase || delegateSpecification.IsSatisfiedBy(mockedType)) return; // Explicit interface implementation must have an implementation so only if !callBase

        var detailType = mockedType.GetTypeDetailInfo();

        var explicitProperties = detailType.ExplicitPropertyDetails.ToArray();
        foreach (var prop in explicitProperties.Where(p => p.SetMethod is null || p.SetMethod.ReflectionInfo.IsPrivate))
        {
            setupHelperService.SetupSingleMethodProperty(prop);
        }

        var explicitAutoProperties = explicitProperties.Where(p => p.SetMethod is not null && p.GetMethod is not null && !p.SetMethod.ReflectionInfo.IsPrivate);
        foreach (var prop in explicitAutoProperties)
        {
            setupHelperService.SetupReadWriteProperty(prop);
        }

        var explicitMethods = detailType.ExplicitMethodDetails.ToArray();
        foreach (var method in explicitMethods)
        {
            setupHelperService.SetupMethod(method);
        }

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
        if (delegateSpecification.IsSatisfiedBy(mockedType))
            return new[] { mockedType.GetTypeInfo().GetTypeDetailInfo().MethodDetails.FirstOrDefault(md => md.Name == "Invoke") }
                .OfType<MethodDetail>();

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
}
