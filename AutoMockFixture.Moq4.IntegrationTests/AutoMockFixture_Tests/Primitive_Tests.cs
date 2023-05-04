using AutoMockFixture.FixtureUtils.Trace;

namespace AutoMockFixture.Tests.AutoMockFixture_Tests;

internal class Primitive_Tests
{
    [Test]
    public void Test_CreateNonAutoMock_CreatesString()
    {
        var fixture = new AbstractAutoMockFixture();
        var str = fixture.CreateNonAutoMock<string>();

        str.Should().NotBeNullOrWhiteSpace();
    }

    [Test]
    public void Test_CreateWithAutoMockDependencies_CreatesString()
    {
        var fixture = new AbstractAutoMockFixture();
        var str = fixture.CreateWithAutoMockDependencies<string>();

        str.Should().NotBeNullOrWhiteSpace();
    }

    [Test]
    public void Test_CreateAutoMock_ThrowsException_WhenCreateString()
    {
        var fixture = new AbstractAutoMockFixture();
        Assert.Throws<InvalidOperationException>(() => fixture.CreateAutoMock<string>());
    }

    [Test]
    public void Test_CreateNonAutoMock_CreatesInt()
    {
        var fixture = new AbstractAutoMockFixture();
        var i = fixture.CreateNonAutoMock<int>();

        i.Should().NotBe(default);
    }

    [Test]
    public void Test_CreateWithAutoMockDependencies_CreatesInt()
    {
        var fixture = new AbstractAutoMockFixture();
        var i = fixture.CreateWithAutoMockDependencies(typeof(int));

        i.Should().NotBe(default);
    }

    [Test]
    public void Test_CreateAutoMock_ThrowsException_WhenCreateInt()
    {
        var fixture = new AbstractAutoMockFixture();
        Assert.Throws<InvalidOperationException>(() => fixture.CreateAutoMock(typeof(int)));
    }

    [Test]
    public void Test_CreateNonAutoMock_CreatesNonAutoMockEventHandler()
    {
        //var autoFixture = new Fixture();
        //var e1 = autoFixture.Create<EventHandler<string>>();
        var fixture = new AbstractAutoMockFixture();
        var info = new TraceInfo();
        fixture.Behaviors.Add(new TraceBehavior(info));

        //var result = new SpecimenContext(fixture).Resolve(typeof(EventHandler<string>));
        //AutoMock.IsAutoMock(result).Should().BeFalse();
        var e = fixture.CreateNonAutoMock<EventHandler<string>>();

        e.Should().NotBeNull();
        AutoMock.IsAutoMock(e).Should().BeFalse();
    }

    [Test]
    public void Test_CreateWithAutoMockDependencies_CreatesNonAutoMockEventHandler()
    {
        var fixture = new AbstractAutoMockFixture();
        var e = fixture.CreateWithAutoMockDependencies<EventHandler<string>>();

        e.Should().NotBeNull();
        AutoMock.IsAutoMock(e).Should().BeFalse();
    }

    [Test]
    public void Test_CreateAutoMock_CreatesMockedEventHandler()
    {
        var fixture = new AbstractAutoMockFixture();
        var e = fixture.CreateAutoMock<EventHandler<string>>();

        e.Should().NotBeNull();
        AutoMock.IsAutoMock(e).Should().BeTrue();
    }
}
