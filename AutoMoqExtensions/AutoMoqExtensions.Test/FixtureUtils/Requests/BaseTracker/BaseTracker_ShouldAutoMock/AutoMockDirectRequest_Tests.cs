using AutoMoqExtensions.FixtureUtils.Requests;
using AutoMoqExtensions.FixtureUtils.Requests.MainRequests;

namespace AutoMoqExtensions.Test.FixtureUtils.Requests.BaseTracker.BaseTracker_ShouldAutoMock;

internal class AutoMockDirectRequest_Tests
{
    [Test]
    public void Test_ReturnsFalse_When_Is_StartObject()
    {
        var request = new AutoMockDependenciesRequest(typeof(TrackerWithFixture), new AbstractAutoMockFixture());
        request.ShouldAutoMock.Should().BeFalse();
    }

    [Test]
    public void Test_ReturnsTrue_When_Is_In_DependencyChain()
    {
        var dependencyRequest = new AutoMockDependenciesRequest(typeof(TrackerWithFixture), new AbstractAutoMockFixture());
        var request = new AutoMockDependenciesRequest(typeof(TrackerWithFixture), dependencyRequest);
        request.ShouldAutoMock.Should().BeTrue();
    }

    [Test]
    public void Test_ReturnsTrue_When_Is_In_MockChain()
    {
        var noMockChainRequest = new AutoMockRequest(typeof(TrackerWithFixture), new AbstractAutoMockFixture())
                                                                                { NoMockDependencies = false };
        var request = new AutoMockDependenciesRequest(typeof(TrackerWithFixture), noMockChainRequest);
        request.ShouldAutoMock.Should().BeTrue();
    }

    [Test]
    public void Test_ReturnsTrue_When_Is_NotIn_MockChain()
    {
        var noMockChainRequest = new NonAutoMockRequest(typeof(TrackerWithFixture), new AbstractAutoMockFixture());
        var request = new AutoMockDependenciesRequest(typeof(TrackerWithFixture), noMockChainRequest);
        request.ShouldAutoMock.Should().BeTrue();
    }
}
