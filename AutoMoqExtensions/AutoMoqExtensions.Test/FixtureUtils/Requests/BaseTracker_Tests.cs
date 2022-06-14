using AutoFixture;
using AutoMoqExtensions.FixtureUtils;
using AutoMoqExtensions.FixtureUtils.Requests;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoMoqExtensions.Test.FixtureUtils.Requests
{
    internal class BaseTracker_Tests
    {
        [Test]
        public void GetChildrensPaths_ReturnsCurrentPath()
        {
            var fixture = new AutoMockFixture(); // Plain Autofixture doesn't work... because of constructors
            var trackerMock = fixture.Create<AutoMock<BaseTracker>>();
            trackerMock.SetupGet(t => t.InstancePath).Returns("Test");
            trackerMock.CallBase = true;

            var tracker = trackerMock.Object;
            var setResult = new object();

            tracker.SetResult(setResult);
            var result = tracker.GetChildrensPaths();
            result.Should().BeEquivalentTo(new Dictionary<string, object> { [tracker.Path] = setResult });
            result.First().Key.Should().EndWith("Test");
        }

        public void GetHashCode_doesNotCauseStackOverflow()
        {
            var request = new AutoMockRequest(typeof(BaseTracker), null);            
            Assert.DoesNotThrow(() => request.GetHashCode());
        }
    }
}
