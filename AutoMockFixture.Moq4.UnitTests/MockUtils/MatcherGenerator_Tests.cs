using AutoMockFixture.MockUtils;

namespace AutoMockFixture.Tests.MockUtils;

public class MatcherGenerator_Tests
{
    //public class Sub : MatcherGenerator_Tests.TestClass { }
    private interface ISomeInterface { }
    public interface ISomeInterface2 { }

    private class TestClass : MatcherGenerator_Tests.ISomeInterface
    {
        public void TestSingleInterface<T>() where T : MatcherGenerator_Tests.ISomeInterface { }
        public void TestMultipleInterfaces<T>() where T : MatcherGenerator_Tests.ISomeInterface, MatcherGenerator_Tests.ISomeInterface2 { }
        public void TestClassNoInterface<T>() where T : TestClass { }
        public void TestClassWithSingleInterface<T>() where T : TestClass, MatcherGenerator_Tests.ISomeInterface { }
        public void TestClassWithMultipleInterfaces<T>() where T : TestClass, MatcherGenerator_Tests.ISomeInterface, MatcherGenerator_Tests.ISomeInterface2 { }
    }

    private void Executer(string methodName)
    {
        var obj = new TestClass();
        var method = typeof(TestClass).GetMethod(methodName);
        var matcher = MatcherGenerator.GetGenericMatcher(method!.GetGenericArguments().First());

        var geneticMethod = method.MakeGenericMethod(matcher);

        matcher.Should().NotBeNull();
        Assert.DoesNotThrow(() => geneticMethod.Invoke(obj, new object[] { }));
    }

    [Test]
    public void Test_GetGenericMatcher_Works_WithSingleInterface() => Executer(nameof(TestClass.TestSingleInterface));

    [Test]
    public void Test_GetGenericMatcher_Works_WithMultipleInterfaces() => Executer(nameof(TestClass.TestMultipleInterfaces));        

    [Test]
    public void Test_GetGenericMatcher_Works_WithClassNoInterface() => Executer(nameof(TestClass.TestClassNoInterface));

    [Test]
    public void Test_GetGenericMatcher_Works_WithClassWithSingleInterface() 
            => Executer(nameof(TestClass.TestClassWithSingleInterface));

    [Test]
    public void Test_GetGenericMatcher_Works_WithClassWithMultipleInterfaces() 
            => Executer(nameof(TestClass.TestClassWithMultipleInterfaces));
}
