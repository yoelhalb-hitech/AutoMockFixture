using AutoMockFixture.FixtureUtils.Requests.MainRequests;

namespace AutoMockFixture.Tests.FixtureUtils.Requests;

internal class AutoMockRequest_Tests
{
    [Test]
    public void Test_AutoMockRequest_RecordWith_WorksCorrectly()
    {
        var fixture = new AbstractAutoMockFixture();

        var autoMockRequest = new AutoMockRequest(typeof(string), fixture) { MockShouldCallbase = true, BypassChecks = true };
        var newRequest = autoMockRequest with { Request = typeof(int) };

        newRequest.Should().NotBeNull();
        newRequest.MockShouldCallbase.Should().BeTrue();
        newRequest.BypassChecks.Should().BeTrue();

        autoMockRequest.Request.Should().Be(typeof(string));
        newRequest.Request.Should().Be(typeof(int));
        newRequest.Fixture.Should().Be(fixture);

        newRequest.Parent.Should().Be(autoMockRequest);
        autoMockRequest.Children.Should().Contain(newRequest);
    }
}
