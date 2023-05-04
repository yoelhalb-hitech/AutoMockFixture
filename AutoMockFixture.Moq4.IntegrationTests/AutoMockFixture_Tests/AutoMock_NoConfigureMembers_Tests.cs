using AutoMockFixture.AutoMockUtils;

namespace AutoMockFixture.Tests.AutoMockFixture_Tests;

internal class AutoMock_NoConfigureMembers_Tests
{
    [Test]
    public void Test_AutoMock_NoConfigureMembers_Abstract()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture(true);
        // Act
        var obj = fixture.CreateAutoMock<AutoMock<InternalAbstractMethodTestClass>>();
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeOfType<AutoMock<InternalAbstractMethodTestClass>>();
        obj.GetMocked().InternalTest.Should().BeNull();
    }

    [Test]
    public void Test_AutoMock_NoConfigureMembers_CallBase()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture(true);
        // Act
        var obj = fixture.CreateAutoMock<WithCtorArgsTestClass>(true);
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<WithCtorArgsTestClass>();

        var mock = AutoMock.Get(obj);
        mock.Should().NotBeNull();

        var inner = (WithCtorArgsTestClass)obj;

        inner.TestCtorArg.Should().NotBeNull();
        inner.TestCtorArg.InternalTest.Should().BeNull();
        AutoMock.IsAutoMock(inner.TestCtorArg).Should().BeTrue();

        inner.TestClassProp.Should().BeNull();
        inner.TestClassProp!.Should().NotBe(inner.TestCtorArg);

        inner.TestClassPropWithPrivateSet.Should().BeNull();
        inner.TestClassPropWithProtectedSet.Should().BeNull();

        inner.TestClassPropGet.Should().BeNull();

        inner.TestClassField.Should().BeNull();
    }

    [Test]
    public void Test_AutoMock_NoConfigureMembers()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture(true);
        // Act
        var obj = fixture.CreateAutoMock<AutoMock<WithCtorArgsTestClass>>();
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeOfType<AutoMock<WithCtorArgsTestClass>>();

        var inner = (WithCtorArgsTestClass)obj;

        inner.TestClassProp.Should().BeNull();
        inner.TestClassPropWithPrivateSet.Should().BeNull();
        inner.TestClassPropWithProtectedSet.Should().BeNull();

        inner.TestClassPropGet.Should().BeNull();
        inner.TestClassField.Should().BeNull();
    }

    [Test]
    public void Test_CreateAutoMock_NoConfigureMembers_NonGeneric()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture(true);
        // Act
        InternalAbstractSimpleTestClass obj = (InternalAbstractSimpleTestClass)fixture.CreateAutoMock(typeof(InternalAbstractSimpleTestClass));

        // Assert
        obj.Should().NotBeNull();
        AutoMock.IsAutoMock(obj).Should().BeTrue();
    }

    [Test]
    public void Test_CreateAutoMock_NoConfigureMembers_NoCtorParams()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture(true);
        // Act
        var obj = fixture.CreateAutoMock<InternalAbstractSimpleTestClass>();

        // Assert
        obj.Should().NotBeNull();
        AutoMock.IsAutoMock(obj).Should().BeTrue();
    }
    [Test]
    public void Test_CreateAutoMock_NoConfigureMembers_WithCtorParams_CallBase()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture(true);
        // Act
        var obj = fixture.CreateAutoMock<WithCtorArgsTestClass>(true);
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<WithCtorArgsTestClass>();

        var inner = (WithCtorArgsTestClass)obj;

        inner.TestCtorArg.Should().NotBeNull();
        inner.TestCtorArg!.InternalTest.Should().BeNull();
        AutoMock.IsAutoMock(inner.TestCtorArg).Should().BeTrue();

        inner.TestClassProp.Should().BeNull();
        inner.TestClassField.Should().BeNull();
    }

    [Test]
    public void Test_CreateAutoMock_NoConfigureMembers_WithCtorParams_NoCallBase()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture(true);
        // Act
        var obj = fixture.CreateAutoMock<WithCtorArgsTestClass>();
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<WithCtorArgsTestClass>();

        var inner = (WithCtorArgsTestClass)obj;

        inner.TestClassProp.Should().BeNull();
        inner.TestClassPropGet.Should().BeNull();
        inner.TestClassField.Should().BeNull();
    }
}
