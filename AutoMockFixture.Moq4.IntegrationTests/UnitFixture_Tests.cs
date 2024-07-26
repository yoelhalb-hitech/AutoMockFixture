using AutoFixture;
using AutoMockFixture.FixtureUtils;
using AutoMockFixture.NUnit3;

namespace AutoMockFixture.Moq4.IntegrationTests;

internal class UnitFixture_Tests
{
    [Test]
    [TestCase<UnitFixture>]
    [TestCase<IAutoMockFixture>]
    [TestCase<Fixture>]
    [TestCase<IFixture>]
    public void Test_UnitFixture_ReturnsSelf<TFixture>()
    {
        var fixture = new UnitFixture();
        var info = fixture.Trace();

        var f = fixture.Create<TFixture>();

        f.Should().BeSameAs(fixture);
    }
}
