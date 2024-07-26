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
                : base(noConfigureMembers, generateDelegates, methodSetupType) {}

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public override T? Create<T>(bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null) where T : default
        => CreateNonAutoMock<T>(callBase, autoMockTypeControl);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public override Task<T?> CreateAsync<T>(bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null) where T : default
        => CreateNonAutoMockAsync<T>(callBase, autoMockTypeControl);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public override object? Create(Type t, bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null)
        => CreateNonAutoMock(t, callBase, autoMockTypeControl);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public override Task<object?> CreateAsync(Type t, bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null)
        => CreateNonAutoMockAsync(t, callBase, autoMockTypeControl);

    internal override TrackerWithFixture GetStartTrackerForAutoMock(Type type, bool? callBase) => new NonAutoMockRequest(type, this) { MockShouldCallBase = callBase ?? CallBase };
}
