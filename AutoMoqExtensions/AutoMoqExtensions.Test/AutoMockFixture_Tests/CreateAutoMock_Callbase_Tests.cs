using AutoMoqExtensions.AutoMockUtils;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.Test.AutoMockFixture_Tests
{
    internal class CreateAutoMock_Callbase_Tests
    {
        [Test]
        public void Test_AutoMock_WithCallBase_Is_CallBase()
        {
            // Arrange
            var fixture = new AutoMockFixture();

            // Act
            var obj = fixture.CreateAutoMock<InternalSimpleTestClass>(true);
            var mock = AutoMockHelpers.GetFromObj(obj);

            // Assert
            obj.Should().NotBeNull();
            obj.Should().BeAssignableTo<InternalSimpleTestClass>();

            mock.Should().NotBeNull();
            mock.Should().BeOfType<AutoMock<InternalSimpleTestClass>>();
            mock!.CallBase.Should().BeTrue();
        }

        [Test]
        public void Test_AutoMock_Abstract_WithCallBase_Is_CallBase()
        {
            // Arrange
            var fixture = new AutoMockFixture();
            // Act
            var obj = fixture.CreateAutoMock<InternalAbstractMethodTestClass>(true);
            var mock = AutoMockHelpers.GetFromObj(obj);
            
            // Assert
            obj.Should().NotBeNull();
            obj.Should().BeAssignableTo<InternalAbstractMethodTestClass>();

            mock.Should().NotBeNull();
            mock.Should().BeOfType<AutoMock<InternalAbstractMethodTestClass>>();
            mock!.CallBase.Should().BeTrue();
        }

        [Test]
        public void Test_AutoMock_Interface_WithCallBase_Is_CallBase()
        {
            // Arrange
            var fixture = new AutoMockFixture();
            // Act
            var obj = fixture.CreateAutoMock<InternalReadOnlyTestInterface>(true);
            var mock = AutoMockHelpers.GetFromObj(obj);

            // Assert
            obj.Should().NotBeNull();
            obj.Should().BeAssignableTo<InternalReadOnlyTestInterface>();

            mock.Should().NotBeNull();
            mock.Should().BeOfType<AutoMock<InternalReadOnlyTestInterface>>();
            mock!.CallBase.Should().BeTrue();
        }

        [Test]
        public void Test_AutoMock_WithCallBase_CallsCtor()
        {
            // Arrange
            var fixture = new AutoMockFixture();
            // Act
            var obj = fixture.CreateAutoMock<WithCtorArgsTestClass>(true);
            var mock = AutoMockHelpers.GetFromObj(obj);

            // Assert

            // We need first to verify that it is an AutoMock, otherwise it is no biggy...
            mock.Should().NotBeNull();
            mock.Should().BeOfType<AutoMock<WithCtorArgsTestClass>>();

            obj.TestCtorArg.Should().NotBeNull();
        }

        [Test]
        public void Test_AutoMock_WithCallBase_CallsBaseDefaultCtor()
        {
            // Arrange
            var fixture = new AutoMockFixture();
            // Act
            var obj = fixture.CreateAutoMock<WithCtorNoArgsTestClass>(true);
            var mock = AutoMockHelpers.GetFromObj(obj);

            // Assert

            // We need first to verify that it is an AutoMock, otherwise it is no biggy...
            mock.Should().NotBeNull();
            mock.Should().BeOfType<AutoMock<WithCtorNoArgsTestClass>>();

            obj.TestCtor.Should().Be(25);
        }

        [Test]
        public void Test_AutoMock_WithCallBase_DoesNotSetup_ReadOnlyProps()
        {
            // Arrange
            var fixture = new AutoMockFixture();
            // Act
            var obj = fixture.CreateAutoMock<WithCtorArgsTestClass>(true);
            var mock = AutoMockHelpers.GetFromObj(obj);

            // Assert

            // We need first to verify that it is an AutoMock, otherwise it is no biggy...
            mock.Should().NotBeNull();
            mock.Should().BeOfType<AutoMock<WithCtorArgsTestClass>>();

            obj.TestClassPropGet.Should().BeNull();
        }

        [Test]
        public void Test_AutoMock_WithCallBase_SetsUp_ReadOnlyProps_WhenInterface()
        {
            // Arrange
            var fixture = new AutoMockFixture();
            // Act
            var obj = fixture.CreateAutoMock<InternalReadOnlyTestInterface>(true);

            // Assert
            obj.InternalTest.Should().NotBeNull();
        }

        [Test]
        public void Test_AutoMock_WithCallBase_SetsUp_ReadOnlyProps_WhenAbstract()
        {
            // Arrange
            var fixture = new AutoMockFixture();
            // Act
            var obj = fixture.CreateAutoMock<InternalAbstractReadonlyPropertyClass>(true);

            // Assert
            obj.InternalTest.Should().NotBeNull();
        }

        [Test]
        public void Test_AutoMock_WithCallBase_SetsUp_ReadWriteProps() // Via the AutoProperties command
        {
            // Arrange
            var fixture = new AutoMockFixture();
            // Act
            var obj = fixture.CreateAutoMock<InternalSimpleTestClass>(true);
            var mock = AutoMockHelpers.GetFromObj(obj);

            // Assert

            // We need first to verify that it is an AutoMock, otherwise it is no biggy...
            mock.Should().NotBeNull();
            mock.Should().BeOfType<AutoMock<InternalSimpleTestClass>>();

            obj.InternalTest.Should().NotBeNull();
        }

        [Test]
        public void Test_AutoMock_WithCallBase_SetsUp_ReadWriteFields() // Via the AutoProperties command
        {
            // Arrange
            var fixture = new AutoMockFixture();
            // Act
            var obj = fixture.CreateAutoMock<InternalTestFields>(true);
            var mock = AutoMockHelpers.GetFromObj(obj);

            // Assert

            // We need first to verify that it is an AutoMock, otherwise it is no biggy...
            mock.Should().NotBeNull();
            mock.Should().BeOfType<AutoMock<InternalTestFields>>();

            obj.InternalTest.Should().NotBeNull();
        }

        [Test]
        public void Test_AutoMock_WithCallBase_DoesNotSetup_NonAbstractMethods()
        {
            // Arrange
            var fixture = new AutoMockFixture();
            // Act
            var obj = fixture.CreateAutoMock<InternalAbstractSimpleTestClass>(true);
            var mock = AutoMockHelpers.GetFromObj(obj);

            // Assert

            // We need first to verify that it is an AutoMock, otherwise it is no biggy...
            mock.Should().NotBeNull();
            mock.Should().BeOfType<AutoMock<InternalAbstractSimpleTestClass>>();

            obj.NonAbstractMethod().Should().BeNull();
        }

        [Test]
        public void Test_AutoMock_WithCallBase_SetsUp_AbstractMethods()
        {
            // Arrange
            var fixture = new AutoMockFixture();
            // Act
            var obj = fixture.CreateAutoMock<InternalTestMethods>(true);

            // Assert
            obj.InternalTestMethod().Should().NotBeNull();
        }

        [Test]
        public void Test_AutoMock_WithCallBase_CallsBase_NonAbstractMethods()
        {
            // Arrange
            var fixture = new AutoMockFixture();
            // Act
            var obj = fixture.CreateAutoMock<InternalAbstractSimpleTestClass>(true);
            var mock = AutoMockHelpers.GetFromObj(obj);

            // Assert

            // We need first to verify that it is an AutoMock, otherwise it is no biggy...
            mock.Should().NotBeNull();
            mock.Should().BeOfType<AutoMock<InternalAbstractSimpleTestClass>>();

            obj.NonAbstractWithValueMethod().Should().Be(10);
        }
    }
}
