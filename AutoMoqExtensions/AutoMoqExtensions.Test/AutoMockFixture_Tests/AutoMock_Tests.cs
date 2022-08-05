
using AutoMoqExtensions.AutoMockUtils;

namespace AutoMoqExtensions.Test.AutoMockFixture_Tests;

internal class AutoMock_Tests
{


    [Test]
    public void Test_AutoMock_Abstract()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateAutoMock<AutoMock<InternalAbstractMethodTestClass>>();
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeOfType<AutoMock<InternalAbstractMethodTestClass>>();
        obj.GetMocked().InternalTest.Should().NotBeNull();
    }

    [Test]
    public void Test_AutoMock_CallBase()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateAutoMock<WithCtorArgsTestClass>(true);
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<WithCtorArgsTestClass>();

        var mock = AutoMockHelpers.GetAutoMock(obj);
        mock.Should().NotBeNull();

        var inner = (WithCtorArgsTestClass)obj;

        inner.TestCtorArg.Should().NotBeNull();
        inner.TestCtorArg.InternalTest.Should().NotBeNull();
        AutoMockUtils.AutoMockHelpers.GetAutoMock(inner.TestCtorArg).Should().NotBeNull();

        inner.TestClassProp.Should().NotBeNull();
        inner.TestClassProp!.InternalTest.Should().NotBeNull();
        inner.TestClassProp!.Should().NotBe(inner.TestCtorArg);
        AutoMockUtils.AutoMockHelpers.GetAutoMock(inner.TestClassProp).Should().NotBeNull();

        inner.TestClassPropWithPrivateSet.Should().BeNull(); // We do not setup private setters so far for callabase
        inner.TestClassPropWithProtectedSet.Should().BeNull(); // We do not setup private setters so far for callabase

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
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateAutoMock<AutoMock<WithCtorArgsTestClass>>();
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeOfType<AutoMock<WithCtorArgsTestClass>>();

        var inner = (WithCtorArgsTestClass)obj;           

        inner.TestClassProp.Should().NotBeNull();
        inner.TestClassProp!.InternalTest.Should().NotBeNull();
        inner.TestClassProp!.Should().NotBe(inner.TestCtorArg);
        AutoMockUtils.AutoMockHelpers.GetAutoMock(inner.TestClassProp).Should().NotBeNull();

        inner.TestClassPropWithPrivateSet.Should().NotBeNull();
        inner.TestClassPropWithPrivateSet!.InternalTest.Should().NotBeNull();
        inner.TestClassPropWithPrivateSet!.Should().NotBe(inner.TestCtorArg);
        AutoMockUtils.AutoMockHelpers.GetAutoMock(inner.TestClassPropWithPrivateSet).Should().NotBeNull();

        inner.TestClassPropWithProtectedSet.Should().NotBeNull();
        inner.TestClassPropWithProtectedSet!.InternalTest.Should().NotBeNull();
        inner.TestClassPropWithProtectedSet!.Should().NotBe(inner.TestCtorArg);
        AutoMockUtils.AutoMockHelpers.GetAutoMock(inner.TestClassPropWithProtectedSet).Should().NotBeNull();

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
        var fixture = new AbstractAutoMockFixture();
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
        var fixture = new AbstractAutoMockFixture();
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
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateAutoMock<WithCtorArgsTestClass>(true);
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<WithCtorArgsTestClass>();

        var inner = (WithCtorArgsTestClass)obj;

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
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateAutoMock<WithCtorArgsTestClass>();
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<WithCtorArgsTestClass>();

        var inner = (WithCtorArgsTestClass)obj;

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
