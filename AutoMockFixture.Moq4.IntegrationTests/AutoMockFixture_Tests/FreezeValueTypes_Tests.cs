using AutoMockFixture.FixtureUtils;
using AutoMockFixture.NUnit3;
using SequelPay.DotNetPowerExtensions;

namespace AutoMockFixture.Tests.AutoMockFixture_Tests;

internal class FreezeValueTypes_Tests
{
    [Test]
    public void Test_int()
    {
        var fix = new UnitFixture();
        var frozen = fix.Freeze<int>();
        var other = fix.Create<int>();
        other.Should().Be(frozen);
    }

    [Test]
    [TestCase<SingletonClass>(true, false)]
    [TestCase<SingletonClass>(false, false)]
    [TestCase<NonSingletonClass>(true, true)]
    [TestCase<NonSingletonClass>(false, true)]
    public void Test_Frozen_WhenAutoMock_AutoMockDependencies<TInner>(bool callBase, bool needsFreeze) where TInner : class
    {
        using var fixture = new AbstractAutoMockFixture();

        if (needsFreeze) fixture.JustFreeze<TInner>();

        var typeControl = new AutoMockTypeControl
        {
            AlwaysAutoMockTypes = [typeof(TInner)]
        };

        var firstWhenMockDependencies = fixture.CreateAutoMock<TInner>(callBase)!;
        ((TInner[])[
                ..GetProps(fixture.CreateWithAutoMockDependencies<UserStruct<TInner>>(callBase)!, callBase),
                ..GetProps(fixture.CreateWithAutoMockDependenciesAsync<UserStruct<TInner>>(callBase)!.Result!, callBase),

                ..GetProps(fixture.CreateWithAutoMockDependencies<UserStruct<AutoMock<TInner>>>(callBase)!, callBase),
                ..GetProps(fixture.CreateWithAutoMockDependenciesAsync<UserStruct<AutoMock<TInner>>>(callBase)!.Result!, callBase),

                ..GetProps(fixture.CreateWithAutoMockDependencies<UserStruct<TInner>>(callBase, typeControl)!, callBase),
                ..GetProps(fixture.CreateWithAutoMockDependenciesAsync<UserStruct<TInner>>(callBase, typeControl)!.Result!, callBase),
            ]).Should().AllBeSameAs(firstWhenMockDependencies);

        var firstWithNoMockDependencies = fixture.CreateNonAutoMock<TInner>(callBase, typeControl)!;
        ((TInner[])[
                ..GetProps(fixture.CreateNonAutoMock<UserStruct<TInner>>(callBase, typeControl)!, callBase),
                ..GetProps(fixture.CreateNonAutoMockAsync<UserStruct<TInner>>(callBase, typeControl)!.Result!, callBase),

                ..GetProps(fixture.CreateNonAutoMock<UserStruct<AutoMock<TInner>>>(callBase)!, callBase),
                ..GetProps(fixture.CreateNonAutoMockAsync<UserStruct<AutoMock<TInner>>>(callBase)!.Result!, callBase),

            ]).Should().AllBeSameAs(firstWithNoMockDependencies);
    }

    [Test]
    [TestCase<UnitFixture, SingletonStruct>(true, false)]
    [TestCase<UnitFixture, SingletonClass>(true, false)]
    [TestCase<UnitFixture, SingletonStruct>(false, false)]
    [TestCase<UnitFixture, SingletonClass>(false, false)]
    [TestCase<IntegrationFixture, SingletonStruct>(true, false)]
    [TestCase<IntegrationFixture, SingletonClass>(true, false)]
    [TestCase<IntegrationFixture, SingletonStruct>(false, false)]
    [TestCase<IntegrationFixture, SingletonClass>(false, false)]
    [TestCase<UnitFixture, NonSingletonStruct>(true, true)]
    [TestCase<UnitFixture, NonSingletonClass>(true, true)]
    [TestCase<UnitFixture, NonSingletonStruct>(false, true)]
    [TestCase<UnitFixture, NonSingletonClass>(false, true)]
    [TestCase<IntegrationFixture, NonSingletonStruct>(true, true)]
    [TestCase<IntegrationFixture, NonSingletonClass>(true, true)]
    [TestCase<IntegrationFixture, NonSingletonStruct>(false, true)]
    [TestCase<IntegrationFixture, NonSingletonClass>(false, true)]
    public void Test_Frozen_WhenNonAutoMock_WhenOuterStruct_AllFixtures<TFixture, TInner>(bool callBase, bool needsFreeze)
        where TFixture : AutoMockFixtureBase, new()
    {
        using var fixture = new TFixture();

        if (needsFreeze) fixture.JustFreeze<TInner>();

        var typeControl = new AutoMockTypeControl // We need this for the case of SingletonClass
        {
            NeverAutoMockTypes = [typeof(TInner)]
        };

        var firstWhenMockDependencies = fixture.CreateWithAutoMockDependencies<TInner>(callBase)!;

        ((TInner[])[
                fixture.CreateWithAutoMockDependencies<TInner>(callBase)!,
                fixture.CreateWithAutoMockDependenciesAsync<TInner>(callBase).Result!,

                ..GetProps(fixture.CreateWithAutoMockDependencies<UserStruct<TInner>>(callBase, typeControl)!, callBase),
                ..GetProps(fixture.CreateWithAutoMockDependenciesAsync<UserStruct<TInner>>(callBase, typeControl)!.Result!, callBase),
            ]).All(x => x!.Equals(firstWhenMockDependencies)).Should().BeTrue();

        var firstWhenNoMockDependencies = fixture.CreateNonAutoMock<TInner>(callBase)!;
        firstWhenNoMockDependencies.Should().NotBeNull().And.Should().NotBe(firstWhenMockDependencies);
        ((TInner[])[
                fixture.CreateNonAutoMock<TInner>(callBase)!,
                fixture.CreateNonAutoMockAsync<TInner>(callBase).Result!,

                ..GetProps(fixture.CreateNonAutoMock<UserStruct<TInner>>(callBase)!, callBase),
                ..GetProps(fixture.CreateNonAutoMockAsync<UserStruct<TInner>>(callBase)!.Result!, callBase),
            ]).All(x => x!.Equals(firstWhenNoMockDependencies)).Should().BeTrue();
    }

    [Test]
    [TestCase<UnitFixture, SingletonStruct>(true, false)]
    [TestCase<UnitFixture, SingletonStruct>(false, false)]
    [TestCase<IntegrationFixture, SingletonStruct>(true, false)]
    [TestCase<IntegrationFixture, SingletonStruct>(false, false)]
    [TestCase<UnitFixture, NonSingletonStruct>(true, true)]
    [TestCase<UnitFixture, NonSingletonStruct>(false, true)]
    [TestCase<IntegrationFixture, NonSingletonStruct>(true, true)]
    [TestCase<IntegrationFixture, NonSingletonStruct>(false, true)]
    public void Test_Frozen_WhenNonAutoMock_WhenOuterClass_AllFixtures<TFixture, TInner>(bool callBase, bool needsFreeze)
        where TFixture : AutoMockFixtureBase, new()
        where TInner : struct
    {
        using var fixture = new TFixture();

        if (needsFreeze) fixture.JustFreeze<TInner>();

        var firstWhenMockDependencies = fixture.CreateWithAutoMockDependencies<TInner>(callBase)!;

        // No need for TypeControl as a struct is never mocked

        ((TInner[])[
                fixture.CreateWithAutoMockDependencies<TInner>(callBase)!,
                fixture.CreateWithAutoMockDependenciesAsync<TInner>(callBase).Result!,

                ..GetProps(fixture.CreateWithAutoMockDependencies<UserClass<TInner>>(callBase)!, callBase),
                ..GetProps(fixture.CreateWithAutoMockDependenciesAsync<UserClass<TInner>>(callBase)!.Result!, callBase),

                ..GetProps(fixture.CreateWithAutoMockDependencies<AutoMock<UserClass<TInner>>>(callBase)!.Object, callBase),
                ..GetProps(fixture.CreateWithAutoMockDependenciesAsync<AutoMock<UserClass<TInner>>>(callBase)!.Result!.Object, callBase),
            ]).All(x => x.Equals(firstWhenMockDependencies)).Should().BeTrue();

        var firstWhenNoMockDependencies = fixture.CreateNonAutoMock<TInner>(callBase)!;
        firstWhenNoMockDependencies.Should().NotBeNull().And.Should().NotBe(firstWhenMockDependencies);
        ((TInner[])[
                fixture.CreateNonAutoMock<TInner>(callBase)!,
                fixture.CreateNonAutoMockAsync<TInner>(callBase).Result!,

                ..GetProps(fixture.CreateNonAutoMock<UserClass<TInner>>(callBase)!, callBase),
                ..GetProps(fixture.CreateNonAutoMockAsync<UserClass<TInner>>(callBase)!.Result!, callBase),

                ..GetProps(fixture.CreateNonAutoMock<AutoMock<UserClass<TInner>>>(callBase)!.Object, callBase),
                ..GetProps(fixture.CreateNonAutoMockAsync<AutoMock<UserClass<TInner>>>(callBase)!.Result!.Object, callBase),
            ]).All(x => x.Equals(firstWhenNoMockDependencies)).Should().BeTrue();
    }

    [Test]
    [TestCase<SingletonStruct>(true, false)]
    [TestCase<SingletonStruct>(false, false)]
    [TestCase<NonSingletonStruct>(true, true)]
    [TestCase<NonSingletonStruct>(false, true)]
    public void Test_Frozen_WhenNonAutoMock_WhenOuterClass_UnitFixtureSpecific<TInner>(bool callBase, bool needsFreeze)
        where TInner : struct
    {
        using var fixture = new UnitFixture();

        if (needsFreeze) fixture.JustFreeze<TInner>();

        var firstWhenMockDependencies = fixture.CreateWithAutoMockDependencies<TInner>(callBase)!;

        // No need for TypeControl as a struct is never mocked

        ((TInner[])[
                ..GetProps(fixture.CreateAutoMock<UserClass<TInner>>(callBase)!, callBase),
                ..GetProps(fixture.CreateAutoMockAsync<UserClass<TInner>>(callBase)!.Result!, callBase),

                ..GetProps(fixture.CreateAutoMock<AutoMock<UserClass<TInner>>>(callBase)!.Object, callBase),
                ..GetProps(fixture.CreateAutoMockAsync<AutoMock<UserClass<TInner>>>(callBase)!.Result!.Object, callBase),

                fixture.Create<TInner>(callBase)!,
                fixture.CreateAsync<TInner>(callBase).Result!,

                ..GetProps(fixture.Create<UserClass<TInner>>(callBase)!, callBase),
                ..GetProps(fixture.CreateAsync<UserClass<TInner>>(callBase)!.Result!, callBase),

                ..GetProps(fixture.Create<AutoMock<UserClass<TInner>>>(callBase)!.Object, callBase),
                ..GetProps(fixture.CreateAsync<AutoMock<UserClass<TInner>>>(callBase)!.Result!.Object, callBase),

                fixture.Freeze<TInner>(callBase)!,
                fixture.FreezeAsync<TInner>(callBase).Result!,

                ..GetProps(fixture.Freeze<UserClass<TInner>>(callBase)!, callBase),
                ..GetProps(fixture.FreezeAsync<UserClass<TInner>>(callBase)!.Result!, callBase),

                ..GetProps(fixture.Freeze<AutoMock<UserClass<TInner>>>(callBase)!.Object, callBase),
                ..GetProps(fixture.FreezeAsync<AutoMock<UserClass<TInner>>>(callBase)!.Result!.Object, callBase),
            ]).All(x => x.Equals(firstWhenMockDependencies)).Should().BeTrue();
    }

    [Test]
    [TestCase<SingletonStruct>(true, false)]
    [TestCase<SingletonStruct>(false, false)]
    [TestCase<SingletonClass>(true, false)]
    [TestCase<SingletonClass>(false, false)]
    [TestCase<NonSingletonStruct>(true, true)]
    [TestCase<NonSingletonStruct>(false, true)]
    [TestCase<NonSingletonClass>(true, true)]
    [TestCase<NonSingletonClass>(false, true)]
    public void Test_Frozen_WhenNonAutoMock_WhenOuterStruct_UnitFixtureSpecific<TInner>(bool callBase, bool needsFreeze)
    {
        using var fixture = new UnitFixture();

        if (needsFreeze) fixture.JustFreeze<TInner>();

        var typeControl = new AutoMockTypeControl // We need this for the case of SingletonClass/NonSingletonClass
        {
            NeverAutoMockTypes = [typeof(TInner)]
        };

        var firstWhenMockDependencies = fixture.CreateWithAutoMockDependencies<TInner>(callBase)!;

        ((TInner[])[
                fixture.Create<TInner>(callBase)!,
                fixture.CreateAsync<TInner>(callBase).Result!,

                ..GetProps(fixture.Create<UserStruct<TInner>>(callBase, typeControl)!, callBase),
                ..GetProps(fixture.CreateAsync<UserStruct<TInner>>(callBase, typeControl)!.Result!, callBase),

                fixture.Freeze<TInner>(callBase)!,
                fixture.FreezeAsync<TInner>(callBase).Result!,
            ]).All(x => x!.Equals(firstWhenMockDependencies)).Should().BeTrue();
    }

    [Test]
    [TestCase<SingletonStruct>(true, false)]
    [TestCase<SingletonStruct>(false, false)]
    [TestCase<NonSingletonStruct>(true, true)]
    [TestCase<NonSingletonStruct>(false, true)]
    public void Test_Frozen_WhenNonAutoMock_WhenOuterClass_IntegrationFixtureSpecific<TInner>(bool callBase, bool needsFreeze)
    where TInner : struct
    {
        using var fixture = new IntegrationFixture();

        if (needsFreeze) fixture.JustFreeze<TInner>();

        var firstWhenMockDependencies = fixture.CreateNonAutoMock<TInner>(callBase)!;

        // No need for TypeControl as a struct is never mocked

        ((TInner[])[
                ..GetProps(fixture.CreateAutoMock<UserClass<TInner>>(callBase)!, callBase),
                ..GetProps(fixture.CreateAutoMockAsync<UserClass<TInner>>(callBase)!.Result!, callBase),

                ..GetProps(fixture.CreateAutoMock<AutoMock<UserClass<TInner>>>(callBase)!.Object, callBase),
                ..GetProps(fixture.CreateAutoMockAsync<AutoMock<UserClass<TInner>>>(callBase)!.Result!.Object, callBase),

                fixture.Create<TInner>(callBase)!,
                fixture.CreateAsync<TInner>(callBase).Result!,

                ..GetProps(fixture.Create<UserClass<TInner>>(callBase)!, callBase),
                ..GetProps(fixture.CreateAsync<UserClass<TInner>>(callBase)!.Result!, callBase),

                ..GetProps(fixture.Create<AutoMock<UserClass<TInner>>>(callBase)!.Object, callBase),
                ..GetProps(fixture.CreateAsync<AutoMock<UserClass<TInner>>>(callBase)!.Result!.Object, callBase),

                fixture.Freeze<TInner>(callBase)!,
                fixture.FreezeAsync<TInner>(callBase).Result!,

                ..GetProps(fixture.Freeze<UserClass<TInner>>(callBase)!, callBase),
                ..GetProps(fixture.FreezeAsync<UserClass<TInner>>(callBase)!.Result!, callBase),

                ..GetProps(fixture.Freeze<AutoMock<UserClass<TInner>>>(callBase)!.Object, callBase),
                ..GetProps(fixture.FreezeAsync<AutoMock<UserClass<TInner>>>(callBase)!.Result!.Object, callBase),
            ]).All(x => x.Equals(firstWhenMockDependencies)).Should().BeTrue();
    }

    [Test]
    [TestCase<SingletonStruct>(true, false)]
    [TestCase<SingletonStruct>(false, false)]
    [TestCase<SingletonClass>(true, false)]
    [TestCase<SingletonClass>(false, false)]
    [TestCase<NonSingletonStruct>(true, true)]
    [TestCase<NonSingletonStruct>(false, true)]
    [TestCase<NonSingletonClass>(true, true)]
    [TestCase<NonSingletonClass>(false, true)]
    public void Test_Frozen_WhenNonAutoMock_WhenOuterStruct_IntegrationFixtureSpecific<TInner>(bool callBase, bool needsFreeze)
    {
        using var fixture = new IntegrationFixture();

        if (needsFreeze) fixture.JustFreeze<TInner>();

        var firstWhenMockDependencies = fixture.CreateNonAutoMock<TInner>(callBase)!;

        ((TInner[])[
                fixture.Create<TInner>(callBase)!,
                fixture.CreateAsync<TInner>(callBase).Result!,

                ..GetProps(fixture.Create<UserStruct<TInner>>(callBase)!, callBase),
                ..GetProps(fixture.CreateAsync<UserStruct<TInner>>(callBase)!.Result!, callBase),

                fixture.Freeze<TInner>(callBase)!,
                fixture.FreezeAsync<TInner>(callBase).Result!,

                ..GetProps(fixture.Freeze<UserStruct<TInner>>(callBase)!, callBase),
                ..GetProps(fixture.FreezeAsync<UserStruct<TInner>>(callBase)!.Result!, callBase),
            ]).All(x => x!.Equals(firstWhenMockDependencies)).Should().BeTrue();
    }

    [Test]
    public void Test_ClassMarkedSingleton_IsDifferent_ByCallBase_WhenAutoMock()
    {
        var fixture = new AbstractAutoMockFixture();
        var mock1 = fixture.CreateAutoMock<UserClass<SingletonStruct>>(false);
        var mock2 = fixture.CreateAutoMock<UserClass<SingletonStruct>>(true);

        mock1.Should().NotBeNull();
        mock2.Should().NotBeNull();

        mock1!.ReadWriteProp.Should().NotBeNull();
        mock2!.ReadWriteProp.Should().NotBeNull();

        mock2!.ReadWriteProp.Equals(mock1!.ReadWriteProp).Should().BeFalse();
    }

    [Test]
    public void Test_ClassMarkedSingleton_IsDifferent_ByCallBase_WhenNotAutoMock()
    {
        var fixture = new AbstractAutoMockFixture();
        var obj1 = fixture.CreateNonAutoMock<SingletonStruct>(false);
        var obj2 = fixture.CreateNonAutoMock<SingletonStruct>(true);

        obj1.Should().NotBeNull();
        obj2.Should().NotBeNull();

        obj2.Equals(obj1).Should().BeFalse();
    }

    [Test]
    public void Test_ClassMarkedSingleton_IsDifferent_ByDependencyType_WhenAutoMock()
    {
        var fixture = new AbstractAutoMockFixture();
        var mock1 = fixture.CreateWithAutoMockDependencies<AutoMock<UserClass<SingletonStruct>>>(true)!.Object;
        var mock2 = fixture.CreateNonAutoMock<AutoMock<UserClass<SingletonStruct>>>(true)!.Object;

        mock1.Should().NotBeNull();
        mock2.Should().NotBeNull();

        mock1!.ReadWriteProp.Should().NotBeNull();
        mock2!.ReadWriteProp.Should().NotBeNull();

        mock2!.ReadWriteProp.Equals(mock1!.ReadWriteProp).Should().BeFalse();
    }

    [Test]
    public void Test_ClassMarkedSingleton_IsDifferent_ByDependencyType_WhenNotAutoMock()
    {
        var fixture = new AbstractAutoMockFixture();
        var obj1 = fixture.CreateWithAutoMockDependencies<SingletonStruct>(true);
        var obj2 = fixture.CreateNonAutoMock<SingletonStruct>(true);

        obj1.Should().NotBeNull();
        obj2.Should().NotBeNull();

        obj2.Equals(obj1).Should().BeFalse();
    }

    #region Classes And Utils

    [Singleton]
    public class SingletonClass { }

    [Singleton]
    public struct SingletonStruct
    {
        public SingletonStruct() { }

        public int TestProp { get; } = new Random().Next(); // This is to ensure that different struct don't have the same value as for struct we check equality by value
    }

    public class NonSingletonClass { }
    public struct NonSingletonStruct
    {
        public NonSingletonStruct() { }
        public int TestProp { get; } = new Random().Next();  // This is to ensure that different struct don't have the same value as for struct we check equality by value
    }


    public struct UserStruct<T>(T arg1, T arg2)
    {
        public T Arg1 { get; } = arg1; // Non virtual to only work with the ctor
        public T Arg2 { get; } = arg2; // Non virtual to only work with the ctor

        public T? ReadWriteProp { get; set; }
        public T? Field;
    }

    public class UserClass<T>(T arg1, T arg2)
    {
        public T Arg1 { get; } = arg1; // Non virtual to only work with the ctor
        public T Arg2 { get; } = arg2; // Non virtual to only work with the ctor

        public virtual T? ReadWriteProp { get; set; }
        public virtual T? ReadOnlyProp { get; }
        public T? Field;
    }

    private T[] GetProps<T>(UserStruct<T> userClass, bool callBase) =>
        [userClass.Arg1, userClass.Arg2, userClass.ReadWriteProp!, userClass.Field!];

    private T[] GetProps<T>(UserClass<T> userClass, bool callBase) =>
        !callBase && userClass is IAutoMocked
            ? [userClass.ReadOnlyProp!, userClass.ReadWriteProp!, userClass.Field!]
            : [userClass.Arg1, userClass.Arg2, userClass.ReadWriteProp!, userClass.Field!];

    #endregion
}
