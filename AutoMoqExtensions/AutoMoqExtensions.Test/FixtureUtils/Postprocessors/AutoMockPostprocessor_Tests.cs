using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using AutoMoqExtensions.FixtureUtils.Requests;
using AutoFixture.Kernel;
using FluentAssertions.Equivalency;
using FluentAssertions;
using AutoMoqExtensions.AutoMockUtils;

namespace AutoMoqExtensions.Test.FixtureUtils.Postprocessors
{
    internal class AutoMockPostprocessor_Tests
    {
        [Test]
        public void Test_SetsTracker()
        {
            var autoMock = new AutoMock<Fixture>();

            var requestMock = new Mock<AutoMockDirectRequest>(autoMock.GetType(), new AutoMockFixture());
            requestMock.SetupGet(x => x.Request).Returns(autoMock.GetType());
            requestMock.Setup(m => m.Equals(It.IsAny<object>())).CallBase(); // We need to do it for .Be() to work
            var request = requestMock.Object;

            var context = Mock.Of<ISpecimenContext>();
            var builder = new Mock<ISpecimenBuilder>();

            builder.Setup(b => b.Create(request, context)).Returns(autoMock);

            var obj = new AutoMockPostprocessor(builder.Object);
            obj.Create(request, context);
            
            autoMock.Tracker.Should().Be(request);
        }

        [Test]
        public void Test_SetsResult()
        {
            var autoMock = new AutoMock<Fixture>();

            var requestMock = new Mock<AutoMockDirectRequest>(autoMock.GetType(), new AutoMockFixture());       
            requestMock.SetupGet(x => x.Request).Returns(autoMock.GetType());
            requestMock.Setup(m => m.Equals(It.IsAny<object>())).CallBase(); // We need to do it for .Be() to work
            var request = requestMock.Object;

            var context = Mock.Of<ISpecimenContext>();
            var builder = new Mock<ISpecimenBuilder>();
            
            builder.Setup(b => b.Create(request, context)).Returns(autoMock);

            var obj = new AutoMockPostprocessor(builder.Object);
            obj.Create(request, context);
          
            requestMock.Verify(m => m.SetResult(autoMock));
        }
    }
}
