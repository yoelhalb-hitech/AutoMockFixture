using AutoMockFixture.FixtureUtils;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;
using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Specifications;
using System.ComponentModel;

namespace AutoMockFixture.FixtureUtils; // Use this namespace not to be in the main namespace (would have made it internal but then the subclasses would also have to be internal)

/// <summary>
/// CAUTION: the methods are not thread safe
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public abstract class IntegrationFixtureBase : AutoMockFixtureBase
{
    public IntegrationFixtureBase(bool noConfigureMembers = false, bool generateDelegates = false, MethodSetupTypes? methodSetupType = null)
                : base(noConfigureMembers, generateDelegates, methodSetupType)
    {
        Customizations.Add(new FilteringSpecimenBuilder(
                            new FixedBuilder(this),
                            new TypeOrRequestSpecification(new TypeSpecification(typeof(IntegrationFixtureBase)), AutoMockHelpers)));
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public override T? Create<T>(bool callbase = false, AutoMockTypeControl? autoMockTypeControl = null) where T : default
        => CreateNonAutoMock<T>(false, autoMockTypeControl);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public override Task<T?> CreateAsync<T>(bool callbase = false, AutoMockTypeControl? autoMockTypeControl = null) where T : default
        => CreateNonAutoMockAsync<T>(false, autoMockTypeControl);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public override object? Create(Type t, bool callbase = false, AutoMockTypeControl? autoMockTypeControl = null)
        => CreateNonAutoMock(t, callbase, autoMockTypeControl);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public override Task<object?> CreateAsync(Type t, bool callbase = false, AutoMockTypeControl? autoMockTypeControl = null)
        => CreateNonAutoMockAsync(t, callbase, autoMockTypeControl);

    internal override TrackerWithFixture GetStartTrackerForAutoMock(Type type, bool callBase) => new NonAutoMockRequest(type, this) { MockShouldCallbase = callBase };
}
