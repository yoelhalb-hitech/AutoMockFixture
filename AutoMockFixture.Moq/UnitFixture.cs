
using AutoMockFixture.AutoMockUtils;
using AutoMockFixture.Moq.AutoMockUtils;

namespace AutoMockFixture.Moq;

/// <summary>
/// CAUTION: the methods are not thread safe
/// </summary>
public class UnitFixture : AutoMockFixture.FixtureUtils.UnitFixture
{
    public UnitFixture(bool noConfigureMembers = false, bool generateDelegates = false, MethodSetupTypes? methodSetupType = null)
            : base(noConfigureMembers, generateDelegates, methodSetupType)
    {
    }

    internal override IAutoMockHelpers AutoMockHelpers => new AutoMockHelpers();
}
