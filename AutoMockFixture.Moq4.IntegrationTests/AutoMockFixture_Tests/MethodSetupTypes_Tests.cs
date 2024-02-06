
namespace AutoMockFixture.Tests.AutoMockFixture_Tests;

internal class MethodSetupTypes_Tests
{
    [Test]
    public void Test_MethodSetupTypes_Eager_SetupUpfront()
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.MethodSetupType = MethodSetupTypes.Eager;

        var obj = fixture.CreateAutoMock<InternalTestMethods>();

        fixture.GetTracker(obj!)!.GetChildrensPaths()!.Keys.Should().Contain("." + nameof(InternalTestMethods.InternalTestMethod));
    }

    [Test]
    [TestCase(MethodSetupTypes.LazySame)]
    [TestCase(MethodSetupTypes.LazyDifferent)]
    public void Test_MethodSetupTypes_Lazy_NotSettingUpfront(MethodSetupTypes methodSetupType)
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.MethodSetupType = methodSetupType;

        var obj = fixture.CreateAutoMock<InternalTestMethods>();

        fixture.GetTracker(obj!)!.GetChildrensPaths()!.Keys.Should().NotContain("." + nameof(InternalTestMethods.InternalTestMethod));

        var retValue = obj!.InternalTestMethod();
        retValue.Should().NotBeNullOrWhiteSpace();
        fixture.GetTracker(obj!)!.GetChildrensPaths()!.Keys.Should().Contain("." + nameof(InternalTestMethods.InternalTestMethod));

    }

    [Test]
    [TestCase(MethodSetupTypes.Eager)]
    [TestCase(MethodSetupTypes.LazySame)]
    public void Test_MethodSetupTypes_Same_WhenNot_LazyDifferrent(MethodSetupTypes methodSetupType)
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.MethodSetupType = methodSetupType;

        var obj = fixture.CreateAutoMock<InternalTestMethods>();

        var first = obj!.InternalTestMethod();
        var second = obj.InternalTestMethod();

        first.Should().Be(second);
    }


    [Test]
    public void Test_MethodSetupTypes_NotSame_When_LazyDifferrent()
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.MethodSetupType = MethodSetupTypes.LazyDifferent;

        var obj = fixture.CreateAutoMock<InternalTestMethods>();

        var first = obj!.InternalTestMethod();
        var second = obj.InternalTestMethod();

        first.Should().NotBe(second);
    }

    [Test]
    [TestCase(MethodSetupTypes.Eager)]
    [TestCase(MethodSetupTypes.LazySame)]
    [TestCase(MethodSetupTypes.LazyDifferent)]
    public void Test_MethodSetupTypes_ReadWriteProperty_SettingUpfront_ForCallBase(MethodSetupTypes methodSetupType)
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.MethodSetupType = methodSetupType;

        var obj = fixture.CreateAutoMock<InternalSimpleTestClass>(callBase: true);

        AutoMock.Get(obj)!.MethodsSetup.Should().NotContainKey(nameof(InternalSimpleTestClass.InternalTest));

        fixture.GetTracker(obj!)!.GetChildrensPaths()!.Keys.Should().Contain("." + nameof(InternalSimpleTestClass.InternalTest));
    }

    [Test]
    [TestCase(MethodSetupTypes.LazySame)]
    [TestCase(MethodSetupTypes.LazyDifferent)]
    public void Test_MethodSetupTypes_ReadWriteProperty_NotSettingUpfront_ForLazyNonCallBase(MethodSetupTypes methodSetupType)
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.MethodSetupType = methodSetupType;

        var obj = fixture.CreateAutoMock<InternalSimpleTestClass>(callBase: false);

        AutoMock.Get(obj)!.MethodsSetup.Should().ContainKey(nameof(InternalSimpleTestClass.InternalTest));

        fixture.GetTracker(obj!)!.GetChildrensPaths()!.Keys.Should().NotContain("." + nameof(InternalSimpleTestClass.InternalTest));
    }

    [Test]
    [TestCase(MethodSetupTypes.Eager)]
    public void Test_MethodSetupTypes_ReadWriteProperty_SettingUpfront_ForEagerNonCallBase(MethodSetupTypes methodSetupType)
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.MethodSetupType = methodSetupType;

        var obj = fixture.CreateAutoMock<InternalSimpleTestClass>(callBase: false);

        AutoMock.Get(obj)!.MethodsSetup.Should().NotContainKey(nameof(InternalSimpleTestClass.InternalTest));

        fixture.GetTracker(obj!)!.GetChildrensPaths()!.Keys.Should().Contain("." + nameof(InternalSimpleTestClass.InternalTest));
    }

    [Test]
    [TestCase(MethodSetupTypes.Eager, true)]
    [TestCase(MethodSetupTypes.Eager, false)]
    [TestCase(MethodSetupTypes.LazySame, true)]
    [TestCase(MethodSetupTypes.LazySame, false)]
    [TestCase(MethodSetupTypes.LazyDifferent, true)]
    [TestCase(MethodSetupTypes.LazyDifferent, false)]
    public void Test_MethodSetupTypes_ReadWriteProperty_Same(MethodSetupTypes methodSetupType, bool callBase)
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.MethodSetupType = methodSetupType;

        var obj = fixture.CreateAutoMock<InternalSimpleTestClass>(callBase: callBase);
        obj!.InternalTest.Should().NotBeNullOrWhiteSpace();

        var first = obj!.InternalTest;
        var second = obj.InternalTest;

        first.Should().Be(second);
    }

    [Test]
    [TestCase(MethodSetupTypes.Eager, true, true)]
    [TestCase(MethodSetupTypes.Eager, true, false)]
    [TestCase(MethodSetupTypes.Eager, false, true)]
    [TestCase(MethodSetupTypes.Eager, false, false)]
    [TestCase(MethodSetupTypes.LazySame, true, true)]
    [TestCase(MethodSetupTypes.LazySame, true, false)]
    [TestCase(MethodSetupTypes.LazySame, false, true)]
    [TestCase(MethodSetupTypes.LazySame, false, false)]
    [TestCase(MethodSetupTypes.LazyDifferent, true, true)]
    [TestCase(MethodSetupTypes.LazyDifferent, true, false)]
    [TestCase(MethodSetupTypes.LazyDifferent, false, true)]
    [TestCase(MethodSetupTypes.LazyDifferent, false, false)]
    public void Test_MethodSetupTypes_ReadWriteProperty_SetPropertyWorks(MethodSetupTypes methodSetupType,
                    bool callBase, bool readFirst)
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.MethodSetupType = methodSetupType;

        var obj = fixture.CreateAutoMock<InternalSimpleTestClass>(callBase: callBase);
        if(readFirst) obj!.InternalTest.Should().NotBeNullOrWhiteSpace();

        var testData = Guid.NewGuid().ToString();

        obj!.InternalTest = testData;
        var str = obj.InternalTest;

        str.Should().Be(testData);

        str = obj.InternalTest;
        str.Should().Be(testData);

        testData = Guid.NewGuid().ToString();

        obj!.InternalTest = testData;
        str = obj.InternalTest;

        str.Should().Be(testData);
    }

    [Test]
    [TestCase(MethodSetupTypes.LazySame)]
    [TestCase(MethodSetupTypes.LazyDifferent)]
    public void Test_MethodSetupTypes_WorksCorrectly_OnNonDefaultCtorType_ForLazyNonCallBase(MethodSetupTypes methodSetupType)
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.MethodSetupType = methodSetupType;

        var obj = fixture.CreateAutoMock<WithCtorArgsTestClass>();
        AutoMock.Get(obj)!.MethodsSetup.Should().ContainKey(nameof(WithCtorArgsTestClass.TestCtorArgVirtualProp));

        fixture.GetTracker(obj!)!.GetChildrensPaths()!.Keys.Should().NotContain("." + nameof(WithCtorArgsTestClass.TestCtorArgVirtualProp));
    }

    [Test]
    public void Test_MethodSetupTypes_ReadOnlyProperty_Eager_SetupUpfront()
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.MethodSetupType = MethodSetupTypes.Eager;

        var obj = fixture.CreateAutoMock<InternalReadonlyPropertyClass>();

        fixture.GetTracker(obj!)!.GetChildrensPaths()!.Keys.Should().Contain("." + nameof(InternalReadonlyPropertyClass.InternalTest));
    }

    [Test]
    [TestCase(MethodSetupTypes.LazySame)]
    [TestCase(MethodSetupTypes.LazyDifferent)]
    public void Test_MethodSetupTypes_ReadOnlyProperty_Lazy_NotSettingUpfront(MethodSetupTypes methodSetupType)
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.MethodSetupType = methodSetupType;

        var obj = fixture.CreateAutoMock<InternalReadonlyPropertyClass>();
        AutoMock.Get(obj)!.MethodsSetup.Should().ContainKey(nameof(InternalSimpleTestClass.InternalTest));

        fixture.GetTracker(obj!)!.GetChildrensPaths()!.Keys.Should().NotContain("." + nameof(InternalReadonlyPropertyClass.InternalTest));

        var prop = obj!.InternalTest;
        prop.Should().NotBeNullOrWhiteSpace();
        fixture.GetTracker(obj!)!.GetChildrensPaths()!.Keys.Should().Contain("." + nameof(InternalReadonlyPropertyClass.InternalTest));
    }


    [Test]
    [TestCase(MethodSetupTypes.Eager)]
    [TestCase(MethodSetupTypes.LazySame)]
    [TestCase(MethodSetupTypes.LazyDifferent)]
    public void Test_MethodSetupTypes_ReadOnlyProperty_Same(MethodSetupTypes methodSetupType)
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.MethodSetupType = methodSetupType;

        var obj = fixture.CreateAutoMock<InternalReadonlyPropertyClass>();

        var first = obj!.InternalTest;
        var second = obj.InternalTest;

        first.Should().Be(second);
    }

    // A private setter doesn't have setter in the actual object which is a subclass so we treat it as a readonly
    [Test]
    public void Test_MethodSetupTypes_PrivateSetterProperty_Eager_SetupUpfront()
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.MethodSetupType = MethodSetupTypes.Eager;

        var obj = fixture.CreateAutoMock<InternalReadonlyPropertyClass>();

        fixture.GetTracker(obj!)!.GetChildrensPaths()!.Keys.Should().Contain("." + nameof(InternalReadonlyPropertyClass.InternalTest));
    }

    // A private setter doesn't have setter in the actual object which is a subclass so we treat it as a readonly
    [Test]
    [TestCase(MethodSetupTypes.LazySame)]
    [TestCase(MethodSetupTypes.LazyDifferent)]
    public void Test_MethodSetupTypes_PrivateSetterProperty_Lazy_NotSettingUpfront(MethodSetupTypes methodSetupType)
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.MethodSetupType = methodSetupType;

        var obj = fixture.CreateAutoMock<WithCtorArgsTestClass>();

        // It only works so far on readonly properties as read write are not being setup
        const string lazyPropPath = "." + nameof(WithCtorArgsTestClass.TestClassPropGet) + "." + nameof(InternalSimpleTestClass.InternalTest);

        fixture.GetTracker(obj!)!.GetChildrensPaths()!.Keys.Should().NotContain(lazyPropPath);

        var prop = obj!.TestClassPropGet!.InternalTest;
        prop.Should().NotBeNullOrWhiteSpace();
        fixture.GetTracker(obj!)!.GetChildrensPaths()!.Keys.Should().Contain(lazyPropPath);
    }

    // A private setter doesn't have setter in the actual object which is a subclass so we treat it as a readonly
    [Test]
    [TestCase(MethodSetupTypes.Eager)]
    [TestCase(MethodSetupTypes.LazySame)]
    [TestCase(MethodSetupTypes.LazyDifferent)]
    public void Test_MethodSetupTypes_PrivateSetterProperty_Same(MethodSetupTypes methodSetupType)
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.MethodSetupType = methodSetupType;

        var obj = fixture.CreateAutoMock<InternalPrivateSetterPropertyClass>();

        var first = obj!.InternalTest;
        var second = obj.InternalTest;

        first.Should().Be(second);
    }

    [Test]
    [TestCase(MethodSetupTypes.Eager)]
    public void Test_MethodSetupTypes_ProtectedSetterProperty_SettingUpfront_OnEagerNonCallBase(MethodSetupTypes methodSetupType)
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.MethodSetupType = methodSetupType;

        var obj = fixture.CreateAutoMock<ProtectedSetterPropertyClass>(callBase: false);
        AutoMock.Get(obj)!.MethodsSetup.Should().NotContainKey(nameof(InternalSimpleTestClass.InternalTest));

        fixture.GetTracker(obj!)!.GetChildrensPaths()!.Keys.Should().Contain("." + nameof(ProtectedSetterPropertyClass.InternalTest));
    }

    [Test]
    [TestCase(MethodSetupTypes.LazySame)]
    [TestCase(MethodSetupTypes.LazyDifferent)]
    public void Test_MethodSetupTypes_ProtectedSetterProperty_NotSettingUpfront_OnLazyNonCallBase(MethodSetupTypes methodSetupType)
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.MethodSetupType = methodSetupType;

        var obj = fixture.CreateAutoMock<ProtectedSetterPropertyClass>(callBase: false);
        AutoMock.Get(obj)!.MethodsSetup.Should().ContainKey(nameof(InternalSimpleTestClass.InternalTest));

        fixture.GetTracker(obj!)!.GetChildrensPaths()!.Keys.Should().NotContain("." + nameof(ProtectedSetterPropertyClass.InternalTest));
    }

    [Test]
    [TestCase(MethodSetupTypes.Eager)]
    [TestCase(MethodSetupTypes.LazySame)]
    [TestCase(MethodSetupTypes.LazyDifferent)]
    public void Test_MethodSetupTypes_ProtectedSetterProperty_Same(MethodSetupTypes methodSetupType)
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.MethodSetupType = methodSetupType;

        var obj = fixture.CreateAutoMock<ProtectedSetterPropertyClass>();

        var first = obj!.InternalTest;
        var second = obj.InternalTest;

        first.Should().Be(second);
    }
}
