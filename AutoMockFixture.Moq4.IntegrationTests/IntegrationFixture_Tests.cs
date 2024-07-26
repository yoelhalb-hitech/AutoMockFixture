using AutoFixture;
using AutoMockFixture.FixtureUtils;
using AutoMockFixture.NUnit3;

namespace AutoMockFixture.Moq4.IntegrationTests;

internal class IntegrationFixture_Tests
{
    [Test]
    [TestCase<IntegrationFixture>]
    [TestCase<IAutoMockFixture>]
    [TestCase<Fixture>]
    [TestCase<IFixture>]
    public void Test_IntegrationFixture_ReturnsSelf<TFixture>()
    {
        var fixture = new IntegrationFixture();
        var info = fixture.Trace();

        var f = fixture.Create<TFixture>();

        f.Should().BeSameAs(fixture);
    }
}
