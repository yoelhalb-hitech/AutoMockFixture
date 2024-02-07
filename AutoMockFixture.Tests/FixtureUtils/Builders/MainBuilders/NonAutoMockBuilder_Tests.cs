using AutoMockFixture.FixtureUtils.Builders.MainBuilders;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;

namespace AutoMockFixture.Tests.FixtureUtils.Builders.MainBuilders;

internal class NonAutoMockBuilder_Tests
{
    public sealed class TestSealedClass { }

    [Test]
    public void Test_Create_BuildsItself_EvenIfSealed()
    {
        var fixture = new AbstractAutoMockFixture();

        var request = new NonAutoMockRequest(typeof(TestSealedClass), fixture);
        var expectedResult = new TestSealedClass();

        var invokerMock = new AutoMock<ISpecimenBuilder>();
        invokerMock.Setup("Create", new { }, (object)expectedResult);

        var builder = new NonAutoMockBuilder(invokerMock.Object, fixture.AutoMockHelpers);

        var contextMock = new AutoMock<ISpecimenContext>();

        var actual = builder.Create(request, contextMock.Object);
        actual.Should().Be(expectedResult);

        invokerMock.Verify(c => c.Create(request, contextMock.Object));
        contextMock.VerifyNoOtherCalls();
    }
}
