using AutoMockFixture.Moq4.AutoMockProxy;
using Moq;

namespace AutoMockFixture.Moq4.UnitTests.AutoMockProxy;

public class InterceptorWithFixInterfaces_Tests
{
    public class TestClass
    {
        public void TestMethod() { }
        public void TestMethod(int i) { }
        public void TestMethod(string s) { }
        public void TestMethod(int i, string s) { }
        public void TestMethod<T>(int i, string s) { }
    }

    [Test]
    public void Test_Intercept_DoesNotCrashOnAmbiguousMethod_BugRepro()
    {
        var method = typeof(TestClass).GetMethods().First();
        var invocation = Mock.Of<Castle.DynamicProxy.IInvocation>(i => i.Proxy == new TestClass() && i.TargetType == typeof(TestClass) && i.Method == method);

        var interceptor = new InterceptorWithFixInterfaces(Mock.Of<Castle.DynamicProxy.IInterceptor>());

        Assert.DoesNotThrow(() => interceptor.Intercept(invocation));
    }
}
