
namespace AutoMockFixture.Moq4.IntegrationTests;

internal class IntegrationFixture_Tests
{
    [Test]
    public void Test_IntegrationFixture_ReturnsSelf()
    {
        var fixture = new IntegrationFixture();
        var info = fixture.Trace();

        var f = fixture.Create<IntegrationFixture>();

        f.Should().BeSameAs(fixture);
    }
}
