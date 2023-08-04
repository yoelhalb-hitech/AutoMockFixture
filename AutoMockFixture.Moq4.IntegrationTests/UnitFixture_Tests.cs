
namespace AutoMockFixture.Moq4.IntegrationTests;

internal class UnitFixture_Tests
{
    [Test]
    public void Test_UnitFixture_ReturnsSelf()
    {
        var fixture = new UnitFixture();
        var info = fixture.Trace();

        var f = fixture.Create<UnitFixture>();

        f.Should().BeSameAs(fixture);
    }
}
