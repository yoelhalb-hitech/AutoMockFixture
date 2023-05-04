using AutoMockFixture.FixtureUtils.Builders.MainBuilders;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;
using Moq;

namespace AutoMockFixture.Tests.FixtureUtils.Builders.MainBuilders;

internal class AutoMockBuilder_Tests
{
    public class Test { }
    [Test]
    public void Test_SetsTracker()
    {
        var fixture = new AbstractAutoMockFixture();
        var autoMock = new AutoMock<Test>();

        var request = new AutoMockDirectRequest(autoMock.GetType(), fixture);

        var context = Mock.Of<ISpecimenContext>();
        var builder = new Mock<ISpecimenBuilder>();

        builder.Setup(b => b.Create(request, context)).Returns(autoMock);

        var obj = new AutoMockBuilder(builder.Object, fixture.AutoMockHelpers);
        obj.Create(request, context);

        autoMock.Tracker.Should().Be(request);
    }

    [Test]
    public void Test_SetsResult()
    {
        var autoMock = new AutoMock<Test>();

        var type = autoMock.GetType();
        var fixture = new AbstractAutoMockFixture();

        var requestMock = new Mock<AutoMockDirectRequest>(type, fixture);
        requestMock.CallBase = true;
        requestMock.SetupGet(r => r.Request).Returns(type);
        requestMock.SetupGet(r => r.Fixture).Returns(fixture);

        var request = requestMock.Object;

        var context = Mock.Of<ISpecimenContext>();
        var builder = new Mock<ISpecimenBuilder>();

        builder.Setup(b => b.Create(request, context)).Returns(autoMock);

        var obj = new AutoMockBuilder(builder.Object, fixture.AutoMockHelpers);
        obj.Create(request, context);

        requestMock.Verify(m => m.SetResult(autoMock, obj));
    }
}
