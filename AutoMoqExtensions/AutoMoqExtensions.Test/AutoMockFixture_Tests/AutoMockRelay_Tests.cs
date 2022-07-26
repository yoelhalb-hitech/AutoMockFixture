﻿using AutoMoqExtensions.AutoMockUtils;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.Test.AutoMockFixture_Tests
{
    internal class AutoMockRelay_Tests
    {
        public void Test_AutoMockRelay_NotMessingUp_BugRepro()
        {
            var fixture = new AutoMockFixture();
            // Act
            var obj = fixture.Create<InternalSimpleTestClass>();

            obj.Should().NotBeNull();
            obj.InternalTest.Should().NotBeNullOrWhiteSpace();

            obj.InternalTest.Should().StartWith(nameof(InternalSimpleTestClass.InternalTest));

            Assert.DoesNotThrow(() => Guid.Parse(obj.InternalTest!.Replace(nameof(InternalSimpleTestClass.InternalTest), "")));

        }

        [Test]
        public void Test_NonAutoMock_Abstract_ViaRelay()
        {
            // Arrange
            var fixture = new AutoMockFixture();
            // Act
            var obj = fixture.Create<InternalAbstractMethodTestClass>();
            // Assert
            obj.Should().NotBeNull();
            obj.Should().BeAssignableTo<InternalAbstractMethodTestClass>();
            var mock = AutoMockHelpers.GetAutoMock(obj);
            mock.Should().NotBeNull();
            obj.InternalTest.Should().NotBeNull();
        }
    }
}