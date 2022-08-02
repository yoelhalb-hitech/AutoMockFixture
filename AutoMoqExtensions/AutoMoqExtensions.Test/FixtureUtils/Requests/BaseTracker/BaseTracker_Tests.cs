using AutoMoqExtensions.FixtureUtils.Requests;
using AutoMoqExtensions.FixtureUtils.Requests.MainRequests;
using Moq;

namespace AutoMoqExtensions.Test.FixtureUtils.Requests.BaseTracker;

internal class BaseTracker_Tests
{
    [Test]
    public void GetChildrensPaths_ReturnsCurrentPath()
    {
        var fixture = new AutoMockFixture(); // Plain Autofixture doesn't work... because of constructors
        var trackerMock = new Mock<TrackerWithFixture>(fixture, null);
        trackerMock.SetupGet(t => t.InstancePath).Returns("Test");
        trackerMock.CallBase = true;

        var tracker = trackerMock.Object;
        var setResult = new object();

        tracker.SetResult(setResult);
        var result = tracker.GetChildrensPaths();
        result.Should().BeEquivalentTo(new Dictionary<string, object> { [tracker.Path] = setResult });
        result.First().Key.Should().EndWith("Test");
    }

    [Test]
    public void GetHashCode_DoesNotCauseStackOverflow()
    {
        var request = new AutoMockRequest(typeof(AutoMoqExtensions.FixtureUtils.Requests.BaseTracker), new AutoMockFixture());
        Assert.DoesNotThrow(() => request.GetHashCode());
    }
}
