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
            var fixture = new Fixture();
            fixture.Customize(new AutoMoqCustomization());

            var autoMock = fixture.Create<AutoMock<Fixture>>();

            var requestMock = fixture.Create<Mock<AutoMockDirectRequest>>();
            requestMock.CallBase = false; // Needed because AutoFixture sets it to true
            requestMock.SetupGet(x => x.Request).Returns(autoMock.GetType());
            requestMock.Setup(m => m.Equals(It.IsAny<object>())).CallBase(); // We need to do it for .Be() to work

            var context = fixture.Create<Mock<ISpecimenContext>>();
            var builder = fixture.Create<Mock<ISpecimenBuilder>>();

            var request = requestMock.Object;
            builder.Setup(b => b.Create(request, context.Object)).Returns(autoMock);

            var obj = new AutoMockPostprocessor(builder.Object);
            obj.Create(request, context.Object);
            
            autoMock.Tracker.Should().Be(request);
        }

        [Test]
        public void Test_SetsResult()
        {
            var fixture = new Fixture();
            fixture.Customize(new AutoMoqCustomization());

            var autoMock = fixture.Create<AutoMock<Fixture>>();

            var requestMock = fixture.Create<Mock<AutoMockDirectRequest>>();
            requestMock.CallBase = false; // Needed because AutoFixture sets it to true
            requestMock.SetupGet(x => x.Request).Returns(autoMock.GetType());
            requestMock.Setup(m => m.Equals(It.IsAny<object>())).CallBase(); // We need to do it for .Be() to work

            var context = fixture.Create<Mock<ISpecimenContext>>();
            var builder = fixture.Create<Mock<ISpecimenBuilder>>();

            var request = requestMock.Object;
            builder.Setup(b => b.Create(request, context.Object)).Returns(autoMock);

            var obj = new AutoMockPostprocessor(builder.Object);
            obj.Create(request, context.Object);
          
            requestMock.Verify(m => m.SetResult(autoMock));
        }
    }
}
