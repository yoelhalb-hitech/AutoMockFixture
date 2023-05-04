using AutoMockFixture.AutoMockUtils;

namespace AutoMockFixture.Tests.AutoMockFixture_Tests;

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
    public void Test_AutoMock_AutoMockInterface()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateAutoMock<AutoMock<ITestInterface>>();
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<AutoMock<ITestInterface>>();
    }

    [Test]
    public void Test_AutoMock_Interface()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateAutoMock<ITestInterface>();
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<ITestInterface>();
    }

    [Test]
    public void Test_PropertyGet_ReturnsSame()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateAutoMock<WithCtorArgsTestClass>();

        var first = obj.TestClassPropGet;
        var second = obj.TestClassPropGet;

        // Assert
        first.Should().NotBeNull();
        second.Should().NotBeNull();
        first.Should().BeSameAs(second);
    }

    [Test]
    public void Test_Method_ReturningSame_WhenType_MethodSetupTypes_Eager()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        fixture.MethodSetupType = MethodSetupTypes.Eager;

        // Act
        var obj = fixture.CreateAutoMock<InternalTestMethods>();

        var first = obj.InternalTestMethod();
        var second = obj.InternalTestMethod();

        // Assert
        first.Should().NotBeNull();
        second.Should().NotBeNull();
        first.Should().BeSameAs(second);
    }

    [Test]
    public void Test_Method_ReturningSame_WhenType_MethodSetupTypes_LazySame()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        fixture.MethodSetupType = MethodSetupTypes.LazySame;

        // Act
        var obj = fixture.CreateAutoMock<InternalTestMethods>();

        var first = obj.InternalTestMethod();
        var second = obj.InternalTestMethod();

        // Assert
        first.Should().NotBeNull();
        second.Should().NotBeNull();
        first.Should().BeSameAs(second);
    }

    [Test]
    public void Test_Method_NotReturningSame_WhenType_MethodSetupTypes_LazyDifferent()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        fixture.MethodSetupType = MethodSetupTypes.LazyDifferent;

        // Act
        var obj = fixture.CreateAutoMock<InternalTestMethods>();

        var first = obj.InternalTestMethod();
        var second = obj.InternalTestMethod();

        // Assert
        first.Should().NotBeNull();
        second.Should().NotBeNull();
        first.Should().NotBeSameAs(second);
    }

    [Test]
    public void Test_Method_ReturningSame_WhenProperty_EvenWhenType_MethodSetupTypes_LazyDifferent()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        fixture.MethodSetupType = MethodSetupTypes.LazyDifferent;

        // Act
        var obj = fixture.CreateAutoMock<InternalSimpleTestClass>();

        var first = obj.InternalTest;
        var second = obj.InternalTest;

        // Assert
        first.Should().NotBeNull();
        second.Should().NotBeNull();
        first.Should().BeSameAs(second);
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

        var mock = AutoMock.Get(obj);
        mock.Should().NotBeNull();

        var inner = (WithCtorArgsTestClass)obj;

        inner.TestCtorArg.Should().NotBeNull();
        inner.TestCtorArg.InternalTest.Should().NotBeNull();
        AutoMock.IsAutoMock(inner.TestCtorArg).Should().BeTrue();

        inner.TestClassProp.Should().NotBeNull();
        inner.TestClassProp!.InternalTest.Should().NotBeNull();
        inner.TestClassProp!.Should().NotBe(inner.TestCtorArg);
        AutoMock.IsAutoMock(inner.TestClassProp).Should().BeTrue();

        inner.TestClassPropWithPrivateSet.Should().BeNull(); // We do not setup private setters so far for callbase
        inner.TestClassPropWithProtectedSet.Should().BeNull(); // We do not setup private setters so far for callbase

        inner.TestClassPropGet.Should().BeNull(); // We do not setup so far for callabase

        inner.TestClassField.Should().NotBeNull();
        inner.TestClassField!.InternalTest.Should().NotBeNull();
        inner.TestClassField!.Should().NotBe(inner.TestCtorArg);
        inner.TestClassField!.Should().NotBe(inner.TestClassProp);
        inner.TestClassField!.Should().NotBe(inner.TestClassPropGet);
        AutoMock.IsAutoMock(inner.TestClassField).Should().BeTrue();
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
        AutoMock.IsAutoMock(inner.TestClassProp).Should().BeTrue();

        inner.TestClassPropWithPrivateSet.Should().NotBeNull();
        inner.TestClassPropWithPrivateSet!.InternalTest.Should().NotBeNull();
        inner.TestClassPropWithPrivateSet!.Should().NotBe(inner.TestCtorArg);
        AutoMock.IsAutoMock(inner.TestClassPropWithPrivateSet).Should().BeTrue();

        inner.TestClassPropWithProtectedSet.Should().NotBeNull();
        inner.TestClassPropWithProtectedSet!.InternalTest.Should().NotBeNull();
        inner.TestClassPropWithProtectedSet!.Should().NotBe(inner.TestCtorArg);
        AutoMock.IsAutoMock(inner.TestClassPropWithProtectedSet).Should().BeTrue();

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
        AutoMock.IsAutoMock(inner.TestClassPropGet).Should().BeTrue();

        inner.TestClassField.Should().NotBeNull();
        inner.TestClassField!.InternalTest.Should().NotBeNull();
        inner.TestClassField!.Should().NotBe(inner.TestCtorArg);
        inner.TestClassField!.Should().NotBe(inner.TestClassProp);
        inner.TestClassField!.Should().NotBe(inner.TestClassPropGet);
        AutoMock.IsAutoMock(inner.TestClassField).Should().BeTrue();
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
        AutoMock.IsAutoMock(obj).Should().BeTrue();
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
        AutoMock.IsAutoMock(obj).Should().BeTrue();
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
        AutoMock.IsAutoMock(inner.TestCtorArg).Should().BeTrue();

        inner.TestClassProp.Should().NotBeNull();
        inner.TestClassProp!.InternalTest.Should().NotBeNull();
        inner.TestClassProp!.Should().NotBe(inner.TestCtorArg);
        AutoMock.IsAutoMock(inner.TestClassProp).Should().BeTrue();

        inner.TestClassField.Should().NotBeNull();
        inner.TestClassField!.InternalTest.Should().NotBeNull();
        inner.TestClassField!.Should().NotBe(inner.TestCtorArg);
        inner.TestClassField!.Should().NotBe(inner.TestClassProp);
        inner.TestClassField!.Should().NotBe(inner.TestClassPropGet);
        AutoMock.IsAutoMock(inner.TestClassField).Should().BeTrue();
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
        AutoMock.IsAutoMock(inner.TestClassProp).Should().BeTrue();

        inner.TestClassPropGet.Should().NotBeNull();
        inner.TestClassPropGet!.InternalTest.Should().NotBeNull();
        inner.TestClassPropGet!.Should().NotBe(inner.TestCtorArg);
        inner.TestClassPropGet!.Should().NotBe(inner.TestClassProp);
        AutoMock.IsAutoMock(inner.TestClassPropGet).Should().BeTrue();

        inner.TestClassField.Should().NotBeNull();
        inner.TestClassField!.InternalTest.Should().NotBeNull();
        inner.TestClassField!.Should().NotBe(inner.TestCtorArg);
        inner.TestClassField!.Should().NotBe(inner.TestClassProp);
        inner.TestClassField!.Should().NotBe(inner.TestClassPropGet);
        AutoMock.IsAutoMock(inner.TestClassField).Should().BeTrue();
    }
}
