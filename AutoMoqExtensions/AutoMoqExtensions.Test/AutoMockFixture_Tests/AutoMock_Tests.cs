using AutoMoqExtensions.AutoMockUtils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.Test.AutoMockFixture_Tests
{
    internal class AutoMock_Tests
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
        public void Test_AutoMock_Abstract()
        {
            // Arrange
            var fixture = new AutoMockFixture();
            // Act
            var obj = fixture.Create<AutoMock<InternalAbstractMethodTestClass>>();
            // Assert
            obj.Should().NotBeNull();
            obj.Should().BeOfType<AutoMock<InternalAbstractMethodTestClass>>();
            obj.GetMocked().InternalTest.Should().NotBeNull();
        }

        [Test]
        public void Test_AutoMock_Abstract_WithCallBase()
        {
            // Arrange
            var fixture = new AutoMockFixture();
            // Act
            var obj = fixture.Create<AutoMock<InternalAbstractMethodTestClass>>();
            // Assert
            obj.Should().NotBeNull();
            obj.Should().BeOfType<AutoMock<InternalAbstractMethodTestClass>>();
            obj.GetMocked().InternalTest.Should().NotBeNull();
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

        [Test]
        public void Test_AutoMock_Array()
        {
            // Arrange
            var fixture = new AutoMockFixture();
           
            // Act
            var obj = fixture.Create<AutoMock<InternalAbstractMethodTestClass>[]>();
            // Assert
            obj.Should().NotBeNull();
            obj.Should().BeOfType<AutoMock<InternalAbstractMethodTestClass>[]>();
            obj.Length.Should().Be(3);
            obj.First().Should().BeOfType<AutoMock<InternalAbstractMethodTestClass>>();
            obj.First().GetMocked().Should().NotBeNull(); 
            obj.First().GetMocked().InternalTest.Should().NotBeNull();
        }

        [Test]
        public void Test_NonAutoMock_Tuple()
        {
            // Arrange
            var fixture = new AutoMockFixture();
           
            // Act
            var obj = fixture.Create<Tuple<AutoMock<InternalAbstractMethodTestClass>, InternalSimpleTestClass, InternalAbstractSimpleTestClass>>();
            // Assert
            obj.Should().NotBeNull();
            obj.Should().BeOfType<Tuple<AutoMock<InternalAbstractMethodTestClass>, InternalSimpleTestClass, InternalAbstractSimpleTestClass>>();
            obj.Item1.Should().BeOfType<AutoMock<InternalAbstractMethodTestClass>>();
            obj.Item1.GetMocked().Should().NotBeNull();
            obj.Item1.GetMocked().InternalTest.Should().NotBeNull();
            obj.Item2.Should().BeOfType<InternalSimpleTestClass>();
            obj.Item2.Should().NotBeNull();
            obj.Item2.InternalTest.Should().NotBeNull();
        }

        [Test]
        public void Test_AutoMock_Task()
        {
            // Arrange
            var fixture = new AutoMockFixture();
            // Act
            Assert.Throws<InvalidOperationException>(() => fixture.Create<AutoMock<Task<InternalAbstractMethodTestClass>>>());
        }

        [Test]
        public void Test_NonAutoMock_Array()
        {
            // Arrange
            var fixture = new AutoMockFixture();

            // Act
            var obj = fixture.Create<InternalSimpleTestClass[]>();
            // Assert
            obj.Should().NotBeNull();
            obj.Should().BeOfType<InternalSimpleTestClass[]>();
            obj.Length.Should().Be(3);
            obj.First().Should().BeOfType<InternalSimpleTestClass>();
        }

        [Test]
        public async Task Test_NonAutoMock_Task()
        {
            // Arrange
            var fixture = new AutoMockFixture();
            // Act
            var obj = fixture.Create<Task<InternalSimpleTestClass>>();
            // Assert
            obj.Should().NotBeNull();
            obj.Should().BeOfType<Task<InternalSimpleTestClass>>();
            var inner = await obj;
            inner.InternalTest.Should().NotBeNull();
        }

        [Test]
        public void Test_AutoMock_CallBase()
        {
            // Arrange
            var fixture = new AutoMockFixture();
            // Act
            var obj = fixture.CreateAutoMock<AutoMockTestClass>();
            // Assert
            obj.Should().NotBeNull();
            obj.Should().BeAssignableTo<AutoMockTestClass>();

            var mock = AutoMockHelpers.GetAutoMock(obj);
            mock.Should().NotBeNull();

            var inner = (AutoMockTestClass)obj;

            inner.TestCtorArg.Should().NotBeNull();
            inner.TestCtorArg.InternalTest.Should().NotBeNull();
            AutoMockUtils.AutoMockHelpers.GetAutoMock(inner.TestCtorArg).Should().NotBeNull();

            inner.TestClassProp.Should().NotBeNull();
            inner.TestClassProp!.InternalTest.Should().NotBeNull();
            inner.TestClassProp!.Should().NotBe(inner.TestCtorArg);
            AutoMockUtils.AutoMockHelpers.GetAutoMock(inner.TestClassProp).Should().NotBeNull();

            inner.TestClassPropGet.Should().BeNull(); // We do not setup so far for callabase       

            inner.TestClassField.Should().NotBeNull();
            inner.TestClassField!.InternalTest.Should().NotBeNull();
            inner.TestClassField!.Should().NotBe(inner.TestCtorArg);
            inner.TestClassField!.Should().NotBe(inner.TestClassProp);
            inner.TestClassField!.Should().NotBe(inner.TestClassPropGet);
            AutoMockUtils.AutoMockHelpers.GetAutoMock(inner.TestClassField).Should().NotBeNull();
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
            InternalAbstractSimpleTestClass obj = (InternalAbstractSimpleTestClass)fixture.CreateAutoMock(typeof(InternalAbstractSimpleTestClass));

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
            var obj = fixture.CreateAutoMock<InternalAbstractSimpleTestClass>();

            // Assert
            obj.Should().NotBeNull();
            AutoMockUtils.AutoMockHelpers.GetAutoMock(obj).Should().NotBeNull();
            obj.Should().NotBeNull();
        }
        [Test]
        public void Test_CreateAutoMock_WithCtorParams_CallBase()
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

            inner.TestClassField.Should().NotBeNull();
            inner.TestClassField!.InternalTest.Should().NotBeNull();
            inner.TestClassField!.Should().NotBe(inner.TestCtorArg);
            inner.TestClassField!.Should().NotBe(inner.TestClassProp);
            inner.TestClassField!.Should().NotBe(inner.TestClassPropGet);
            AutoMockUtils.AutoMockHelpers.GetAutoMock(inner.TestClassField).Should().NotBeNull();
        }

        [Test]
        public void Test_CreateAutoMock_WithCtorParams_NoCallBase()
        {
            // Arrange
            var fixture = new AutoMockFixture();
            // Act
            var obj = fixture.CreateAutoMock<AutoMockTestClass>(true);
            // Assert
            obj.Should().NotBeNull();
            obj.Should().BeAssignableTo<AutoMockTestClass>();

            var inner = (AutoMockTestClass)obj;

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
