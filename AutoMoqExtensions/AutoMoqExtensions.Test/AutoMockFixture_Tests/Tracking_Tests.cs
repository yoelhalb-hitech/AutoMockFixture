using AutoMoqExtensions.AutoMockUtils;
using AutoMoqExtensions.MockUtils;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoMoqExtensions.Test.AutoMockFixture_Tests
{
    internal class Tracking_Tests
    {
        [Test]
        public void Test_Create_AddsToTrackerDict()
        {
            var fixture = new AutoMockFixture();
            var result = fixture.Create<SingletonUserClass>();

            fixture.TrackerDict.Should().HaveCount(1);
            fixture.TrackerDict.First().Key.Should().Be(result);
        }

        [Test]
        public void Test_Create_AddsUnderlyingMockToTrackerDict()
        {
            var fixture = new AutoMockFixture();
            var result = fixture.CreateAutoMock<SingletonUserClass>();

            fixture.TrackerDict.Should().HaveCount(1);
            fixture.TrackerDict.First().Key.Should().Be(Mock.Get(result));
        }

        [Test]
        public void Test_ListsSetupMethods()
        {
            var fixture = new AutoMockFixture();
            var result = fixture.CreateAutoMock<AutoMockTestClass>();
            var mock = AutoMockHelpers.GetAutoMock(result);

            mock!.MethodsSetup.Should().ContainKey("TestClassPropGet");
            mock!.MethodsNotSetup.Should().ContainKey("TestClassProp");
            mock!.MethodsNotSetup["TestClassProp"].Reason.Should().Be(CannotSetupMethodException.CannotSetupReason.NonVirtual);
        }

    }
}
