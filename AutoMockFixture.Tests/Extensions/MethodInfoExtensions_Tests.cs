﻿using System.Reflection;

namespace AutoMockFixture.Tests.Extensions;

public class MethodInfoExtensions_Tests
{
    [Test]
    [TestCase( nameof(TestClass.NonOverload), false)]
    [TestCase(nameof(TestClass.NonOverloadWithArgs), false)]
    [TestCase(nameof(TestClass.NonOverloadWithGenericArgs), false)]
    [TestCase(nameof(TestClass.NonOverloadWithNonUsingGenericArgs), false)]
    [TestCase(nameof(TestClass.OverloadSameArgNumber), true)]
    [TestCase(nameof(TestClass.OverloadDifferentArgNumber), true)]
    public void Test_HasOverloads_ReturnsCorrectly(string name, bool expectedHasOverload)
    {
        var hasOverload = GetMethod(m => m.Name == name).HasOverloads();

        hasOverload.Should().Be(expectedHasOverload);
    }

    [Test]
    [TestCase(nameof(TestClass.NonOverload), false)]
    [TestCase(nameof(TestClass.NonOverloadWithArgs), false)]
    [TestCase(nameof(TestClass.NonOverloadWithGenericArgs), false)]
    [TestCase(nameof(TestClass.NonOverloadWithNonUsingGenericArgs), false)]
    [TestCase(nameof(TestClass.OverloadSameArgNumber), true)]
    [TestCase(nameof(TestClass.OverloadDifferentArgNumber), true)]
    public void Test_HasOverloads_ReturnsCorrectly_OnSubClass(string name, bool expectedHasOverload)
    {
        var hasOverload = GetSubClassMethod(m => m.Name == name).HasOverloads();

        hasOverload.Should().Be(expectedHasOverload);
    }

    [TestCase(typeof(TestClassOverloadsFirstNonGeneric))] // CATUION: Declared order of methods matters when enumerating
    [TestCase(typeof(TestSubClassOverloadsFirstNonGeneric))]
    [TestCase(typeof(TestClassOverloadsFirstGeneric))]
    [TestCase(typeof(TestSubClassOverloadsFirstGeneric))]
    public void Test_HasOverloads_DoesNotThrow_BugRepro(Type t)
    {
        foreach (var method in t.GetAllMethods())
        {
            Assert.DoesNotThrow(() => method.HasOverloads());
            Assert.DoesNotThrow(() => method.HasOverloadSameCount());
        }
    }

    [Test]
    [TestCase(nameof(TestClass.NonOverload), false)]
    [TestCase(nameof(TestClass.NonOverloadWithArgs), false)]
    [TestCase(nameof(TestClass.NonOverloadWithGenericArgs), false)]
    [TestCase(nameof(TestClass.NonOverloadWithNonUsingGenericArgs), false)]
    [TestCase(nameof(TestClass.OverloadSameArgNumber), true)]
    [TestCase(nameof(TestClass.OverloadDifferentArgNumber), false)]
    public void Test_HasOverloadSameCount_ReturnsCorrectly(string name, bool expectedHasSameCount)
    {
        var hasSameCount = GetMethod(m => m.Name == name).HasOverloadSameCount();

        hasSameCount.Should().Be(expectedHasSameCount);
    }

    [Test]
    [TestCase(nameof(TestClass.NonOverload), false)]
    [TestCase(nameof(TestClass.NonOverloadWithArgs), false)]
    [TestCase(nameof(TestClass.NonOverloadWithGenericArgs), false)]
    [TestCase(nameof(TestClass.NonOverloadWithNonUsingGenericArgs), false)]
    [TestCase(nameof(TestClass.OverloadSameArgNumber), true)]
    [TestCase(nameof(TestClass.OverloadDifferentArgNumber), false)]
    public void Test_HasOverloadSameCount_ReturnsCorrectly_OnSubClass(string name, bool expectedHasSameCount)
    {
        var hasSameCount = GetSubClassMethod(m => m.Name == name).HasOverloadSameCount();

        hasSameCount.Should().Be(expectedHasSameCount);
    }

    [Test]
    [TestCase(nameof(TestClass.NonOverload), "NonOverload")]
    [TestCase(nameof(TestClass.NonOverloadWithArgs), "NonOverloadWithArgs")]
    [TestCase(nameof(TestClass.NonOverloadWithGenericArgs), "NonOverloadWithGenericArgs`1")]
    [TestCase(nameof(TestClass.NonOverloadWithNonUsingGenericArgs), "NonOverloadWithNonUsingGenericArgs`1")]
    public void TestGetTrackingPath_ReturnsCorrectly_ForNonOverloads(string name, string expectedTrackingPath)
    {
        var trackingPath = GetMethod(m => m.Name == name).GetTrackingPath(false);

        trackingPath.Should().Be(expectedTrackingPath);
    }

    [Test]
    [TestCase(nameof(TestClass.NonOverload), "NonOverload")]
    [TestCase(nameof(TestClass.NonOverloadWithArgs), "NonOverloadWithArgs")]
    [TestCase(nameof(TestClass.NonOverloadWithGenericArgs), "NonOverloadWithGenericArgs`1")]
    [TestCase(nameof(TestClass.NonOverloadWithNonUsingGenericArgs), "NonOverloadWithNonUsingGenericArgs`1")]
    public void TestGetTrackingPath_ReturnsCorrectly_ForNonOverloads_OnSubClass(string name, string expectedTrackingPath)
    {
        var trackingPath = GetSubClassMethod(m => m.Name == name).GetTrackingPath(false);

        trackingPath.Should().Be(expectedTrackingPath);
    }

    [Test]
    [TestCase(nameof(TestClass.NonOverloadWithGenericArgs), typeof(int), "NonOverloadWithGenericArgs<int>")]
    [TestCase(nameof(TestClass.NonOverloadWithGenericArgs), typeof(string), "NonOverloadWithGenericArgs<string>")]
    [TestCase(nameof(TestClass.NonOverloadWithNonUsingGenericArgs), typeof(int), "NonOverloadWithNonUsingGenericArgs<int>")]
    [TestCase(nameof(TestClass.NonOverloadWithNonUsingGenericArgs), typeof(string), "NonOverloadWithNonUsingGenericArgs<string>")]
    public void TestGetTrackingPath_ReturnsCorrectly_ForConstructudNonOverloads(string name, Type type, string expectedTrackingPath)
    {
        var trackingPath = GetMethod(m => m.Name == name).MakeGenericMethod(type).GetTrackingPath(false);

        trackingPath.Should().Be(expectedTrackingPath);
    }

    [Test]
    [TestCase(nameof(TestClass.NonOverloadWithGenericArgs), typeof(int), "NonOverloadWithGenericArgs<int>")]
    [TestCase(nameof(TestClass.NonOverloadWithGenericArgs), typeof(string), "NonOverloadWithGenericArgs<string>")]
    [TestCase(nameof(TestClass.NonOverloadWithNonUsingGenericArgs), typeof(int), "NonOverloadWithNonUsingGenericArgs<int>")]
    [TestCase(nameof(TestClass.NonOverloadWithNonUsingGenericArgs), typeof(string), "NonOverloadWithNonUsingGenericArgs<string>")]
    public void TestGetTrackingPath_ReturnsCorrectly_ForConstructudNonOverloads_OnSubClass(string name, Type type, string expectedTrackingPath)
    {
        var trackingPath = GetSubClassMethod(m => m.Name == name).MakeGenericMethod(type).GetTrackingPath(false);

        trackingPath.Should().Be(expectedTrackingPath);
    }

    [Test]
    [TestCaseSource(nameof(OverloadMethods))]
    [TestCaseSource(nameof(SubClassOverloadMethods))]
    public void TestGetTrackingPath_ReturnsCorrectly_WithOverloads(MethodInfo method, string expectedTrackingPath)
    {
        var trackingPath = method.GetTrackingPath(false);

        trackingPath.Should().Be(expectedTrackingPath);
    }

    #region Utils

    #region Util Classes

    internal class TestClass
    {
        public void NonOverload() { }
        public void NonOverloadWithArgs(int _) { }
        public void NonOverloadWithGenericArgs<T>(T _) { }
        public void NonOverloadWithNonUsingGenericArgs<T>() { }
        public void OverloadSameArgNumber(int _) { }
        public void OverloadSameArgNumber(string _) { }
        public void OverloadSameArgNumber<T>(T _) { }
        public void OverloadSameArgNumber<T1, T2>(T1 _) { }
        public void OverloadDifferentArgNumber() { }
        public void OverloadDifferentArgNumber(string _) { }
        public void OverloadDifferentArgNumber(string _, int i) { }
        public void OverloadDifferentArgNumber<T>(string _, int i, T t) { }
        public void OverloadDifferentArgNumber<T1, T2>(string _, int i, string s2, T1 t) { }
    }

    internal class TestSubClass : TestClass { }

    internal class TestClassOverloadsFirstNonGeneric
    {
        public void OverloadSameArgNumber(int _) { }
        public void OverloadSameArgNumber(string _) { }
        public void OverloadSameArgNumber<T>(T _) { }
        public void OverloadSameArgNumber<T1, T2>(T1 _) { }
        public void OverloadDifferentArgNumber() { }
        public void OverloadDifferentArgNumber(string _) { }
        public void OverloadDifferentArgNumber(string _, int i) { }
        public void OverloadDifferentArgNumber<T>(string _, int i, T t) { }
        public void OverloadDifferentArgNumber<T1, T2>(string _, int i, string s2, T1 t) { }
    }

    internal class TestSubClassOverloadsFirstNonGeneric : TestClassOverloadsFirstNonGeneric { }

    internal class TestClassOverloadsFirstGeneric
    {
        public void OverloadSameArgNumber<T>(T _) { }
        public void OverloadSameArgNumber<T1, T2>(T1 _) { }
        public void OverloadSameArgNumber(int _) { }
        public void OverloadSameArgNumber(string _) { }
        public void OverloadDifferentArgNumber<T>(string _, int i, T t) { }
        public void OverloadDifferentArgNumber<T1, T2>(string _, int i, string s2, T1 t) { }
        public void OverloadDifferentArgNumber() { }
        public void OverloadDifferentArgNumber(string _) { }
        public void OverloadDifferentArgNumber(string _, int i) { }
    }

    internal class TestSubClassOverloadsFirstGeneric : TestClassOverloadsFirstGeneric { }

    #endregion

    #region Util Methods

    static MethodInfo GetMethod(Func<MethodInfo, bool> func) => typeof(TestClass).GetMethods().First(func);
    static MethodInfo GetSubClassMethod(Func<MethodInfo, bool> func) => typeof(TestSubClass).GetMethods().First(func);

    public static IEnumerable<TestCaseData> OverloadMethods
        => GetOverloadMethods(GetGetMethod(GetMethod))
            .Select(t => t.SetName(nameof(TestGetTrackingPath_ReturnsCorrectly_WithOverloads) + "(BaseTests_" + t.Arguments.Last()!.ToString() + ")"));

    public static IEnumerable<TestCaseData> SubClassOverloadMethods
        => GetOverloadMethods(GetGetMethod(GetSubClassMethod))
            .Select(t => t.SetName(nameof(TestGetTrackingPath_ReturnsCorrectly_WithOverloads) + "(SubClassTests_" + t.Arguments.Last()!.ToString() + ")"));

    #endregion

    #region Data Utils

    private delegate MethodInfo GetMethodDelegate(Func<MethodInfo, bool> func);

    private static Func<string, int?, int?, Type?, MethodInfo> GetGetMethod(GetMethodDelegate func)
        => (string name, int? parameters, int? genericArgs, Type? parameterType)
            => func(m => m.Name == name
                    && (!parameters.HasValue || m.GetParameters().Length == parameters.Value)
                    && (!genericArgs.HasValue
                            || (m.IsGenericMethod && m.GetGenericArguments().Length == genericArgs.Value))
                    && (parameterType is null || (m.GetParameters().FirstOrDefault()?.ParameterType == parameterType)));

    public static IEnumerable<TestCaseData> GetOverloadMethods(Func<string, int?, int?, Type?, MethodInfo> getMethod)
    {
        yield return new TestCaseData(getMethod(nameof(TestClass.OverloadSameArgNumber), null, null, typeof(int)),
                        "OverloadSameArgNumber(int)");
        yield return new TestCaseData(getMethod(nameof(TestClass.OverloadSameArgNumber), null, null, typeof(string)),
                        "OverloadSameArgNumber(string)");
        yield return new TestCaseData(getMethod(nameof(TestClass.OverloadSameArgNumber), null, 1, null),
                        "OverloadSameArgNumber`1(T)");
        yield return new TestCaseData(getMethod(nameof(TestClass.OverloadSameArgNumber), null, 1, null)
                                        .MakeGenericMethod(typeof(string)),
                        "OverloadSameArgNumber<string>(string)");
        yield return new TestCaseData(getMethod(nameof(TestClass.OverloadSameArgNumber), null, 2, null),
                                                                    "OverloadSameArgNumber`2(T1)");
        yield return new TestCaseData(getMethod(nameof(TestClass.OverloadSameArgNumber), null, 2, null)
                                        .MakeGenericMethod(typeof(string), typeof(int)),
                        "OverloadSameArgNumber<string,int>(string)");

        yield return new TestCaseData(getMethod(nameof(TestClass.OverloadDifferentArgNumber), 0, null, null),
                        "OverloadDifferentArgNumber(`0)");
        yield return new TestCaseData(getMethod(nameof(TestClass.OverloadDifferentArgNumber), 1, null, null),
                        "OverloadDifferentArgNumber(`1)");
        yield return new TestCaseData(getMethod(nameof(TestClass.OverloadDifferentArgNumber), 2, null, null),
                        "OverloadDifferentArgNumber(`2)");
        yield return new TestCaseData(getMethod(nameof(TestClass.OverloadDifferentArgNumber), 3, null, null),
                        "OverloadDifferentArgNumber`1(`3)");
        yield return new TestCaseData(getMethod(nameof(TestClass.OverloadDifferentArgNumber), 3, null, null)
                                    .MakeGenericMethod(typeof(string)),
                        "OverloadDifferentArgNumber<string>(`3)");
        yield return new TestCaseData(getMethod(nameof(TestClass.OverloadDifferentArgNumber), 4, null, null),
                        "OverloadDifferentArgNumber`2(`4)");
        yield return new TestCaseData(getMethod(nameof(TestClass.OverloadDifferentArgNumber), 4, null, null)
                                .MakeGenericMethod(typeof(string), typeof(int)),
                        "OverloadDifferentArgNumber<string,int>(`4)");
    }

    #endregion

    #endregion
}
