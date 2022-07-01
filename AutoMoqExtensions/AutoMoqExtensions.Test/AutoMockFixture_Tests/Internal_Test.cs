using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.Test.AutoMockFixture_Tests
{
    internal class Internal_Test
    {
        [Test]
        public void Test_SetInternalProperties()
        {
            // Arrange
            var fixture = new AutoMockFixture();
            // Act
            var obj = fixture.Create<InternalTestClass>();
            // Assert
            obj.Should().NotBeNull();
            obj.Should().BeOfType<InternalTestClass>();

            obj.InternalTest.Should().NotBeNull();
        }

        [Test]
        public void Test_SetInternalFields()
        {
            // Arrange
            var fixture = new AutoMockFixture();
            // Act
            var obj = fixture.Create<InternalTestFields>();
            // Assert
            obj.Should().NotBeNull();
            obj.Should().BeOfType<InternalTestClass>();

            obj.InternalTest.Should().NotBeNull();
        }

        [Test]
        public void Test_SetsUpInternalMethods_ForAutomock()
        {
            // Arrange
            var fixture = new AutoMockFixture();
            // Act
            var obj = fixture.Create<InternalTestMethods>();
            // Assert
            obj.Should().NotBeNull();
            obj.Should().BeOfType<InternalTestClass>();

            obj.InternalTestMethod().Should().NotBeNull();
        }
    }
}
