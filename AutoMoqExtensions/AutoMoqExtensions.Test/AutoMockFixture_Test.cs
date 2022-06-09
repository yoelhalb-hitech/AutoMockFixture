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
        [Test]
        public void Test_SetInternalProperties()
        {
            // Arrange
            var fixture = new AutoMockFixture();
            // Act
            var obj = fixture.Create<InternalTestClass>();
            // Assert
            obj.Should().NotBeNull();
        }

        public class AutoMockTestClass
        {
            public readonly InternalTestClass TestCtorArg;// This way we will get the one that was passed
            public AutoMockTestClass(InternalTestClass testArg)
            {
                this.TestCtorArg = testArg;
            }
            public InternalTestClass? TestClassProp { get; set; }
            public virtual InternalTestClass? TestClassPropGet { get; }
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
            AutoMockUtils.AutoMockHelpers.GetAutoMock(inner.TestCtorArg).Should().NotBeNull();

            inner.TestClassProp.Should().NotBeNull();
            AutoMockUtils.AutoMockHelpers.GetAutoMock(inner.TestClassProp).Should().NotBeNull();

            inner.TestClassPropGet.Should().NotBeNull();
            AutoMockUtils.AutoMockHelpers.GetAutoMock(inner.TestClassPropGet).Should().NotBeNull();

            inner.TestClassField.Should().NotBeNull();
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
            AutoMockUtils.AutoMockHelpers.GetAutoMock(obj.TestCtorArg).Should().NotBeNull();
        }
    }
}
