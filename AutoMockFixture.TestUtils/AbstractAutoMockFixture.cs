using AutoMockFixture.AutoMockUtils;
using AutoMockFixture.FixtureUtils;
using AutoMockFixture.Moq4.AutoMockUtils;

namespace AutoMockFixture.TestUtils;

public class AbstractAutoMockFixture : FixtureUtils.AutoMockFixture
{ 
    public AbstractAutoMockFixture(bool noConfigureMembers = false) : base(noConfigureMembers) { }

    internal override IAutoMockHelpers AutoMockHelpers => new AutoMockHelpers();

    public override object Create(Type t, AutoMockTypeControl? autoMockTypeControl = null) => throw new NotSupportedException();
    public override T Freeze<T>()
    {
        try
        {
            base.Freeze<T>();
        }
        catch { }
#pragma warning disable CS8603 // Possible null reference return.
        return default;
#pragma warning restore CS8603 // Possible null reference return.
    }
}
