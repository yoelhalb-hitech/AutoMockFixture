using AutoMockFixture.FixtureUtils;
using FluentAssertions.Collections;
using FluentAssertions.Execution;
using SequelPay.DotNetPowerExtensions;
using static AutoMockFixture.Tests.AutoMockFixture_Tests.Freeze_Tests;

namespace AutoMockFixture.Tests.AutoMockFixture_Tests;

internal static class FreezeExtensions
{
    [CustomAssertion]
    public static AndConstraint<GenericCollectionAssertions<SingletonClass>> AllBe(
                    this GenericCollectionAssertions<SingletonClass> assertions, SingletonClass expected)
    {
        if (expected is not null) assertions.AllNonNull(); // This way the message will be clearer

        CollectionAssertions.ExecuteInternal(assertions, item => object.ReferenceEquals(item, expected),
                                        $"not the same as expected");

        return new AndConstraint<GenericCollectionAssertions<SingletonClass>>(assertions);
    }
}
internal class Freeze_Tests
{
    #region Singleton

    private (T t1, T t2) Exec<T>(Func<bool?, AutoMockTypeControl?, T> func, bool callBase) => (func(callBase, null), func(callBase, null));
    private SingletonClass[] GetProps(SingletonUserClass userClass, bool callBase) =>
        !callBase ? [userClass.SingletonProp!, userClass.SingletonField!]
            : [userClass.Class1, userClass.Class2, userClass.SingletonPropGet!, userClass.SingletonProp!, userClass.SingletonField!];

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
        }.Should().AllNonNull();

        SingletonUserClass mockDep1 = mockByDepend1.GetMocked(), mockDep2 = mockByDepend2.GetMocked();
        new[] {
            singletonMock2!, mock1!.GetMocked()!, mock2!.GetMocked()!,

            obj1!.SingletonProp!, obj1.SingletonField!, obj2!.SingletonProp!, obj2.SingletonField!,
            mockDep1.SingletonProp!, mockDep1.SingletonField!, mockDep2.SingletonProp!, mockDep2.SingletonField!,
            depend1!.SingletonProp!, depend1.SingletonField!, depend2!.SingletonProp!, depend2.SingletonField!,
        }.Should().AllBe(singletonMock1!);

        if(!callBase)
        {
            new[] {
                obj1.SingletonPropGet!, obj2.SingletonPropGet!, mockDep1.SingletonPropGet!, mockDep2.SingletonPropGet!,
            }.Should().AllBe(singletonMock1!);
        }
        else
        {
            new[] { obj1!.Class1!, obj1.Class2!, obj2!.Class1!, obj2.Class2!,
                mockDep1.Class1!, mockDep1.Class2!, mockDep2.Class1!, mockDep2.Class2!,
                depend1!.Class1!, depend1.Class2!, depend2!.Class1!, depend2.Class2!,
            }.Should().AllBe(singletonMock1!);
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
        }.Should().AllNonNull();

        SingletonUserClass mockDep1 = dependMock1.GetMocked(), mockDep2 = dependMock2.GetMocked();
        new[] {
            obj1!.SingletonProp!, obj1.SingletonField!,
            obj2!.SingletonProp!, obj2.SingletonField!,
            mockDep1.SingletonProp!, mockDep1.SingletonField!,
            mockDep2.SingletonProp!,  mockDep2.SingletonField!,
            depend1!.SingletonProp!, depend1.SingletonField!,
            depend2!.SingletonProp!, depend2.SingletonField!,
        }.Should().AllBe(singleton1!);

        if (!callBase)
        {
            new[] {
                obj1.SingletonPropGet!, obj2.SingletonPropGet!, mockDep1.SingletonPropGet!, mockDep2.SingletonPropGet!,
            }.Should().AllBe(singleton1!);
        }
        else
        {
            new[] { obj1!.Class1!, obj1.Class2!, obj2!.Class1!, obj2.Class2!,
                mockDep1.Class1!, mockDep1.Class2!, mockDep2.Class1!, mockDep2.Class2!,
                depend1!.Class1!, depend1.Class2!, depend2!.Class1!, depend2.Class2!,
            }.Should().AllBe(singleton1!);
        }
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

        new object[] { singletonMock1!, plainMock1!, plainMock2!, plain1!, plain2!, auto1!, auto2! }.Should().AllNonNull();

        new[] {
            plainMock1!.GetMocked(), plainMock2!.GetMocked(),
            plain1!.SingletonProp!, plain1.SingletonField!,
            plain2!.SingletonProp!, plain2.SingletonField!,
            auto1!, auto2!,
        }.Should().AllBe(singletonMock1!);

        if (callBase)
        {
            new[] { plain1!.Class1!, plain1.Class2!, plain2!.Class1!, plain2.Class2!, }.Should().AllBe(singletonMock1!);
        }
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

        new object[] { singletonMock1!, plainMock1!, plainMock2!, plain1!, plain2!, auto1!, auto2! }.Should().AllNonNull();

        SingletonUserClass mockDep1 = plainMock1!.GetMocked(), mockDep2 = plainMock2!.GetMocked();
        new[] {
            plain1!, plain2!,
            mockDep1.SingletonProp!, mockDep1.SingletonField!,
            mockDep2.SingletonProp!, mockDep2.SingletonField!,
            auto1!.SingletonProp!, auto1.SingletonField!,
            auto2!.SingletonProp!, auto2.SingletonField!,
        }.Should().AllBe(singletonMock1!);

        if (!callBase)
        {
            new[] { mockDep1.SingletonPropGet!, mockDep2.SingletonPropGet!, }.Should().AllBe(singletonMock1!);
        }
        else
        {
            new[] { mockDep1.Class1!, mockDep1.Class2!, mockDep2.Class1!, mockDep2.Class2!, }.Should().AllBe(singletonMock1!);
        }
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
        }.Should().AllNonNull();

        new[] {
            singletonMock2!.GetMocked(), dependAuto1!, dependAuto2!,

            depend1!.Class1!, depend1.Class2!, depend1.SingletonProp!, depend1.SingletonField!,
            depend2!.Class1!, depend2.Class2!, depend2.SingletonProp!, depend2.SingletonField!,
        }.Should().AllBe(singletonMock1!.GetMocked());
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
        }.Should().AllNonNull();

        SingletonUserClass mockDep1 = dependAuto1!.GetMocked(), mockDep2 = dependAuto2!.GetMocked();
        new[] {
            singleton2!,
            mockDep1.SingletonProp!, mockDep1.SingletonField!,
            mockDep2.SingletonProp!, mockDep2.SingletonField!,
            depend1!.SingletonProp!, depend1.SingletonField!,
            depend2!.SingletonProp!, depend2.SingletonField!,
        }.Should().AllBe(singleton1!);

        if (!callBase)
        {
            new[] { mockDep1.SingletonPropGet!, mockDep2.SingletonPropGet!, }.Should().AllBe(singleton1!);
        }
        else
        {
            new[] {  mockDep1.Class1!, mockDep1.Class2!, mockDep2.Class1!, mockDep2.Class2!,
                    depend1!.Class1!, depend1.Class2!, depend2!.Class1!, depend2.Class2!,
            }.Should().AllBe(singleton1!);
        }
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

        new object[] { singletonMock1!, plainMock1!, plainMock2!, plain1!, plain2!, auto1!, auto2! }.Should().AllNonNull();

        new[] {
            plainMock1!.GetMocked(), plainMock2!.GetMocked(),
            plain1!.Class1!, plain1.Class2!, plain1.SingletonProp!, plain1.SingletonField!,
            plain2!.Class1!, plain2.Class2!, plain2.SingletonProp!, plain2.SingletonField!,
            auto1!, auto2!,
        }.Should().AllBe(singletonMock1!);
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
            depenedMock1!, depenedMock1!.GetMocked(), dependMock2!, dependMock2!.GetMocked()!, }.Should().AllNonNull();

        SingletonUserClass depend1 = depenedMock1!.GetMocked(), depend2 = dependMock2!.GetMocked();
        new[] {
            obj1!, obj2!,
            depend1.SingletonProp!, depend1.SingletonField!,
            depend2.SingletonProp!, depend2.SingletonField!,
        }.Should().AllBe(singleton1!);

        if (!callBase)
        {
            new[] { depend1.SingletonPropGet!, depend2.SingletonPropGet!, }.Should().AllBe(singleton1!);
        }
        else
        {
            new[] { depend1!.Class1!, depend1.Class2!, depend2!.Class1!, depend2.Class2!, }.Should().AllBe(singleton1!);
        }
    }

    [Test]
    public void Test_ClassMarkedSingleton_IsDifferent_ByCallBase_WhenAutoMock()
    {
        var fixture = new AbstractAutoMockFixture();
        var mock1 = fixture.CreateAutoMock<SingletonClass>(false);
        var mock2 = fixture.CreateAutoMock<SingletonClass>(true);

        mock1.Should().NotBeNull();
        mock2.Should().NotBeNull();

        mock2.Should().NotBe(mock1);
    }

    [Test]
    public void Test_ClassMarkedSingleton_IsDifferent_ByCallBase_WhenNotAutoMock()
    {
        var fixture = new AbstractAutoMockFixture();
        var obj1 = fixture.CreateNonAutoMock<SingletonClass>(false);
        var obj2 = fixture.CreateNonAutoMock<SingletonClass>(true);

        obj1.Should().NotBeNull();
        obj2.Should().NotBeNull();

        obj2.Should().NotBe(obj1);
    }

    [Test]
    public void Test_ClassMarkedSingleton_IsDifferent_ByDepndencyType_WhenAutoMock()
    {
        var fixture = new AbstractAutoMockFixture();
        var mock1 = fixture.CreateWithAutoMockDependencies<AutoMock<SingletonClass>>(false);
        var mock2 = fixture.CreateNonAutoMock<AutoMock<SingletonClass>>(true);

        mock1.Should().NotBeNull();
        mock2.Should().NotBeNull();

        mock2.Should().NotBe(mock1);
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

    #region Freeze

    [Test]
    public void Test_Works_UnitFixture_WithCreateAutoMock()
    {
        var fixture = new UnitFixture();
        var obj1 = fixture.Freeze<AutoMock<NonSingletonClass>>()!.Object;
        var obj2 = fixture.CreateAutoMock<NonSingletonClass>();
        var obj3 = fixture.CreateWithAutoMockDependencies<AutoMock<NonSingletonClass>>()!.Object;

        obj1.Should().NotBeNull();
        obj2.Should().NotBeNull();
        obj3.Should().NotBeNull();
        obj1.Should().BeSameAs(obj2);
        obj1.Should().BeSameAs(obj3);
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
        obj2.Should().NotBeNull();
        obj3.Should().NotBeNull();
        AutoMock.Get(obj1).CallBase.Should().BeFalse();
        AutoMock.Get(obj2).CallBase.Should().BeFalse();
        AutoMock.Get(obj3).CallBase.Should().BeFalse();
        obj1.Should().BeSameAs(obj2);
        obj1.Should().BeSameAs(obj3);
    }

    [Test]
    public void Test_Works_UnitFixture_When_AlwaysAutoMock()
    {
        var fixture = new UnitFixture();
        fixture.AutoMockTypeControl.AlwaysAutoMockTypes.Add(typeof(NonSingletonClass));

        var frozen = fixture.Freeze<AutoMock<NonSingletonClass>>()!.Object;
        var obj = fixture.CreateWithAutoMockDependencies<NonSingletonUserClass>();

        frozen.Should().NotBeNull();
        obj.Should().NotBeNull();
        obj!.Class1.Should().Be(frozen);
        obj!.Class2.Should().Be(frozen);
        obj!.NonSingletonProp.Should().Be(frozen);
        obj!.NonSingletonField.Should().Be(frozen);
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
        obj!.Class1.Should().Be(frozen);
        obj!.Class2.Should().Be(frozen);
        obj!.NonSingletonProp.Should().Be(frozen);
        obj!.NonSingletonField.Should().Be(frozen);
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
        obj!.Class1.Should().Be(frozen);
        obj!.Class2.Should().Be(frozen);
        obj!.NonSingletonProp.Should().Be(frozen);
        obj!.NonSingletonField.Should().Be(frozen);
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
