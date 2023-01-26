using AutoMockFixture.FixtureUtils;
using AutoMockFixture.FixtureUtils.Specifications;
using System.ComponentModel;

namespace AutoMockFixture.FixtureUtils; // Use this namespace not to be in the main namespace (would have made it internal but then the subclasses would also have to be internal)

/// <summary>
/// CAUTION: the methods are not thread safe
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class UnitFixture : AutoMockFixture
{
    public UnitFixture(bool noConfigureMembers = false, bool generateDelegates = false, MethodSetupTypes? methodSetupType = null) 
                : base(noConfigureMembers, generateDelegates, methodSetupType)
    {
        Customizations.Add(new FilteringSpecimenBuilder(
                                new FixedBuilder(this),
                                new TypeOrRequestSpecification(new TypeSpecification(typeof(UnitFixture)), AutoMockHelpers)));
    }

    public override object? Create(Type t, AutoMockTypeControl? autoMockTypeControl = null)
        => CreateWithAutoMockDependencies(t, false, autoMockTypeControl);
}
