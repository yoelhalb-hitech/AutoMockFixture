using AutoMockFixture.FixtureUtils;
using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;
using AutoMockFixture.FixtureUtils.Requests.SpecialRequests;
using System.Diagnostics.CodeAnalysis;

namespace AutoMockFixture.Tests.FixtureUtils;

internal class Cache_Tests
{
    class CacheSub : Cache
    {
        public CacheSub() : base(new AbstractAutoMockFixture())
        {
        }

        public new bool IsRequestEquals(TrackerWithFixture request, TrackerWithFixture inCache)
            => base.IsRequestEquals(request, inCache);
    }

    public class Testing { }
    record TrackerWithFixtureNonAbstract : TrackerWithFixture
    {
        [SetsRequiredMembers]
        public TrackerWithFixtureNonAbstract(IAutoMockFixture fixture) : base(typeof(Testing), fixture){}

        [SetsRequiredMembers]
        public TrackerWithFixtureNonAbstract(ITracker tracker) : base(typeof(Testing), tracker) { }

        public override bool MockDependencies => true;
    }

    [Test]
    public void Test_IsRequestEquals_ReturnsFalse_WhenDifferentFixture()
    {
        var request = new TrackerWithFixtureNonAbstract(new AbstractAutoMockFixture());
        var request2 = new TrackerWithFixtureNonAbstract(new AbstractAutoMockFixture());

        new CacheSub().IsRequestEquals(request, request2).Should().BeFalse();
    }

    [Test]
    public void Test_IsRequestEquals_ReturnsFalse_WhenDifferentCallBase()
    {
        var fixture = new AbstractAutoMockFixture();

        var callBaseTrue = new TrackerWithFixtureNonAbstract(fixture) { MockShouldCallBase = true };
        var callBaseFalse = new TrackerWithFixtureNonAbstract(fixture) { MockShouldCallBase = false };

        var request = new TrackerWithFixtureNonAbstract(callBaseTrue);
        var request2 = new TrackerWithFixtureNonAbstract(callBaseFalse);

        new CacheSub().IsRequestEquals(request, request2).Should().BeFalse();
    }

    [Test]
    public void Test_IsRequestEquals_ReturnsTrue_WhenSameFixture_AndCallBaseTrue()
    {
        var fixture = new AbstractAutoMockFixture();

        var callBaseTrue1 = new AutoMockRequest(typeof(string), fixture) { MockShouldCallBase = true };
        var callBaseTrue2 = new AutoMockRequest(typeof(string), fixture) { MockShouldCallBase = true };

        var request = new TrackerWithFixtureNonAbstract(callBaseTrue1);
        var request2 = new TrackerWithFixtureNonAbstract(callBaseTrue2);

        new CacheSub().IsRequestEquals(request, request2).Should().BeTrue();
    }

    [Test]
    public void Test_IsRequestEquals_ReturnsTrue_WhenSameFixture_AndCallBaseFalse()
    {
        var fixture = new AbstractAutoMockFixture();

        var callBaseFalse1 = new AutoMockRequest(typeof(string), fixture) { MockShouldCallBase = false };
        var callBaseFalse2 = new AutoMockRequest(typeof(string), fixture) { MockShouldCallBase = false };

        var request = new TrackerWithFixtureNonAbstract(callBaseFalse1);
        var request2 = new TrackerWithFixtureNonAbstract(callBaseFalse2);

        new CacheSub().IsRequestEquals(request, request2).Should().BeTrue();
    }
}
