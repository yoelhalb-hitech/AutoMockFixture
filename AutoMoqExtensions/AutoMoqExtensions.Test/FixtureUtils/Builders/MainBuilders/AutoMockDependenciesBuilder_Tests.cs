using AutoMoqExtensions.AutoMockUtils;
using AutoMoqExtensions.FixtureUtils;
using AutoMoqExtensions.FixtureUtils.Builders.MainBuilders;
using AutoMoqExtensions.FixtureUtils.Requests.HelperRequests.NonAutoMock;
using AutoMoqExtensions.FixtureUtils.Requests.MainRequests;
using AutoMoqExtensions.Test.AutoMockFixture_Tests;
using Moq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace AutoMoqExtensions.Test.FixtureUtils.Builders.MainBuilders
{
    internal class AutoMockDependenciesBuilder_Tests
    {
        [Test]
        public void Test_Create_CreatesAutoMockRequest_WhenRequestIsAutoMock()
        {
            var innerType = typeof(AutoMockDependenciesBuilder_Tests);
            var requestMock = new Mock<AutoMockDependenciesRequest>(AutoMockHelpers.GetAutoMockType(innerType),
                                  new AbstractAutoMockFixture()){ CallBase = true };
            var expectedResult = new AutoMock<object>();

            var builder = new AutoMockDependenciesBuilder(Mock.Of<ISpecimenBuilder>());
            var contextMock = new Mock<ISpecimenContext>();
            contextMock.Setup(c => c.Resolve(It.IsAny<AutoMockRequest>())).Returns(expectedResult.Object);

            var result = builder.Create(requestMock.Object, contextMock.Object);
            result.Should().Be(expectedResult);

            contextMock.Verify(c => c.Resolve(It.Is<AutoMockRequest>(r => r.Request == innerType 
                                                                && r.Parent == requestMock.Object)));
            contextMock.VerifyNoOtherCalls();

            requestMock.Verify(r => r.SetResult(expectedResult));
        }

        [Test]
        public void Test_Create_AsksForCallBase_WhenRequestIsAutoMock_EvenIfCallBaseIsFalse()
        {
            var innerType = typeof(AutoMockDependenciesBuilder_Tests);
            var request = new AutoMockDependenciesRequest(AutoMockHelpers.GetAutoMockType(innerType),
                                    new AbstractAutoMockFixture()) { MockShouldCallbase = false };

            var builder = new AutoMockDependenciesBuilder(Mock.Of<ISpecimenBuilder>());
            var contextMock = new Mock<ISpecimenContext>();
            contextMock.Setup(c => c.Resolve(It.IsAny<AutoMockRequest>())).Returns(AutoMock.Of<object>());

            builder.Create(request, contextMock.Object);

            contextMock.Verify(c => c.Resolve(It.Is<AutoMockRequest>(r => r.MockShouldCallbase == true)));
            contextMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Test_Create_ReturnsNoSpecimen_WhenRequestIsAutoMock_AndResultIsNotAutoMock()
        {
            var innerType = typeof(AutoMockDependenciesBuilder_Tests);
            var request = new AutoMockDependenciesRequest(AutoMockHelpers.GetAutoMockType(innerType),
                                                                            new AbstractAutoMockFixture());

            var expectedResult = new object();

            var builder = new AutoMockDependenciesBuilder(Mock.Of<ISpecimenBuilder>());
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
            var innerType = typeof(AutoMockDependenciesBuilder_Tests);
            var topRequest = new AutoMockRequest(innerType, new AbstractAutoMockFixture());
            var request = new AutoMockDependenciesRequest(AutoMockHelpers.GetAutoMockType(innerType),
                                                                            topRequest);

            var builder = new AutoMockDependenciesBuilder(Mock.Of<ISpecimenBuilder>());
            var contextMock = new Mock<ISpecimenContext>();

            var result = builder.Create(request, contextMock.Object);
            result.Should().BeOfType<NoSpecimen>();

            contextMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Test_Create_CreatesAutoMockRequest_WhenRequestIsAutoMock_AndHasAutoMockOnAnotherLevel()
        {
            var innerType = typeof(AutoMockDependenciesBuilder_Tests);
            var topRequest = new AutoMockRequest(innerType, new AbstractAutoMockFixture());
            var propertyRequest = new PropertyRequest(AutoMockHelpers.GetAutoMockType(innerType), Mock.Of<PropertyInfo>(),
                                                                            topRequest);

            var requestMock = new Mock<AutoMockDependenciesRequest>(AutoMockHelpers.GetAutoMockType(innerType),
                                        propertyRequest) { CallBase = true };
            var expectedResult = new AutoMock<object>();

            var builder = new AutoMockDependenciesBuilder(Mock.Of<ISpecimenBuilder>());
            var contextMock = new Mock<ISpecimenContext>();
            contextMock.Setup(c => c.Resolve(It.IsAny<AutoMockRequest>())).Returns(expectedResult.Object);

            var result = builder.Create(requestMock.Object, contextMock.Object);
            result.Should().Be(expectedResult);

            contextMock.Verify(c => c.Resolve(It.Is<AutoMockRequest>(r => r.Request == innerType
                                                                && r.Parent == requestMock.Object)));
            contextMock.VerifyNoOtherCalls();

            requestMock.Verify(r => r.SetResult(expectedResult));
        }

        public interface TestInterface { }
        public abstract class TestAbstract { }
        [Test]
        [TestCase(typeof(TestInterface))]
        [TestCase(typeof(TestAbstract))]
        public void Test_Create_CreatesAutoMockRequest_WhenRequestIsInterfaceAbstract(Type type)
        {
            var requestMock = new Mock<AutoMockDependenciesRequest>(type,
                                                                            new AbstractAutoMockFixture())
            {
                CallBase = true
            };
            var expectedResult = new AutoMock<object>();

            var builder = new AutoMockDependenciesBuilder(Mock.Of<ISpecimenBuilder>());
            var contextMock = new Mock<ISpecimenContext>();
            contextMock.Setup(c => c.Resolve(It.IsAny<AutoMockRequest>())).Returns(expectedResult.Object);

            var result = builder.Create(requestMock.Object, contextMock.Object);
            result.Should().Be(expectedResult.Object);

            contextMock.Verify(c => c.Resolve(It.Is<AutoMockRequest>(r => r.Request == type
                                                                && r.Parent == requestMock.Object)));
            contextMock.VerifyNoOtherCalls();

            requestMock.Verify(r => r.SetResult(expectedResult.Object));
        }

        [Test]
        [TestCase(typeof(TestInterface))]
        [TestCase(typeof(TestAbstract))]
        public void Test_Create_AsksForCallBase_WhenRequestIsInterfaceAbstract_EvenIfCallBaseIsFalse(Type type)
        {
            var request = new AutoMockDependenciesRequest(type, new AbstractAutoMockFixture())
            {
                MockShouldCallbase = false,
            };

            var builder = new AutoMockDependenciesBuilder(Mock.Of<ISpecimenBuilder>());
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
            var requestMock = new Mock<AutoMockDependenciesRequest>(type,
                                     new AbstractAutoMockFixture()) { CallBase = true };
            var expectedResult = new object();

            var builder = new AutoMockDependenciesBuilder(Mock.Of<ISpecimenBuilder>());
            var contextMock = new Mock<ISpecimenContext>();
            contextMock.Setup(c => c.Resolve(It.IsAny<AutoMockRequest>())).Returns(expectedResult);

            var result = builder.Create(requestMock.Object, contextMock.Object);
            result.Should().Be(expectedResult);

            contextMock.Verify(c => c.Resolve(It.IsAny<AutoMockRequest>()));
            contextMock.VerifyNoOtherCalls();

            requestMock.Verify(r => r.SetResult(expectedResult));
        }

        [Test]
        [TestCase(typeof(TestInterface))]
        [TestCase(typeof(TestAbstract))]
        public void Test_Create_ReturnsNoSpecimen_WhenRequestIsInterfaceAbstarct_AndHasAutoMockOnSameLevel(Type type)
        {
            var topRequest = new AutoMockRequest(type, new AbstractAutoMockFixture());
            var request = new AutoMockDependenciesRequest(type, topRequest);

            var builder = new AutoMockDependenciesBuilder(Mock.Of<ISpecimenBuilder>());
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
            var topRequest = new AutoMockRequest(type, new AbstractAutoMockFixture());
            var propertyRequest = new PropertyRequest(type, Mock.Of<PropertyInfo>(),
                                                                            topRequest);

            var requestMock = new Mock<AutoMockDependenciesRequest>(type, propertyRequest) { CallBase = true };
            var expectedResult = new AutoMock<object>();

            var builder = new AutoMockDependenciesBuilder(Mock.Of<ISpecimenBuilder>());
            var contextMock = new Mock<ISpecimenContext>();
            contextMock.Setup(c => c.Resolve(It.IsAny<AutoMockRequest>())).Returns(expectedResult.Object);

            var result = builder.Create(requestMock.Object, contextMock.Object);
            result.Should().Be(expectedResult.Object);

            contextMock.Verify(c => c.Resolve(It.Is<AutoMockRequest>(r => r.Request == type
                                                                && r.Parent == requestMock.Object)));
            contextMock.VerifyNoOtherCalls();

            requestMock.Verify(r => r.SetResult(expectedResult.Object));
        }
    }
}
