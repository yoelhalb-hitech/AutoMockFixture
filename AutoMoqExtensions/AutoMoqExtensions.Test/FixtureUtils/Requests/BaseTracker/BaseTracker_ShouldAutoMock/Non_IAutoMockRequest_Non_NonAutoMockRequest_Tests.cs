using AutoMoqExtensions.FixtureUtils.Requests;
using AutoMoqExtensions.FixtureUtils.Requests.HelperRequests.NonAutoMock;
using AutoMoqExtensions.FixtureUtils.Requests.MainRequests;
using Moq;
using System.Reflection;

namespace AutoMoqExtensions.Test.FixtureUtils.Requests.BaseTracker.BaseTracker_ShouldAutoMock;

internal class Non_IAutoMockRequest_Non_NonAutoMockRequest_Tests
{
    [Test]
    public void Test_ReturnsTrue_When_Is_In_DependencyChain()
    {
        var dependencyRequest = new AutoMockDependenciesRequest(typeof(TrackerWithFixture), new AbstractAutoMockFixture());
        var request = new ConstructorArgumentRequest(typeof(TrackerWithFixture),
                                                            Mock.Of<ParameterInfo>(), dependencyRequest);
        request.ShouldAutoMock.Should().BeTrue();
    }

    [Test]
    public void Test_ReturnsTrue_When_Is_In_MockChain()
    {
        var noMockChainRequest = new AutoMockRequest(typeof(TrackerWithFixture), new AbstractAutoMockFixture()) 
                                                                                { NoMockDependencies = false };
        var request = new FieldRequest(typeof(TrackerWithFixture), Mock.Of<FieldInfo>(), noMockChainRequest);
        request.ShouldAutoMock.Should().BeTrue();
    }

    [Test]
    public void Test_ReturnsFalse_When_Is_NotIn_MockChain()
    {
        var noMockChainRequest = new NonAutoMockRequest(typeof(TrackerWithFixture), new AbstractAutoMockFixture());
        var request = new PropertyRequest(typeof(TrackerWithFixture), Mock.Of<PropertyInfo>(), noMockChainRequest);
        request.ShouldAutoMock.Should().BeFalse();
    }
}
