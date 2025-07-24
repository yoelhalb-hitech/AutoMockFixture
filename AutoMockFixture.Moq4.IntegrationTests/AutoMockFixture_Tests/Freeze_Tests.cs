using AutoMockFixture.FixtureUtils;
using AutoMockFixture.NUnit3;
using SequelPay.DotNetPowerExtensions;

namespace AutoMockFixture.Tests.AutoMockFixture_Tests;

internal class Freeze_Tests
{
    #region Singleton

    private (T t1, T t2) Exec<T>(Func<bool?, AutoMockTypeControl?, T> func, bool callBase) => (func(callBase, null), func(callBase, null));
    private SingletonClass[] GetProps(SingletonUserClass userClass, bool callBase) =>
        !callBase && userClass is IAutoMocked ? [userClass.SingletonPropGet!, userClass.SingletonProp!, userClass.SingletonField!]
            : [userClass.Class1, userClass.Class2, userClass.SingletonProp!, userClass.SingletonField!];
    private SingletonClass[] GetProps(SingletonUserClass[] objs, bool callBase) =>
        objs.SelectMany(o => GetProps(o, callBase)).ToArray();

    [Test]
    public void Test_ClassMarkedSingleton_IsFrozen_WhenAutoMock_AutoMockDependencies([Values(true, false)] bool callBase)
    {
        (T t1, T t2) exec<T>(Func<bool?, AutoMockTypeControl?, T> func) => Exec(func, callBase);

        var fixture = new AbstractAutoMockFixture();

        var (singletonMock1, singletonMock2)    = exec(fixture.CreateAutoMock<SingletonClass>);
        var (obj1, obj2)                        = exec(fixture.CreateAutoMock<SingletonUserClass>);
        var (mock1, mock2)                      = exec(fixture.CreateAutoMock<AutoMock<SingletonClass>>);
        var (mockByDepend1, mockByDepend2)      = exec(fixture.CreateAutoMock<AutoMock<SingletonUserClass>>);

        var (depend1, depend2)                  = exec(fixture.CreateWithAutoMockDependencies<SingletonUserClass>);
        var (dependMock1, dependMock2)          = exec(fixture.CreateWithAutoMockDependencies<AutoMock<SingletonClass>>);

        fixture.AutoMockTypeControl.AlwaysAutoMockTypes.Add(typeof(SingletonClass));

        var (dependAuto1, dependAuto2)          = exec(fixture.CreateWithAutoMockDependencies<SingletonClass>);
        var (mockAuto1, mockAuto2)              = exec(fixture.CreateAutoMock<SingletonClass>);

        singletonMock1.Should().Should().NotBeNull();
        singletonMock1.Should().BeAssignableTo<SingletonClass>();

        new object[] {
            obj1!, obj2!, mock1!, mock2!,
            mockByDepend1!, mockByDepend1!.GetMocked()!, mockByDepend2!, mockByDepend2!.GetMocked()!,
            depend1!, depend2!, dependMock1!, dependMock2!, dependAuto1!, dependAuto2!,
            mockAuto1!, mockAuto2!,
        }.Should().AllBeNonNull();

        SingletonUserClass mockDep1 = mockByDepend1.GetMocked(), mockDep2 = mockByDepend2.GetMocked();
        new[] {
            singletonMock2!, mock1!.GetMocked()!, mock2!.GetMocked()!,

            obj1!.SingletonProp!, obj1.SingletonField!, obj2!.SingletonProp!, obj2.SingletonField!,
            mockDep1.SingletonProp!, mockDep1.SingletonField!, mockDep2.SingletonProp!, mockDep2.SingletonField!,
            depend1!.SingletonProp!, depend1.SingletonField!, depend2!.SingletonProp!, depend2.SingletonField!,
        }.Should().AllBeSameAs(singletonMock1!);

        if(!callBase)
        {
            new[] {
                obj1.SingletonPropGet!, obj2.SingletonPropGet!, mockDep1.SingletonPropGet!, mockDep2.SingletonPropGet!,
            }.Should().AllBeSameAs(singletonMock1!);
        }
        else
        {
            new[] { obj1!.Class1!, obj1.Class2!, obj2!.Class1!, obj2.Class2!,
                mockDep1.Class1!, mockDep1.Class2!, mockDep2.Class1!, mockDep2.Class2!,
                depend1!.Class1!, depend1.Class2!, depend2!.Class1!, depend2.Class2!,
            }.Should().AllBeSameAs(singletonMock1!);
        }
    }

    [Test]
    public void Test_ClassMarkedSingleton_IsFrozen_WhenNonAutoMock_AutoMockDependencies([Values(true, false)] bool callBase)
    {
        (T t1, T t2) exec<T>(Func<bool?, AutoMockTypeControl?, T> func) => Exec(func, callBase);

        var fixture = new AbstractAutoMockFixture();

        var (singleton1, singleton2) = exec(fixture.CreateWithAutoMockDependencies<SingletonClass>);

        fixture.AutoMockTypeControl.NeverAutoMockTypes.Add(typeof(SingletonClass));

        var (obj1, obj2) = exec(fixture.CreateAutoMock<SingletonUserClass>);
        var (depend1, depend2) = exec(fixture.CreateWithAutoMockDependencies<SingletonUserClass>);
        var (dependMock1, dependMock2) = exec(fixture.CreateWithAutoMockDependencies<AutoMock<SingletonUserClass>>);

        var (dependAuto1, dependAuto2) = exec(fixture.CreateWithAutoMockDependencies<SingletonClass>);
        var (mockAuto1, mockAuto2) = exec(fixture.CreateAutoMock<SingletonClass>);

        singleton1.Should().Should().NotBeNull();
        singleton1.Should().BeAssignableTo<SingletonClass>();

        new object[] {
            singleton1!, singleton2!, obj1!, obj2!,depend1!, depend2!,
            dependMock1!, dependMock1!.GetMocked()!, dependMock2!, dependMock2!.GetMocked()!,
            mockAuto1!, mockAuto2!,
        }.Should().AllBeNonNull();

        ((SingletonClass[])[
            singleton2!, mockAuto1!, mockAuto2!,
            ..GetProps([obj1!, obj2!, dependMock1.Object, dependMock2.Object, depend1!, depend2!],
                    callBase),
        ]).Should().AllBeSameAs(singleton1!);
    }

    [Test]
    public void Test_ClassMarkedSingleton_IsFrozen_WhenAutoMock_UnitFixture([Values(true, false)] bool callBase)
    {
        var fixture = new UnitFixture();

        var singletonMock1 = fixture.CreateAutoMock<SingletonClass>(callBase);

        var (plain1, plain2)            = Exec(fixture.Create<SingletonUserClass>, callBase);
        var (plainMock1, plainMock2)    = Exec(fixture.Create<AutoMock<SingletonClass>>, callBase);

        fixture.AutoMockTypeControl.AlwaysAutoMockTypes.Add(typeof(SingletonClass));

        var (auto1, auto2)              = Exec(fixture.Create<SingletonClass>, callBase);

        new object[] { singletonMock1!, plainMock1!, plainMock2!, plain1!, plain2!, auto1!, auto2! }.Should().AllBeNonNull();

        ((SingletonClass[])[
            auto1!, auto2!, plainMock1!.Object, plainMock2!.Object,
            ..GetProps([plain1!, plain2!], callBase),
        ]).Should().AllBeSameAs(singletonMock1!);
    }

    [Test]
    public void Test_ClassMarkedSingleton_IsFrozen_WhenNotAutoMock_UnitFixture([Values(true, false)] bool callBase)
    {
        var fixture = new UnitFixture();

        var singletonMock1 = fixture.CreateWithAutoMockDependencies<SingletonClass>(callBase);

        var (plain1, plain2) = Exec(fixture.Create<SingletonClass>, callBase);

        fixture.AutoMockTypeControl.NeverAutoMockTypes.Add(typeof(SingletonClass));

        var (plainMock1, plainMock2) = Exec(fixture.Create<AutoMock<SingletonUserClass>>, callBase);
        var (auto1, auto2) = Exec(fixture.Create<SingletonUserClass>, callBase);

        new object[] { singletonMock1!, plainMock1!, plainMock2!, plain1!, plain2!, auto1!, auto2! }.Should().AllBeNonNull();

        ((SingletonClass[])[
            plain1!, plain2!,
            ..GetProps([plainMock1!.Object, plainMock2!.Object, auto1!, auto2!], callBase),
        ]).Should().AllBeSameAs(singletonMock1!);
    }

    [Test]
    public void Test_ClassMarkedSingleton_IsFrozen_WhenAutoMock_NonAutoMockDependencies([Values(true, false)] bool callBase)
    {
        var fixture = new AbstractAutoMockFixture();

        var (singletonMock1, singletonMock2) = Exec(fixture.CreateNonAutoMock<AutoMock<SingletonClass>>, callBase);

        fixture.AutoMockTypeControl.AlwaysAutoMockTypes.Add(typeof(SingletonClass));

        var (depend1, depend2)                  = Exec(fixture.CreateNonAutoMock<SingletonUserClass>,callBase);
        var (dependAuto1, dependAuto2)          = Exec(fixture.CreateNonAutoMock<SingletonClass>,callBase);

        new object[] {
            singletonMock1!.GetMocked(), singletonMock2!.GetMocked(),
            depend1!, depend2!, dependAuto1!, dependAuto2!,
        }.Should().AllBeNonNull();

        ((SingletonClass[])[
            singletonMock2!.Object, dependAuto1!, dependAuto2!,
            ..GetProps([depend1!, depend2!], callBase),
        ]).Should().AllBeSameAs(singletonMock1.Object!);
    }

    [Test]
    public void Test_ClassMarkedSingleton_IsFrozen_WhenNonAutoMock_NonAutoMockDependencies([Values(true, false)] bool callBase)
    {
        var fixture = new AbstractAutoMockFixture();

        var (singleton1, singleton2) = Exec(fixture.CreateNonAutoMock<SingletonClass>, callBase);
        var (depend1, depend2) = Exec(fixture.CreateNonAutoMock<SingletonUserClass>, callBase);

        fixture.AutoMockTypeControl.NeverAutoMockTypes.Add(typeof(SingletonClass));

        var (dependAuto1, dependAuto2) = Exec(fixture.CreateNonAutoMock<AutoMock<SingletonUserClass>>, callBase);

        new object[] {
            singleton1!, singleton2!,
            depend1!, depend2!, dependAuto1!, dependAuto1!.GetMocked()!, dependAuto2!, dependAuto2!.GetMocked()!,
        }.Should().AllBeNonNull();


        ((SingletonClass[])[
            singleton2!,
            ..GetProps([dependAuto1!.Object, dependAuto2!.Object, depend1!, depend2!], callBase),
        ]).Should().AllBeSameAs(singleton1!);
    }

    [Test]
    public void Test_ClassMarkedSingleton_IsFrozen_WhenAutoMock_IntegrationFixture([Values(true, false)] bool callBase)
    {
        var fixture = new IntegrationFixture();

        var singletonMock1 = fixture.CreateNonAutoMock<AutoMock<SingletonClass>>(callBase);

        var (plainMock1, plainMock2) = Exec(fixture.Create<AutoMock<SingletonClass>>, callBase);

        fixture.AutoMockTypeControl.AlwaysAutoMockTypes.Add(typeof(SingletonClass));

        var (plain1, plain2)        = Exec(fixture.Create<SingletonUserClass>, callBase);
        var (auto1, auto2)          = Exec(fixture.Create<SingletonClass>, callBase);

        new object[] { singletonMock1!, plainMock1!, plainMock2!, plain1!, plain2!, auto1!, auto2! }.Should().AllBeNonNull();

        ((SingletonClass[])[
            plainMock1!.Object, plainMock2!.Object, auto1!, auto2!,
            ..GetProps([plain1!, plain2!], callBase),
        ]).Should().AllBeSameAs(singletonMock1!);
    }

    [Test]
    public void Test_ClassMarkedSingleton_IsFrozen_WhenNotAutoMock_IntegrationFixture([Values(true, false)] bool callBase)
    {
        var fixture = new IntegrationFixture();

        var singleton1 = fixture.CreateNonAutoMock<SingletonClass>(callBase);

        var (obj1, obj2) = Exec(fixture.Create<SingletonClass>, callBase);

        fixture.AutoMockTypeControl.NeverAutoMockTypes.Add(typeof(SingletonClass));

        var (depenedMock1, dependMock2) = Exec(fixture.Create<AutoMock<SingletonUserClass>>, callBase);

        new object[] { singleton1!, obj1!, obj2!,
            depenedMock1!, depenedMock1!.GetMocked(), dependMock2!, dependMock2!.GetMocked()!, }.Should().AllBeNonNull();

        ((SingletonClass[])[
            obj1!, obj2!, ..GetProps([depenedMock1!.Object, dependMock2!.Object], callBase),
        ]).Should().AllBeSameAs(singleton1!);
    }

    [Test]
    public void Test_ClassMarkedSingleton_IsDifferent_ByCallBase_WhenAutoMock()
    {
        var fixture = new AbstractAutoMockFixture();
        var mock1 = fixture.CreateAutoMock<SingletonClass>(false);
        var mock2 = fixture.CreateAutoMock<SingletonClass>(true);

        mock1.Should().NotBeNull();
        mock2.Should().NotBeNull();

        mock2.Should().NotBeSameAs(mock1);
    }

    [Test]
    public void Test_ClassMarkedSingleton_IsDifferent_ByCallBase_WhenNotAutoMock()
    {
        var fixture = new AbstractAutoMockFixture();
        var obj1 = fixture.CreateNonAutoMock<SingletonClass>(false);
        var obj2 = fixture.CreateNonAutoMock<SingletonClass>(true);

        obj1.Should().NotBeNull();
        obj2.Should().NotBeNull();

        obj2.Should().NotBeSameAs(obj1);
    }

    [Test]
    public void Test_ClassMarkedSingleton_IsDifferent_ByDepndencyType_WhenAutoMock()
    {
        var fixture = new AbstractAutoMockFixture();
        var mock1 = fixture.CreateWithAutoMockDependencies<AutoMock<SingletonClass>>(false);
        var mock2 = fixture.CreateNonAutoMock<AutoMock<SingletonClass>>(true);

        mock1.Should().NotBeNull();
        mock2.Should().NotBeNull();

        mock2.Should().NotBeSameAs(mock1);
        mock2!.Object.Should().NotBeSameAs(mock1!.Object);
    }

    [Test]
    public void Test_ClassMarkedSingleton_IsDifferent_ByDepndencyType_WhenNotAutoMock()
    {
        var fixture = new AbstractAutoMockFixture();
        var obj1 = fixture.CreateWithAutoMockDependencies<SingletonClass>(false);
        var obj2 = fixture.CreateNonAutoMock<SingletonClass>(true);

        obj1.Should().NotBeNull();
        obj2.Should().NotBeNull();

        obj2.Should().NotBe(obj1);
    }

    #endregion

    #region Freeze Non Singleton

    private NonSingletonClass[] GetProps(NonSingletonUserClass userClass, bool callBase = false) =>
        !callBase && userClass is IAutoMocked ? [userClass.NonSingletonPropGet!, userClass.NonSingletonProp!, userClass.NonSingletonField!]
            : [userClass.Class1, userClass.Class2, userClass.NonSingletonProp!, userClass.NonSingletonField!];


    [Test]
    public void Test_Works_UnitFixture_WithCreateAutoMock()
    {
        var fixture = new UnitFixture();
        var obj1 = fixture.Freeze<AutoMock<NonSingletonClass>>()!.Object;
        var obj2 = fixture.CreateAutoMock<NonSingletonClass>();
        var obj3 = fixture.CreateWithAutoMockDependencies<AutoMock<NonSingletonClass>>()!.Object;
        var obj4 = fixture.CreateAutoMock<NonSingletonClass>();

        obj1.Should().NotBeNull();
        obj2.Should().NotBeNull().And.NotBeSameAs(obj1);
        obj3.Should().NotBeNull().And.BeSameAs(obj1);
        obj4.Should().NotBeNull().And.BeSameAs(obj2);
    }

    public interface IFace { }
    public interface DefualtIFace { public int Test => 10; }
    public class OuterClass
    {
        public IFace? IFace { get; set; }
        public DefualtIFace? DefualtIFace { get; set; }
    }

    [Test]
    [TestCase<UnitFixture>]
    [TestCase<IntegrationFixture>]
    public void Test_Works_WhenIFace<TFixture>() where TFixture: AutoMockFixtureBase, new()
    {
        // For Iface we ignore callbase on itself as long as the call base in general as same
        var fixture = new TFixture();
        var iface = fixture.Freeze<IFace>();
        var ifaceAutoMock = fixture.Freeze<AutoMock<IFace>>();

        var outer = fixture.Create<OuterClass>();
        var outerMockedFreeze = fixture.Freeze<AutoMock<OuterClass>>();
        var outerMocked = fixture.Create<AutoMock<OuterClass>>();

        var outerViaMocked = fixture.CreateAutoMock<OuterClass>();
        var outerViaMocked2 = fixture.CreateAutoMock<OuterClass>();

        iface.Should().BeAutoMock();
        ifaceAutoMock.Should().BeAutoMock();
        ifaceAutoMock!.GetMocked().Should().Be(iface);

        outer.Should().NotBeNull().And.NotBeAutoMock();

        outerMockedFreeze.Should().BeAutoMock();
        outerMockedFreeze!.GetMocked().Should().NotBeNull();

        outerMocked.Should().Be(outerMockedFreeze);

        outerViaMocked.Should().BeAutoMock().And.NotBe(outerMockedFreeze);
        outerViaMocked2.Should().BeAutoMock().And.Be(outerViaMocked);

        new[] {  ifaceAutoMock!.GetMocked(), outer!.IFace,
            outerMockedFreeze!.GetMocked().IFace, outerViaMocked!.IFace, outerViaMocked2!.IFace }.Should().AllBeSameAs(iface);
    }

    [Test]
    public void Test_UnitFixture_Works_WhenDefaultIFace()
    {
        // For default iface we do care on callbase and therefore for Unit fixture there will be a difference between the SUT and dependency
        var fixture = new UnitFixture();

        var defaultIFace = fixture.Freeze<DefualtIFace>();
        var defaultIFaceMock = fixture.Freeze<AutoMock<DefualtIFace>>();

        var outer = fixture.Create<OuterClass>();
        var outerMockedFreeze = fixture.Freeze<AutoMock<OuterClass>>();
        var outerMocked = fixture.Create<AutoMock<OuterClass>>();

        var outerViaMocked = fixture.CreateAutoMock<OuterClass>();
        var outerViaMocked2 = fixture.CreateAutoMock<OuterClass>();

        defaultIFace.Should().BeAutoMock();
        defaultIFaceMock.Should().BeAutoMock();
        defaultIFaceMock!.GetMocked().Should().Be(defaultIFace);

        outer.Should().NotBeNull().And.NotBeAutoMock();

        outerMockedFreeze.Should().BeAutoMock();
        outerMockedFreeze!.GetMocked().Should().NotBeNull();

        outerMocked.Should().Be(outerMockedFreeze);

        outerViaMocked.Should().BeAutoMock().And.NotBe(outerMockedFreeze);
        outerViaMocked2.Should().BeAutoMock().And.Be(outerViaMocked);

        new[] { outer!.DefualtIFace, outerMockedFreeze!.GetMocked().DefualtIFace, outerViaMocked!.DefualtIFace }
                    .Should().AllNotBeSameAs(defaultIFace);
    }

    public class Test { }
    public class SubTest : Test { }
    [Test]
    public void Test_DoesNotFreezeSubclass()
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.Freeze<Test>();
        var t1 = fixture.CreateNonAutoMock<SubTest>();
        var t2 = fixture.CreateNonAutoMock(typeof(Test));
        t1.Should().NotBeSameAs(t2);
    }

    [Test]
    public void Test_Works_IntegrationFixture_WithCreateAutoMock()
    {
        var fixture = new IntegrationFixture();
        var obj1 = fixture.Freeze<AutoMock<NonSingletonClass>>()!.Object;
        var obj2 = fixture.CreateAutoMock<NonSingletonClass>();
        var obj3 = fixture.CreateNonAutoMock<AutoMock<NonSingletonClass>>()!.Object;

        obj1.Should().NotBeNull();
        obj2.Should().NotBeNull().And.BeSameAs(obj1);
        obj3.Should().NotBeNull().And.BeSameAs(obj1);
    }

    [Test]
    public void Test_Works_UnitFixture_When_AlwaysAutoMock()
    {
        var fixture = new UnitFixture();
        fixture.AutoMockTypeControl.AlwaysAutoMockTypes.Add(typeof(NonSingletonClass));

        var frozen = fixture.Freeze<AutoMock<NonSingletonClass>>()!.Object;
        var obj = fixture.CreateWithAutoMockDependencies<NonSingletonUserClass>();
        var obj2 = fixture.CreateWithAutoMockDependencies<NonSingletonUserClass>();

        frozen.Should().NotBeNull();
        obj.Should().NotBeNull();
        obj2.Should().NotBeNull();

        obj!.Class1.Should().NotBeSameAs(frozen);
        new[] {
            obj!.Class1, obj!.Class2, obj!.NonSingletonProp, obj!.NonSingletonField,
            obj2!.Class1, obj2!.Class2, obj2!.NonSingletonProp, obj2!.NonSingletonField,
        }.Should().OnlyContain(x => x != frozen);

        new[] {
            obj!.Class2, obj!.NonSingletonProp, obj!.NonSingletonField,
            obj2.Class1, obj2!.Class2, obj2!.NonSingletonProp, obj2!.NonSingletonField,
        }.Should().OnlyContain(x => x == obj.Class1);
    }

    [Test]
    public void Test_Works_UnitFixture_When_NeverAutoMock()
    {
        var fixture = new UnitFixture();
        fixture.AutoMockTypeControl.NeverAutoMockTypes.Add(typeof(NonSingletonClass));

        var frozen = fixture.Freeze<NonSingletonClass>();
        var obj = fixture.CreateWithAutoMockDependencies<NonSingletonUserClass>();

        frozen.Should().NotBeNull();
        obj.Should().NotBeNull();
        obj!.Class1.Should().Be(frozen);
        obj!.Class2.Should().Be(frozen);
        obj!.NonSingletonProp.Should().Be(frozen);
        obj!.NonSingletonField.Should().Be(frozen);
    }

    [Test]
    public void Test_Works_IntegrationFixture_When_AlwaysAutoMock()
    {
        var fixture = new IntegrationFixture();
        fixture.AutoMockTypeControl.AlwaysAutoMockTypes.Add(typeof(NonSingletonClass));

        var frozen = fixture.Freeze<AutoMock<NonSingletonClass>>()!.Object;
        var obj = fixture.CreateNonAutoMock<NonSingletonUserClass>();

        frozen.Should().NotBeNull();
        obj.Should().NotBeNull();

        GetProps(obj!).Should().AllBeSameAs(frozen!);
    }

    [Test]
    public void Test_Works_IntegrationFixture_When_NeverAutoMock()
    {
        var fixture = new IntegrationFixture();
        fixture.AutoMockTypeControl.NeverAutoMockTypes.Add(typeof(NonSingletonClass));

        var frozen = fixture.Freeze<NonSingletonClass>();
        var obj = fixture.CreateNonAutoMock<NonSingletonUserClass>();

        frozen.Should().NotBeNull();
        obj.Should().NotBeNull();

        GetProps(obj!).Should().AllBeSameAs(frozen!);
    }


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

        GetProps(obj1!, true).Should().AllBeSameAs(obj1.Class1!);

        obj2.Should().NotBeNull();
        GetProps(obj2!, true).Should().AllBeSameAs(obj1.Class1!);

        nonSingletonMock1.Should().Be(obj1.Class1);
        nonSngletonMock2.Should().Be(obj1.Class1);

        mock1.Should().NotBeNull();
        mock2.Should().NotBeNull();
        mock1!.GetMocked().Should().Be(obj1.Class1);
        mock2!.GetMocked().Should().Be(obj1.Class1);

        depend1.Should().NotBeNull();
        GetProps(depend1!, true).Should().AllBeSameAs(obj1.Class1!);

        depend2.Should().NotBeNull();
        GetProps(depend2!, true).Should().AllBeSameAs(obj1.Class1!);
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
    public void Test_Freeze_NonSingleton_AutoMock_IsDifferent_ByCallBase()
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.JustFreeze<NonSingletonClass>();
        var obj1 = fixture.CreateAutoMock<NonSingletonClass>(false);
        var obj12 = fixture.CreateAutoMock<NonSingletonClass>(false);

        var obj2 = fixture.CreateAutoMock<NonSingletonClass>(true);
        var obj22 = fixture.CreateAutoMock<NonSingletonClass>(true);

        obj1.Should().NotBeNull();
        obj12.Should().NotBeNull();
        obj2.Should().NotBeNull();
        obj22.Should().NotBeNull();

        obj2.Should().NotBeSameAs(obj1);
        obj2.Should().NotBeSameAs(obj1);

        obj12.Should().BeSameAs(obj1);
        obj22.Should().BeSameAs(obj2);
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
