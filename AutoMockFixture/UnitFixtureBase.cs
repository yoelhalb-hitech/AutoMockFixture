using AutoMockFixture.FixtureUtils;
using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;
using AutoMockFixture.FixtureUtils.Specifications;
using System.ComponentModel;

namespace AutoMockFixture.FixtureUtils; // Use this namespace not to be in the main namespace (would have made it internal but then the subclasses would also have to be internal)

/// <summary>
/// CAUTION: the methods are not thread safe
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public abstract class UnitFixtureBase : AutoMockFixtureBase
{
    public UnitFixtureBase(bool noConfigureMembers = false, bool generateDelegates = false, MethodSetupTypes? methodSetupType = null)
                : base(noConfigureMembers, generateDelegates, methodSetupType) {}

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public override T? Create<T>(bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null) where T : default
        => CreateWithAutoMockDependencies<T>(callBase, autoMockTypeControl);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public override Task<T?> CreateAsync<T>(bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null) where T : default
        => CreateWithAutoMockDependenciesAsync<T>(callBase, autoMockTypeControl);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public override object? Create(Type t, bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null)
        => CreateWithAutoMockDependencies(t, callBase, autoMockTypeControl);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public override Task<object?> CreateAsync(Type t, bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null)
        => CreateWithAutoMockDependenciesAsync(t, callBase, autoMockTypeControl);

    internal override TrackerWithFixture GetStartTrackerForAutoMock(Type type, bool? callBase) => new AutoMockDependenciesRequest(type, this) { MockShouldCallBase = callBase ?? CallBase };
}
