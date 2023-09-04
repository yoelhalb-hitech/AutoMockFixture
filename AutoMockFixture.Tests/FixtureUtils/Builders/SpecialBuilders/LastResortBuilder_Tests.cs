using AutoMockFixture.FixtureUtils.Builders.SpecialBuilders;
using AutoMockFixture.FixtureUtils.Requests.HelperRequests.NonAutoMock;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;
using Moq;
using System.Reflection;

namespace AutoMockFixture.Tests.FixtureUtils.Builders.SpecialBuilders;

internal class LastResortBuilder_Tests
{
    [Test]
    public void Test_When_AutoMock_Creates_AutoMockDependencies()
    {
        var fixture = new AbstractAutoMockFixture();
        var context = Mock.Of<ISpecimenContext>();

        var request = new AutoMockRequest(typeof(LastResortBuilder_Tests), fixture);

        var builder = new LastResortBuilder(fixture.AutoMockHelpers);
        builder.Create(request, context);

        Mock.Get(context).Verify(c =>
            c.Resolve(It.Is<AutoMockDependenciesRequest>(r => r.Request == request.Request && Object.ReferenceEquals(r.Parent, request))),
                Times.Once());
        Mock.Get(context).VerifyNoOtherCalls();
    }

    [Test]
    public void Test_When_AutoMockDependencies_Creates_NonAutoMock()
    {
        var fixture = new AbstractAutoMockFixture();
        var context = Mock.Of<ISpecimenContext>();

        var request = new AutoMockDependenciesRequest(typeof(LastResortBuilder_Tests), fixture);

        var builder = new LastResortBuilder(fixture.AutoMockHelpers);
        builder.Create(request, context);

        Mock.Get(context).Verify(c => c.Resolve(It.Is<NonAutoMockRequest>(r => r.Request == request.Request && Object.ReferenceEquals(r.Parent, request))), Times.Once());
        Mock.Get(context).VerifyNoOtherCalls();
    }

    [Test]
    public void Test_When_RequestWithType_SendsRequestType()
    {
        var fixture = new AbstractAutoMockFixture();
        var context = Mock.Of<ISpecimenContext>();

        var request = new NonAutoMockRequest(typeof(LastResortBuilder_Tests), fixture);

        var builder = new LastResortBuilder(fixture.AutoMockHelpers);
        builder.Create(request, context);

        Mock.Get(context).Verify(c => c.Resolve(It.Is<Type>(r => r == request.Request)), Times.Once());
        Mock.Get(context).VerifyNoOtherCalls();
    }

    [Test]
    public void Test_When_NotRequestWithType_SendsRequest()
    {
        var fixture = new AbstractAutoMockFixture();
        var context = Mock.Of<ISpecimenContext>();

        var request = new object();

        var builder = new LastResortBuilder(fixture.AutoMockHelpers);
        builder.Create(request, context);

        Mock.Get(context).Verify(c => c.Resolve(It.Is<object>(r => r == request)), Times.Once());
        Mock.Get(context).VerifyNoOtherCalls();
    }

    [Test]
    public void Test_When_AutoMockDependencies_AndRecursion_SendsRequestType()
    {
        var fixture = new AbstractAutoMockFixture();
        var context = Mock.Of<ISpecimenContext>();

        var nonAutoMock = new NonAutoMockRequest(typeof(LastResortBuilder_Tests), fixture);
        var request = new AutoMockDependenciesRequest(typeof(LastResortBuilder_Tests), nonAutoMock);

        var builder = new LastResortBuilder(fixture.AutoMockHelpers);
        builder.Create(request, context);

        Mock.Get(context).Verify(c => c.Resolve(It.Is<Type>(r => r == request.Request)), Times.Once());
        Mock.Get(context).VerifyNoOtherCalls();
    }

    [Test]
    public void Test_When_AutoMockDependencies_AndRecursionOnDifferentLevel_Creates_NonAutoMock()
    {
        var fixture = new AbstractAutoMockFixture();
        var context = Mock.Of<ISpecimenContext>();

        var nonAutoMock = new NonAutoMockRequest(typeof(LastResortBuilder_Tests), fixture);
        var propertyRequest = new PropertyRequest(typeof(LastResortBuilder_Tests), Mock.Of<PropertyInfo>(), nonAutoMock);
        var request = new AutoMockDependenciesRequest(typeof(LastResortBuilder_Tests), propertyRequest);

        var builder = new LastResortBuilder(fixture.AutoMockHelpers);
        builder.Create(request, context);

        Mock.Get(context).Verify(c => c.Resolve(It.Is<NonAutoMockRequest>(r => r.Request == request.Request && Object.ReferenceEquals(r.Parent, request))), Times.Once());
        Mock.Get(context).VerifyNoOtherCalls();
    }

    [Test]
    public void Test_Throws_When_ResultIsSame()
    {
        var fixture = new AbstractAutoMockFixture();
        var context = Mock.Of<ISpecimenContext>();

        var request = new object();
        Mock.Get(context).Setup(c => c.Resolve(It.IsAny<object>())).Returns(request);

        var builder = new LastResortBuilder(fixture.AutoMockHelpers);
        Assert.Throws<Exception>(() => builder.Create(request, context));
    }

    [Test]
    public void Test_Returns_When_ResultIsSame_AndRequestIsType()
    {
        var fixture = new AbstractAutoMockFixture();
        var context = Mock.Of<ISpecimenContext>();

        var request = typeof(Type);
        Mock.Get(context).Setup(c => c.Resolve(It.IsAny<Type>())).Returns(request);

        var builder = new LastResortBuilder(fixture.AutoMockHelpers);
        object? result = default!;

        Assert.DoesNotThrow(() => result = builder.Create(request, context));
        result.Should().Be(request);
    }

    [Test]
    public void Test_DoesNotAllow_AutoMock()
    {
        var fixture = new AbstractAutoMockFixture();

        var builder = new LastResortBuilder(fixture.AutoMockHelpers);
        var result = builder.Create(typeof(AutoMock<string>), AutoMock.Of<ISpecimenContext>());

        result.Should().BeAssignableTo<NoSpecimen>();
    }
}
