using AutoMockFixture.FixtureUtils;
using AutoMockFixture.FixtureUtils.Postprocessors;
using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;
using Moq;

namespace AutoMockFixture.Tests.FixtureUtils.Postprocessors;

internal class PostprocessorWithRecursion_Tests
{
    [Test]
    public void Test_AddsToProcessorDict_BeforeCommand()
    {
        var fixture = new AbstractAutoMockFixture();

        var specimen = new object();

        var builderMock = new AutoMock<ISpecimenBuilder>().Setup(nameof(ISpecimenBuilder.Create), new { }, specimen);

        var commandMock = new AutoMock<ISpecimenCommand>();
        var hasSpecimen = false;
        commandMock.Setup(c => c.Execute(It.IsAny<object>(), It.IsAny<ISpecimenContext>()),
                            s => s.Callback(() => hasSpecimen = fixture.ProcessingTrackerDict.ContainsKey(specimen)));

        var pp = new PostprocessorWithRecursion(fixture, builderMock.Object, commandMock.Object);

        pp.Create(Mock.Of<ITracker>(), null!);

        hasSpecimen.Should().BeTrue();
    }

    [Test]
    public void Test_AddsToProcessorDict_BeforeExtraCommand()
    {
        var fixture = new AbstractAutoMockFixture();

        var specimen = new object();

        var builderMock = new AutoMock<ISpecimenBuilder>().Setup(nameof(ISpecimenBuilder.Create), new { }, specimen);

        var commandMock = new AutoMock<ISpecimenCommand>();
        var hasSpecimen = false;
        commandMock.Setup(c => c.Execute(It.IsAny<object>(), It.IsAny<ISpecimenContext>()),
                            s => s.Callback(() => hasSpecimen = fixture.ProcessingTrackerDict.ContainsKey(specimen)));

        var pp = new PostprocessorWithRecursion(fixture, builderMock.Object,
                                    Mock.Of<ISpecimenCommand>(), null, commandMock.Object);

        pp.Create(Mock.Of<ITracker>(), null!);

        hasSpecimen.Should().BeTrue();
    }

    [Test]
    public void Test_RemovesProcessorDict_AfterCommand()
    {
        var fixture = new AbstractAutoMockFixture();

        var specimen = new object();

        var builderMock = new AutoMock<ISpecimenBuilder>().Setup(nameof(ISpecimenBuilder.Create), new { }, specimen);

        var commandMock = new AutoMock<ISpecimenCommand>();
        var hasSpecimen = false;
        commandMock.Setup(c => c.Execute(It.IsAny<object>(), It.IsAny<ISpecimenContext>()),
                            s => s.Callback(() => hasSpecimen = fixture.ProcessingTrackerDict.ContainsKey(specimen)));

        var pp = new PostprocessorWithRecursion(fixture, builderMock.Object, commandMock.Object);

        pp.Create(Mock.Of<ITracker>(), null!);

        hasSpecimen.Should().BeTrue();
        fixture.ProcessingTrackerDict.ContainsKey(specimen).Should().BeFalse();
    }

    [Test]
    public void Test_RemovesProcessorDict_WhenCommandThrows()
    {
        var fixture = new AbstractAutoMockFixture();

        var specimen = new object();

        var builderMock = new AutoMock<ISpecimenBuilder>().Setup(nameof(ISpecimenBuilder.Create), new { }, specimen);

        var commandMock = new AutoMock<ISpecimenCommand>();
        var hasSpecimen = false;
        commandMock.Setup(c => c.Execute(It.IsAny<object>(), It.IsAny<ISpecimenContext>()),
                                s => s.Callback(() => hasSpecimen = fixture.ProcessingTrackerDict.ContainsKey(specimen))
                                    .Throws(new Exception()));

        var pp = new PostprocessorWithRecursion(fixture, builderMock.Object, commandMock.Object);

        Assert.Throws<Exception>(() => pp.Create(Mock.Of<ITracker>(), null!));

        hasSpecimen.Should().BeTrue();
        fixture.ProcessingTrackerDict.ContainsKey(specimen).Should().BeFalse();
    }

    [Test]
    public void Test_DoesNotExecuteCommand_WhenRecursive_AndRequestIsType_AndSpecimenEquals()
    {
        var fixture = new AbstractAutoMockFixture();

        var request = typeof(object);
        var specimen = new object();

        var builderMock = new AutoMock<ISpecimenBuilder>().Setup(nameof(ISpecimenBuilder.Create), new { }, specimen);

        var commandMock = new AutoMock<ISpecimenCommand>();
        var extraCommandMock = new AutoMock<ISpecimenCommand>();

        var context = new RecursionContext(builderMock.Object, fixture);
        context.BuilderCache[request] = specimen;

        var pp = new PostprocessorWithRecursion(fixture, builderMock.Object, commandMock.Object, null, extraCommandMock.Object);

        pp.Create(request, context);

        commandMock.VerifyNoOtherCalls();
        extraCommandMock.VerifyNoOtherCalls();
    }

    [Test]
    public void Test_DoesNotExecuteCommand_WhenRecursive_AndRequestIsRequestWithType_AndSpecimenEquals()
    {
        var fixture = new AbstractAutoMockFixture();

        var request = new AutoMockRequest(typeof(object), fixture);
        var specimen = new object();

        var builderMock = new AutoMock<ISpecimenBuilder>().Setup(nameof(ISpecimenBuilder.Create), new { }, specimen);

        var commandMock = new AutoMock<ISpecimenCommand>();
        var extraCommandMock = new AutoMock<ISpecimenCommand>();

        var context = new RecursionContext(builderMock.Object, fixture);
        context.BuilderCache[request.Request] = specimen;

        var pp = new PostprocessorWithRecursion(fixture, builderMock.Object, commandMock.Object, null, extraCommandMock.Object);

        pp.Create(request, context);

        commandMock.VerifyNoOtherCalls();
        extraCommandMock.VerifyNoOtherCalls();
    }

    [Test]
    public void Test_ExecutesCommand_WhenRecursive_AndRequestIsType_AndSpecimenNotEquals()
    {
        var fixture = new AbstractAutoMockFixture();

        var request = typeof(object);
        var specimen = new object();

        var builderMock = new AutoMock<ISpecimenBuilder>().Setup(nameof(ISpecimenBuilder.Create), new { }, specimen);

        var commandMock = new AutoMock<ISpecimenCommand>();
        var extraCommandMock = new AutoMock<ISpecimenCommand>();

        var context = new RecursionContext(builderMock.Object, fixture);
        context.BuilderCache[request] = new object();

        var pp = new PostprocessorWithRecursion(fixture, builderMock.Object, commandMock.Object, null, extraCommandMock.Object);

        pp.Create(request, context);

        commandMock.Verify(c => c.Execute(specimen, context));
        extraCommandMock.Verify(c => c.Execute(specimen, context));
    }

    [Test]
    public void Test_ExecutesCommand_WhenRecursive_AndRequestIsRequestWithType_AndSpecimenNotEquals()
    {
        var fixture = new AbstractAutoMockFixture();

        var request = new AutoMockRequest(typeof(object), fixture);
        var specimen = new object();

        var builderMock = new AutoMock<ISpecimenBuilder>().Setup(nameof(ISpecimenBuilder.Create), new { }, specimen);

        var commandMock = new AutoMock<ISpecimenCommand>();
        var extraCommandMock = new AutoMock<ISpecimenCommand>();

        var context = new RecursionContext(builderMock.Object, fixture);
        context.BuilderCache[request.Request] = new object();

        var pp = new PostprocessorWithRecursion(fixture, builderMock.Object, commandMock.Object, null, extraCommandMock.Object);

        pp.Create(request, context);

        commandMock.Verify(c => c.Execute(specimen, context));
        extraCommandMock.Verify(c => c.Execute(specimen, context));
    }
}
