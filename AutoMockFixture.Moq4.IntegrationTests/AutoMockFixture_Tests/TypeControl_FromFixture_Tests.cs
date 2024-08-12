using AutoMockFixture.FixtureUtils;
using AutoMockFixture.Moq4.AutoMockUtils;

namespace AutoMockFixture.Tests.AutoMockFixture_Tests;

internal class TypeControl_FromFixture_Tests
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

    private bool IsCallBase<T>(T obj) where T : class => AutoMock.Get(obj)?.CallBase == true;

    T CreateNonAutoMock<T>(AutoMockTypeControl typeControl) => fixture.CreateNonAutoMock<T>(autoMockTypeControl: typeControl)!;

    [Test]
    public void Test_AlwaysAutoMockTypes_WillAutoMock_WhenNonAutoMock_FromFixture_ObjectItself()
    {
        fixture.AutoMockTypeControl = GetMockTypeControl();
        var obj = fixture.CreateNonAutoMock<InternalSimpleTestClass>()!;

        AutoMock.IsAutoMock(obj).Should().BeTrue();
    }

    [Test]
    public void Test_AlwaysAutoMockTypes_WillAutoMock_WhenNonDependency_FromFixture_ObjectItself()
    {
        fixture.AutoMockTypeControl = GetMockTypeControl();
        var obj = fixture.CreateWithAutoMockDependencies<InternalSimpleTestClass>();

        AutoMock.IsAutoMock(obj).Should().BeTrue();
    }

    [Test]
    public void Test_AlwaysAutoMockTypes_WillNotCallBase_WhenNonAutoMock_FromFixture_ObjectItself_WhenCallBaseFalse()
    {
        fixture.AutoMockTypeControl = GetMockTypeControl();
        var obj = fixture.CreateNonAutoMock<InternalSimpleTestClass>(callBase: false)!;

        IsCallBase(obj).Should().BeFalse();
    }

    [Test]
    public void Test_AlwaysAutoMockTypes_WillCallBase_WhenNonAutoMock_FromFixture_ObjectItself_WhenNoCallbaseSpecified()
    {
        fixture.AutoMockTypeControl = GetMockTypeControl();
        var obj = fixture.CreateNonAutoMock<InternalSimpleTestClass>()!;

        IsCallBase(obj).Should().BeTrue();
    }

    [Test]
    public void Test_AlwaysAutoMockTypes_WillCallBase_WhenCreateDependencies_FromFixture_WhenCallBase_ObjectItself()
    {
        fixture.AutoMockTypeControl = GetMockTypeControl();
        var obj = fixture.CreateWithAutoMockDependencies<InternalSimpleTestClass>(true)!;

        IsCallBase(obj).Should().BeTrue();
    }

    [Test]
    public void Test_AlwaysAutoMockTypes_WillCallBase_WhenCreateAutoMock_FromFixture_WhenCallBase_ObjectItself()
    {
        fixture.AutoMockTypeControl = GetMockTypeControl();
        var obj = fixture.CreateAutoMock<InternalSimpleTestClass>(true)!;

        IsCallBase(obj).Should().BeTrue();
    }

    [Test]
    public void Test_AlwaysAutoMockTypes_WillAutoMock_WhenNonAutoMock_FromFixture_Dependencies()
    {
        fixture.AutoMockTypeControl = GetMockTypeControl();
        var obj = fixture.CreateNonAutoMock<WithCtorArgsTestClass>()!;

        AutoMock.IsAutoMock(obj).Should().BeFalse();

        AutoMock.IsAutoMock(obj.TestCtorArg).Should().BeTrue();
        AutoMock.IsAutoMock(obj!.TestClassProp!).Should().BeTrue();
    }

    [Test]
    public void Test_AlwaysAutoMockTypes_WillAutoMock_WhenNonDependency_FromFixture_Dependencies()
    {
        var obj = fixture.CreateWithAutoMockDependencies<WithCtorArgsTestClass>(false, GetMockTypeControl())!;

        AutoMock.IsAutoMock(obj).Should().BeFalse();

        AutoMock.IsAutoMock(obj.TestCtorArg).Should().BeTrue();
        AutoMock.IsAutoMock(obj!.TestClassProp!).Should().BeTrue();
    }

    [Test]
    public void Test_AlwaysAutoMockTypes_WillNotCallBase_WhenNonAutoMock_FromFixture_Dependencies_WhenCallBaseFalse()
    {
        fixture.AutoMockTypeControl = GetMockTypeControl();
        var obj = fixture.CreateNonAutoMock<WithCtorArgsTestClass>(callBase: false)!;

        IsCallBase(obj.TestCtorArg).Should().BeFalse();
        IsCallBase(obj!.TestClassProp!).Should().BeFalse();
    }

    [Test]
    public void Test_AlwaysAutoMockTypes_WillCallBase_WhenNonAutoMock_FromFixture_Dependencies_WhenNoCallBaseSpecified()
    {
        fixture.AutoMockTypeControl = GetMockTypeControl();
        var obj = fixture.CreateNonAutoMock<WithCtorArgsTestClass>()!;

        IsCallBase(obj.TestCtorArg).Should().BeTrue();
        IsCallBase(obj!.TestClassProp!).Should().BeTrue();
    }

    [Test]
    public void Test_AlwaysAutoMockTypes_WillCallBase_WhenNonDependency_FromFixture_WhenCallBase_Dependencies()
    {
        fixture.AutoMockTypeControl = GetMockTypeControl();
        var obj = fixture.CreateWithAutoMockDependencies<WithCtorArgsTestClass>(true)!;

        IsCallBase(obj.TestCtorArg).Should().BeTrue();
        IsCallBase(obj!.TestClassProp!).Should().BeTrue();
    }

    [Test]
    public void Test_AlwaysAutoMockTypes_WillCallBase_WhenCreateAutoMock_FromFixture_WhenCallBase_Dependencies()
    {
        fixture.AutoMockTypeControl = GetMockTypeControl();
        var obj = fixture.CreateAutoMock<WithCtorArgsTestClass>(true)!;

        IsCallBase(obj.TestCtorArg).Should().BeTrue();
        IsCallBase(obj!.TestClassProp!).Should().BeTrue();
    }

    [Test]
    public void Test_NeverAutoMockTypes_WillNotAutoMock_WhenAutoMock_FromFixture_ObjectItself()
    {
        fixture.AutoMockTypeControl = GetNonMockTypeControl();
        var obj = fixture.CreateAutoMock<InternalSimpleTestClass>(false)!;

        AutoMock.IsAutoMock(obj).Should().BeFalse();
    }

    [Test]
    public void Test_NeverAutoMockTypes_WillNotAutoMock_WhenAutoMock_FromFixture_Dependencies()
    {
        fixture.AutoMockTypeControl = GetNonMockTypeControl();
        // Needs to be true to get the ctor args
        var obj = fixture.CreateAutoMock<WithCtorArgsTestClass>(true)!;

        AutoMock.IsAutoMock(obj).Should().BeTrue();

        AutoMock.IsAutoMock(obj.TestCtorArg).Should().BeFalse();
        AutoMock.IsAutoMock(obj!.TestClassProp!).Should().BeFalse();
    }

    [Test]
    public void Test_NeverAutoMockTypes_WillNotAutoMock_WhenNonDependency_FromFixture_Dependencies()
    {
        fixture.AutoMockTypeControl = GetMockTypeControl();
        var obj = fixture.CreateWithAutoMockDependencies<WithCtorArgsTestClass>(false);

        obj.Should().NotBeNull();
        AutoMock.IsAutoMock(obj).Should().BeFalse();

        AutoMock.IsAutoMock(obj!.TestCtorArg).Should().BeTrue();
        AutoMock.IsAutoMock(obj!.TestClassProp!).Should().BeTrue();
    }
}
