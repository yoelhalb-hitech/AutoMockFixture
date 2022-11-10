using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.Test.AutoMockFixture_Tests
{
    public class GenericClassWithGenericMethod<T>
    {
        public virtual T Get(T t) => t;
    }
    public class NonGenericClassWithGenericMethod
    {
        public virtual T Get<T>(T t) => t;
    }
    internal class GarbageCollection_Tests
    {
        public static Type[] AutoMockTypes =
        {
            typeof(InternalReadOnlyTestInterface),
            typeof(InternalSimpleTestClass),
            typeof(InternalReadonlyPropertyClass),
            typeof(InternalAbstractSimpleTestClass),
            typeof(AutoMock<InternalReadOnlyTestInterface>),
            typeof(AutoMock<InternalSimpleTestClass>),
            typeof(AutoMock<InternalReadonlyPropertyClass>),
            typeof(AutoMock<InternalAbstractSimpleTestClass>),
            typeof(GenericClassWithGenericMethod<string>),
            typeof(AutoMock<GenericClassWithGenericMethod<string>>),
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
            [ValueSource(nameof(AutoMockTypes))] Type type, [ValueSource(nameof(SetupTypeLists))] MethodSetupTypes setupType)
        {
            var fixture = new AbstractAutoMockFixture();
            fixture.MethodSetupType = setupType;

            IsGarbageCollectible(() => fixture.CreateAutoMock(type)!).Should().BeTrue();
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
            [ValueSource(nameof(GetAllTypes))] Type type, [ValueSource(nameof(SetupTypeLists))] MethodSetupTypes setupType)
        {
            var fixture = new AbstractAutoMockFixture();
            fixture.MethodSetupType = setupType;

            IsGarbageCollectible(() => fixture.CreateWithAutoMockDependencies(type)!).Should().BeTrue();
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
}
