﻿using AutoMockFixture.FixtureUtils.Builders.MainBuilders;
using AutoMockFixture.FixtureUtils.Requests.HelperRequests.NonAutoMock;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;
using Moq;
using System.Reflection;

namespace AutoMockFixture.Tests.FixtureUtils.Builders.MainBuilders;

internal class AutoMockDependenciesBuilder_Tests
{
    [Test]
    public void Test_Create_CreatesAutoMockRequest_WhenRequestIsAutoMock()
    {
        var fixture = new AbstractAutoMockFixture();

        var innerType = typeof(AutoMockDependenciesBuilder_Tests);
        var requestMock = new Mock<AutoMockDependenciesRequest>(fixture.AutoMockHelpers.GetAutoMockType(innerType), fixture){ CallBase = true };
        var expectedResult = new AutoMock<object>();

        var builder = new AutoMockDependenciesBuilder(Mock.Of<ISpecimenBuilder>(), fixture.AutoMockHelpers);
        var contextMock = new Mock<ISpecimenContext>();
        contextMock.Setup(c => c.Resolve(It.IsAny<AutoMockRequest>())).Returns(expectedResult.Object);

        var result = builder.Create(requestMock.Object, contextMock.Object);
        result.Should().Be(expectedResult);

        contextMock.Verify(c => c.Resolve(It.Is<AutoMockRequest>(r => r.Request == innerType
                                                            && r.Parent == requestMock.Object)));
        contextMock.VerifyNoOtherCalls();

        requestMock.Verify(r => r.SetResult(expectedResult, builder));
    }

    [Test]
    public void Test_Create_AsksForCallBase_WhenRequestIsAutoMock_BasedOnCallBase([Values(true, false)] bool callbase)
    {
        var fixture = new AbstractAutoMockFixture();

        var innerType = typeof(AutoMockDependenciesBuilder_Tests);
        var request = new AutoMockDependenciesRequest(fixture.AutoMockHelpers.GetAutoMockType(innerType), fixture) { MockShouldCallbase = callbase };

        var builder = new AutoMockDependenciesBuilder(Mock.Of<ISpecimenBuilder>(), fixture.AutoMockHelpers);
        var contextMock = new Mock<ISpecimenContext>();
        contextMock.Setup(c => c.Resolve(It.IsAny<AutoMockRequest>())).Returns(AutoMock.Of<object>());

        builder.Create(request, contextMock.Object);

        contextMock.Verify(c => c.Resolve(It.Is<AutoMockRequest>(r => r.MockShouldCallbase == callbase)));
        contextMock.VerifyNoOtherCalls();
    }

    [Test]
    public void Test_Create_ReturnsNoSpecimen_WhenRequestIsAutoMock_AndResultIsNotAutoMock()
    {
        var fixture = new AbstractAutoMockFixture();

        var innerType = typeof(AutoMockDependenciesBuilder_Tests);
        var request = new AutoMockDependenciesRequest(fixture.AutoMockHelpers.GetAutoMockType(innerType), fixture);

        var expectedResult = new object();

        var builder = new AutoMockDependenciesBuilder(Mock.Of<ISpecimenBuilder>(), fixture.AutoMockHelpers);
        var contextMock = new Mock<ISpecimenContext>();
        contextMock.Setup(c => c.Resolve(It.IsAny<AutoMockRequest>())).Returns(expectedResult);

        var result = builder.Create(request, contextMock.Object);
        result.Should().BeOfType<NoSpecimen>();

        contextMock.Verify(c => c.Resolve(It.IsAny<AutoMockRequest>()));
        contextMock.VerifyNoOtherCalls();
    }

    [Test]
    public void Test_Create_ReturnsNoSpecimen_WhenRequestIsAutoMock_AndHasAutoMockOnSameLevel()
    {
        var fixture = new AbstractAutoMockFixture();

        var innerType = typeof(AutoMockDependenciesBuilder_Tests);
        var topRequest = new AutoMockRequest(innerType, new AbstractAutoMockFixture());
        var request = new AutoMockDependenciesRequest(fixture.AutoMockHelpers.GetAutoMockType(innerType), topRequest);

        var builder = new AutoMockDependenciesBuilder(Mock.Of<ISpecimenBuilder>(), fixture.AutoMockHelpers);
        var contextMock = new Mock<ISpecimenContext>();

        var result = builder.Create(request, contextMock.Object);
        result.Should().BeOfType<NoSpecimen>();

        contextMock.VerifyNoOtherCalls();
    }

    [Test]
    public void Test_Create_CreatesAutoMockRequest_WhenRequestIsAutoMock_AndHasAutoMockOnAnotherLevel()
    {
        var fixture = new AbstractAutoMockFixture();

        var innerType = typeof(AutoMockDependenciesBuilder_Tests);
        var topRequest = new AutoMockRequest(innerType, new AbstractAutoMockFixture());
        var propertyRequest = new PropertyRequest(fixture.AutoMockHelpers.GetAutoMockType(innerType), Mock.Of<PropertyInfo>(), topRequest);

        var requestMock = new Mock<AutoMockDependenciesRequest>(fixture.AutoMockHelpers.GetAutoMockType(innerType), propertyRequest) { CallBase = true };
        var expectedResult = new AutoMock<object>();

        var builder = new AutoMockDependenciesBuilder(Mock.Of<ISpecimenBuilder>(), fixture.AutoMockHelpers);
        var contextMock = new Mock<ISpecimenContext>();
        contextMock.Setup(c => c.Resolve(It.IsAny<AutoMockRequest>())).Returns(expectedResult.Object);

        var result = builder.Create(requestMock.Object, contextMock.Object);
        result.Should().Be(expectedResult);

        contextMock.Verify(c => c.Resolve(It.Is<AutoMockRequest>(r => r.Request == innerType
                                                            && r.Parent == requestMock.Object)));
        contextMock.VerifyNoOtherCalls();

        requestMock.Verify(r => r.SetResult(expectedResult, builder));
    }

    public interface TestInterface { }
    public abstract class TestAbstract { }
    [Test]
    [TestCase(typeof(TestInterface))]
    [TestCase(typeof(TestAbstract))]
    public void Test_Create_CreatesAutoMockRequest_WhenRequestIsInterfaceAbstract(Type type)
    {
        var fixture = new AbstractAutoMockFixture();

        var requestMock = new Mock<AutoMockDependenciesRequest>(type, fixture) { CallBase = true };
        var expectedResult = new AutoMock<object>();

        var builder = new AutoMockDependenciesBuilder(Mock.Of<ISpecimenBuilder>(), fixture.AutoMockHelpers);
        var contextMock = new Mock<ISpecimenContext>();
        contextMock.Setup(c => c.Resolve(It.IsAny<AutoMockRequest>())).Returns(expectedResult.Object);

        var result = builder.Create(requestMock.Object, contextMock.Object);
        result.Should().Be(expectedResult.Object);

        contextMock.Verify(c => c.Resolve(It.Is<AutoMockRequest>(r => r.Request == type && r.Parent == requestMock.Object)));
        contextMock.VerifyNoOtherCalls();

        requestMock.Verify(r => r.SetResult(expectedResult.Object, builder));
    }

    [Test]
    [TestCase(typeof(TestInterface))]
    [TestCase(typeof(TestAbstract))]
    public void Test_Create_AsksForCallBase_WhenRequestIsInterfaceAbstract_EvenIfCallBaseIsFalse(Type type)
    {
        var fixture = new AbstractAutoMockFixture();

        var request = new AutoMockDependenciesRequest(type, fixture) { MockShouldCallbase = false, };

        var builder = new AutoMockDependenciesBuilder(Mock.Of<ISpecimenBuilder>(), fixture.AutoMockHelpers);
        var contextMock = new Mock<ISpecimenContext>();
        contextMock.Setup(c => c.Resolve(It.IsAny<AutoMockRequest>())).Returns(AutoMock.Of<object>());

        builder.Create(request, contextMock.Object);

        contextMock.Verify(c => c.Resolve(It.Is<AutoMockRequest>(r => r.MockShouldCallbase == true)));
        contextMock.VerifyNoOtherCalls();
    }

    [Test]
    [TestCase(typeof(TestInterface))]
    [TestCase(typeof(TestAbstract))]
    public void Test_Create_ReturnsResult_WhenRequestIsInterfaceAbstarct_AndResultIsNotAutoMock(Type type)
    {
        var fixture = new AbstractAutoMockFixture();

        var requestMock = new Mock<AutoMockDependenciesRequest>(type, fixture) { CallBase = true };
        var expectedResult = new object();

        var builder = new AutoMockDependenciesBuilder(Mock.Of<ISpecimenBuilder>(), fixture.AutoMockHelpers);
        var contextMock = new Mock<ISpecimenContext>();
        contextMock.Setup(c => c.Resolve(It.IsAny<AutoMockRequest>())).Returns(expectedResult);

        var result = builder.Create(requestMock.Object, contextMock.Object);
        result.Should().Be(expectedResult);

        contextMock.Verify(c => c.Resolve(It.IsAny<AutoMockRequest>()));
        contextMock.VerifyNoOtherCalls();

        requestMock.Verify(r => r.SetResult(expectedResult, builder));
    }

    [Test]
    [TestCase(typeof(TestInterface))]
    [TestCase(typeof(TestAbstract))]
    public void Test_Create_ReturnsNoSpecimen_WhenRequestIsInterfaceAbstarct_AndHasAutoMockOnSameLevel(Type type)
    {
        var fixture = new AbstractAutoMockFixture();

        var topRequest = new AutoMockRequest(type, fixture);
        var request = new AutoMockDependenciesRequest(type, topRequest);

        var builder = new AutoMockDependenciesBuilder(Mock.Of<ISpecimenBuilder>(), fixture.AutoMockHelpers);
        var contextMock = new Mock<ISpecimenContext>();

        var result = builder.Create(request, contextMock.Object);
        result.Should().BeOfType<NoSpecimen>();

        contextMock.VerifyNoOtherCalls();
    }

    [Test]
    [TestCase(typeof(TestInterface))]
    [TestCase(typeof(TestAbstract))]
    public void Test_Create_CreatesAutoMockRequest_WhenRequestIsInterfaceAbstarct_AndHasAutoMockOnAnotherLevel(Type type)
    {
        var fixture = new AbstractAutoMockFixture();

        var topRequest = new AutoMockRequest(type, fixture);
        var propertyRequest = new PropertyRequest(type, Mock.Of<PropertyInfo>(),
                                                                        topRequest);

        var requestMock = new Mock<AutoMockDependenciesRequest>(type, propertyRequest) { CallBase = true };
        var expectedResult = new AutoMock<object>();

        var builder = new AutoMockDependenciesBuilder(Mock.Of<ISpecimenBuilder>(), fixture.AutoMockHelpers);
        var contextMock = new Mock<ISpecimenContext>();
        contextMock.Setup(c => c.Resolve(It.IsAny<AutoMockRequest>())).Returns(expectedResult.Object);

        var result = builder.Create(requestMock.Object, contextMock.Object);
        result.Should().Be(expectedResult.Object);

        contextMock.Verify(c => c.Resolve(It.Is<AutoMockRequest>(r => r.Request == type
                                                            && r.Parent == requestMock.Object)));
        contextMock.VerifyNoOtherCalls();

        requestMock.Verify(r => r.SetResult(expectedResult.Object, builder));
    }
}
