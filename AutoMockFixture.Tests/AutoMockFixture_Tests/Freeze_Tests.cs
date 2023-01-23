using SequelPay.DotNetPowerExtensions;

namespace AutoMoqExtensions.Test.AutoMockFixture_Tests;

internal class Freeze_Tests
{
    #region Singleton

    [Test]
    public void Test_ClassMarkedSingleton_IsFrozen_WhenAutoMock_AndCallBase()
    {
        var fixture = new AbstractAutoMockFixture();

        var obj1 = fixture.CreateAutoMock<SingletonUserClass>(true);
        var obj2 = fixture.CreateAutoMock<SingletonUserClass>(true);

        var singletonMock1 = fixture.CreateAutoMock<SingletonClass>(true);
        var singletonMock2 = fixture.CreateAutoMock<SingletonClass>(true);

        var mock1 = fixture.CreateAutoMock<AutoMock<SingletonClass>>(true);
        var mock2 = fixture.CreateAutoMock<AutoMock<SingletonClass>>(true);

        var depend1 = fixture.CreateWithAutoMockDependencies<SingletonUserClass>(true);
        var depend2 = fixture.CreateWithAutoMockDependencies<SingletonUserClass>(true);

        obj1.Should().NotBeNull();
        obj1!.Class1.Should().NotBeNull();
        obj1.Class1.Should().BeAssignableTo<SingletonClass>();
        obj1.Class2.Should().Be(obj1.Class1);
        obj1.SingletonProp.Should().Be(obj1.Class1);
        obj1.SingletonField.Should().Be(obj1.Class1);

        obj2.Should().NotBeNull();
        obj2!.Class1.Should().Be(obj1.Class1);
        obj2.Class2.Should().Be(obj1.Class1);
        obj2.SingletonProp.Should().Be(obj1.Class1);
        obj2.SingletonField.Should().Be(obj1.Class1);

        singletonMock1.Should().Be(obj1.Class1);
        singletonMock2.Should().Be(obj1.Class1);

        mock1.Should().NotBeNull();
        mock2.Should().NotBeNull();
        mock1!.GetMocked().Should().Be(obj1.Class1);
        mock2!.GetMocked().Should().Be(obj1.Class1);

        depend1.Should().NotBeNull();
        depend1!.Class1.Should().Be(obj1.Class1);
        depend1.Class2.Should().Be(obj1.Class1);
        depend1.SingletonProp.Should().Be(obj1.Class1);
        depend1.SingletonField.Should().Be(obj1.Class1);

        depend2.Should().NotBeNull();
        depend2!.Class1.Should().Be(obj1.Class1);
        depend2.Class2.Should().Be(obj1.Class1);
        depend2.SingletonProp.Should().Be(obj1.Class1);
        depend2.SingletonField.Should().Be(obj1.Class1);
    }

    [Test]
    public void Test_ClassMarkedSingleton_IsFrozen_WhenAutoMock_AndNonCallBase()
    {
        var fixture = new AbstractAutoMockFixture();
        var obj1 = fixture.CreateAutoMock<SingletonUserClass>(false);
        var obj2 = fixture.CreateAutoMock<SingletonUserClass>(false);

        var singletonMock1 = fixture.CreateAutoMock<SingletonClass>(false);
        var singletonMock2 = fixture.CreateAutoMock<SingletonClass>(false);

        var mock1 = fixture.CreateAutoMock<AutoMock<SingletonClass>>(false);
        var mock2 = fixture.CreateAutoMock<AutoMock<SingletonClass>>(false);

        var depend1 = fixture.CreateWithAutoMockDependencies<SingletonUserClass>(false);
        var depend2 = fixture.CreateWithAutoMockDependencies<SingletonUserClass>(false);

        obj1.Should().NotBeNull();
        obj1!.SingletonProp.Should().NotBeNull();
        obj1.SingletonField.Should().Be(obj1.SingletonProp);

        obj2.Should().NotBeNull();
        obj2!.SingletonProp.Should().Be(obj1.SingletonProp);
        obj2.SingletonField.Should().Be(obj1.SingletonProp);

        singletonMock1.Should().Be(obj1.SingletonProp);
        singletonMock2.Should().Be(obj1.SingletonProp);

        mock1.Should().NotBeNull();
        mock2.Should().NotBeNull();
        mock1!.GetMocked().Should().Be(obj1.SingletonProp);
        mock2!.GetMocked().Should().Be(obj1.SingletonProp);

        depend1.Should().NotBeNull();
        depend1!.Class1.Should().Be(obj1.SingletonProp);
        depend1.Class2.Should().Be(obj1.SingletonProp);
        depend1.SingletonProp.Should().Be(obj1.SingletonProp);
        depend1.SingletonField.Should().Be(obj1.SingletonProp);

        depend2.Should().NotBeNull();
        depend2!.Class1.Should().Be(obj1.SingletonProp);
        depend2.Class2.Should().Be(obj1.SingletonProp);
        depend2.SingletonProp.Should().Be(obj1.SingletonProp);
        depend2.SingletonField.Should().Be(obj1.SingletonProp);
    }

    [Test]
    public void Test_ClassMarkedSingleton_IsDifferent_ByCallBase()
    {
        var fixture = new AbstractAutoMockFixture();
        var obj1 = fixture.CreateAutoMock<SingletonClass>(false);
        var obj2 = fixture.CreateAutoMock<SingletonClass>(true);

        obj1.Should().NotBeNull();
        obj2.Should().NotBeNull();

        obj2.Should().NotBe(obj1);
    }

    #endregion

    #region Freeze

    [Test]
    public void Test_Freeze_IsFrozen_WhenAutoMock_AndCallBase()
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.Freeze<NonSingletonClass>();

        var obj1 = fixture.CreateAutoMock<NonSingletonUserClass>(true);
        var obj2 = fixture.CreateAutoMock<NonSingletonUserClass>(true);

        var nonSingletonMock1 = fixture.CreateAutoMock<NonSingletonClass>(true);
        var nonSngletonMock2 = fixture.CreateAutoMock<NonSingletonClass>(true);

        var mock1 = fixture.CreateAutoMock<AutoMock<NonSingletonClass>>(true);
        var mock2 = fixture.CreateAutoMock<AutoMock<NonSingletonClass>>(true);

        var depend1 = fixture.CreateWithAutoMockDependencies<NonSingletonUserClass>(true);
        var depend2 = fixture.CreateWithAutoMockDependencies<NonSingletonUserClass>(true);

        obj1.Should().NotBeNull();
        obj1!.Class1.Should().NotBeNull();
        obj1.Class1.Should().BeAssignableTo<NonSingletonClass>();
        obj1.Class2.Should().Be(obj1.Class1);
        obj1.NonSingletonProp.Should().Be(obj1.Class1);
        obj1.NonSingletonField.Should().Be(obj1.Class1);

        obj2.Should().NotBeNull();
        obj2!.Class1.Should().Be(obj1.Class1);
        obj2.Class2.Should().Be(obj1.Class1);
        obj2.NonSingletonProp.Should().Be(obj1.Class1);
        obj2.NonSingletonField.Should().Be(obj1.Class1);

        nonSingletonMock1.Should().Be(obj1.Class1);
        nonSngletonMock2.Should().Be(obj1.Class1);

        mock1.Should().NotBeNull();
        mock2.Should().NotBeNull();
        mock1!.GetMocked().Should().Be(obj1.Class1);
        mock2!.GetMocked().Should().Be(obj1.Class1);

        depend1.Should().NotBeNull();
        depend1!.Class1.Should().Be(obj1.Class1);
        depend1.Class2.Should().Be(obj1.Class1);
        depend1.NonSingletonProp.Should().Be(obj1.Class1);
        depend1.NonSingletonField.Should().Be(obj1.Class1);

        depend2.Should().NotBeNull();
        depend2!.Class1.Should().Be(obj1.Class1);
        depend2.Class2.Should().Be(obj1.Class1);
        depend2.NonSingletonProp.Should().Be(obj1.Class1);
        depend2.NonSingletonField.Should().Be(obj1.Class1);
    }

    [Test]
    public void Test_Freeze_IsFrozen_WhenAutoMock_AndNonCallBase()
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.Freeze<NonSingletonClass>();

        var obj1 = fixture.CreateAutoMock<NonSingletonUserClass>(false);
        var obj2 = fixture.CreateAutoMock<NonSingletonUserClass>(false);

        var nonSingletonMock1 = fixture.CreateAutoMock<NonSingletonClass>(false);
        var nonSingletonMock2 = fixture.CreateAutoMock<NonSingletonClass>(false);

        var mock1 = fixture.CreateAutoMock<AutoMock<NonSingletonClass>>(false);
        var mock2 = fixture.CreateAutoMock<AutoMock<NonSingletonClass>>(false);

        var depend1 = fixture.CreateWithAutoMockDependencies<NonSingletonUserClass>(false);
        var depend2 = fixture.CreateWithAutoMockDependencies<NonSingletonUserClass>(false);

        obj1.Should().NotBeNull();
        obj1!.NonSingletonProp.Should().NotBeNull();
        obj1.NonSingletonField.Should().Be(obj1.NonSingletonProp);

        obj2.Should().NotBeNull();
        obj2!.NonSingletonProp.Should().Be(obj1.NonSingletonProp);
        obj2.NonSingletonField.Should().Be(obj1.NonSingletonProp);

        nonSingletonMock1.Should().Be(obj1.NonSingletonProp);
        nonSingletonMock2.Should().Be(obj1.NonSingletonProp);

        mock1.Should().NotBeNull();
        mock2.Should().NotBeNull();
        mock1!.GetMocked().Should().Be(obj1.NonSingletonProp);
        mock2!.GetMocked().Should().Be(obj1.NonSingletonProp);

        depend1.Should().NotBeNull();
        depend1!.Class1.Should().Be(obj1.NonSingletonProp);
        depend1.Class2.Should().Be(obj1.NonSingletonProp);
        depend1.NonSingletonProp.Should().Be(obj1.NonSingletonProp);
        depend1.NonSingletonField.Should().Be(obj1.NonSingletonProp);

        depend2.Should().NotBeNull();
        depend2!.Class1.Should().Be(obj1.NonSingletonProp);
        depend2.Class2.Should().Be(obj1.NonSingletonProp);
        depend2.NonSingletonProp.Should().Be(obj1.NonSingletonProp);
        depend2.NonSingletonField.Should().Be(obj1.NonSingletonProp);
    }

    [Test]
    public void Test_Freeze_IsDifferent_ByCallBase()
    {
        var fixture = new AbstractAutoMockFixture();
        var obj1 = fixture.CreateAutoMock<NonSingletonClass>(false);
        var obj2 = fixture.CreateAutoMock<NonSingletonClass>(true);

        obj1.Should().NotBeNull();
        obj2.Should().NotBeNull();

        obj2.Should().NotBe(obj1);
    }

    #endregion

    #region Classes

    [Singleton]
    public class SingletonClass { }

    public class SingletonUserClass
    {
        public SingletonClass Class1 { get; } // Non virtual to only work with the ctor
        public SingletonClass Class2 { get; } // Non virtual to only work with the ctor
        public SingletonUserClass(SingletonClass class1, SingletonClass class2)
        {
            Class1 = class1;
            Class2 = class2;
        }
        public virtual SingletonClass? SingletonProp { get; set; }
        public virtual SingletonClass? SingletonPropGet { get; }
        public SingletonClass? SingletonField;
    }

    public class NonSingletonClass { }

    public class NonSingletonUserClass
    {
        public NonSingletonClass Class1 { get; } // Non virtual to only work with the ctor
        public NonSingletonClass Class2 { get; } // Non virtual to only work with the ctor
        public NonSingletonUserClass(NonSingletonClass class1, NonSingletonClass class2)
        {
            Class1 = class1;
            Class2 = class2;
        }
        public virtual NonSingletonClass? NonSingletonProp { get; set; }
        public virtual NonSingletonClass? NonSingletonPropGet { get; }
        public NonSingletonClass? NonSingletonField;
    }


    #endregion
}
