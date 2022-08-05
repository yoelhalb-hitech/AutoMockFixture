

using AutoMoqExtensions.FixtureUtils.Requests;
using AutoMoqExtensions.FixtureUtils.Requests.MainRequests;
using AutoMoqExtensions.FixtureUtils.Requests.SpecialRequests;

namespace AutoMoqExtensions.Test.FixtureUtils.Requests;

internal class TrackerWithFixture_Tests
{
    [Test]
    public void Test_IsRequestEquals_ReturnsFalse_WhenNot_TrackerWithFixture()
    {
        var request = new TrackerWithFixture(new AbstractAutoMockFixture());
        var other = new TupleItemRequest(typeof(string), 0, false, request);

        request.IsRequestEquals(other).Should().BeFalse();
    }

    [Test]
    public void Test_IsRequestEquals_ReturnsFalse_WhenDifferentFixture()
    {
        var request = new TrackerWithFixture(new AbstractAutoMockFixture());
        var request2 = new TrackerWithFixture(new AbstractAutoMockFixture());            

        request.IsRequestEquals(request2).Should().BeFalse();
    }

    [Test]
    public void Test_IsRequestEquals_ReturnsFalse_WhenDifferentCallBase()
    {
        var fixture = new AbstractAutoMockFixture();

        var callBaseTrue = new TrackerWithFixture(fixture) { MockShouldCallbase = true };
        var callBaseFalse = new TrackerWithFixture(fixture) { MockShouldCallbase = false };

        var request = new TrackerWithFixture(fixture, callBaseTrue);
        var request2 = new TrackerWithFixture(fixture, callBaseFalse);

        request.IsRequestEquals(request2).Should().BeFalse();
    }

    [Test]
    public void Test_IsRequestEquals_ReturnsTrue_WhenSameFixture_AndCallBaseTrue()
    {
        var fixture = new AbstractAutoMockFixture();

        var callBaseTrue1 = new AutoMockRequest(typeof(string), fixture) { MockShouldCallbase = true };
        var callBaseTrue2 = new AutoMockRequest(typeof(string), fixture) { MockShouldCallbase = true };

        var request = new TrackerWithFixture(fixture, callBaseTrue1);
        var request2 = new TrackerWithFixture(fixture, callBaseTrue2);

        request.IsRequestEquals(request2).Should().BeTrue();
    }

    [Test]
    public void Test_IsRequestEquals_ReturnsTrue_WhenSameFixture_AndCallBaseFalse()
    {
        var fixture = new AbstractAutoMockFixture();

        var callBaseFalse1 = new AutoMockRequest(typeof(string), fixture) { MockShouldCallbase = false };
        var callBaseFalse2 = new AutoMockRequest(typeof(string), fixture) { MockShouldCallbase = false };

        var request = new TrackerWithFixture(fixture, callBaseFalse1);
        var request2 = new TrackerWithFixture(fixture, callBaseFalse2);

        request.IsRequestEquals(request2).Should().BeTrue();
    }
}
