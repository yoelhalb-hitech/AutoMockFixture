using AutoMoqExtensions.FixtureUtils.Builders.SpecialBuilders;
using AutoMoqExtensions.FixtureUtils.Requests.HelperRequests.NonAutoMock;
using AutoMoqExtensions.FixtureUtils.Requests.MainRequests;
using Moq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.Test.FixtureUtils.Builders.SpecialBuilders
{
    internal class LastResortBuilder_Tests
    {
        [Test]
        [UnitAutoData]
        public void Test_When_AutoMock_Creates_AutoMockDependencies([AutoMock] ISpecimenContext context)
        {            
            var request = new AutoMockRequest(typeof(LastResortBuilder_Tests), new AbstractAutoMockFixture());

            var builder = new LastResortBuilder();
            builder.Create(request, context);

            Mock.Get(context).Verify(c => 
                c.Resolve(It.Is<AutoMockDependenciesRequest>(r => r.Request == request.Request && r.Parent == request)),
                    Times.Once());
            Mock.Get(context).VerifyNoOtherCalls();
        }

        [Test]
        [UnitAutoData]
        public void Test_When_AutoMockDependencies_Creates_NonAutoMock([AutoMock] ISpecimenContext context)
        {
            var request = new AutoMockDependenciesRequest(typeof(LastResortBuilder_Tests), new AbstractAutoMockFixture());

            var builder = new LastResortBuilder();
            builder.Create(request, context);

            Mock.Get(context).Verify(c => c.Resolve(It.Is<NonAutoMockRequest>(r => r.Request == request.Request && r.Parent == request)), Times.Once());
            Mock.Get(context).VerifyNoOtherCalls();
        }

        [Test]
        [UnitAutoData]
        public void Test_When_RequestWithType_SendsRequestType([AutoMock] ISpecimenContext context)
        {
            var request = new NonAutoMockRequest(typeof(LastResortBuilder_Tests), new AbstractAutoMockFixture());

            var builder = new LastResortBuilder();
            builder.Create(request, context);

            Mock.Get(context).Verify(c => c.Resolve(It.Is<Type>(r => r == request.Request)), Times.Once());
            Mock.Get(context).VerifyNoOtherCalls();
        }

        [Test]
        [UnitAutoData]
        public void Test_When_NotRequestWithType_SendsRequest([AutoMock] ISpecimenContext context)
        {
            var request = new object();

            var builder = new LastResortBuilder();
            builder.Create(request, context);

            Mock.Get(context).Verify(c => c.Resolve(It.Is<object>(r => r == request)), Times.Once());
            Mock.Get(context).VerifyNoOtherCalls();
        }

        [Test]
        [UnitAutoData]
        public void Test_When_AutoMockDependencies_AndRecursion_SendsRequestType([AutoMock] ISpecimenContext context)
        {
            var nonAutoMock = new NonAutoMockRequest(typeof(LastResortBuilder_Tests), new AbstractAutoMockFixture());
            var request = new AutoMockDependenciesRequest(typeof(LastResortBuilder_Tests), nonAutoMock);

            var builder = new LastResortBuilder();
            builder.Create(request, context);

            Mock.Get(context).Verify(c => c.Resolve(It.Is<Type>(r => r == request.Request)), Times.Once());
            Mock.Get(context).VerifyNoOtherCalls();
        }

        [Test]
        [UnitAutoData]
        public void Test_When_AutoMockDependencies_AndRecursionOnDifferentLevel_Creates_NonAutoMock([AutoMock] ISpecimenContext context)
        {
            var nonAutoMock = new NonAutoMockRequest(typeof(LastResortBuilder_Tests), new AbstractAutoMockFixture());
            var propertyRequest = new PropertyRequest(typeof(LastResortBuilder_Tests), Mock.Of<PropertyInfo>(), nonAutoMock);
            var request = new AutoMockDependenciesRequest(typeof(LastResortBuilder_Tests), propertyRequest);

            var builder = new LastResortBuilder();
            builder.Create(request, context);

            Mock.Get(context).Verify(c => c.Resolve(It.Is<NonAutoMockRequest>(r => r.Request == request.Request && r.Parent == request)), Times.Once());
            Mock.Get(context).VerifyNoOtherCalls();
        }

        [Test]
        [UnitAutoData]
        public void Test_Throws_When_ResultIsSame([AutoMock] ISpecimenContext context)
        {           
            var request = new object();
            Mock.Get(context).Setup(c => c.Resolve(It.IsAny<object>())).Returns(request);

            var builder = new LastResortBuilder();
            Assert.Throws<Exception>(() => builder.Create(request, context));   
        }

        [Test]
        [UnitAutoData]
        public void Test_Returns_When_ResultIsSame_AndRequestIsType([AutoMock] ISpecimenContext context)
        {
            var request = typeof(Type);
            Mock.Get(context).Setup(c => c.Resolve(It.IsAny<Type>())).Returns(request);

            var builder = new LastResortBuilder();
            object? result = default!;

            Assert.DoesNotThrow(() => result = builder.Create(request, context));
            result.Should().Be(request);
        }
    }
}
