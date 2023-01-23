
namespace AutoMockFixture.Tests.AutoMockFixture_Tests;

public class GenericClassWithGenericMethod<T>
{
    public virtual T Get(T t) => t;
}
public class NonGenericClassWithGenericMethod
{
    public virtual T Get<T>(T t) => t;
}
public class SelfReferencing
{
    private readonly SelfReferencing selfReferencing;

    public SelfReferencing(SelfReferencing selfReferencing)
    {
        this.selfReferencing = selfReferencing;
    }
}

public class IndirectSelfReferencingOuter
{
    private readonly IndirectSelfReferencingInner selfReferencingInner;

    public IndirectSelfReferencingOuter(IndirectSelfReferencingInner selfReferencingInner)
    {
        this.selfReferencingInner = selfReferencingInner;
    }
}

public class IndirectSelfReferencingInner
{
    private readonly IndirectSelfReferencingOuter selfReferencingOuter;

    public IndirectSelfReferencingInner(IndirectSelfReferencingOuter selfReferencingOuter)
    {
        this.selfReferencingOuter = selfReferencingOuter;
    }
}
public class GarbageCollection_Tests
{
    public static Type[] AutoMockTypes =
    {
        typeof(InternalReadOnlyTestInterface),
        typeof(AutoMock<InternalReadOnlyTestInterface>),
        typeof(InternalSimpleTestClass),
        typeof(AutoMock<InternalSimpleTestClass>),
        typeof(InternalReadonlyPropertyClass),
        typeof(AutoMock<InternalReadonlyPropertyClass>),
        typeof(InternalAbstractSimpleTestClass),
        typeof(AutoMock<InternalAbstractSimpleTestClass>),
        typeof(InternalAbstractReadonlyPropertyClass),
        typeof(AutoMock<InternalAbstractReadonlyPropertyClass>),
        typeof(InternalReadonlyPropertyClass),
        typeof(AutoMock<InternalReadonlyPropertyClass>),
        typeof(InternalPrivateSetterPropertyClass),
        typeof(AutoMock<InternalPrivateSetterPropertyClass>),
        typeof(ProtectedSetterPropertyClass),
        typeof(AutoMock<ProtectedSetterPropertyClass>),
        typeof(InternalSimpleTestClass),
        typeof(AutoMock<InternalSimpleTestClass>),
        typeof(InternalTestFields),
        typeof(AutoMock<InternalTestFields>),
        typeof(InternalTestMethods),
        typeof(AutoMock<InternalTestMethods>),
        typeof(ITestInterface),
        typeof(AutoMock<ITestInterface>),
        typeof(InternalAbstractMethodTestClass),
        typeof(AutoMock<InternalAbstractMethodTestClass>),
        typeof(WithCtorArgsTestClass),
        typeof(AutoMock<WithCtorArgsTestClass>),
        typeof(WithComplexTestClass),
        typeof(AutoMock<WithComplexTestClass>),
        typeof(WithCtorNoArgsTestClass),
        typeof(AutoMock<WithCtorNoArgsTestClass>),

        typeof(GenericClassWithGenericMethod<string>),
        typeof(AutoMock<GenericClassWithGenericMethod<string>>),

        typeof(NonGenericClassWithGenericMethod),
        typeof(AutoMock<NonGenericClassWithGenericMethod>),
        typeof(SelfReferencing),
        typeof(AutoMock<SelfReferencing>),
        typeof(IndirectSelfReferencingOuter),
        typeof(AutoMock<IndirectSelfReferencingOuter>),
        typeof(IndirectSelfReferencingInner),
        typeof(AutoMock<IndirectSelfReferencingInner>),
    };

    public static Type[] NonAutoMockTypes =
    {
        typeof(string),
        typeof(Task),
        typeof(IEnumerable<string>),
    };

    public static IEnumerable<Type> GetAllTypes() => AutoMockTypes.Union(NonAutoMockTypes);
    public static MethodSetupTypes[] SetupTypeLists =
    {
        MethodSetupTypes.Eager,
        MethodSetupTypes.LazySame,
        MethodSetupTypes.LazyDifferent,
    };

    [Test, Pairwise]
    public void Test_IsGarabaseCollectible_WhenAutoMock(
                    [ValueSource(nameof(AutoMockTypes))] Type type,
                    [ValueSource(nameof(SetupTypeLists))] MethodSetupTypes setupType,
                    [Values(true, false)] bool callbase)
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.MethodSetupType = setupType;

        IsGarbageCollectible(() => fixture.CreateAutoMock(type, callbase)!).Should().BeTrue();
    }

    [Test, Pairwise]
    public void Test_IsGarabaseCollectible_WhenNonAutoMock(
        [ValueSource(nameof(GetAllTypes))] Type type, [ValueSource(nameof(SetupTypeLists))] MethodSetupTypes setupType)
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.MethodSetupType = setupType;

        IsGarbageCollectible(() => fixture.CreateNonAutoMock(type)!).Should().BeTrue();
    }

    [Test, Pairwise]
    public void Test_IsGarabaseCollectible_WhenAutoMockDependencies(
                    [ValueSource(nameof(AutoMockTypes))] Type type,
                    [ValueSource(nameof(SetupTypeLists))] MethodSetupTypes setupType,
                    [Values(true, false)] bool callbase)
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.MethodSetupType = setupType;

        IsGarbageCollectible(() => fixture.CreateWithAutoMockDependencies(type, callbase)!).Should().BeTrue();
    }


    [Test, Pairwise]
    public void Test_IsGarabaseCollectible_GenericMethod_AndAutoMock(
                [ValueSource(nameof(SetupTypeLists))] MethodSetupTypes setupType)
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.MethodSetupType = setupType;

        IsGarbageCollectible(() =>
        {
            var specimen = fixture.CreateAutoMock<GenericClassWithGenericMethod<string>>()!;
            _ = specimen.Get(fixture.CreateNonAutoMock<string>()!);

            return specimen;
        }).Should().BeTrue();

        IsGarbageCollectible(() =>
        {
            var specimen = fixture.CreateAutoMock<NonGenericClassWithGenericMethod>()!;
            _ = specimen.Get(fixture.CreateNonAutoMock<string>()!);

            return specimen;
        }).Should().BeTrue();

        IsGarbageCollectible(() =>
        {
            var specimen = fixture.CreateAutoMock<AutoMock<GenericClassWithGenericMethod<string>>>()!;
            _ = specimen.Object.Get(fixture.CreateNonAutoMock<string>()!);

            return specimen;
        }).Should().BeTrue();

        IsGarbageCollectible(() =>
        {
            var specimen = fixture.CreateAutoMock<AutoMock<NonGenericClassWithGenericMethod>>()!;
            _ = specimen.Object.Get(fixture.CreateNonAutoMock<string>()!);

            return specimen;
        }).Should().BeTrue();
    }

    private static WeakReference GetWeakReference<T>(Func<T> func) where T : class
    {
        var obj = func();

        var reference = new WeakReference(obj, true);
        obj = null;

        return reference;
    }
    static bool IsGarbageCollectible<T>(Func<T> func) where T : class
    {
        var reference = GetWeakReference(func); // The object needs to be in a separate function otherwise it doesn't collect it

        GC.Collect();
        GC.WaitForPendingFinalizers();

        return !reference.IsAlive;
    }
}