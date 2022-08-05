using AutoMoqExtensions.AutoMockUtils;

namespace AutoMoqExtensions.Test.AutoMockFixture_Tests;

internal class CreateAutoMock_NonCallbase_Tests
{
    [Test]
    public void Test_AutoMock_WithNonCallBase_IsNot_CallBase()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();

        // Act
        var obj = fixture.CreateAutoMock<InternalSimpleTestClass>(false);
        var mock = AutoMockHelpers.GetFromObj(obj);

        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<InternalSimpleTestClass>();

        mock.Should().NotBeNull();
        mock.Should().BeOfType<AutoMock<InternalSimpleTestClass>>();
        mock!.CallBase.Should().BeFalse();
    }

    [Test]
    public void Test_AutoMock_Abstract_WithNonCallBase_IsNot_CallBase()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateAutoMock<InternalAbstractMethodTestClass>(false);
        var mock = AutoMockHelpers.GetFromObj(obj);
        
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<InternalAbstractMethodTestClass>();

        mock.Should().NotBeNull();
        mock.Should().BeOfType<AutoMock<InternalAbstractMethodTestClass>>();
        mock!.CallBase.Should().BeFalse();
    }

    [Test]
    public void Test_AutoMock_Interface_WithNonCallBase_IsNot_CallBase()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateAutoMock<InternalReadOnlyTestInterface>(false);
        var mock = AutoMockHelpers.GetFromObj(obj);

        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<InternalReadOnlyTestInterface>();

        mock.Should().NotBeNull();
        mock.Should().BeOfType<AutoMock<InternalReadOnlyTestInterface>>();
        mock!.CallBase.Should().BeFalse();
    }

    [Test]
    public void Test_AutoMock_WithNonCallBase_DoesNot_CallCtor()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateAutoMock<WithCtorArgsTestClass>(false);
        var mock = AutoMockHelpers.GetFromObj(obj);

        // Assert

        // We need first to verify that it is an AutoMock, otherwise it is no biggy...
        mock.Should().NotBeNull();
        mock.Should().BeOfType<AutoMock<WithCtorArgsTestClass>>();

        obj.TestCtorArg.Should().BeNull();
    }

    [Test]
    public void Test_AutoMock_WithNonCallBase_DoesNotCall_BaseDefaultCtor()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateAutoMock<WithCtorNoArgsTestClass>(false);

        // Assert
        obj.TestCtor.Should().NotBe(25);
    }

    [Test]
    public void Test_AutoMock_WithNonCallBase_Setsup_ReadOnlyProps()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateAutoMock<WithCtorArgsTestClass>(false);
        var mock = AutoMockHelpers.GetFromObj(obj);

        // Assert
        obj.TestClassPropGet.Should().NotBeNull();
    }

    [Test]
    public void Test_AutoMock_WithNonCallBase_SetsUp_ReadOnlyProps_WhenInterface()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateAutoMock<InternalReadOnlyTestInterface>(false);            

        // Assert
        obj.InternalTest.Should().NotBeNull();
    }

    [Test]
    public void Test_AutoMock_WithNonCallBase_SetsUp_ReadOnlyProps_WhenAbstract()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateAutoMock<InternalAbstractReadonlyPropertyClass>(false);        

        // Assert
        obj.InternalTest.Should().NotBeNull();
    }

    [Test]
    public void Test_AutoMock_WithNonCallBase_SetsUp_ReadWriteProps()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateAutoMock<InternalSimpleTestClass>(false);
        var mock = AutoMockHelpers.GetFromObj(obj);

        // Assert

        // We need first to verify that it is an AutoMock, otherwise it is no biggy...
        mock.Should().NotBeNull();
        mock.Should().BeOfType<AutoMock<InternalSimpleTestClass>>();

        obj.InternalTest.Should().NotBeNull();
    }

    [Test]
    public void Test_AutoMock_WithNonCallBase_SetsUp_ReadWriteFields() // Via the AutoProperties command
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateAutoMock<InternalTestFields>(false);
        var mock = AutoMockHelpers.GetFromObj(obj);

        // Assert

        // We need first to verify that it is an AutoMock, otherwise it is no biggy...
        mock.Should().NotBeNull();
        mock.Should().BeOfType<AutoMock<InternalTestFields>>();

        obj.InternalTest.Should().NotBeNull();
    }

    [Test]
    public void Test_AutoMock_WithNonCallBase_SetsUp_NonAbstractMethods()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateAutoMock<InternalAbstractSimpleTestClass>(false);

        // Assert
        obj.NonAbstractMethod().Should().NotBeNull();
    }

    [Test]
    public void Test_AutoMock_WithNonCallBase_SetsUp_AbstractMethods()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateAutoMock<InternalTestMethods>(false);

        // Assert
        obj.InternalTestMethod().Should().NotBeNull();
    }

    [Test]
    public void Test_AutoMock_WithNonCallBase_DoesNotCallBase_NonAbstractMethods()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateAutoMock<InternalAbstractSimpleTestClass>(false);
        var mock = AutoMockHelpers.GetFromObj(obj);

        // Assert

        // We need first to verify that it is an AutoMock, otherwise it is no biggy...
        mock.Should().NotBeNull();
        mock.Should().BeOfType<AutoMock<InternalAbstractSimpleTestClass>>();

        obj.NonAbstractWithValueMethod().Should().NotBe(10);
    }
}
