
using AutoMockFixture.FixtureUtils;

namespace AutoMockFixture.Moq4.IntegrationTests.AutoMockFixture_Tests;

internal class InvalidRequests_Tests
{
    [Test]
    public void Test_Throws_OnAutoMockType_WhenNotMockable()
    {
        var fixture = new AbstractAutoMockFixture();

        Assert.Throws<InvalidOperationException>(() => fixture.CreateAutoMock<AutoMock<Task>>());
        Assert.ThrowsAsync<InvalidOperationException>(async () => await fixture.CreateAutoMockAsync<AutoMock<Task>>());

        Assert.Throws<InvalidOperationException>(() => fixture.CreateWithAutoMockDependencies<AutoMock<Task>>());
        Assert.ThrowsAsync<InvalidOperationException>(async () => await fixture.CreateWithAutoMockDependenciesAsync<AutoMock<Task>>());

        Assert.Throws<InvalidOperationException>(() => fixture.CreateNonAutoMock<AutoMock<Task>>());
        Assert.ThrowsAsync<InvalidOperationException>(async () => await fixture.CreateNonAutoMockAsync<AutoMock<Task>>());
    }

    class Test { }

    [Test]
    public void Test_Throws_OnAutoMockType_WhenFixtureTypeControlNotMockable()
    {
        var typeControl = new AutoMockTypeControl();
        typeControl.NeverAutoMockTypes.Add(typeof(Test));

        var fixture = new AbstractAutoMockFixture();
        fixture.AutoMockTypeControl = typeControl;

        Assert.Throws<InvalidOperationException>(() => fixture.CreateAutoMock<AutoMock<Test>>());
        Assert.ThrowsAsync<InvalidOperationException>(async () => await fixture.CreateAutoMockAsync<AutoMock<Task>>());

        Assert.Throws<InvalidOperationException>(() => fixture.CreateWithAutoMockDependencies<AutoMock<Test>>());
        Assert.ThrowsAsync<InvalidOperationException>(async () => await fixture.CreateWithAutoMockDependenciesAsync<AutoMock<Task>>());

        Assert.Throws<InvalidOperationException>(() => fixture.CreateNonAutoMock<AutoMock<Test>>());
        Assert.ThrowsAsync<InvalidOperationException>(async () => await fixture.CreateNonAutoMockAsync<AutoMock<Test>>());
    }


    [Test]
    public void Test_Throws_OnAutoMockType_WhenArgTypeControlNotMockable()
    {
        var typeControl = new AutoMockTypeControl();
        typeControl.NeverAutoMockTypes.Add(typeof(Test));

        var fixture = new AbstractAutoMockFixture();

        Assert.Throws<InvalidOperationException>(() => fixture.CreateAutoMock<AutoMock<Test>>(autoMockTypeControl: typeControl));
        Assert.ThrowsAsync<InvalidOperationException>(async () => await fixture.CreateAutoMockAsync<AutoMock<Task>>(autoMockTypeControl: typeControl));

        Assert.Throws<InvalidOperationException>(() => fixture.CreateWithAutoMockDependencies<AutoMock<Test>>(autoMockTypeControl: typeControl));
        Assert.ThrowsAsync<InvalidOperationException>(async () => await fixture.CreateWithAutoMockDependenciesAsync<AutoMock<Task>>(autoMockTypeControl: typeControl));

        Assert.Throws<InvalidOperationException>(() => fixture.CreateNonAutoMock<AutoMock<Test>>(autoMockTypeControl: typeControl));
        Assert.ThrowsAsync<InvalidOperationException>(async () => await fixture.CreateNonAutoMockAsync<AutoMock<Test>>(autoMockTypeControl: typeControl));
    }
}
