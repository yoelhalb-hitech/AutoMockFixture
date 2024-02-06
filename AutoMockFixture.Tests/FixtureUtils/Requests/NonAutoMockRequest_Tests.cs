using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;

namespace AutoMockFixture.Tests.FixtureUtils.Requests;

internal class NonAutoMockRequest_Tests
{
    [Test]
    public void Test_NonAutoMockRequest_RecordWith_WorksCorrectly()
    {
        var fixture = new AbstractAutoMockFixture();

        var nonAutoMockRequest = new NonAutoMockRequest(typeof(string), fixture) { MockShouldCallBase = true };
        var newRequest = nonAutoMockRequest with { Request = typeof(int) };

        newRequest.Should().NotBeNull();
        newRequest.MockShouldCallBase.Should().BeTrue();

        nonAutoMockRequest.Request.Should().Be(typeof(string));
        newRequest.Request.Should().Be(typeof(int));
        newRequest.Fixture.Should().Be(fixture);

        newRequest.Parent.Should().Be(nonAutoMockRequest);
        nonAutoMockRequest.Children.Should().Contain(newRequest);
    }

    [Test]
    public void Test_MockDependecies_DoesNotCauseStackOverflow()
    {
        var fixture = new AbstractAutoMockFixture();

        var autoMockRequest = new NonAutoMockRequest(typeof(string), fixture);
        Assert.DoesNotThrow(() => _ = autoMockRequest.MockDependencies);
    }

    [Test]
    public void Test_MockDependecies_ReturnsBaseTracker()
    {
        var trackerMock = new AutoMock<TrackerWithFixture>(new AbstractAutoMockFixture(), null) { CallBase = true };
        trackerMock.Setup(t => t.MockDependencies).Returns(true);

        var autoMockRequest = new NonAutoMockRequest(typeof(string), trackerMock.Object);
        autoMockRequest.MockDependencies.Should().BeTrue();
    }

    [Test]
    public void Test_MockDependecies_ReturnsDefaultCorrectly()
    {
        var autoMockRequest = new NonAutoMockRequest(typeof(string), new AbstractAutoMockFixture());
        autoMockRequest.MockDependencies.Should().BeFalse();
    }
}
