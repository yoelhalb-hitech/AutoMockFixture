using AutoMockFixture.Moq4.AutoMockUtils;

namespace AutoMockFixture.Moq4;

/// <summary>
/// CAUTION: the methods are not thread safe
/// </summary>
public class IntegrationFixture : AutoMockFixture.FixtureUtils.IntegrationFixtureBase
{
    public IntegrationFixture(bool noConfigureMembers = false, bool generateDelegates = false, MethodSetupTypes? methodSetupType = null)
                : base(noConfigureMembers, generateDelegates, methodSetupType)
    {
    }

    internal override IAutoMockHelpers AutoMockHelpers => new AutoMockHelpers();
}
