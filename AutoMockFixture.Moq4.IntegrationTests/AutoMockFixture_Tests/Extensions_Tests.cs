
namespace AutoMockFixture.Moq4.IntegrationTests.AutoMockFixture_Tests;

internal class Extensions_Tests
{
    public class Test
    {
        public virtual void TestMethod() { }
    }
    [Test]
    public void Test_GetAutoMocks_Creates_OnFreezeAndCreate()
    {
        var fixture = new UnitFixture();
        Test? t = null;
        Assert.DoesNotThrow(() => t = fixture.GetAutoMock<Test>(true));

        t.Should().NotBeNull();
        t.Should().BeAssignableTo<Test>();

        AutoMockFixture.Moq4.AutoMock.Get(t).Should().NotBeNull();
    }

    [Test]
    public void Test_Verify_DoesNotThrow_BugRepro()
    {
        var fixture = new UnitFixture();
        fixture.GetAutoMock<Test>(true);
        fixture.Verify();
    }

    [Test]
    public void Test_Verify_VerifiesAll()
    {
        var fixture = new UnitFixture();

        var first = fixture.Create<AutoMock<Test>>()!; // Do not use here `GetAutoMock<Test>(true)` as it will freeze it...
        first.Setup(t1 => t1.TestMethod()).Verifiable();

        var second = fixture.Create<AutoMock<Test>>()!;
        second.Setup(t2 => t2.TestMethod()).Verifiable();

        first.Object.TestMethod();

        Assert.Throws<Moq.MockException>(() => fixture.Verify());
    }
}
