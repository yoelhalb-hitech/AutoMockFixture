using AutoMockFixture.FixtureUtils.Requests;
using Moq;

namespace AutoMockFixture.Tests.Extensions;

internal class TrackerExtensions_Tests
{
    [Test]
    public void Test_GetParentsOnCurrentLevel_RetrunsNothing_WhenNoParent()
    {
        var tracker = Mock.Of<ITracker>();

        var list = tracker.GetParentsOnCurrentLevel();

        list.Should().BeEmpty();
    }

    [Test]
    public void Test_GetParentsOnCurrentLevel_RetrunsNothing_WhenParentEqualsSelf()
    {
        var trackerMock = new Mock<ITracker>();
        trackerMock.SetupGet(t => t.Parent).Returns(trackerMock.Object);

        var list = trackerMock.Object.GetParentsOnCurrentLevel();

        list.Should().BeEmpty();
    }

    [Test]
    public void Test_GetParentsOnCurrentLevel_RetrunsNothing_WhenNothingOnSameLevel()
    {
        var tracker1 = Mock.Of<ITracker>();
        var tracker2Mock = new Mock<ITracker>();
        tracker2Mock.SetupGet(t => t.Parent).Returns(tracker1);
        tracker2Mock.SetupGet(t => t.Path).Returns("Test");

        var list = tracker2Mock.Object.GetParentsOnCurrentLevel();

        list.Should().BeEmpty();
    }

    [Test]
    public void Test_GetParentsOnCurrentLevel_RetrunsOtherOnSameLevel()
    {
        var tracker1 = Mock.Of<ITracker>();
        var tracker2Mock = new Mock<ITracker>();
        var tracker3Mock = new Mock<ITracker>();            

        const string path = "Test";
        tracker2Mock.SetupGet(t => t.Parent).Returns(tracker1);
        tracker2Mock.SetupGet(t => t.Path).Returns(path);

        tracker3Mock.SetupGet(t => t.Parent).Returns(tracker2Mock.Object);
        tracker3Mock.SetupGet(t => t.Path).Returns(path);

        var list = tracker3Mock.Object.GetParentsOnCurrentLevel();

        list.Should().BeEquivalentTo(tracker2Mock.Object);
    }

    [Test]
    public void Test_GetParentsOnCurrentLevel_RetrunsAllOnSameLevel()
    {
        var tracker1 = Mock.Of<ITracker>();
        var tracker2Mock = new Mock<ITracker>();
        var tracker3Mock = new Mock<ITracker>();
        var tracker4Mock = new Mock<ITracker>();

        const string path = "Test";
        tracker2Mock.SetupGet(t => t.Parent).Returns(tracker1);
        tracker2Mock.SetupGet(t => t.Path).Returns(path);

        tracker3Mock.SetupGet(t => t.Parent).Returns(tracker2Mock.Object);
        tracker3Mock.SetupGet(t => t.Path).Returns(path);

        tracker4Mock.SetupGet(t => t.Parent).Returns(tracker3Mock.Object);
        tracker4Mock.SetupGet(t => t.Path).Returns(path);

        var list = tracker4Mock.Object.GetParentsOnCurrentLevel();

        list.Should().BeEquivalentTo(tracker2Mock.Object, tracker3Mock.Object);
    }
}
