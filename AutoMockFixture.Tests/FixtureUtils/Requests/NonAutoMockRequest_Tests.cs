using AutoMockFixture.FixtureUtils.Requests.MainRequests;

namespace AutoMockFixture.Tests.FixtureUtils.Requests;

internal class NonAutoMockRequest_Tests
{
    [Test]
    public void Test_NonAutoMockRequest_RecordWith_WorksCorrectly()
    {
        var fixture = new AbstractAutoMockFixture();

        var nonAutoMockRequest = new NonAutoMockRequest(typeof(string), fixture) { MockShouldCallbase = true };
        var newRequest = nonAutoMockRequest with { Request = typeof(int) };

        newRequest.Should().NotBeNull();
        newRequest.MockShouldCallbase.Should().BeTrue();

        nonAutoMockRequest.Request.Should().Be(typeof(string));
        newRequest.Request.Should().Be(typeof(int));
        newRequest.Fixture.Should().Be(fixture);

        newRequest.Parent.Should().Be(nonAutoMockRequest);
        nonAutoMockRequest.Children.Should().Contain(newRequest);
    }
}
