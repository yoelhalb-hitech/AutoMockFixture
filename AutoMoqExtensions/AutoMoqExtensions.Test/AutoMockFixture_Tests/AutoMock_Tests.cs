using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.Test.AutoMockFixture_Tests
{
    internal class AutoMock_Tests
    {
        [Test]
        public void Test_AutoMock_Abstract()
        {
            // Arrange
            var fixture = new AutoMockFixture();
            // Act
            var obj = fixture.Create<AutoMock<InternalTestClass2>>();
            // Assert
            obj.Should().NotBeNull();
            obj.Should().BeOfType<AutoMock<InternalTestClass2>>();
            obj.GetMocked().InternalTest.Should().NotBeNull();
        }

        [Test]
        public void Test_AutoMock()
        {
            // Arrange
            var fixture = new AutoMockFixture();
            // Act
            var obj = fixture.Create<AutoMock<AutoMockTestClass>>();
            // Assert
            obj.Should().NotBeNull();
            obj.Should().BeOfType<AutoMock<AutoMockTestClass>>();

            var inner = (AutoMockTestClass)obj;

            inner.TestCtorArg.Should().NotBeNull();
            inner.TestCtorArg.InternalTest.Should().NotBeNull();
            AutoMockUtils.AutoMockHelpers.GetAutoMock(inner.TestCtorArg).Should().NotBeNull();

            inner.TestClassProp.Should().NotBeNull();
            inner.TestClassProp!.InternalTest.Should().NotBeNull();
            inner.TestClassProp!.Should().NotBe(inner.TestCtorArg);
            AutoMockUtils.AutoMockHelpers.GetAutoMock(inner.TestClassProp).Should().NotBeNull();

            inner.TestClassPropGet.Should().NotBeNull();
            inner.TestClassPropGet!.InternalTest.Should().NotBeNull();
            inner.TestClassPropGet!.TestMethod().Should().NotBeNull();
            inner.TestClassPropGet!.TestMethod().Should().NotBe("67");
            var result = inner.TestClassPropGet!.TestOutParam(out var s);
            s.Should().NotBeNull();
            result.Should().NotBeNull();
            s.Should().NotBe(result); // Unlike in the original code...
            // TODO... so far we have an issue with these
            //var result1 = inner.TestClassPropGet!.TestOutParam1(out var s1);
            //s1.Should().NotBeNull();
            //result1.Should().NotBeNull();
            //s1.Should().NotBe(result1); // Unlike in the original code...

            inner.TestClassPropGet!.Should().NotBe(inner.TestCtorArg);
            inner.TestClassPropGet!.Should().NotBe(inner.TestClassProp);
            AutoMockUtils.AutoMockHelpers.GetAutoMock(inner.TestClassPropGet).Should().NotBeNull();

            inner.TestClassField.Should().NotBeNull();
            inner.TestClassField!.InternalTest.Should().NotBeNull();
            inner.TestClassField!.Should().NotBe(inner.TestCtorArg);
            inner.TestClassField!.Should().NotBe(inner.TestClassProp);
            inner.TestClassField!.Should().NotBe(inner.TestClassPropGet);
            AutoMockUtils.AutoMockHelpers.GetAutoMock(inner.TestClassField).Should().NotBeNull();
        }

        [Test]
        public void Test_CreateAutoMock_NonGeneric()
        {
            // Arrange
            var fixture = new AutoMockFixture();
            // Act
            InternalTestClass obj = (InternalTestClass)fixture.CreateAutoMock(typeof(InternalTestClass));

            // Assert
            obj.Should().NotBeNull();
            AutoMockUtils.AutoMockHelpers.GetAutoMock(obj).Should().NotBeNull();
            obj.Should().NotBeNull();
        }

        [Test]
        public void Test_CreateAutoMock_NoCtorParams()
        {
            // Arrange
            var fixture = new AutoMockFixture();
            // Act
            var obj = fixture.CreateAutoMock<InternalTestClass>();

            // Assert
            obj.Should().NotBeNull();
            AutoMockUtils.AutoMockHelpers.GetAutoMock(obj).Should().NotBeNull();
            obj.Should().NotBeNull();
        }
        [Test]
        public void Test_CreateAutoMock_WithCtorParams()
        {
            // Arrange
            var fixture = new AutoMockFixture();
            // Act
            var obj = fixture.CreateAutoMock<AutoMockTestClass>();
            // Assert
            obj.Should().NotBeNull();
            obj.Should().BeAssignableTo<AutoMockTestClass>();

            var inner = (AutoMockTestClass)obj;

            inner.TestCtorArg.Should().NotBeNull();
            inner.TestCtorArg!.InternalTest.Should().NotBeNull();
            AutoMockUtils.AutoMockHelpers.GetAutoMock(inner.TestCtorArg).Should().NotBeNull();

            inner.TestClassProp.Should().NotBeNull();
            inner.TestClassProp!.InternalTest.Should().NotBeNull();
            inner.TestClassProp!.Should().NotBe(inner.TestCtorArg);
            AutoMockUtils.AutoMockHelpers.GetAutoMock(inner.TestClassProp).Should().NotBeNull();

            inner.TestClassPropGet.Should().NotBeNull();
            inner.TestClassPropGet!.InternalTest.Should().NotBeNull();
            inner.TestClassPropGet!.Should().NotBe(inner.TestCtorArg);
            inner.TestClassPropGet!.Should().NotBe(inner.TestClassProp);
            AutoMockUtils.AutoMockHelpers.GetAutoMock(inner.TestClassPropGet).Should().NotBeNull();

            inner.TestClassField.Should().NotBeNull();
            inner.TestClassField!.InternalTest.Should().NotBeNull();
            inner.TestClassField!.Should().NotBe(inner.TestCtorArg);
            inner.TestClassField!.Should().NotBe(inner.TestClassProp);
            inner.TestClassField!.Should().NotBe(inner.TestClassPropGet);
            AutoMockUtils.AutoMockHelpers.GetAutoMock(inner.TestClassField).Should().NotBeNull();
        }
    }
}
