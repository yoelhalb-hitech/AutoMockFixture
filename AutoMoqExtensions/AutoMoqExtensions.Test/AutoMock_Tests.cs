
using AutoMoqExtensions.Test.AutoMockFixture_Tests;
using Moq;
using System.Reflection;

namespace AutoMoqExtensions.Test;

public class AutoMock_Tests
{
    [Test]
    public void Test_ResetGenerator()
    {
        var moqAssembly = typeof(Mock).Assembly;

        var proxyFactoryType = moqAssembly.GetType("Moq.ProxyFactory");
        var castleProxyFactoryInstance = proxyFactoryType!.GetProperty("Instance")!.GetValue(null);

        var castleProxyFactoryType = moqAssembly.GetType("Moq.CastleProxyFactory");
        var generatorFieldInfo = castleProxyFactoryType!.GetField("generator", BindingFlags.NonPublic | BindingFlags.Instance);
        
        var originalProxyGenerator = generatorFieldInfo!.GetValue(castleProxyFactoryInstance);

        var mock = new AutoMock<WithCtorArgsTestClass>();
        var obj = mock.Object;

        generatorFieldInfo!.GetValue(castleProxyFactoryInstance).Should().Be(originalProxyGenerator);
    }

    [Test]
    public void Test_ImplicitConversion()
    {
        var mock = new AutoMock<List<string>>();
        List<string> l = mock;

        l.Should().BeSameAs(mock.Object);
    }

    internal class TestingCtor 
    {
        public TestingCtor() { throw new Exception(); }        
    }

    [Test]
    public void Test_AutoMock_DoesNotCallCtor_WhenNotCallBase()
    {
        var mock = new AutoMock<TestingCtor>();
        mock.CallBase = false;
        Assert.DoesNotThrow(() => mock.GetMocked());
    }

    [Test]
    public void Test_AutoMock_CallsCtor_WhenCallBase()
    {
        var mock = new AutoMock<TestingCtor>();
        mock.CallBase = true;
        Assert.Catch(() => mock.GetMocked());
    }

    public class TestingTarget
    {
        public virtual string? Testing { get; set; }
    }

    [Test]
    public void Test_AutoMock_SetTarget()
    {
        var target = new TestingTarget { Testing = "FromTarget" };
        var mock = new AutoMock<TestingTarget>();
        mock.CallBase = true;
        mock.SetTarget(target);
        mock.Object.Testing.Should().Be(target.Testing);
    }

    #region Action

    [Test]
    public void Test_Setup_Action_Works()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(m => m.TestMethod1, Times.Once());
        mock.Object.TestMethod1();

        Assert.DoesNotThrow(() => mock.VerifyAll());
    }

    [Test]
    public void Test_Setup_Action_Works_WithString()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(nameof(Test.TestMethod1), Times.Once());
        mock.Object.TestMethod1();

        Assert.DoesNotThrow(() => mock.VerifyAll());
    }

    [Test]
    public void Test_Setup_Action_ThrowsOnVerify()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(m => m.TestMethod1, Times.Exactly(2));
        mock.Object.TestMethod1();

        Assert.Throws<MockException>(() => mock.VerifyAll());
    }

    [Test]
    public void Test_Setup_Action_ThrowsOnVerify_WithString()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(nameof(Test.TestMethod1), Times.Exactly(2));
        mock.Object.TestMethod1();

        Assert.Throws<MockException>(() => mock.VerifyAll());
    }

    [Test]
    public void Test_Setup_Action_Works_WithParams()
    {
        var mock = new AutoMock<Test>();

        mock.Setup<string, int, decimal>(m => m.TestMethod2, Times.Once());
        mock.Object.TestMethod2("str", 10, 6.95m);

        Assert.DoesNotThrow(() => mock.VerifyAll());
    }


    [Test]
    public void Test_Setup_Action_Works_WithParams_WithString()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(nameof(Test.TestMethod2), Times.Once());
        mock.Object.TestMethod2("str", 10, 6.95m);

        Assert.DoesNotThrow(() => mock.VerifyAll());
    }


    [Test]
    public void Test_Setup_Action_ThrowsOnVerify_WithParams()
    {
        var mock = new AutoMock<Test>();

        mock.Setup<string, int, decimal>(m => m.TestMethod2, Times.Exactly(2));
        mock.Object.TestMethod2("str", 10, 6.95m);

        Assert.Throws<MockException>(() => mock.VerifyAll());
    }

    [Test]
    public void Test_Setup_Action_ThrowsOnVerify_WithParams_WithString()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(nameof(Test.TestMethod2), Times.Exactly(2));
        mock.Object.TestMethod2("str", 10, 6.95m);

        Assert.Throws<MockException>(() => mock.VerifyAll());
    }

    [Test]
    public void Test_Setup_Action_Works_BasedOnParam()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(m => (Action<string, int, decimal>)m.TestMethod2, new { i = 15 }, Times.Once());
        mock.Object.TestMethod2("str", 15, 6.95m);

        Assert.DoesNotThrow(() => mock.VerifyAll());
    }

    [Test]
    public void Test_Setup_Action_Works_BasedOnParam_WithString()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(nameof(Test.TestMethod2), new { i = 15 }, Times.Once());
        mock.Object.TestMethod2("str", 15, 6.95m);

        Assert.DoesNotThrow(() => mock.VerifyAll());
    }

    [Test]
    public void Test_Setup_Action_ThrowsOnVerify_BasedOnParam()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(m => (Action<string, int, decimal>)m.TestMethod2, new { i = 15 }, Times.Once());
        mock.Object.TestMethod2("str", 10, 6.95m);

        Assert.Throws<MockException>(() => mock.VerifyAll());
    }

    [Test]
    public void Test_Setup_Action_ThrowsOnVerify_BasedOnParam_WithString()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(nameof(Test.TestMethod2), new { i = 15 }, Times.Once());
        mock.Object.TestMethod2("str", 10, 6.95m);

        Assert.Throws<MockException>(() => mock.VerifyAll());
    }

    #endregion
    #region Func

    [Test]
    public void Test_Setup_Func_Works()
    {
        var mock = new AutoMock<Test>();

        mock.Setup<int>(m => m.TestMethod3, Times.Once());
        mock.Object.TestMethod3();

        Assert.DoesNotThrow(() => mock.VerifyAll());
    }

    [Test]
    public void Test_Setup_Func_Works_WithString()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(nameof(Test.TestMethod3), Times.Once());
        mock.Object.TestMethod3();

        Assert.DoesNotThrow(() => mock.VerifyAll());
    }

    [Test]
    public void Test_Setup_Func_ThrowsOnVerify()
    {
        var mock = new AutoMock<Test>();

        mock.Setup<int>(m => m.TestMethod3, Times.Exactly(2));
        mock.Object.TestMethod3();

        Assert.Throws<MockException>(() => mock.VerifyAll());
    }

    [Test]
    public void Test_Setup_Func_ThrowsOnVerify_WithString()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(nameof(Test.TestMethod3), Times.Exactly(2));
        mock.Object.TestMethod3();

        Assert.Throws<MockException>(() => mock.VerifyAll());
    }

    [Test]
    public void Test_Setup_Func_Works_WithParams()
    {
        var mock = new AutoMock<Test>();

        mock.Setup<string, int, decimal, int>(m => m.TestMethod4, Times.Once());
        mock.Object.TestMethod4("str", 10, 6.95m);

        Assert.DoesNotThrow(() => mock.VerifyAll());
    }


    [Test]
    public void Test_Setup_Func_Works_WithParams_WithString()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(nameof(Test.TestMethod4), Times.Once());
        mock.Object.TestMethod4("str", 10, 6.95m);

        Assert.DoesNotThrow(() => mock.VerifyAll());
    }


    [Test]
    public void Test_Setup_Func_ThrowsOnVerify_WithParams()
    {
        var mock = new AutoMock<Test>();

        mock.Setup<string, int, decimal, int>(m => m.TestMethod4, Times.Exactly(2));
        mock.Object.TestMethod4("str", 10, 6.95m);

        Assert.Throws<MockException>(() => mock.VerifyAll());
    }

    [Test]
    public void Test_Setup_Func_ThrowsOnVerify_WithParams_WithString()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(nameof(Test.TestMethod4), Times.Exactly(2));
        mock.Object.TestMethod4("str", 10, 6.95m);

        Assert.Throws<MockException>(() => mock.VerifyAll());
    }

    [Test]
    public void Test_Setup_Func_Works_BasedOnParam()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(m => (Func<string, int, decimal, int>)m.TestMethod4, new { i = 15 }, Times.Once());
        mock.Object.TestMethod4("str", 15, 6.95m);

        Assert.DoesNotThrow(() => mock.VerifyAll());
    }

    [Test]
    public void Test_Setup_Func_Works_BasedOnParam_WithString()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(nameof(Test.TestMethod4), new { i = 15 }, Times.Once());
        mock.Object.TestMethod4("str", 15, 6.95m);

        Assert.DoesNotThrow(() => mock.VerifyAll());
    }

    [Test]
    public void Test_Setup_Func_ThrowsOnVerify_BasedOnParam()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(m => (Func<string, int, decimal, int>)m.TestMethod4, new { i = 15 }, Times.Once());
        mock.Object.TestMethod4("str", 10, 6.95m);

        Assert.Throws<MockException>(() => mock.VerifyAll());
    }

    [Test]
    public void Test_Setup_Func_ThrowsOnVerify_BasedOnParam_WithString()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(nameof(Test.TestMethod4), new { i = 15 }, Times.Once());
        mock.Object.TestMethod4("str", 10, 6.95m);

        Assert.Throws<MockException>(() => mock.VerifyAll());
    }

    #endregion



    public class Test
    {
        public virtual void TestMethod1() { }
        public virtual void TestMethod2(string str, int i, decimal m) {}
        public virtual int TestMethod3() { return 10; }
        public virtual int TestMethod4(string str, int i, decimal m) { return 10; }
    }
}
