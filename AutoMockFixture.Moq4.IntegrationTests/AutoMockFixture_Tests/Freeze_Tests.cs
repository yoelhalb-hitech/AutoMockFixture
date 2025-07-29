using AutoFixture;
using AutoMockFixture.FixtureUtils;
using AutoMockFixture.NUnit3;
using SequelPay.DotNetPowerExtensions;

namespace AutoMockFixture.Tests.AutoMockFixture_Tests;

internal class Freeze_Tests
{
    private T[] GetProps<T>(UserClass<T> userClass, bool callBase) =>
        !callBase && userClass is IAutoMocked
            ? [userClass.ReadOnlyProp!, userClass.ReadWriteProp!, userClass.Field!]
            : [userClass.Arg1, userClass.Arg2, userClass.ReadWriteProp!, userClass.Field!];
    private T[] GetProps<T>(UserClass<T>[] objs, bool callBase) =>
        objs.SelectMany(o => GetProps(o, callBase)).ToArray();

    #region Singleton

    [Test]
    [TestCase<UnitFixture>(true)]
    [TestCase<UnitFixture>(false)]
    [TestCase<IntegrationFixture>(true)]
    [TestCase<IntegrationFixture>(false)]
    public void Test_ClassMarkedSingleton_IsFrozen_WhenAutoMock_AllFixtures<TFixture>(bool callBase)
        where TFixture : AutoMockFixtureBase, new()
    {
        using var fixture = new TFixture();

        var typeControl = new AutoMockTypeControl
        {
            AlwaysAutoMockTypes = [typeof(SingletonClass)]
        };

        var firstWhenMockDependencies = fixture.CreateWithAutoMockDependencies<AutoMock<SingletonClass>>(callBase)!.Object!;
        firstWhenMockDependencies.Should().NotBeNull();
        ((SingletonClass[])[
                fixture.CreateWithAutoMockDependencies<SingletonClass>(callBase, typeControl)!,
                fixture.CreateWithAutoMockDependenciesAsync<SingletonClass>(callBase, typeControl).Result!,

                fixture.CreateWithAutoMockDependencies<AutoMock<SingletonClass>>(callBase)!.Object,
                fixture.CreateWithAutoMockDependenciesAsync<AutoMock<SingletonClass>>(callBase).Result!,

                ..GetProps(fixture.CreateWithAutoMockDependencies<UserClass<SingletonClass>>(callBase)!, callBase),
                ..GetProps(fixture.CreateWithAutoMockDependenciesAsync<UserClass<SingletonClass>>(callBase)!.Result!, callBase),

                ..GetProps(fixture.CreateWithAutoMockDependencies<AutoMock<UserClass<SingletonClass>>>(callBase)!.Object, callBase),
                ..GetProps(fixture.CreateWithAutoMockDependenciesAsync<AutoMock<UserClass<SingletonClass>>>(callBase)!.Result!.Object, callBase),

                ..GetProps(fixture.CreateWithAutoMockDependencies<UserClass<AutoMock<SingletonClass>>>(callBase)!, callBase),
                ..GetProps(fixture.CreateWithAutoMockDependenciesAsync<UserClass<AutoMock<SingletonClass>>>(callBase)!.Result!, callBase),

                ..GetProps(fixture.CreateWithAutoMockDependencies<AutoMock<UserClass<AutoMock<SingletonClass>>>>(callBase)!.Object, callBase),
                ..GetProps(fixture.CreateWithAutoMockDependenciesAsync<AutoMock<UserClass<AutoMock<SingletonClass>>>>(callBase)!.Result!.Object, callBase),
            ]).Should().AllBeSameAs(firstWhenMockDependencies);

        var firstWhenNoMockDependencies = fixture.CreateNonAutoMock<SingletonClass>(callBase, typeControl)!;
        firstWhenNoMockDependencies.Should().NotBeNull().And.Should().NotBe(firstWhenMockDependencies);
        ((SingletonClass[])[
                fixture.CreateNonAutoMock<SingletonClass>(callBase, typeControl)!,
                fixture.CreateNonAutoMockAsync<SingletonClass>(callBase, typeControl).Result!,

                fixture.CreateNonAutoMock<AutoMock<SingletonClass>>(callBase)!.Object,
                fixture.CreateNonAutoMockAsync<AutoMock<SingletonClass>>(callBase).Result!,

                ..GetProps(fixture.CreateNonAutoMock<UserClass<SingletonClass>>(callBase, typeControl)!, callBase),
                ..GetProps(fixture.CreateNonAutoMockAsync<UserClass<SingletonClass>>(callBase, typeControl)!.Result!, callBase),

                ..GetProps(fixture.CreateNonAutoMock<AutoMock<UserClass<SingletonClass>>>(callBase, typeControl)!.Object, callBase),
                ..GetProps(fixture.CreateNonAutoMockAsync<AutoMock<UserClass<SingletonClass>>>(callBase, typeControl)!.Result!.Object, callBase),

                ..GetProps(fixture.CreateNonAutoMock<UserClass<AutoMock<SingletonClass>>>(callBase)!, callBase),
                ..GetProps(fixture.CreateNonAutoMockAsync<UserClass<AutoMock<SingletonClass>>>(callBase)!.Result!, callBase),

                ..GetProps(fixture.CreateNonAutoMock<AutoMock<UserClass<AutoMock<SingletonClass>>>>(callBase)!.Object, callBase),
                ..GetProps(fixture.CreateNonAutoMockAsync<AutoMock<UserClass<AutoMock<SingletonClass>>>>(callBase)!.Result!.Object, callBase),
            ]).Should().AllBeSameAs(firstWhenNoMockDependencies);
    }

    [Test]
    public void Test_ClassMarkedSingleton_IsFrozen_WhenAutoMock_UnitFixtureSpecific([Values(true, false)] bool callBase)
    {
        using var fixture = new UnitFixture();

        var typeControl = new AutoMockTypeControl
        {
            AlwaysAutoMockTypes = [typeof(SingletonClass)]
        };

        var firstWhenMockDependencies = fixture.CreateWithAutoMockDependencies<AutoMock<SingletonClass>>(callBase)!.Object!;
        firstWhenMockDependencies.Should().NotBeNull();
        ((SingletonClass[])[
                fixture.CreateAutoMock<SingletonClass>(callBase)!,
                fixture.CreateAutoMockAsync<SingletonClass>(callBase).Result!,

                fixture.CreateAutoMock<AutoMock<SingletonClass>>(callBase)!.Object,
                fixture.CreateAutoMockAsync<AutoMock<SingletonClass>>(callBase)!.Result!.Object,

                ..GetProps(fixture.CreateAutoMock<UserClass<SingletonClass>>(callBase)!, callBase),
                ..GetProps(fixture.CreateAutoMockAsync<UserClass<SingletonClass>>(callBase)!.Result!, callBase),

                ..GetProps(fixture.CreateAutoMock<AutoMock<UserClass<SingletonClass>>>(callBase)!.Object, callBase),
                ..GetProps(fixture.CreateAutoMockAsync<AutoMock<UserClass<SingletonClass>>>(callBase)!.Result!.Object, callBase),

                ..GetProps(fixture.CreateAutoMock<UserClass<AutoMock<SingletonClass>>>(callBase)!, callBase),
                ..GetProps(fixture.CreateAutoMockAsync<UserClass<AutoMock<SingletonClass>>>(callBase)!.Result!, callBase),

                ..GetProps(fixture.CreateAutoMock<AutoMock<UserClass<AutoMock<SingletonClass>>>>(callBase)!.Object, callBase),
                ..GetProps(fixture.CreateAutoMockAsync<AutoMock<UserClass<AutoMock<SingletonClass>>>>(callBase)!.Result!.Object, callBase),

                fixture.Create<SingletonClass>(callBase, typeControl)!,
                fixture.CreateAsync<SingletonClass>(callBase, typeControl).Result!,

                fixture.Create<AutoMock<SingletonClass>>(callBase)!.Object,
                fixture.CreateAsync<AutoMock<SingletonClass>>(callBase).Result!,

                ..GetProps(fixture.Create<UserClass<SingletonClass>>(callBase)!, callBase),
                ..GetProps(fixture.CreateAsync<UserClass<SingletonClass>>(callBase)!.Result!, callBase),

                ..GetProps(fixture.Create<AutoMock<UserClass<SingletonClass>>>(callBase)!.Object, callBase),
                ..GetProps(fixture.CreateAsync<AutoMock<UserClass<SingletonClass>>>(callBase)!.Result!.Object, callBase),

                ..GetProps(fixture.Create<UserClass<AutoMock<SingletonClass>>>(callBase)!, callBase),
                ..GetProps(fixture.CreateAsync<UserClass<AutoMock<SingletonClass>>>(callBase)!.Result!, callBase),

                ..GetProps(fixture.Create<AutoMock<UserClass<AutoMock<SingletonClass>>>>(callBase)!.Object, callBase),
                ..GetProps(fixture.CreateAsync<AutoMock<UserClass<AutoMock<SingletonClass>>>>(callBase)!.Result!.Object, callBase),

                fixture.Freeze<AutoMock<SingletonClass>>(callBase)!.Object,
                fixture.FreezeAsync<AutoMock<SingletonClass>>(callBase).Result!,

                ..GetProps(fixture.Freeze<UserClass<SingletonClass>>(callBase)!, callBase),
                ..GetProps(fixture.FreezeAsync<UserClass<SingletonClass>>(callBase)!.Result!, callBase),

                ..GetProps(fixture.Freeze<AutoMock<UserClass<SingletonClass>>>(callBase)!.Object, callBase),
                ..GetProps(fixture.FreezeAsync<AutoMock<UserClass<SingletonClass>>>(callBase)!.Result!.Object, callBase),

                ..GetProps(fixture.Freeze<UserClass<AutoMock<SingletonClass>>>(callBase)!, callBase),
                ..GetProps(fixture.FreezeAsync<UserClass<AutoMock<SingletonClass>>>(callBase)!.Result!, callBase),

                ..GetProps(fixture.Freeze<AutoMock<UserClass<AutoMock<SingletonClass>>>>(callBase)!.Object, callBase),
                ..GetProps(fixture.FreezeAsync<AutoMock<UserClass<AutoMock<SingletonClass>>>>(callBase)!.Result!.Object, callBase),
            ]).Should().AllBeSameAs(firstWhenMockDependencies);
    }

    [Test]
    public void Test_ClassMarkedSingleton_IsFrozen_WhenAutoMock_IntegrationFixtureSpecific([Values(true, false)] bool callBase)
    {
        using var fixture = new IntegrationFixture();

        var typeControl = new AutoMockTypeControl
        {
            AlwaysAutoMockTypes = [typeof(SingletonClass)]
        };

        var firstWhenNoMockDependencies = fixture.CreateNonAutoMock<AutoMock<SingletonClass>>(callBase)!.Object!;
        firstWhenNoMockDependencies.Should().NotBeNull();
        ((SingletonClass[])[
                fixture.CreateAutoMock<SingletonClass>(callBase)!,
                fixture.CreateAutoMockAsync<SingletonClass>(callBase).Result!,

                fixture.CreateAutoMock<AutoMock<SingletonClass>>(callBase)!.Object,
                fixture.CreateAutoMockAsync<AutoMock<SingletonClass>>(callBase)!.Result!.Object,

                ..GetProps(fixture.CreateAutoMock<UserClass<SingletonClass>>(callBase, typeControl)!, callBase),
                ..GetProps(fixture.CreateAutoMockAsync<UserClass<SingletonClass>>(callBase, typeControl)!.Result!, callBase),

                ..GetProps(fixture.CreateAutoMock<AutoMock<UserClass<SingletonClass>>>(callBase, typeControl)!.Object, callBase),
                ..GetProps(fixture.CreateAutoMockAsync<AutoMock<UserClass<SingletonClass>>>(callBase, typeControl)!.Result!.Object, callBase),

                ..GetProps(fixture.CreateAutoMock<UserClass<AutoMock<SingletonClass>>>(callBase)!, callBase),
                ..GetProps(fixture.CreateAutoMockAsync<UserClass<AutoMock<SingletonClass>>>(callBase)!.Result!, callBase),

                ..GetProps(fixture.CreateAutoMock<AutoMock<UserClass<AutoMock<SingletonClass>>>>(callBase)!.Object, callBase),
                ..GetProps(fixture.CreateAutoMockAsync<AutoMock<UserClass<AutoMock<SingletonClass>>>>(callBase)!.Result!.Object, callBase),

                fixture.Create<SingletonClass>(callBase, typeControl)!,
                fixture.CreateAsync<SingletonClass>(callBase, typeControl).Result!,

                fixture.Create<AutoMock<SingletonClass>>(callBase)!.Object,
                fixture.CreateAsync<AutoMock<SingletonClass>>(callBase).Result!,

                ..GetProps(fixture.Create<UserClass<SingletonClass>>(callBase, typeControl)!, callBase),
                ..GetProps(fixture.CreateAsync<UserClass<SingletonClass>>(callBase, typeControl)!.Result!, callBase),

                ..GetProps(fixture.Create<AutoMock<UserClass<SingletonClass>>>(callBase, typeControl)!.Object, callBase),
                ..GetProps(fixture.CreateAsync<AutoMock<UserClass<SingletonClass>>>(callBase, typeControl)!.Result!.Object, callBase),

                ..GetProps(fixture.Create<UserClass<AutoMock<SingletonClass>>>(callBase)!, callBase),
                ..GetProps(fixture.CreateAsync<UserClass<AutoMock<SingletonClass>>>(callBase)!.Result!, callBase),

                ..GetProps(fixture.Create<AutoMock<UserClass<AutoMock<SingletonClass>>>>(callBase)!.Object, callBase),
                ..GetProps(fixture.CreateAsync<AutoMock<UserClass<AutoMock<SingletonClass>>>>(callBase)!.Result!.Object, callBase),

                fixture.Freeze<AutoMock<SingletonClass>>(callBase)!.Object,
                fixture.FreezeAsync<AutoMock<SingletonClass>>(callBase).Result!,

                ..GetProps(fixture.Freeze<UserClass<AutoMock<SingletonClass>>>(callBase)!, callBase),
                ..GetProps(fixture.FreezeAsync<UserClass<AutoMock<SingletonClass>>>(callBase)!.Result!, callBase),

                ..GetProps(fixture.Freeze<AutoMock<UserClass<AutoMock<SingletonClass>>>>(callBase)!.Object, callBase),
                ..GetProps(fixture.FreezeAsync<AutoMock<UserClass<AutoMock<SingletonClass>>>>(callBase)!.Result!.Object, callBase),
            ]).Should().AllBeSameAs(firstWhenNoMockDependencies);
    }

    [Test]
    [TestCase<UnitFixture>(true)]
    [TestCase<UnitFixture>(false)]
    [TestCase<IntegrationFixture>(true)]
    [TestCase<IntegrationFixture>(false)]
    public void Test_ClassMarkedSingleton_IsFrozen_WhenNonAutoMock_AllFixtures<TFixture>(bool callBase)
        where TFixture : AutoMockFixtureBase, new()
    {
        using var fixture = new TFixture();

        var typeControl = new AutoMockTypeControl
        {
            NeverAutoMockTypes = [typeof(SingletonClass)]
        };

        var firstWhenMockDependencies = fixture.CreateWithAutoMockDependencies<SingletonClass>(callBase)!;

        ((SingletonClass[])[
                fixture.CreateWithAutoMockDependencies<SingletonClass>(callBase)!,
                fixture.CreateWithAutoMockDependenciesAsync<SingletonClass>(callBase).Result!,

                ..GetProps(fixture.CreateWithAutoMockDependencies<UserClass<SingletonClass>>(callBase, typeControl)!, callBase),
                ..GetProps(fixture.CreateWithAutoMockDependenciesAsync<UserClass<SingletonClass>>(callBase, typeControl)!.Result!, callBase),

                ..GetProps(fixture.CreateWithAutoMockDependencies<AutoMock<UserClass<SingletonClass>>>(callBase, typeControl)!.Object, callBase),
                ..GetProps(fixture.CreateWithAutoMockDependenciesAsync<AutoMock<UserClass<SingletonClass>>>(callBase, typeControl)!.Result!.Object, callBase),
            ]).Should().AllBeSameAs(firstWhenMockDependencies);

        var firstWhenNoMockDependencies = fixture.CreateNonAutoMock<SingletonClass>(callBase)!;
        firstWhenNoMockDependencies.Should().NotBeNull().And.Should().NotBe(firstWhenMockDependencies);
        ((SingletonClass[])[
                fixture.CreateNonAutoMock<SingletonClass>(callBase)!,
                fixture.CreateNonAutoMockAsync<SingletonClass>(callBase).Result!,

                ..GetProps(fixture.CreateNonAutoMock<UserClass<SingletonClass>>(callBase)!, callBase),
                ..GetProps(fixture.CreateNonAutoMockAsync<UserClass<SingletonClass>>(callBase)!.Result!, callBase),

                ..GetProps(fixture.CreateNonAutoMock<AutoMock<UserClass<SingletonClass>>>(callBase)!.Object, callBase),
                ..GetProps(fixture.CreateNonAutoMockAsync<AutoMock<UserClass<SingletonClass>>>(callBase)!.Result!.Object, callBase),
            ]).Should().AllBeSameAs(firstWhenNoMockDependencies);
    }

    [Test]
    public void Test_ClassMarkedSingleton_IsFrozen_WhenNonAutoMock_UnitFixtureSpecific([Values(true, false)] bool callBase)
    {
        using var fixture = new UnitFixture();

        var typeControl = new AutoMockTypeControl
        {
            NeverAutoMockTypes = [typeof(SingletonClass)]
        };

        var firstWhenMockDependencies = fixture.CreateWithAutoMockDependencies<SingletonClass>(callBase)!;

        ((SingletonClass[])[
                fixture.CreateAutoMock<SingletonClass>(callBase, typeControl)!,
                fixture.CreateAutoMockAsync<SingletonClass>(callBase, typeControl).Result!,

                ..GetProps(fixture.CreateAutoMock<UserClass<SingletonClass>>(callBase, typeControl)!, callBase),
                ..GetProps(fixture.CreateAutoMockAsync<UserClass<SingletonClass>>(callBase, typeControl)!.Result!, callBase),

                ..GetProps(fixture.CreateAutoMock<AutoMock<UserClass<SingletonClass>>>(callBase, typeControl)!.Object, callBase),
                ..GetProps(fixture.CreateAutoMockAsync<AutoMock<UserClass<SingletonClass>>>(callBase, typeControl)!.Result!.Object, callBase),

                fixture.Create<SingletonClass>(callBase)!,
                fixture.CreateAsync<SingletonClass>(callBase).Result!,

                ..GetProps(fixture.Create<UserClass<SingletonClass>>(callBase, typeControl)!, callBase),
                ..GetProps(fixture.CreateAsync<UserClass<SingletonClass>>(callBase, typeControl)!.Result!, callBase),

                ..GetProps(fixture.Create<AutoMock<UserClass<SingletonClass>>>(callBase, typeControl)!.Object, callBase),
                ..GetProps(fixture.CreateAsync<AutoMock<UserClass<SingletonClass>>>(callBase, typeControl)!.Result!.Object, callBase),

                fixture.Freeze<SingletonClass>(callBase)!,
                fixture.FreezeAsync<SingletonClass>(callBase).Result!,
            ]).Should().AllBeSameAs(firstWhenMockDependencies);
    }

    [Test]
    public void Test_ClassMarkedSingleton_IsFrozen_WhenNonAutoMock_IntegrationFixtureSpecific([Values(true, false)] bool callBase)
    {
        using var fixture = new IntegrationFixture();

        var typeControl = new AutoMockTypeControl
        {
            NeverAutoMockTypes = [typeof(SingletonClass)]
        };

        var firstWhenNoMockDependencies = fixture.CreateNonAutoMock<SingletonClass>(callBase)!;

        ((SingletonClass[])[
                fixture.CreateAutoMock<SingletonClass>(callBase, typeControl)!,
                fixture.CreateAutoMockAsync<SingletonClass>(callBase, typeControl).Result!,

                ..GetProps(fixture.CreateAutoMock<UserClass<SingletonClass>>(callBase)!, callBase),
                ..GetProps(fixture.CreateAutoMockAsync<UserClass<SingletonClass>>(callBase)!.Result!, callBase),

                ..GetProps(fixture.CreateAutoMock<AutoMock<UserClass<SingletonClass>>>(callBase)!.Object, callBase),
                ..GetProps(fixture.CreateAutoMockAsync<AutoMock<UserClass<SingletonClass>>>(callBase)!.Result!.Object, callBase),

                fixture.Create<SingletonClass>(callBase)!,
                fixture.CreateAsync<SingletonClass>(callBase).Result!,

                ..GetProps(fixture.Create<UserClass<SingletonClass>>(callBase)!, callBase),
                ..GetProps(fixture.CreateAsync<UserClass<SingletonClass>>(callBase)!.Result!, callBase),

                ..GetProps(fixture.Create<AutoMock<UserClass<SingletonClass>>>(callBase)!.Object, callBase),
                ..GetProps(fixture.CreateAsync<AutoMock<UserClass<SingletonClass>>>(callBase)!.Result!.Object, callBase),

                fixture.Freeze<SingletonClass>(callBase)!,
                fixture.FreezeAsync<SingletonClass>(callBase).Result!,

                ..GetProps(fixture.Freeze<UserClass<SingletonClass>>(callBase)!, callBase),
                ..GetProps(fixture.FreezeAsync<UserClass<SingletonClass>>(callBase)!.Result!, callBase),

                ..GetProps(fixture.Freeze<AutoMock<UserClass<SingletonClass>>>(callBase)!.Object, callBase),
                ..GetProps(fixture.FreezeAsync<AutoMock<UserClass<SingletonClass>>>(callBase)!.Result!.Object, callBase),

            ]).Should().AllBeSameAs(firstWhenNoMockDependencies);
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
    public void Test_ClassMarkedSingleton_IsDifferent_ByDependencyType_WhenAutoMock()
    {
        var fixture = new AbstractAutoMockFixture();
        var mock1 = fixture.CreateWithAutoMockDependencies<AutoMock<SingletonClass>>(true);
        var mock2 = fixture.CreateNonAutoMock<AutoMock<SingletonClass>>(true);

        mock1.Should().NotBeNull();
        mock2.Should().NotBeNull();

        mock2.Should().NotBeSameAs(mock1);
        mock2!.Object.Should().NotBeSameAs(mock1!.Object);
    }

    [Test]
    public void Test_ClassMarkedSingleton_IsDifferent_ByDependencyType_WhenNotAutoMock()
    {
        var fixture = new AbstractAutoMockFixture();
        var obj1 = fixture.CreateWithAutoMockDependencies<SingletonClass>(true);
        var obj2 = fixture.CreateNonAutoMock<SingletonClass>(true);

        obj1.Should().NotBeNull();
        obj2.Should().NotBeNull();

        obj2.Should().NotBe(obj1);
    }

    #endregion

    #region Freeze Non Singleton

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
        var obj = fixture.CreateWithAutoMockDependencies<UserClass<NonSingletonClass>>();
        var obj2 = fixture.CreateWithAutoMockDependencies<UserClass<NonSingletonClass>>();

        frozen.Should().NotBeNull();
        obj.Should().NotBeNull();
        obj2.Should().NotBeNull();

        obj!.Arg1.Should().NotBeSameAs(frozen);
        new[] {
            obj!.Arg1, obj!.Arg2, obj!.ReadWriteProp, obj!.Field,
            obj2!.Arg1, obj2!.Arg2, obj2!.ReadWriteProp, obj2!.Field,
        }.Should().OnlyContain(x => x != frozen);

        new[] {
            obj!.Arg2, obj!.ReadWriteProp, obj!.Field,
            obj2.Arg1, obj2!.Arg2, obj2!.ReadWriteProp, obj2!.Field,
        }.Should().OnlyContain(x => x == obj.Arg1);
    }

    [Test]
    public void Test_Works_UnitFixture_When_NeverAutoMock()
    {
        var fixture = new UnitFixture();
        fixture.AutoMockTypeControl.NeverAutoMockTypes.Add(typeof(NonSingletonClass));

        var frozen = fixture.Freeze<NonSingletonClass>();
        var obj = fixture.CreateWithAutoMockDependencies<UserClass<NonSingletonClass>>();

        frozen.Should().NotBeNull();
        obj.Should().NotBeNull();
        obj!.Arg1.Should().Be(frozen);
        obj!.Arg2.Should().Be(frozen);
        obj!.ReadWriteProp.Should().Be(frozen);
        obj!.Field.Should().Be(frozen);
    }

    [Test]
    public void Test_Works_IntegrationFixture_When_AlwaysAutoMock()
    {
        var fixture = new IntegrationFixture();
        fixture.AutoMockTypeControl.AlwaysAutoMockTypes.Add(typeof(NonSingletonClass));

        var frozen = fixture.Freeze<AutoMock<NonSingletonClass>>()!.Object;
        var obj = fixture.CreateNonAutoMock<UserClass<NonSingletonClass>>();

        frozen.Should().NotBeNull();
        obj.Should().NotBeNull();

        GetProps(obj!, false).Should().AllBeSameAs(frozen!);
    }

    [Test]
    public void Test_Works_IntegrationFixture_When_NeverAutoMock()
    {
        var fixture = new IntegrationFixture();
        fixture.AutoMockTypeControl.NeverAutoMockTypes.Add(typeof(NonSingletonClass));

        var frozen = fixture.Freeze<NonSingletonClass>();
        var obj = fixture.CreateNonAutoMock<UserClass<NonSingletonClass>>();

        frozen.Should().NotBeNull();
        obj.Should().NotBeNull();

        GetProps(obj!, false).Should().AllBeSameAs(frozen!);
    }


    [Test]
    public void Test_Freeze_IsFrozen_WhenFreezeIsNotAutoMock_WhenAutoMock_ByCallBase([Values(true, false)] bool callBase)
    {
        var fixture = new UnitFixture();
        fixture.Freeze<NonSingletonClass>(callBase);
        var frozen = fixture.CreateAutoMock<NonSingletonClass>(callBase)!;

        GetAllTestClasses(callBase).Should().AllBeSameAs(frozen);

        var nonDirectlyFrozenCallBase = !callBase;
        var differentFrozen = fixture.CreateAutoMock<NonSingletonClass>(nonDirectlyFrozenCallBase)!;

        differentFrozen.Should().NotBeSameAs(frozen);

        GetAllTestClasses(nonDirectlyFrozenCallBase).Should().AllBeSameAs(differentFrozen);
        GetAllTestClasses(nonDirectlyFrozenCallBase).Should().AllNotBeSameAs(frozen);

        NonSingletonClass[] GetAllTestClasses(bool callBase)
        {
            return ((NonSingletonClass[])[
                fixture.CreateAutoMock<NonSingletonClass>(callBase)!,
                fixture.CreateAutoMockAsync<NonSingletonClass>(callBase).Result!,

                fixture.CreateAutoMock<AutoMock<NonSingletonClass>>(callBase)!.Object,
                fixture.CreateAutoMockAsync<AutoMock<NonSingletonClass>>(callBase).Result!.Object,

                ..GetProps(fixture.CreateAutoMock<UserClass<NonSingletonClass>>(callBase)!, callBase),
                ..GetProps(fixture.CreateAutoMockAsync<UserClass<NonSingletonClass>>(callBase).Result!, callBase),

                ..GetProps(fixture.CreateAutoMock<AutoMock<UserClass<NonSingletonClass>>>(callBase)!.Object!, callBase),
                ..GetProps(fixture.CreateAutoMockAsync<AutoMock<UserClass<NonSingletonClass>>>(callBase).Result!.Object, callBase),

                ..GetProps(fixture.CreateAutoMock<UserClass<AutoMock<NonSingletonClass>>>(callBase)!, callBase),
                ..GetProps(fixture.CreateAutoMockAsync<UserClass<AutoMock<NonSingletonClass>>>(callBase).Result!, callBase),

                ..GetProps(fixture.CreateAutoMock<AutoMock<UserClass<AutoMock<NonSingletonClass>>>>(callBase)!.Object!, callBase),
                ..GetProps(fixture.CreateAutoMockAsync<AutoMock<UserClass<AutoMock<NonSingletonClass>>>>(callBase).Result!.Object, callBase),

                fixture.CreateWithAutoMockDependencies<AutoMock<NonSingletonClass>>(callBase)!.Object,
                fixture.CreateWithAutoMockDependenciesAsync<AutoMock<NonSingletonClass>>(callBase).Result!.Object,

                ..GetProps(fixture.CreateWithAutoMockDependencies<UserClass<NonSingletonClass>>(callBase)!, callBase),
                ..GetProps(fixture.CreateWithAutoMockDependenciesAsync<UserClass<NonSingletonClass>>(callBase).Result!, callBase),

                ..GetProps(fixture.CreateWithAutoMockDependencies<AutoMock<UserClass<NonSingletonClass>>>(callBase)!.Object, callBase),
                ..GetProps(fixture.CreateWithAutoMockDependenciesAsync<AutoMock<UserClass<NonSingletonClass>>>(callBase).Result!.Object, callBase),

                ..GetProps(fixture.CreateWithAutoMockDependencies<UserClass<AutoMock<NonSingletonClass>>>(callBase)!, callBase),
                ..GetProps(fixture.CreateWithAutoMockDependenciesAsync<UserClass<AutoMock<NonSingletonClass>>>(callBase).Result!, callBase),

                ..GetProps(fixture.CreateWithAutoMockDependencies<AutoMock<UserClass<AutoMock<NonSingletonClass>>>>(callBase)!.Object, callBase),
                ..GetProps(fixture.CreateWithAutoMockDependenciesAsync<AutoMock<UserClass<AutoMock<NonSingletonClass>>>>(callBase).Result!.Object, callBase),

                fixture.Create<AutoMock<NonSingletonClass>>(callBase)!.Object,
                fixture.CreateAsync<AutoMock<NonSingletonClass>>(callBase).Result!.Object,

                ..GetProps(fixture.Create<UserClass<NonSingletonClass>>(callBase)!, callBase),
                ..GetProps(fixture.CreateAsync<UserClass<NonSingletonClass>>(callBase).Result!, callBase),

                ..GetProps(fixture.Create<AutoMock<UserClass<NonSingletonClass>>>(callBase)!.Object, callBase),
                ..GetProps(fixture.CreateAsync<AutoMock<UserClass<NonSingletonClass>>>(callBase).Result!.Object, callBase),

                ..GetProps(fixture.Create<UserClass<AutoMock<NonSingletonClass>>>(callBase)!, callBase),
                ..GetProps(fixture.CreateAsync<UserClass<AutoMock<NonSingletonClass>>>(callBase).Result!, callBase),

                ..GetProps(fixture.Create<AutoMock<UserClass<AutoMock<NonSingletonClass>>>>(callBase)!.Object, callBase),
                ..GetProps(fixture.CreateAsync<AutoMock<UserClass<AutoMock<NonSingletonClass>>>>(callBase).Result!.Object, callBase),

                fixture.Freeze<AutoMock<NonSingletonClass>>(callBase)!.Object,
                fixture.FreezeAsync<AutoMock<NonSingletonClass>>(callBase).Result!.Object,

                ..GetProps(fixture.Freeze<UserClass<NonSingletonClass>>(callBase)!, callBase),
                ..GetProps(fixture.FreezeAsync<UserClass<NonSingletonClass>>(callBase).Result!, callBase),

                ..GetProps(fixture.Freeze<AutoMock<UserClass<NonSingletonClass>>>(callBase)!.Object, callBase),
                ..GetProps(fixture.FreezeAsync<AutoMock<UserClass<NonSingletonClass>>>(callBase).Result!.Object, callBase),

                ..GetProps(fixture.Freeze<UserClass<AutoMock<NonSingletonClass>>>(callBase)!, callBase),
                ..GetProps(fixture.FreezeAsync<UserClass<AutoMock<NonSingletonClass>>>(callBase).Result!, callBase),

                ..GetProps(fixture.Freeze<AutoMock<UserClass<AutoMock<NonSingletonClass>>>>(callBase)!.Object, callBase),
                ..GetProps(fixture.FreezeAsync<AutoMock<UserClass<AutoMock<NonSingletonClass>>>>(callBase).Result!.Object, callBase),
            ]);
        }
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

    #region Freeze Outside Arg

    [Test]
    [TestCase<UnitFixture>]
    [TestCase<IntegrationFixture>]
    public void Test_FreezeOutsideObject_NonAutoMock_NotDifferentByCallbase_AndAlsoNotByDependencies<TFixture>()
            where TFixture : AutoMockFixtureBase, new()
    {
        using var fixture = new TFixture();
        var userClass = new UserClass<NonSingletonClass>(new NonSingletonClass(), new NonSingletonClass());

        fixture.JustFreeze(userClass);

        ((UserClass<NonSingletonClass>[])[
            fixture.CreateNonAutoMock<UserClass<NonSingletonClass>>(false)!,
            fixture.CreateNonAutoMock<UserClass<NonSingletonClass>>(true)!,
            fixture.CreateNonAutoMock<UserClass<NonSingletonClass>>()!, // Remember that default is not always false
            fixture.CreateNonAutoMockAsync<UserClass<NonSingletonClass>>(false).Result!,
            fixture.CreateNonAutoMockAsync<UserClass<NonSingletonClass>>(true).Result!,
            fixture.CreateNonAutoMockAsync<UserClass<NonSingletonClass>>().Result!, // Remember that default is not always false

            fixture.CreateWithAutoMockDependencies<UserClass<NonSingletonClass>>(false)!,
            fixture.CreateWithAutoMockDependencies<UserClass<NonSingletonClass>>(true)!,
            fixture.CreateWithAutoMockDependencies<UserClass<NonSingletonClass>>()!,
            fixture.CreateWithAutoMockDependenciesAsync<UserClass<NonSingletonClass>>(false).Result!,
            fixture.CreateWithAutoMockDependenciesAsync<UserClass<NonSingletonClass>>(true).Result!,
            fixture.CreateWithAutoMockDependenciesAsync<UserClass<NonSingletonClass>>().Result!,

            fixture.Create<UserClass<NonSingletonClass>>(false)!,
            fixture.Create<UserClass<NonSingletonClass>>(true)!,
            fixture.Create<UserClass<NonSingletonClass>>()!,
            fixture.CreateAsync<UserClass<NonSingletonClass>>(false).Result!,
            fixture.CreateAsync<UserClass<NonSingletonClass>>(true).Result!,
            fixture.CreateAsync<UserClass<NonSingletonClass>>().Result!,

            fixture.Freeze<UserClass<NonSingletonClass>>(false)!,
            fixture.Freeze<UserClass<NonSingletonClass>>(true)!,
            fixture.Freeze<UserClass<NonSingletonClass>>()!,
            fixture.FreezeAsync<UserClass<NonSingletonClass>>(false).Result!,
            fixture.FreezeAsync<UserClass<NonSingletonClass>>(true).Result!,
            fixture.FreezeAsync<UserClass<NonSingletonClass>>().Result!,

            ..GetProps(fixture.CreateNonAutoMock<UserClass<UserClass<NonSingletonClass>>>(false)!, false),
            ..GetProps(fixture.CreateNonAutoMock<UserClass<UserClass<NonSingletonClass>>>(true)!, false),
            ..GetProps(fixture.CreateNonAutoMock<UserClass<UserClass<NonSingletonClass>>>()!, false), // Remember that default is not always false
            ..GetProps(fixture.CreateNonAutoMockAsync<UserClass<UserClass<NonSingletonClass>>>(false).Result!, false),
            ..GetProps(fixture.CreateNonAutoMockAsync<UserClass<UserClass<NonSingletonClass>>>(true).Result!, false),
            ..GetProps(fixture.CreateNonAutoMockAsync<UserClass<UserClass<NonSingletonClass>>>().Result!, false), // Remember that default is not always false

        ]).Should().AllBeSameAs(userClass);
    }

    [Test]
    [TestCase<UnitFixture>(true)]
    [TestCase<UnitFixture>(false)]
    [TestCase<IntegrationFixture>(true)]
    [TestCase<IntegrationFixture>(false)]
    public void Test_FreezeOutsideObject_AutoMock_ProvidedObject_DependsOnCallbase<TFixture>(bool callBase)
        where TFixture : AutoMockFixtureBase, new()
    {
        using var fixture = new TFixture();
        var userClass = new AutoMock<UserClass<NonSingletonClass>>(new NonSingletonClass(), new NonSingletonClass())
        {
            CallBase = callBase
        }.Object;

        fixture.JustFreeze(userClass);

        ((UserClass<NonSingletonClass>[])[
            fixture.CreateNonAutoMock<AutoMock<UserClass<NonSingletonClass>>>(callBase)!.Object,
            fixture.CreateNonAutoMockAsync<AutoMock<UserClass<NonSingletonClass>>>(callBase).Result!.Object,

            fixture.CreateWithAutoMockDependencies<AutoMock<UserClass<NonSingletonClass>>>(callBase)!.Object,
            fixture.CreateWithAutoMockDependenciesAsync<AutoMock<UserClass<NonSingletonClass>>>(callBase).Result!.Object,

            fixture.CreateAutoMock<AutoMock<UserClass<NonSingletonClass>>>(callBase)!.Object,
            fixture.CreateAutoMockAsync<AutoMock<UserClass<NonSingletonClass>>>(callBase).Result!.Object,

            fixture.CreateAutoMock<UserClass<NonSingletonClass>>(callBase)!,
            fixture.CreateAutoMockAsync<UserClass<NonSingletonClass>>(callBase).Result!,

            fixture.Create<AutoMock<UserClass<NonSingletonClass>>>(callBase)!.Object,
            fixture.CreateAsync<AutoMock<UserClass<NonSingletonClass>>>(callBase).Result!.Object,

            fixture.Freeze<AutoMock<UserClass<NonSingletonClass>>>(callBase)!.Object,

            ..GetProps(fixture.CreateWithAutoMockDependencies<UserClass<UserClass<NonSingletonClass>>>(callBase)!, callBase),
            ..GetProps(fixture.CreateWithAutoMockDependenciesAsync<UserClass<UserClass<NonSingletonClass>>>(callBase).Result!, callBase),

            ..GetProps(fixture.CreateWithAutoMockDependencies<AutoMock<UserClass<UserClass<NonSingletonClass>>>>(callBase)!.Object, callBase),
            ..GetProps(fixture.CreateWithAutoMockDependenciesAsync<AutoMock<UserClass<UserClass<NonSingletonClass>>>>(callBase).Result!.Object, callBase),
        ]).Should().AllBeSameAs(userClass);
    }

    // The differenc ebetween and the one above is that the above worked with the mocked Object while this deal with the AutoMock itself
    [Test]
    [TestCase<UnitFixture>(true)]
    [TestCase<UnitFixture>(false)]
    [TestCase<IntegrationFixture>(true)]
    [TestCase<IntegrationFixture>(false)]
    public void Test_FreezeOutsideObject_AutoMock_ProvidedMock_DependsOnCallbase<TFixture>(bool callBase)
        where TFixture : AutoMockFixtureBase, new()
    {
        using var fixture = new TFixture();
        var userMock = new AutoMock<UserClass<NonSingletonClass>>(new NonSingletonClass(), new NonSingletonClass())
        {
            CallBase = callBase
        };

        fixture.JustFreeze(userMock);

        ((AutoMock<UserClass<NonSingletonClass>>[])[
            fixture.CreateNonAutoMock<AutoMock<UserClass<NonSingletonClass>>>(callBase)!,
            fixture.CreateNonAutoMockAsync<AutoMock<UserClass<NonSingletonClass>>>(callBase).Result!,

            fixture.CreateWithAutoMockDependencies<AutoMock<UserClass<NonSingletonClass>>>(callBase)!,
            fixture.CreateWithAutoMockDependenciesAsync<AutoMock<UserClass<NonSingletonClass>>>(callBase).Result!,

            fixture.CreateAutoMock<AutoMock<UserClass<NonSingletonClass>>>(callBase)!,
            fixture.CreateAutoMockAsync<AutoMock<UserClass<NonSingletonClass>>>(callBase).Result!,

            AutoMock.Get(fixture.CreateAutoMock<UserClass<NonSingletonClass>>(callBase)!)!,
            AutoMock.Get(fixture.CreateAutoMockAsync<UserClass<NonSingletonClass>>(callBase).Result!)!,

            fixture.Create<AutoMock<UserClass<NonSingletonClass>>>(callBase)!,
            fixture.CreateAsync<AutoMock<UserClass<NonSingletonClass>>>(callBase).Result!,

            fixture.Freeze<AutoMock<UserClass<NonSingletonClass>>>(callBase)!,

            ..GetProps(fixture.CreateWithAutoMockDependencies<UserClass<UserClass<NonSingletonClass>>>(callBase)!, callBase)
                .Select(x => AutoMock.Get(x)!),
            ..GetProps(fixture.CreateWithAutoMockDependenciesAsync<UserClass<UserClass<NonSingletonClass>>>(callBase).Result!, callBase)
                .Select(x => AutoMock.Get(x)!),

            ..GetProps(fixture.CreateWithAutoMockDependencies<AutoMock<UserClass<UserClass<NonSingletonClass>>>>(callBase)!.Object, callBase)
                .Select(x => AutoMock.Get(x)!),
            ..GetProps(fixture.CreateWithAutoMockDependenciesAsync<AutoMock<UserClass<UserClass<NonSingletonClass>>>>(callBase).Result!.Object, callBase)
                .Select(x => AutoMock.Get(x)!),
        ]).Should().AllBeSameAs(userMock);
    }

    #endregion

    #region Classes

    [Singleton]
    public class SingletonClass { }

    public class NonSingletonClass { }

    public class UserClass<T>(T arg1, T arg2)
    {
        public T Arg1 => arg1; // Non virtual to only work with the ctor
        public T Arg2 => arg2; // Non virtual to only work with the ctor

        public virtual T? ReadWriteProp { get; set; }
        public virtual T? ReadOnlyProp { get; }
        public T? Field;
    }

    #endregion
}
