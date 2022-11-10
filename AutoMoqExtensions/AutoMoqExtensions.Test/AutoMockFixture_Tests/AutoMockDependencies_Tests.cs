
using AutoMoqExtensions.AutoMockUtils;

namespace AutoMoqExtensions.Test.AutoMockFixture_Tests;

internal class AutoMockDependencies_Tests
{
    [Test]
    public void Test_MainObject_NotAutoMocked()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<WithCtorArgsTestClass>();
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeOfType<WithCtorArgsTestClass>();
        AutoMockHelpers.GetAutoMock(obj).Should().BeNull();
    }

    #region When AutoMock

    [Test]
    public void Test_MainObject_AutoMocked_WhenAutoMock()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<AutoMock<WithCtorArgsTestClass>>();
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<AutoMock<WithCtorArgsTestClass>>();
        obj!.GetMocked().Should().NotBeNull();
    }

    [Test]
    public void Test_MainObject_AlwaysCallBase_WhenAutoMock()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<AutoMock<WithCtorArgsTestClass>>(callBase: false);
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<AutoMock<WithCtorArgsTestClass>>();
        obj!.CallBase.Should().BeTrue();
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void Test_Dependencies_CallBase_BasedOnCallBase_WhenAutoMock(bool callBase)
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<AutoMock<WithCtorArgsTestClass>>(callBase: callBase);
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<AutoMock<WithCtorArgsTestClass>>();
        AutoMockHelpers.GetAutoMock(obj!.Object.TestCtorArgVirtualProp).Should().NotBeNull();
        AutoMockHelpers.GetAutoMock(obj!.Object.TestCtorArgVirtualProp)!.CallBase.Should().Be(callBase);
    }

    #endregion

    #region When Abstarct

    [Test]
    public void Test_MainObject_AutoMocked_WhenAbstract()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<InternalAbstractMethodTestClass>();
        // Assert
        obj.Should().NotBeNull();
        AutoMockHelpers.GetAutoMock(obj).Should().NotBeNull();
    }

    [Test]
    public void Test_MainObject_AlwaysCallBase_WhenAbstract()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<InternalAbstractMethodTestClass>(callBase: false);
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<InternalAbstractMethodTestClass>();

        var mockObj = AutoMockHelpers.GetAutoMock(obj);
        mockObj.Should().NotBeNull();
        mockObj.Should().BeAssignableTo<AutoMock<InternalAbstractMethodTestClass>>();
        mockObj!.CallBase.Should().BeTrue();
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void Test_Dependencies_CallBase_BasedOnCallBase_WhenAbstract(bool callBase)
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<InternalAbstractMethodTestClass>(callBase: callBase);
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<InternalAbstractMethodTestClass>();

        obj!.InternalTestMethodsObj.Should().NotBeNull();
        obj!.InternalTestMethodsObj.Should().BeAssignableTo<InternalTestMethods>();

        var mockObj = AutoMockHelpers.GetAutoMock(obj);
        mockObj.Should().NotBeNull();
        mockObj.Should().BeAssignableTo<AutoMock<InternalAbstractMethodTestClass>>();

        var innerMock = AutoMockHelpers.GetAutoMock(obj!.InternalTestMethodsObj);
        innerMock.Should().NotBeNull();
        innerMock.Should().BeAssignableTo<AutoMock<InternalTestMethods>>();
        innerMock!.CallBase.Should().Be(callBase);
    }

    #endregion

    #region When Interface

    [Test]
    public void Test_MainObject_AutoMocked_WhenInterface()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<ITestInterface>();
        // Assert
        obj.Should().NotBeNull();
        AutoMockHelpers.GetAutoMock(obj).Should().NotBeNull();

        obj!.TestProp.Should().NotBeNull();
        obj.TestMethod().Should().NotBeNull();
    }

    [Test]
    public void Test_MainObject_AlwaysCallBase_WhenInterface()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<ITestInterface>(callBase: false);
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<ITestInterface>();

        var mockObj = AutoMockHelpers.GetAutoMock(obj);
        mockObj.Should().NotBeNull();
        mockObj.Should().BeAssignableTo<AutoMock<ITestInterface>>();
        mockObj!.CallBase.Should().BeTrue();
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void Test_Dependencies_CallBase_BasedOnCallBase_WhenInterface(bool callBase)
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<ITestInterface>(callBase: callBase);
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<ITestInterface>();
        var mockObj = AutoMockHelpers.GetAutoMock(obj);
        obj!.InternalTestMethodsObj.Should().NotBeNull();
        obj!.InternalTestMethodsObj.Should().BeAssignableTo<InternalTestMethods>();

        //var mockObj = AutoMockHelpers.GetAutoMock(obj);
        mockObj.Should().NotBeNull();
        mockObj.Should().BeAssignableTo<AutoMock<ITestInterface>>();

        var innerMock = AutoMockHelpers.GetAutoMock(obj!.InternalTestMethodsObj);
        innerMock.Should().NotBeNull();
        innerMock.Should().BeAssignableTo<AutoMock<InternalTestMethods>>();
        innerMock!.CallBase.Should().Be(callBase);
    }

    #endregion

    [Test]
    public void Test_CtorArguments_AutoMocked()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<WithCtorArgsTestClass>();
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeOfType<WithCtorArgsTestClass>();
        AutoMockHelpers.GetAutoMock(obj).Should().BeNull();

        obj!.TestCtorArg.Should().NotBeNull();
        AutoMockHelpers.GetAutoMock(obj.TestCtorArg).Should().NotBeNull();

        obj.TestCtorArgProp.Should().NotBeNull();
        AutoMockHelpers.GetAutoMock(obj.TestCtorArgProp).Should().NotBeNull();

        obj.TestCtorArgPrivateProp.Should().NotBeNull();
        AutoMockHelpers.GetAutoMock(obj.TestCtorArgPrivateProp).Should().NotBeNull();

        obj.TestCtorArgVirtualProp.Should().NotBeNull();
        AutoMockHelpers.GetAutoMock(obj.TestCtorArgVirtualProp).Should().NotBeNull();

        obj.TestCtorArgVirtualPrivateProp.Should().NotBeNull();
        AutoMockHelpers.GetAutoMock(obj.TestCtorArgVirtualPrivateProp).Should().NotBeNull();
    }

    [Test]
    public void Test_Properties_AutoMocked()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<WithCtorArgsTestClass>();
        // Assert
        obj.TestClassProp.Should().NotBeNull();
        obj.TestClassProp!.InternalTest.Should().NotBeNull();
        AutoMockHelpers.GetAutoMock(obj.TestClassProp).Should().NotBeNull();
    }

    [Test]
    public void Test_PropertiesPrivateSetter_AutoMocked_WhenNotCallBase()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<WithCtorArgsTestClass>(callBase: false);
        // Assert
        obj.TestClassPropWithPrivateSet.Should().NotBeNull();
        obj.TestClassPropWithPrivateSet!.InternalTest.Should().NotBeNull();
        AutoMockHelpers.GetAutoMock(obj.TestClassPropWithPrivateSet).Should().NotBeNull();

        obj.TestClassPrivateNonVirtualProp.Should().NotBeNull();
        obj.TestClassPrivateNonVirtualProp!.InternalTest.Should().NotBeNull();
        AutoMockHelpers.GetAutoMock(obj.TestClassPrivateNonVirtualProp).Should().NotBeNull();
    }

    [Test]
    public void Test_PropertiesNoSetter_AutoMocked_WhenNotCallBaseAndAutoMock()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<AutoMock<WithCtorArgsTestClass>>(callBase: false);
        var inner = obj.GetMocked();

        // Assert
        inner.Should().NotBeNull();
        inner.TestClassPropGet.Should().NotBeNull();
        inner.TestClassPropGet!.InternalTest.Should().NotBeNull();
        AutoMockHelpers.GetAutoMock(inner.TestClassPropGet).Should().NotBeNull();
    }

    [Test]
    public void Test_PropertiesPrivateSetter_NotAutoMocked_WhenCallBase()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<WithCtorArgsTestClass>(callBase: true);
        // Assert
        obj.TestClassPropWithPrivateSet.Should().BeNull();
        obj.TestClassPrivateNonVirtualProp.Should().BeNull();
    }

    [Test]
    public void Test_PropertiesNoSetter_NotAutoMocked_WhenCallBaseAndAutoMock()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<AutoMock<WithCtorArgsTestClass>>(callBase: true);
        var inner = obj.GetMocked();
        // Assert
        inner.Should().NotBeNull();
        inner.TestClassPropGet.Should().BeNull();        
    }

    [Test]
    public void Test_Fields_AutoMocked()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<WithCtorArgsTestClass>();
        // Assert
        obj.TestClassField.Should().NotBeNull();
        obj.TestClassField!.InternalTest.Should().NotBeNull();
        AutoMockHelpers.GetAutoMock(obj.TestClassField).Should().NotBeNull();
    }
}
