using AutoMockFixture.FixtureUtils;
using AutoMockFixture.FixtureUtils.Specifications;

namespace AutoMockFixture.FixtureUtils; // Use this namespace not to be in the main namespace (would have made it internal but then the subclasses would also have to be internal)

public abstract class IntegrationFixture : AutoMockFixture
{
    public IntegrationFixture(bool noConfigureMembers = false, bool generateDelegates = false, MethodSetupTypes? methodSetupType = null) 
                : base(noConfigureMembers, generateDelegates, methodSetupType)
    {
        Customizations.Add(new FilteringSpecimenBuilder(
                            new FixedBuilder(this),
                            new TypeOrRequestSpecification(new TypeSpecification(typeof(IntegrationFixture)), AutoMockHelpers)));
    }

    public override object? Create(Type t, AutoMockTypeControl? autoMockTypeControl = null)
        => CreateNonAutoMock(t, autoMockTypeControl);
}
