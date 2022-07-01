using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.Test.AutoMockFixture_Tests
{
    internal class DependencyInjection_Tests
    {       

        [Test]
        public void Test_CtorArguments_AutoMocked()
        {
            // Arrange
            var fixture = new AutoMockFixture();
            // Act
            var obj = fixture.Create<AutoMockTestClass>();
            // Assert
            obj.Should().NotBeNull();
            obj.Should().BeOfType<AutoMockTestClass>();
            AutoMockUtils.AutoMockHelpers.GetAutoMock(obj).Should().BeNull();

            obj.TestCtorArg.Should().NotBeNull();
            obj.TestCtorArg!.InternalTest.Should().NotBeNull();
            AutoMockUtils.AutoMockHelpers.GetAutoMock(obj.TestCtorArg).Should().NotBeNull();
        }


    }
}
