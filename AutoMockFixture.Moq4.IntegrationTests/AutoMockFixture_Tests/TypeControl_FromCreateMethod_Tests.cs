using AutoMockFixture.FixtureUtils;

namespace AutoMockFixture.Tests.AutoMockFixture_Tests;

internal class TypeControl_FromCreateMethod_Tests
{
    private AbstractAutoMockFixture fixture = default!;
    [SetUp]
    public void SetupFixture()
    {
        fixture = new AbstractAutoMockFixture();
    }

    private AutoMockTypeControl GetMockTypeControl()
        => new AutoMockTypeControl
        {
            AlwaysAutoMockTypes = new List<Type> { typeof(InternalSimpleTestClass) }
        };

    private AutoMockTypeControl GetNonMockTypeControl()
        => new AutoMockTypeControl
        {
            NeverAutoMockTypes = new List<Type> { typeof(InternalSimpleTestClass) }
        };

    private bool IsCallbase<T>(T obj) where T : class => AutoMock.Get(obj)?.CallBase == true;

    T? CreateNonAutoMock<T>(AutoMockTypeControl typeControl) => fixture.CreateNonAutoMock<T>(typeControl);

    [Test]
    public void Test_AlwaysAutoMockTypes_WillAutoMock_WhenNonAutoMock_FromCreateMethod_ObjectItself()
    {
        var obj = fixture.CreateNonAutoMock<InternalSimpleTestClass>(GetMockTypeControl());

        AutoMock.IsAutoMock(obj).Should().BeTrue();
    }

    [Test]
    public void Test_AlwaysAutoMockTypes_WillAutoMock_WhenNonDependency_FromCreateMethod_ObjectItself()
    {
        var obj = fixture.CreateWithAutoMockDependencies<InternalSimpleTestClass>(false, GetMockTypeControl());

        AutoMock.IsAutoMock(obj).Should().BeTrue();
    }

    [Test]
    public void Test_AlwaysAutoMockTypes_WillNotCallBase_WhenNonAutoMock_FromCreateMethod_ObjectItself()
    {
        var obj = fixture.CreateNonAutoMock<InternalSimpleTestClass>(GetMockTypeControl());

        obj.Should().NotBeNull();
        IsCallbase(obj!).Should().BeFalse();
    }

    [Test]
    public void Test_AlwaysAutoMockTypes_WillCallBase_WhenCreateDependencies_FromCreateMethod_WhenCallbase_ObjectItself()
    {
        var obj = fixture.CreateWithAutoMockDependencies<InternalSimpleTestClass>(true, GetMockTypeControl());

        obj.Should().NotBeNull();
        IsCallbase(obj!).Should().BeTrue();
    }

    [Test]
    public void Test_AlwaysAutoMockTypes_WillCallBase_WhenCreateAutoMock_FromCreateMethod_WhenCallbase_ObjectItself()
    {
        var obj = fixture.CreateAutoMock<InternalSimpleTestClass>(true, GetMockTypeControl());

        obj.Should().NotBeNull();
        IsCallbase(obj!).Should().BeTrue();
    }

    [Test]
    public void Test_AlwaysAutoMockTypes_WillAutoMock_WhenNonAutoMock_FromCreateMethod_Dependencies()
    {
        var obj = fixture.CreateNonAutoMock<WithCtorArgsTestClass>(GetMockTypeControl());

        obj.Should().NotBeNull();
        AutoMock.IsAutoMock(obj).Should().BeFalse();

        AutoMock.IsAutoMock(obj!.TestCtorArg).Should().BeTrue();
        AutoMock.IsAutoMock(obj!.TestClassProp!).Should().BeTrue();
    }

    [Test]
    public void Test_AlwaysAutoMockTypes_WillAutoMock_WhenNonDependency_FromCreateMethod_Dependencies()
    {
        var obj = fixture.CreateWithAutoMockDependencies<WithCtorArgsTestClass>(false, GetMockTypeControl());

        obj.Should().NotBeNull();
        AutoMock.IsAutoMock(obj).Should().BeFalse();

        AutoMock.IsAutoMock(obj!.TestCtorArg).Should().BeTrue();
        AutoMock.IsAutoMock(obj!.TestClassProp!).Should().BeTrue();
    }

    [Test]
    public void Test_AlwaysAutoMockTypes_WillNotCallBase_WhenNonAutoMock_FromCreateMethod_Dependencies()
    {
        var obj = fixture.CreateNonAutoMock<WithCtorArgsTestClass>(GetMockTypeControl());
        obj.Should().NotBeNull();

        IsCallbase(obj!.TestCtorArg).Should().BeFalse();
        IsCallbase(obj!.TestClassProp!).Should().BeFalse();
    }

    [Test]
    public void Test_AlwaysAutoMockTypes_WillCallBase_WhenNonDependency_FromCreateMethod_WhenCallbase_Dependencies()
    {
        var obj = fixture.CreateWithAutoMockDependencies<WithCtorArgsTestClass>(true, GetMockTypeControl());
        obj.Should().NotBeNull();

        IsCallbase(obj!.TestCtorArg).Should().BeTrue();
        IsCallbase(obj!.TestClassProp!).Should().BeTrue();
    }

    [Test]
    public void Test_AlwaysAutoMockTypes_WillCallBase_WhenCreateAutoMock_FromCreateMethod_WhenCallbase_Dependencies()
    {
        var obj = fixture.CreateAutoMock<WithCtorArgsTestClass>(true, GetMockTypeControl());
        obj.Should().NotBeNull();

        IsCallbase(obj!.TestCtorArg).Should().BeTrue();
        IsCallbase(obj!.TestClassProp!).Should().BeTrue();
    }

    [Test]
    public void Test_NeverAutoMockTypes_WillNotAutoMock_WhenAutoMock_FromCreateMethod_ObjectItself()
    {
        var obj = fixture.CreateAutoMock<InternalSimpleTestClass>(false, GetNonMockTypeControl());

        AutoMock.IsAutoMock(obj).Should().BeFalse();
    }

    [Test]
    public void Test_NeverAutoMockTypes_WillNotAutoMock_WhenAutoMock_FromCreateMethod_Dependencies()
    {
        // Needs to be true to get the ctor args
        var obj = fixture.CreateAutoMock<WithCtorArgsTestClass>(true, GetNonMockTypeControl());

        obj.Should().NotBeNull();
        AutoMock.IsAutoMock(obj).Should().BeTrue();

        AutoMock.IsAutoMock(obj!.TestCtorArg).Should().BeFalse();
        AutoMock.IsAutoMock(obj!.TestClassProp!).Should().BeFalse();
    }

    [Test]
    public void Test_NeverAutoMockTypes_WillNotAutoMock_WhenNonDependency_FromCreateMethod_Dependencies()
    {
        var obj = fixture.CreateWithAutoMockDependencies<WithCtorArgsTestClass>(false, GetNonMockTypeControl());

        obj.Should().NotBeNull();
        AutoMock.IsAutoMock(obj).Should().BeFalse();

        AutoMock.IsAutoMock(obj!.TestCtorArg).Should().BeFalse();
        AutoMock.IsAutoMock(obj!.TestClassProp!).Should().BeFalse();
    }
}
