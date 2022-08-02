using AutoMoqExtensions.FixtureUtils.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.Test.FixtureUtils.Requests.BaseTracker.BaseTracker_ShouldAutoMock
{
    internal class AutoMockRequest_Tests
    {
        [Test]
        public void Test_ReturnsTrue_When_Is_StartObject_When_MockDependencies()
        {
            var request = new AutoMockRequest(typeof(TrackerWithFixture), new AutoMockFixture()) { NoMockDependencies = false };
            request.ShouldAutoMock.Should().BeTrue();
        }

        [Test]
        public void Test_ReturnsTrue_When_Is_StartObject_When_NoMockDependencies()
        {
            var request = new AutoMockRequest(typeof(TrackerWithFixture), new AutoMockFixture()) { NoMockDependencies = true };
            request.ShouldAutoMock.Should().BeTrue();
        }

        [Test]
        public void Test_ReturnsTrue_When_Is_In_DependencyChain_When_MockDependencies()
        {
            var dependencyRequest = new AutoMockDependenciesRequest(typeof(TrackerWithFixture), new AutoMockFixture());
            var request = new AutoMockRequest(typeof(TrackerWithFixture), dependencyRequest) { NoMockDependencies = false };
            request.ShouldAutoMock.Should().BeTrue();
        }

        [Test]
        public void Test_ReturnsTrue_When_Is_In_DependencyChain_When_NoMockDependencies()
        {
            var dependencyRequest = new AutoMockDependenciesRequest(typeof(TrackerWithFixture), new AutoMockFixture());
            var request = new AutoMockRequest(typeof(TrackerWithFixture), dependencyRequest) { NoMockDependencies = true };
            request.ShouldAutoMock.Should().BeTrue();
        }

        [Test]
        public void Test_ReturnsTrue_When_Is_In_MockChain_When_MockDependencies()
        {
            var noMockChainRequest = new AutoMockRequest(typeof(TrackerWithFixture), new AutoMockFixture()) { NoMockDependencies = false };
            var request = new AutoMockRequest(typeof(TrackerWithFixture), noMockChainRequest) { NoMockDependencies = false };
            request.ShouldAutoMock.Should().BeTrue();
        }

        [Test]
        public void Test_ReturnsTrue_When_Is_In_MockChain_When_NoMockDependencies()
        {
            var noMockChainRequest = new AutoMockRequest(typeof(TrackerWithFixture), new AutoMockFixture()) { NoMockDependencies = false };
            var request = new AutoMockRequest(typeof(TrackerWithFixture), noMockChainRequest) { NoMockDependencies = true };
            request.ShouldAutoMock.Should().BeTrue();
        }

        [Test]
        public void Test_ReturnsTrue_When_Is_NotIn_MockChain_When_MockDependencies()
        {
            var noMockChainRequest = new NonAutoMockRequest(typeof(TrackerWithFixture), new AutoMockFixture());
            var request = new AutoMockRequest(typeof(TrackerWithFixture), noMockChainRequest) { NoMockDependencies = false };
            request.ShouldAutoMock.Should().BeTrue();
        }

        [Test]
        public void Test_ReturnsTrue_When_Is_NotIn_MockChain_When_NoMockDependencies()
        {
            var noMockChainRequest = new NonAutoMockRequest(typeof(TrackerWithFixture), new AutoMockFixture());
            var request = new AutoMockRequest(typeof(TrackerWithFixture), noMockChainRequest) { NoMockDependencies = true };
            request.ShouldAutoMock.Should().BeTrue();
        }
    }
}
