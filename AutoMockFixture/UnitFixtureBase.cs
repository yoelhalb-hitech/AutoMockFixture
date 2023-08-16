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
                : base(noConfigureMembers, generateDelegates, methodSetupType)
    {
        Customizations.Add(new FilteringSpecimenBuilder(
                                new FixedBuilder(this),
                                new TypeOrRequestSpecification(new TypeSpecification(typeof(UnitFixtureBase)), AutoMockHelpers)));
    }

    public override object? Create(Type t, AutoMockTypeControl? autoMockTypeControl = null)
        => CreateWithAutoMockDependencies(t, false, autoMockTypeControl);

    public override object? Create(Type t, bool callbase = false, AutoMockTypeControl? autoMockTypeControl = null)
        => CreateWithAutoMockDependencies(t, callbase, autoMockTypeControl);

    internal override TrackerWithFixture GetStartTrackerForAutoMock(Type type, bool callBase) => new AutoMockDependenciesRequest(type, this) { MockShouldCallbase = callBase };
}
