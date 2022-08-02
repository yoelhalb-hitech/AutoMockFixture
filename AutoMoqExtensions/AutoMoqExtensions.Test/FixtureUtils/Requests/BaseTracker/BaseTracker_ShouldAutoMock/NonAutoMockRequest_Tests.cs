

using AutoMoqExtensions.FixtureUtils.Requests;
using AutoMoqExtensions.FixtureUtils.Requests.MainRequests;

namespace AutoMoqExtensions.Test.FixtureUtils.Requests.BaseTracker.BaseTracker_ShouldAutoMock;

internal class NonAutoMockRequest_Tests
{
    [Test]
    public void Test_ReturnsFalse_When_Is_StartObject()
    {
        var request = new NonAutoMockRequest(typeof(TrackerWithFixture), new AutoMockFixture());
        request.ShouldAutoMock.Should().BeFalse();
    }

    [Test]
    public void Test_ReturnsFalse_When_Is_In_DependencyChain()
    {
        var dependencyRequest = new AutoMockDependenciesRequest(typeof(TrackerWithFixture), new AutoMockFixture());
        var request = new NonAutoMockRequest(typeof(TrackerWithFixture), dependencyRequest);
        request.ShouldAutoMock.Should().BeFalse();
    }


    [Test]
    public void Test_ReturnsFalse_When_Is_In_MockChain()
    {
        var noMockChainRequest = new AutoMockRequest(typeof(TrackerWithFixture), new AutoMockFixture()) { NoMockDependencies = false };
        var request = new NonAutoMockRequest(typeof(TrackerWithFixture), noMockChainRequest);
        request.ShouldAutoMock.Should().BeFalse();
    }

    [Test]
    public void Test_ReturnsFalse_When_Is_NotIn_MockChain()
    {
        var noMockChainRequest = new NonAutoMockRequest(typeof(TrackerWithFixture), new AutoMockFixture());
        var request = new NonAutoMockRequest(typeof(TrackerWithFixture), noMockChainRequest);
        request.ShouldAutoMock.Should().BeFalse();
    }
}
