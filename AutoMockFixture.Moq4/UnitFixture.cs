using AutoMockFixture.Moq4.AutoMockUtils;

namespace AutoMockFixture.Moq4;

/// <summary>
/// CAUTION: the methods are not thread safe
/// </summary>
public class UnitFixture : AutoMockFixture.FixtureUtils.UnitFixtureBase
{
    public UnitFixture() : this(false) {}

    public UnitFixture(bool noConfigureMembers) : this(noConfigureMembers, false) {}

    public UnitFixture(bool noConfigureMembers, bool generateDelegates)
                                : this(noConfigureMembers, generateDelegates, null) {}

    public UnitFixture(bool noConfigureMembers, bool generateDelegates, MethodSetupTypes? methodSetupType)
                                : base(noConfigureMembers, generateDelegates, methodSetupType) {}

    internal override IAutoMockHelpers AutoMockHelpers => new AutoMockHelpers();
}
