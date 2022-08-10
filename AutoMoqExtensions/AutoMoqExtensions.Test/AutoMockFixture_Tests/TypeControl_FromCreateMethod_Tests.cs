
using AutoMoqExtensions.AutoMockUtils;
using AutoMoqExtensions.FixtureUtils;

namespace AutoMoqExtensions.Test.AutoMockFixture_Tests;

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

    private bool IsAutoMock(object obj) => obj is not null && AutoMockHelpers.GetFromObj(obj) is not null;
    private bool IsNotAutoMock(object obj) => obj is not null && AutoMockHelpers.GetFromObj(obj) is null;
    private bool IsCallbase(object obj) => AutoMockHelpers.GetFromObj(obj)?.CallBase == true;

    T CreateNonAutoMock<T>(AutoMockTypeControl typeControl) => fixture.CreateNonAutoMock<T>(typeControl);

    [Test]       
    public void Test_AlwaysAutoMockTypes_WillAutoMock_WhenNonAutoMock_FromCreateMethod_ObjectItself()
    {
        var obj = fixture.CreateNonAutoMock<InternalSimpleTestClass>(GetMockTypeControl());
        
        IsAutoMock(obj).Should().BeTrue();
    }

    [Test]
    public void Test_AlwaysAutoMockTypes_WillAutoMock_WhenNonDependency_FromCreateMethod_ObjectItself()
    {
        var obj = fixture.CreateWithAutoMockDependencies<InternalSimpleTestClass>(false, GetMockTypeControl());

        IsAutoMock(obj).Should().BeTrue();
    }

    [Test]
    public void Test_AlwaysAutoMockTypes_WillNotCallBase_WhenNonAutoMock_FromCreateMethod_ObjectItself()
    {
        var obj = fixture.CreateNonAutoMock<InternalSimpleTestClass>(GetMockTypeControl());

        IsCallbase(obj).Should().BeFalse();
    }

    [Test]
    public void Test_AlwaysAutoMockTypes_WillCallBase_WhenCreateDependencies_FromCreateMethod_WhenCallbase_ObjectItself()
    {
        var obj = fixture.CreateWithAutoMockDependencies<InternalSimpleTestClass>(true, GetMockTypeControl());

        IsCallbase(obj).Should().BeTrue();
    }

    [Test]
    public void Test_AlwaysAutoMockTypes_WillCallBase_WhenCreateAutoMock_FromCreateMethod_WhenCallbase_ObjectItself()
    {
        var obj = fixture.CreateAutoMock<InternalSimpleTestClass>(true, GetMockTypeControl());

        IsCallbase(obj).Should().BeTrue();
    }

    [Test]
    public void Test_AlwaysAutoMockTypes_WillAutoMock_WhenNonAutoMock_FromCreateMethod_Dependencies()
    {
        var obj = fixture.CreateNonAutoMock<WithCtorArgsTestClass>(GetMockTypeControl());

        IsNotAutoMock(obj).Should().BeTrue();

        IsAutoMock(obj.TestCtorArg).Should().BeTrue();
        IsAutoMock(obj!.TestClassProp!).Should().BeTrue();
    }

    [Test]
    public void Test_AlwaysAutoMockTypes_WillAutoMock_WhenNonDependency_FromCreateMethod_Dependencies()
    {
        var obj = fixture.CreateWithAutoMockDependencies<WithCtorArgsTestClass>(false, GetMockTypeControl());

        IsNotAutoMock(obj).Should().BeTrue();

        IsAutoMock(obj.TestCtorArg).Should().BeTrue();
        IsAutoMock(obj!.TestClassProp!).Should().BeTrue();
    }

    [Test]
    public void Test_AlwaysAutoMockTypes_WillNotCallBase_WhenNonAutoMock_FromCreateMethod_Dependencies()
    {
        var obj = fixture.CreateNonAutoMock<WithCtorArgsTestClass>(GetMockTypeControl());

        IsCallbase(obj.TestCtorArg).Should().BeFalse();
        IsCallbase(obj!.TestClassProp!).Should().BeFalse();
    }

    [Test]
    public void Test_AlwaysAutoMockTypes_WillCallBase_WhenNonDependency_FromCreateMethod_WhenCallbase_Dependencies()
    {          
        var obj = fixture.CreateWithAutoMockDependencies<WithCtorArgsTestClass>(true, GetMockTypeControl());

        IsCallbase(obj.TestCtorArg).Should().BeTrue();
        IsCallbase(obj!.TestClassProp!).Should().BeTrue();
    }

    [Test]
    public void Test_AlwaysAutoMockTypes_WillCallBase_WhenCreateAutoMock_FromCreateMethod_WhenCallbase_Dependencies()
    {
        var obj = fixture.CreateAutoMock<WithCtorArgsTestClass>(true, GetMockTypeControl());

        IsCallbase(obj.TestCtorArg).Should().BeTrue();
        IsCallbase(obj!.TestClassProp!).Should().BeTrue();
    }

    [Test]
    public void Test_NeverAutoMockTypes_WillNotAutoMock_WhenAutoMock_FromCreateMethod_ObjectItself()
    {
        var obj = fixture.CreateAutoMock<InternalSimpleTestClass>(false, GetNonMockTypeControl());

        IsNotAutoMock(obj).Should().BeTrue();
    }

    [Test]
    public void Test_NeverAutoMockTypes_WillNotAutoMock_WhenAutoMock_FromCreateMethod_Dependencies()
    {
        // Needs to be true to get the ctor args
        var obj = fixture.CreateAutoMock<WithCtorArgsTestClass>(true, GetNonMockTypeControl());

        IsAutoMock(obj).Should().BeTrue();

        IsNotAutoMock(obj.TestCtorArg).Should().BeTrue();
        IsNotAutoMock(obj!.TestClassProp!).Should().BeTrue();
    }

    [Test]
    public void Test_NeverAutoMockTypes_WillNotAutoMock_WhenNonDependency_FromCreateMethod_Dependencies()
    {
        var obj = fixture.CreateWithAutoMockDependencies<WithCtorArgsTestClass>(false, GetNonMockTypeControl());

        IsNotAutoMock(obj).Should().BeTrue();

        IsNotAutoMock(obj.TestCtorArg).Should().BeTrue();
        IsNotAutoMock(obj!.TestClassProp!).Should().BeTrue();
    }
}
