using AutoMoqExtensions.FixtureUtils.Trace;

namespace AutoMoqExtensions.Test.Trace;

internal class TraceBehavior_IntegrationTests
{
    [Test]
    public void TraceBehavior_GeneratesInfo()
    {
        var fixture = new AutoMockFixture();
        var info = new TraceInfo();
        fixture.Behaviors.Add(new TraceBehavior(info));
        
        _ = fixture.CreateNonAutoMock<TestClass>();

        info.TraceValues.Count.Should().BePositive();        
    }

    [Test]
    public void TraceBehaviour_ContainsTheResult()
    {
        var fixture = new AutoMockFixture();
        var info = new TraceInfo();
        fixture.Behaviors.Add(new TraceBehavior(info));

        var result = fixture.CreateNonAutoMock<TestClass>();

        info.GetWithValues().Count.Should().BePositive();
        info.GetWithValues().Any(v => v.response == result).Should().BeTrue();
    }

    [Test]
    public void TraceBehaviour_IsInOrder()
    {
        var fixture = new AutoMockFixture();
        var info = new TraceInfo();
        fixture.Behaviors.Add(new TraceBehavior(info));
        
        _ = fixture.CreateNonAutoMock<TestClass>();

        // TraceValues can branch mutiple times but GetWithValues should be ordered straight
        var values = info.GetWithValues();
        values
            .Select((v, i) => new { Depth = v.depth, LastDepth = i == 0 ? 0 : values[i - 1].depth })
            .All(x => x.Depth == 0 || x.Depth == 1 || x.Depth == x.LastDepth || x.Depth == x.LastDepth + 1)
            .Should().BeTrue();
    }
}
