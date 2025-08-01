﻿using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;

namespace AutoMockFixture.Tests.FixtureUtils.Requests;

internal class AutoMockDirectRequest_Tests
{
    [Test]
    public void Test_AutoMockDirectRequest_RecordWith_WorksCorrectly()
    {
        var fixture = new AbstractAutoMockFixture();

        var autoMockRequest = new AutoMockDirectRequest(typeof(string), fixture) { MockShouldCallBase = true };
        var newRequest = autoMockRequest with { Request = typeof(int) };

        newRequest.Should().NotBeNull();
        newRequest.MockShouldCallBase.Should().BeTrue();

        autoMockRequest.Request.Should().Be(typeof(string));
        newRequest.Request.Should().Be(typeof(int));
        newRequest.Fixture.Should().Be(fixture);

        newRequest.Parent.Should().Be(autoMockRequest);
        autoMockRequest.Children.Should().Contain(newRequest);
    }

    [Test]
    public void Test_MockDependecies_DoesNotCauseStackOverflow()
    {
        var fixture = new AbstractAutoMockFixture();

        var autoMockRequest = new AutoMockDirectRequest(typeof(string), fixture);
        Assert.DoesNotThrow(() => _ = autoMockRequest.MockDependencies);
    }

    [Test]
    public void Test_MockDependecies_ReturnsBaseTracker()
    {
        var trackerMock = new AutoMock<TrackerWithFixture>(typeof(Type), new AbstractAutoMockFixture()) { CallBase = true };
        trackerMock.Setup(t => t.MockDependencies).Returns(false);

        var autoMockRequest = new AutoMockDirectRequest(typeof(string), trackerMock.Object);
        autoMockRequest.MockDependencies.Should().BeFalse();
    }

    [Test]
    public void Test_MockDependecies_ReturnsDefaultCorrectly()
    {
        var autoMockRequest = new AutoMockDirectRequest(typeof(string), new AbstractAutoMockFixture());
        autoMockRequest.MockDependencies.Should().BeTrue();
    }
}
