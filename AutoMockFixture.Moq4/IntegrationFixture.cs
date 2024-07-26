using AutoMockFixture.Moq4.AutoMockUtils;

namespace AutoMockFixture.Moq4;

/// <summary>
/// CAUTION: the methods are not thread safe
/// </summary>
public class IntegrationFixture : AutoMockFixture.FixtureUtils.IntegrationFixtureBase
{
    public IntegrationFixture() : this(false) { }

    public IntegrationFixture(bool noConfigureMembers) : this(noConfigureMembers, false) { }

    public IntegrationFixture(bool noConfigureMembers, bool generateDelegates)
                                : this(noConfigureMembers, generateDelegates, null) { }

    public IntegrationFixture(bool noConfigureMembers, bool generateDelegates, MethodSetupTypes? methodSetupType)
                                : base(noConfigureMembers, generateDelegates, methodSetupType) { }


    internal override IAutoMockHelpers AutoMockHelpers => new AutoMockHelpers();
}
