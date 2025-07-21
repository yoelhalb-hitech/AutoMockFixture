using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.HelperRequests.AutoMock;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;
using Moq;

namespace AutoMockFixture.Tests.FixtureUtils.Requests.BaseTracker;

internal class BaseTracker_Tests
{
    [Test]
    public void GetChildrensPaths_ReturnsCurrentPath()
    {
        var fixture = new AbstractAutoMockFixture(); // Plain Autofixture doesn't work... because of constructors
        var trackerMock = new AutoMock<TrackerWithFixture>(typeof(Type), fixture);
        trackerMock.SetupGet(t => t.InstancePath).Returns("Test");
        trackerMock.CallBase = true;

        var tracker = trackerMock.Object;
        var setResult = new object();

        tracker.SetResult(setResult, (ISpecimenBuilder?)null);
        var result = tracker.GetChildrensPaths();
        result.Should().BeEquivalentTo(new Dictionary<string, object> { [tracker.Path] = setResult });
        result!.Any().Should().BeTrue();
        result!.First()!.Key.Should().EndWith("Test");
    }

    [Test]
    public void GetHashCode_DoesNotCauseStackOverflow()
    {
        var request = new AutoMockRequest(typeof(global::AutoMockFixture.FixtureUtils.Requests.BaseTracker), new AbstractAutoMockFixture());
        Assert.DoesNotThrow(() => request.GetHashCode());
    }

    [Test]
    public void Test_SetResult_SetsWhenPath_EvenIfNotInstancePath_BugRepro()
    {
        var type = typeof(global::AutoMockFixture.FixtureUtils.Requests.BaseTracker);
        var prop = type.GetProperties().First();

        var startRequest = new AutoMockRequest(typeof(global::AutoMockFixture.FixtureUtils.Requests.BaseTracker), new AbstractAutoMockFixture());
        var propRequest = new AutoMockPropertyRequest(type, prop, startRequest);

        var request1 = new AutoMockRequest(typeof(global::AutoMockFixture.FixtureUtils.Requests.BaseTracker), propRequest);
        var request2 = new AutoMockRequest(typeof(global::AutoMockFixture.FixtureUtils.Requests.BaseTracker), request1);
        var request3 = new AutoMockRequest(typeof(global::AutoMockFixture.FixtureUtils.Requests.BaseTracker), request2);

        var result = new object();
        request3.SetResult(result, null);
        request1.Result.Should().NotBeNull();
        request1.Result!.Target.Should().Be(result);
    }
}
