using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;
using AutoMockFixture.FixtureUtils.Requests.SpecialRequests;

namespace AutoMockFixture.Tests.FixtureUtils.Requests;

internal class TrackerWithFixture_Tests
{
    class TrackerWithFixtureNonAbstract : TrackerWithFixture
    {
        public TrackerWithFixtureNonAbstract(AutoMockFixture fixture, ITracker? tracker = null) : base(fixture, tracker)
        {
        }

        public override bool MockDependencies => true;
    }

    [Test]
    public void Test_IsRequestEquals_ReturnsFalse_WhenNot_TrackerWithFixture()
    {
        var request = new TrackerWithFixtureNonAbstract(new AbstractAutoMockFixture());
        var other = new InnerRequest(typeof(string), new NonAutoMockRequest(typeof(string), request));

        request.IsRequestEquals(other).Should().BeFalse();
    }

    [Test]
    public void Test_IsRequestEquals_ReturnsFalse_WhenDifferentFixture()
    {
        var request = new TrackerWithFixtureNonAbstract(new AbstractAutoMockFixture());
        var request2 = new TrackerWithFixtureNonAbstract(new AbstractAutoMockFixture());            

        request.IsRequestEquals(request2).Should().BeFalse();
    }

    [Test]
    public void Test_IsRequestEquals_ReturnsFalse_WhenDifferentCallBase()
    {
        var fixture = new AbstractAutoMockFixture();

        var callBaseTrue = new TrackerWithFixtureNonAbstract(fixture) { MockShouldCallbase = true };
        var callBaseFalse = new TrackerWithFixtureNonAbstract(fixture) { MockShouldCallbase = false };

        var request = new TrackerWithFixtureNonAbstract(fixture, callBaseTrue);
        var request2 = new TrackerWithFixtureNonAbstract(fixture, callBaseFalse);

        request.IsRequestEquals(request2).Should().BeFalse();
    }

    [Test]
    public void Test_IsRequestEquals_ReturnsTrue_WhenSameFixture_AndCallBaseTrue()
    {
        var fixture = new AbstractAutoMockFixture();

        var callBaseTrue1 = new AutoMockRequest(typeof(string), fixture) { MockShouldCallbase = true };
        var callBaseTrue2 = new AutoMockRequest(typeof(string), fixture) { MockShouldCallbase = true };

        var request = new TrackerWithFixtureNonAbstract(fixture, callBaseTrue1);
        var request2 = new TrackerWithFixtureNonAbstract(fixture, callBaseTrue2);

        request.IsRequestEquals(request2).Should().BeTrue();
    }

    [Test]
    public void Test_IsRequestEquals_ReturnsTrue_WhenSameFixture_AndCallBaseFalse()
    {
        var fixture = new AbstractAutoMockFixture();

        var callBaseFalse1 = new AutoMockRequest(typeof(string), fixture) { MockShouldCallbase = false };
        var callBaseFalse2 = new AutoMockRequest(typeof(string), fixture) { MockShouldCallbase = false };

        var request = new TrackerWithFixtureNonAbstract(fixture, callBaseFalse1);
        var request2 = new TrackerWithFixtureNonAbstract(fixture, callBaseFalse2);

        request.IsRequestEquals(request2).Should().BeTrue();
    }
}
