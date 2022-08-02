using AutoMoqExtensions.FixtureUtils.Requests;
using Moq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.Test.FixtureUtils.Requests.BaseTracker.BaseTracker_ShouldAutoMock
{
    internal class IAutoMockRequest_Non_AutoMockRequest_Tests
    {
        [Test]
        public void Test_ReturnsTrue_When_Is_In_DependencyChain()
        {
            var dependencyRequest = new AutoMockDependenciesRequest(typeof(TrackerWithFixture), new AutoMockFixture());
            var request = new AutoMockConstructorArgumentRequest(typeof(TrackerWithFixture),
                                                                Mock.Of<ParameterInfo>(), dependencyRequest);
            request.ShouldAutoMock.Should().BeTrue();
        }

        [Test]
        public void Test_ReturnsTrue_When_Is_In_MockChain()
        {
            var noMockChainRequest = new AutoMockRequest(typeof(TrackerWithFixture), new AutoMockFixture()) 
                                                                                    { NoMockDependencies = false };
            var request = new AutoMockFieldRequest(typeof(TrackerWithFixture), Mock.Of<FieldInfo>(), noMockChainRequest);
            request.ShouldAutoMock.Should().BeTrue();
        }

        [Test]
        public void Test_ReturnsTrue_When_Is_NotIn_MockChain()
        {
            var noMockChainRequest = new NonAutoMockRequest(typeof(TrackerWithFixture), new AutoMockFixture());
            var request = new AutoMockPropertyRequest(typeof(TrackerWithFixture), Mock.Of<PropertyInfo>(), noMockChainRequest);
            request.ShouldAutoMock.Should().BeTrue();
        }
    }
}
