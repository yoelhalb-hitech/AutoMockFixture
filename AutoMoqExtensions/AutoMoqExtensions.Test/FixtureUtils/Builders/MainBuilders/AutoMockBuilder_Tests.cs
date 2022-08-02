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
        public class Test { }
        [Test]
        public void Test_SetsTracker()
        {
            var autoMock = new AutoMock<Test>();

            var request = new AutoMockDirectRequest(autoMock.GetType(), new AutoMockFixture());           

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
            var autoMock = new AutoMock<Test>();

            var type = autoMock.GetType();
            var fixture = new AutoMockFixture();

            var requestMock = new Mock<AutoMockDirectRequest>(type, fixture);
            requestMock.CallBase = true;
            requestMock.SetupGet(r => r.Request).Returns(type);
            requestMock.SetupGet(r => r.Fixture).Returns(fixture);

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
