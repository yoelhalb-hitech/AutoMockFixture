
namespace AutoMockFixture.Tests.AutoMockFixture_Tests;

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
        AutoMock.IsAutoMock(obj).Should().BeFalse();
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
    public void Test_MainObject_WhenAutoMock_CallBase_DependsOnArgs([Values(true, false)] bool callbase)
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<AutoMock<WithCtorArgsTestClass>>(callBase: callbase);
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<AutoMock<WithCtorArgsTestClass>>();
        obj!.CallBase.Should().Be(callbase);
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
        AutoMock.IsAutoMock(obj!.Object.TestCtorArgVirtualProp).Should().BeTrue();
        AutoMock.Get(obj!.Object.TestCtorArgVirtualProp)!.CallBase.Should().Be(callBase);
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
        AutoMock.IsAutoMock(obj).Should().BeTrue();
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

        var mockObj = AutoMock.Get(obj);
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

        var mockObj = AutoMock.Get(obj);
        mockObj.Should().NotBeNull();
        mockObj.Should().BeAssignableTo<AutoMock<InternalAbstractMethodTestClass>>();

        var innerMock = AutoMock.Get(obj!.InternalTestMethodsObj);
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
        AutoMock.IsAutoMock(obj).Should().BeTrue();

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

        var mockObj = AutoMock.Get(obj);
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
        var mockObj = AutoMock.Get(obj);
        obj!.InternalTestMethodsObj.Should().NotBeNull();
        obj!.InternalTestMethodsObj.Should().BeAssignableTo<InternalTestMethods>();

        //var mockObj = AutoMock.Get(obj);
        mockObj.Should().NotBeNull();
        mockObj.Should().BeAssignableTo<AutoMock<ITestInterface>>();

        var innerMock = AutoMock.Get(obj!.InternalTestMethodsObj);
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
        AutoMock.IsAutoMock(obj).Should().BeFalse();

        obj!.TestCtorArg.Should().NotBeNull();
        AutoMock.IsAutoMock(obj.TestCtorArg).Should().BeTrue();

        obj.TestCtorArgProp.Should().NotBeNull();
        AutoMock.IsAutoMock(obj.TestCtorArgProp).Should().BeTrue();

        obj.TestCtorArgPrivateProp.Should().NotBeNull();
        AutoMock.IsAutoMock(obj.TestCtorArgPrivateProp).Should().BeTrue();

        obj.TestCtorArgVirtualProp.Should().NotBeNull();
        AutoMock.IsAutoMock(obj.TestCtorArgVirtualProp).Should().BeTrue();

        obj.TestCtorArgVirtualPrivateProp.Should().NotBeNull();
        AutoMock.IsAutoMock(obj.TestCtorArgVirtualPrivateProp).Should().BeTrue();
    }

    [Test]
    public void Test_Properties_AutoMocked()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<WithCtorArgsTestClass>();

        // Assert
        obj.Should().NotBeNull();
        obj!.TestClassProp.Should().NotBeNull();
        obj.TestClassProp!.InternalTest.Should().NotBeNull();
        AutoMock.IsAutoMock(obj.TestClassProp).Should().BeTrue();
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void Test_MainObject_PropertiesPrivateOrNoSetter_AlwaysNotSet(bool callbase)
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<WithCtorArgsTestClass>(callbase);
        // Assert
        obj.Should().NotBeNull();
        obj!.TestClassPropWithPrivateSet.Should().BeNull();
        obj.TestClassPrivateNonVirtualProp.Should().BeNull();
        obj.TestClassPropGet.Should().BeNull();
    }

    [Test]
    public void Test_MainObject_AutoMock_PropertiesPrivateOrNoSetter_NotSetWhenCallbase()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<AutoMock<WithCtorArgsTestClass>>(true);
        // Assert
        obj.Should().NotBeNull();
        obj!.Object.TestClassPropWithPrivateSet.Should().BeNull();
        obj.Object.TestClassPrivateNonVirtualProp.Should().BeNull();
        obj.Object.TestClassPropGet.Should().BeNull();
    }

    [Test]
    public void Test_MainObject_AutoMock_PublicPropertiesWithPrivateOrNoSetter_SetWhenNotCallBase()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<AutoMock<WithCtorArgsTestClass>>(false);
        // Assert
        obj.Should().NotBeNull();
        obj!.Object.TestClassPropWithPrivateSet.Should().NotBeNull();
        obj!.Object.TestClassPropGet.Should().NotBeNull();

        obj!.Object.TestClassPrivateNonVirtualProp.Should().NotBeNull();
    }

    [Test]
    public void Test_Dependencies_PropertiesPrivateOrNoSetter_AutoMocked_WhenNotCallBase()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        fixture.MethodSetupType = MethodSetupTypes.Eager;
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<WithComplexTestClass>(callBase: false);
        // Assert
        obj.Should().NotBeNull();
        obj!.WithCtorArgs.Should().NotBeNull();

        obj.WithCtorArgs!.TestClassPropWithPrivateSet.Should().NotBeNull();
        AutoMock.IsAutoMock(obj.WithCtorArgs.TestClassPropWithPrivateSet).Should().BeTrue();

        obj.WithCtorArgs.TestClassPrivateNonVirtualProp.Should().NotBeNull();
        AutoMock.IsAutoMock(obj.WithCtorArgs.TestClassPrivateNonVirtualProp).Should().BeTrue();

        obj.WithCtorArgs.TestClassPropGet.Should().NotBeNull();
        AutoMock.IsAutoMock(obj.WithCtorArgs.TestClassPropGet).Should().BeTrue();
    }

    [Test]
    public void Test_Dependencies_PropertiesPrivateOrNoSetter_AutoMocked_WhenNotCallBaseAndAutoMock()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<AutoMock<WithComplexTestClass>>(callBase: false);
        // Assert
        obj.Should().NotBeNull();
        obj!.Object.WithCtorArgs.Should().NotBeNull();

        obj.Object.WithCtorArgs!.TestClassPropWithPrivateSet.Should().NotBeNull();
        AutoMock.IsAutoMock(obj.Object.WithCtorArgs.TestClassPropWithPrivateSet).Should().BeTrue();

        obj.Object.WithCtorArgs.TestClassPrivateNonVirtualProp.Should().NotBeNull();
        AutoMock.IsAutoMock(obj.Object.WithCtorArgs.TestClassPrivateNonVirtualProp).Should().BeTrue();

        obj.Object.WithCtorArgs.TestClassPropGet.Should().NotBeNull();
        AutoMock.IsAutoMock(obj.Object.WithCtorArgs.TestClassPropGet).Should().BeTrue();
    }

    [Test]
    public void Test_Dependencies_PropertiesPrivateOrNoSetter_NotSet_WhenCallBase()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<WithComplexTestClass>(callBase: true);
        // Assert
        obj!.WithCtorArgs.Should().NotBeNull();

        obj.WithCtorArgs!.TestClassPropWithPrivateSet.Should().BeNull();
        obj.WithCtorArgs.TestClassPrivateNonVirtualProp.Should().BeNull();
        obj.WithCtorArgs.TestClassPropGet.Should().BeNull();
    }

    [Test]
    public void Test_Dependencies_PropertiesPrivateOrNoSetter_NotSet_WhenCallBaseAndAutoMock()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<AutoMock<WithComplexTestClass>>(callBase: true);
        // Assert
        obj.Should().NotBeNull();
        obj!.Object.WithCtorArgs.Should().NotBeNull();

        obj.Object.WithCtorArgs!.TestClassPropWithPrivateSet.Should().BeNull();
        obj.Object.WithCtorArgs.TestClassPrivateNonVirtualProp.Should().BeNull();
        obj.Object.WithCtorArgs.TestClassPropGet.Should().BeNull();
    }

    [Test]
    public void Test_Fields_AutoMocked()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<WithCtorArgsTestClass>();
        // Assert
        obj.Should().NotBeNull();
        obj!.TestClassField.Should().NotBeNull();
        obj.TestClassField!.InternalTest.Should().NotBeNull();
        AutoMock.IsAutoMock(obj.TestClassField).Should().BeTrue();
    }
}
