using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using AutoMoqExtensions;
using System.Reflection;

namespace AutoMoqExtensions.Test
{
    public class AutoMockFixture_Test
    {
        public class InternalTestClass
        {
            internal string? InternalTest { get; set; }
        }
        public class InternalTestClass1
        {
            internal string? InternalTest { get; set; }
        }
        public class InternalTestClass2
        {
            internal string? InternalTest { get; set; }
        }

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

        public class AutoMockTestClass
        {
            public readonly InternalTestClass TestCtorArg;// This way we will get the one that was passed
            public AutoMockTestClass(InternalTestClass testArg)
            {
                this.TestCtorArg = testArg;
            }
            public InternalTestClass1? TestClassProp { get; set; }
            public virtual InternalTestClass2? TestClassPropGet { get; }
            public InternalTestClass? TestClassField;
        }
        public class AutoMockTestClass1 : AutoMockTestClass
        {
            public AutoMockTestClass1(InternalTestClass testArg) : base(testArg)
            {
            }
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
