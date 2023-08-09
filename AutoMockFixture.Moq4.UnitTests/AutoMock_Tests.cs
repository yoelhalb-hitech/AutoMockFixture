using AutoMockFixture.Moq4.AutoMockProxy;
using Moq;
using System.Reflection;

namespace AutoMockFixture.Tests;

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

        var mock = new AutoMock<WithCtorArgsTestClass>();
        var obj = mock.Object;

        var currentValue = generatorFieldInfo!.GetValue(castleProxyFactoryInstance);

        currentValue.Should().BeOfType<AutoMockProxyGenerator>();

        (currentValue as AutoMockProxyGenerator)!.Target.Should().BeNull();
        (currentValue as AutoMockProxyGenerator)!.Callbase.Should().BeNull();
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

    #region Target

    #region Target Basic

    public class TestingTarget
    {
        public virtual string? Testing { get; set; }
    }

    [Test]
    public void Test_AutoMock_SetTarget_HasTargetValue()
    {
        var target = new TestingTarget { Testing = "FromTarget" };
        var mock = new AutoMock<TestingTarget>();
        mock.CallBase = true;
        mock.SetTarget(target);
        mock.Object.Testing.Should().Be(target.Testing);

        target.Testing = "Testing2";
        mock.Object.Testing.Should().Be(target.Testing);

        mock.Target.Should().Be(target);
    }

    [Test]
    public void Test_AutoMock_SetTarget_HasTargetValueAfterUpdate()
    {
        var target = new TestingTarget { Testing = "FromTarget" };
        var mock = new AutoMock<TestingTarget>();
        mock.CallBase = true;
        mock.SetTarget(target);
        mock.Object.Testing.Should().Be(target.Testing);

        target.Testing = "TestingUpdate";
        mock.Object.Testing.Should().Be("TestingUpdate");

        mock.Target.Should().Be(target);
    }

    [Test]
    public void Test_AutoMock_SetTarget_UpdatesTarget()
    {
        var target = new TestingTarget { Testing = "FromTarget" };
        var mock = new AutoMock<TestingTarget>();
        mock.CallBase = true;
        mock.SetTarget(target);
        mock.Object.Testing.Should().Be(target.Testing);

        mock.Object.Testing = "TestingUpdatesTarget";
        target.Testing.Should().Be("TestingUpdatesTarget");

        mock.Target.Should().Be(target);
    }

    #endregion

    #region Target Complex


    public interface ITestingTargetComplex
    {
        string? TestProp { get; set; }
        string TestPropGet { get; }
        string Test();
    }
    public abstract class AbstractTestingTargetComplex : ITestingTargetComplex
    {
        public abstract string TestPropGet { get; }
        public abstract string? TestProp { get; set; }
        public abstract string Test();
    }
    class TestingTargetComplex : AbstractTestingTargetComplex
    {
        private bool firstTime = true;
        public override string TestPropGet
        {
            get
            {
                if (!firstTime) return "TestPropGet_SecondTime";
                firstTime = false;
                return "TestPropGet_FirstTime";
            }
        }
        public override string? TestProp { get; set; }
        public override string Test() => "In Test";
    }

    [Test]
    public void Test_AutoMock_SetTarget_ByInterface()
    {
        var target = new TestingTargetComplex { TestProp = "234" };
        var mock = new AutoMock<ITestingTargetComplex>();
        mock.CallBase = true;
        mock.SetTarget(target);


        mock.Object.TestProp.Should().Be("234");
        target.TestProp = "8888";
        mock.Object.TestProp.Should().Be("8888");

        mock.Object.TestProp = "5555";
        target.TestProp.Should().Be("5555");

        mock.Object.TestPropGet.Should().Be("TestPropGet_FirstTime");
        mock.Object.TestPropGet.Should().Be("TestPropGet_SecondTime");

        mock.Object.Test().Should().Be("In Test");

        mock.Target.Should().Be(target);
    }

    [Test]
    public void Test_AutoMock_SetTarget_ByAbstract()
    {
        var target = new TestingTargetComplex { TestProp = "234" };
        var mock = new AutoMock<AbstractTestingTargetComplex>();
        mock.CallBase = true;
        mock.SetTarget(target);


        mock.Object.TestProp.Should().Be("234");
        target.TestProp = "8888";
        mock.Object.TestProp.Should().Be("8888");

        mock.Object.TestProp = "5555";
        target.TestProp.Should().Be("5555");

        mock.Object.TestPropGet.Should().Be("TestPropGet_FirstTime");
        mock.Object.TestPropGet.Should().Be("TestPropGet_SecondTime");

        mock.Object.Test().Should().Be("In Test");

        mock.Target.Should().Be(target);
    }

    #endregion

    #endregion

    #region Action

    [Test]
    [Obsolete]
    public void Test_Setup_Action_Works()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(m => m.TestMethod1, Times.Once());
        mock.Object.TestMethod1();

        Assert.DoesNotThrow(() => mock.Verify());
    }

    [Test]
    public void Test_Setup_Action_Works_WithString()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(nameof(Test.TestMethod1), Times.Once());
        mock.Object.TestMethod1();

        Assert.DoesNotThrow(() => mock.Verify());
    }

    [Test]
    [Obsolete]
    public void Test_Setup_Action_ThrowsOnVerify()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(m => m.TestMethod1, Times.Exactly(2));
        mock.Object.TestMethod1();

        Assert.Throws<MockException>(() => mock.Verify());
    }

    [Test]
    public void Test_Setup_Action_ThrowsOnVerify_WithString()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(nameof(Test.TestMethod1), Times.Exactly(2));
        mock.Object.TestMethod1();

        Assert.Throws<MockException>(() => mock.Verify());
    }

    [Test]
    [Obsolete]
    public void Test_Setup_Action_Works_WithParams()
    {
        var mock = new AutoMock<Test>();

        mock.Setup<string, int, decimal>(m => m.TestMethod2, Times.Once());
        mock.Object.TestMethod2("str", 10, 6.95m);

        Assert.DoesNotThrow(() => mock.Verify());
    }


    [Test]
    public void Test_Setup_Action_Works_WithParams_WithString()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(nameof(Test.TestMethod2), Times.Once());
        mock.Object.TestMethod2("str", 10, 6.95m);

        Assert.DoesNotThrow(() => mock.Verify());
    }


    [Test]
    [Obsolete]
    public void Test_Setup_Action_ThrowsOnVerify_WithParams()
    {
        var mock = new AutoMock<Test>();

        mock.Setup<string, int, decimal>(m => m.TestMethod2, Times.Exactly(2));
        mock.Object.TestMethod2("str", 10, 6.95m);

        Assert.Throws<MockException>(() => mock.Verify());
    }

    [Test]
    public void Test_Setup_Action_ThrowsOnVerify_WithParams_WithString()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(nameof(Test.TestMethod2), Times.Exactly(2));
        mock.Object.TestMethod2("str", 10, 6.95m);

        Assert.Throws<MockException>(() => mock.Verify());
    }

    [Test]
    [Obsolete]
    public void Test_Setup_Action_Works_BasedOnParam()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(m => (Action<string, int, decimal>)m.TestMethod2, new { i = 15 }, Times.Once());
        mock.Object.TestMethod2("str", 15, 6.95m);

        Assert.DoesNotThrow(() => mock.Verify());
    }

    [Test]
    public void Test_Setup_Action_Works_BasedOnParam_WithString()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(nameof(Test.TestMethod2), new { i = 15 }, Times.Once());
        mock.Object.TestMethod2("str", 15, 6.95m);

        Assert.DoesNotThrow(() => mock.Verify());
    }

    [Test]
    [Obsolete]
    public void Test_Setup_Action_ThrowsOnVerify_BasedOnParam()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(m => (Action<string, int, decimal>)m.TestMethod2, new { i = 15 }, Times.Once());
        mock.Object.TestMethod2("str", 10, 6.95m);

        Assert.Throws<MockException>(() => mock.Verify());
    }

    [Test]
    public void Test_Setup_Action_ThrowsOnVerify_BasedOnParam_WithString()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(nameof(Test.TestMethod2), new { i = 15 }, Times.Once());
        mock.Object.TestMethod2("str", 10, 6.95m);

        Assert.Throws<MockException>(() => mock.Verify());
    }

    #endregion
    #region Func

    [Test]
    [Obsolete]
    public void Test_Setup_Func_Works()
    {
        var mock = new AutoMock<Test>();

        mock.Setup<int>(m => m.TestMethod3, Times.Once());
        mock.Object.TestMethod3();

        Assert.DoesNotThrow(() => mock.Verify());
    }

    [Test]
    public void Test_Setup_Func_Works_WithString()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(nameof(Test.TestMethod3), Times.Once());
        mock.Object.TestMethod3();

        Assert.DoesNotThrow(() => mock.Verify());
    }

    [Test]
    [Obsolete]
    public void Test_Setup_Func_ThrowsOnVerify()
    {
        var mock = new AutoMock<Test>();

        mock.Setup<int>(m => m.TestMethod3, Times.Exactly(2));
        mock.Object.TestMethod3();

        Assert.Throws<MockException>(() => mock.Verify());
    }

    [Test]
    public void Test_Setup_Func_ThrowsOnVerify_WithString()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(nameof(Test.TestMethod3), Times.Exactly(2));
        mock.Object.TestMethod3();

        Assert.Throws<MockException>(() => mock.Verify());
    }

    [Test]
    [Obsolete]
    public void Test_Setup_Func_Works_WithParams()
    {
        var mock = new AutoMock<Test>();

        mock.Setup<string, int, decimal, int>(m => m.TestMethod4, Times.Once());
        mock.Object.TestMethod4("str", 10, 6.95m);

        Assert.DoesNotThrow(() => mock.Verify());
    }


    [Test]
    public void Test_Setup_Func_Works_WithParams_WithString()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(nameof(Test.TestMethod4), Times.Once());
        mock.Object.TestMethod4("str", 10, 6.95m);

        Assert.DoesNotThrow(() => mock.Verify());
    }


    [Test]
    [Obsolete]
    public void Test_Setup_Func_ThrowsOnVerify_WithParams()
    {
        var mock = new AutoMock<Test>();

        mock.Setup<string, int, decimal, int>(m => m.TestMethod4, Times.Exactly(2));
        mock.Object.TestMethod4("str", 10, 6.95m);

        Assert.Throws<MockException>(() => mock.Verify());
    }

    [Test]
    public void Test_Setup_Func_ThrowsOnVerify_WithParams_WithString()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(nameof(Test.TestMethod4), Times.Exactly(2));
        mock.Object.TestMethod4("str", 10, 6.95m);

        Assert.Throws<MockException>(() => mock.Verify());
    }

    [Test]
    [Obsolete]
    public void Test_Setup_Func_Works_BasedOnParam()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(m => (Func<string, int, decimal, int>)m.TestMethod4, new { i = 15 }, Times.Once());
        mock.Object.TestMethod4("str", 15, 6.95m);

        Assert.DoesNotThrow(() => mock.Verify());
    }

    [Test]
    public void Test_Setup_Func_Works_BasedOnParam_WithString()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(nameof(Test.TestMethod4), new { i = 15 }, Times.Once());
        mock.Object.TestMethod4("str", 15, 6.95m);

        Assert.DoesNotThrow(() => mock.Verify());
    }

    [Test]
    [Obsolete]
    public void Test_Setup_Func_ThrowsOnVerify_BasedOnParam()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(m => (Func<string, int, decimal, int>)m.TestMethod4, new { i = 15 }, Times.Once());
        mock.Object.TestMethod4("str", 10, 6.95m);

        Assert.Throws<MockException>(() => mock.Verify());
    }

    [Test]
    public void Test_Setup_Func_ThrowsOnVerify_BasedOnParam_WithString()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(nameof(Test.TestMethod4), new { i = 15 }, Times.Once());
        mock.Object.TestMethod4("str", 10, 6.95m);

        Assert.Throws<MockException>(() => mock.Verify());
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
