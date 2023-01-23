using System.Reflection;

namespace AutoMockFixture.Tests.Extensions;

internal class MethodInfoExtensions_Tests
{
    [Test]
    [TestCase(nameof(TestClass.NonOverload), false)]
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
    [TestCase(nameof(TestClass.OverloadDifferentArgNumber), false)]
    public void Test_HasOverloadSameCount_ReturnsCorrectly(string name, bool expectedHasSameCount)
    {
        var hasSameCount = GetMethod(m => m.Name == name).HasOverloadSameCount();

        hasSameCount.Should().Be(expectedHasSameCount);
    }

    [Test]
    [TestCase(nameof(TestClass.NonOverload),"NonOverload")]
    [TestCase(nameof(TestClass.NonOverloadWithArgs), "NonOverloadWithArgs")]
    [TestCase(nameof(TestClass.NonOverloadWithGenericArgs),"NonOverloadWithGenericArgs`1")]      
    [TestCase(nameof(TestClass.NonOverloadWithNonUsingGenericArgs), "NonOverloadWithNonUsingGenericArgs`1")]        
    public void TestGetTrackingPath_ReturnsCorrectly_ForNonOverloads(string name, string expectedTrackingPath)
    {
        var trackingPath = GetMethod(m => m.Name == name).GetTrackingPath();

        trackingPath.Should().Be(expectedTrackingPath);
    }

    [Test]
    [TestCase(nameof(TestClass.NonOverloadWithGenericArgs), typeof(int), "NonOverloadWithGenericArgs<Int32>")]
    [TestCase(nameof(TestClass.NonOverloadWithGenericArgs), typeof(string), "NonOverloadWithGenericArgs<String>")]
    [TestCase(nameof(TestClass.NonOverloadWithNonUsingGenericArgs), typeof(int), "NonOverloadWithNonUsingGenericArgs<Int32>")]
    [TestCase(nameof(TestClass.NonOverloadWithNonUsingGenericArgs), typeof(string), "NonOverloadWithNonUsingGenericArgs<String>")]
    public void TestGetTrackingPath_ReturnsCorrectly_ForConstructudNonOverloads(string name, Type type, string expectedTrackingPath)
    {
        var trackingPath = GetMethod(m => m.Name == name).MakeGenericMethod(type).GetTrackingPath();

        trackingPath.Should().Be(expectedTrackingPath);
    }

    static MethodInfo GetMethod(Func<MethodInfo, bool> func) => typeof(TestClass).GetMethods().First(func);

    public static object[][] OverloadMethods = new object[][]
    {
        new object[]{ GetMethod(m => m.Name == nameof(TestClass.OverloadSameArgNumber)
                && m.GetParameters().First().ParameterType == typeof(int)),
            "OverloadSameArgNumber(Int32)"},
        new object[]{ GetMethod(m => m.Name == nameof(TestClass.OverloadSameArgNumber)
                && m.GetParameters().First().ParameterType == typeof(string)),
            "OverloadSameArgNumber(String)"},
        new object[]{ GetMethod(m => m.Name == nameof(TestClass.OverloadSameArgNumber)
                && m.IsGenericMethod && m.GetGenericArguments().Length == 1),
            "OverloadSameArgNumber`1(T)"},
        new object[]{ GetMethod(m => m.Name == nameof(TestClass.OverloadSameArgNumber)
                    && m.IsGenericMethod && m.GetGenericArguments().Length == 1)
                .MakeGenericMethod(typeof(string)),
            "OverloadSameArgNumber<String>(String)"},
        new object[]{ GetMethod(m => m.Name == nameof(TestClass.OverloadSameArgNumber)
                && m.IsGenericMethod && m.GetGenericArguments().Length == 2),
            "OverloadSameArgNumber`2(T1)"},
        new object[]{ GetMethod(m => m.Name == nameof(TestClass.OverloadSameArgNumber)
                    && m.IsGenericMethod && m.GetGenericArguments().Length == 2)
                .MakeGenericMethod(typeof(string), typeof(int)),
            "OverloadSameArgNumber<String,Int32>(String)"},

        new object[]{ GetMethod(m => m.Name == nameof(TestClass.OverloadDifferentArgNumber)
                && !m.GetParameters().Any()),
            "OverloadDifferentArgNumber(`0)"},
        new object[]{ GetMethod(m => m.Name == nameof(TestClass.OverloadDifferentArgNumber)
                && m.GetParameters().Length == 1),
            "OverloadDifferentArgNumber(`1)"},
        new object[]{ GetMethod(m => m.Name == nameof(TestClass.OverloadDifferentArgNumber)
                && m.GetParameters().Length == 2),
            "OverloadDifferentArgNumber(`2)"},
        new object[]{ GetMethod(m => m.Name == nameof(TestClass.OverloadDifferentArgNumber)
                && m.GetParameters().Length == 3),
            "OverloadDifferentArgNumber`1(`3)"},
        new object[]{ GetMethod(m => m.Name == nameof(TestClass.OverloadDifferentArgNumber)
                   && m.GetParameters().Length == 3)
                .MakeGenericMethod(typeof(string)),
            "OverloadDifferentArgNumber<String>(`3)"},
        new object[]{ GetMethod(m => m.Name == nameof(TestClass.OverloadDifferentArgNumber)
                && m.GetParameters().Length == 4),
            "OverloadDifferentArgNumber`2(`4)"},
        new object[]{ GetMethod(m => m.Name == nameof(TestClass.OverloadDifferentArgNumber)
                    && m.GetParameters().Length == 4)
                .MakeGenericMethod(typeof(string), typeof(int)),
            "OverloadDifferentArgNumber<String,Int32>(`4)"},
    };

       

    [Test]
    [TestCaseSource(nameof(MethodInfoExtensions_Tests.OverloadMethods))]

    public void TestGetTrackingPath_ReturnsCorrectly_WithOverloads(MethodInfo method, string expectedTrackingPath)
    {
        var trackingPath = method.HasOverloadSameCount();

        trackingPath.Should().Equals(expectedTrackingPath);
    }

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
}
