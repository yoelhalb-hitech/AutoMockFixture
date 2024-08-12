using AutoMockFixture.FixtureUtils.Builders.MainBuilders;
using AutoMockFixture.FixtureUtils.Requests.HelperRequests.NonAutoMock;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;
using AutoMockFixture.Tests.FixtureUtils.Requests;
using Moq;
using System.Reflection;

namespace AutoMockFixture.Tests.FixtureUtils.Builders.MainBuilders;

internal class AutoMockDependenciesBuilder_Tests
{
    public sealed class TestSealedClass { }

    [Test]
    public void Test_Create_BuildsItself_EvenIfSealed()
    {
        var fixture = new AbstractAutoMockFixture();

        var request = new AutoMockDependenciesRequest(typeof(TestSealedClass), fixture);
        var expectedResult = new TestSealedClass();

        var invokerMock = new AutoMock<ISpecimenBuilder>();
        invokerMock.Setup("Create", new { }, (object)expectedResult);

        var builder = new AutoMockDependenciesBuilder(invokerMock.Object, fixture.AutoMockHelpers);

        var contextMock = new AutoMock<ISpecimenContext>();

        var actual = builder.Create(request, contextMock.Object);
        actual.Should().Be(expectedResult);

        invokerMock.Verify(c => c.Create(request, contextMock.Object));
        contextMock.VerifyNoOtherCalls();
    }


    [Test]
    public void Test_Create_ReturnsNoSpecimen_WhenRequestIsAutoMock()
    {
        var fixture = new AbstractAutoMockFixture();

        var innerType = typeof(NonAutoMockRequest_Tests);
        var request = new AutoMockDependenciesRequest(fixture.AutoMockHelpers.GetAutoMockType(innerType), fixture);

        var builder = new AutoMockDependenciesBuilder(Mock.Of<ISpecimenBuilder>(), fixture.AutoMockHelpers);
        var contextMock = new Mock<ISpecimenContext>();

        var result = builder.Create(request, contextMock.Object);
        result.Should().BeOfType<NoSpecimen>();

        contextMock.VerifyNoOtherCalls();
    }
}
