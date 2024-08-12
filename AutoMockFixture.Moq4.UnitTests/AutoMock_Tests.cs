using AutoMockFixture.Moq4.AutoMockProxy;
using Castle.DynamicProxy;
using DotNetPowerExtensions.Reflection;
using Moq;
using System.Collections;
using System.Reflection;

namespace AutoMockFixture.Tests;

public class AutoMock_Tests
{
    [Test]
    public void Test_ResetGenerator_ResetsCorrectly_BugRepro()
    {
        // The bug was that the code for the capturing the Moq generator was done in the static ctor for `AutoMock<T>`
        // So when `AutoMock<IEnumerableTest>.Object` was called it changed the generator to the `AutoMockProxyGenerator`
        // While in that state and as part of the object generation it created `AutoMock<TypeInfo>`
        // So the ctor of `AutoMock<TypeInfo>` captured `AutoMockProxyGenerator` instead of the original Moq generator!!
        // While it was first reset when the `ResetGenerator()` was called for `IEnumerableTest`
        // But when later trying to setup for `GetEnumerator()` CastleProxy called again `AutoMock<TypeInfo>.Object` to get the return type of the method
        // But now when `ResetGenerator()` was called it reset to the one captured by `AutoMock<TypeInfo>` which was `AutoMockProxyGenerator`!!

        using var fixture = new AbstractAutoMockFixture();
        _ = fixture.CreateAutoMock<IEnumerableTest>()!;

        var castleProxyFactoryType = typeof(Moq.CastleProxyFactory);
        var generatorFieldInfo = castleProxyFactoryType.GetField("generator", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var generator = (ProxyGenerator)generatorFieldInfo.GetValue(Moq.ProxyFactory.Instance)!;

        generator.Should().BeOfType<Castle.DynamicProxy.ProxyGenerator>();
    }

    [Test]
    public void Test_ResetGenerator()
    {
        var generatorFieldInfo = typeof(Moq.CastleProxyFactory)!.GetField("generator", BindingFlags.NonPublic | BindingFlags.Instance);

        var mock = new AutoMock<WithCtorArgsTestClass>();
        var obj = mock.Object;

        var currentValue = generatorFieldInfo!.GetValue(Moq.ProxyFactory.Instance);

        currentValue.Should().BeAssignableTo<ProxyGenerator>();

        if(currentValue is AutoMockProxyGenerator automockGenerator)
        {
            automockGenerator.Target.Should().BeNull();
            automockGenerator.CallBase.Should().BeNull();
        }
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
    public void Test_Setup_Func_Works_WithString_WhenComplexType()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(nameof(Test.TestMethod5), Times.Once());
        mock.Object.TestMethod5();

        Assert.DoesNotThrow(() => mock.Verify());
    }

    [Test]
    public void Test_Setup_Func_Works_WithString_WhenProtected()
    {
        var mock = new AutoMock<Test>();

        mock.Setup("TestProtected1", Times.Once());
        typeof(Test).GetMethod("TestProtected1", BindingFlagsExtensions.AllBindings)!.Invoke(mock.Object, new object[] { });

        Assert.DoesNotThrow(() => mock.Verify());
    }


    [Test]
    public void Test_Setup_Func_Works_WithStringAndReturn()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(nameof(Test.TestMethod3), new { }, 10, Times.Once());
        var result = mock.Object.TestMethod3();

        Assert.DoesNotThrow(() => mock.Verify());

        result.Should().Be(10);
    }

    [Test]
    public void Test_Setup_Func_Works_WithStringAndReturn_WhenComplexType()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(nameof(Test.TestMethod5), new { }, new EmptyType(), Times.Once());
        var result = mock.Object.TestMethod5();

        Assert.DoesNotThrow(() => mock.Verify());

        result.Should().NotBeNull();
    }

    [Test]
    public void Test_Setup_Func_Works_WithStringAndReturn_WhenProtected()
    {
        var mock = new AutoMock<Test>();

        mock.Setup("TestProtected1", new { }, 10, Times.Once());
        var result = typeof(Test).GetMethod("TestProtected1", BindingFlagsExtensions.AllBindings)!.Invoke(mock.Object, new object[] { });

        Assert.DoesNotThrow(() => mock.Verify());

        result.Should().Be(10);
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
    public void Test_Setup_Func_Works_WithParams_WithString_WhenComplexType()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(nameof(Test.TestMethod6), Times.Once());
        mock.Object.TestMethod6("str", 10, 6.95m);

        Assert.DoesNotThrow(() => mock.Verify());
    }

    [Test]
    public void Test_Setup_Func_Works_WithParams_WithStringAndReturn()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(nameof(Test.TestMethod4), new { }, 10, Times.Once());
        var result = mock.Object.TestMethod4("str", 10, 6.95m);

        Assert.DoesNotThrow(() => mock.Verify());

        result.Should().Be(10);
    }


    [Test]
    public void Test_Setup_Func_Works_WithParams_WithStringAndReturn_WhenComplexType()
    {
        var mock = new AutoMock<Test>();

        mock.Setup(nameof(Test.TestMethod6), new { }, new EmptyType(), Times.Once());
        var result = mock.Object.TestMethod6("str", 10, 6.95m);

        Assert.DoesNotThrow(() => mock.Verify());

        result.Should().NotBeNull();
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
        public virtual EmptyType? TestMethod5() { return null; }
        public virtual EmptyType? TestMethod6(string str, int i, decimal m) { return null; }
        protected virtual int TestProtected1() { return 10; }
        protected virtual int TestProtected2(string str, int i, decimal m) { return 10; }
    }

    public abstract class IEnumerableTest : IEnumerable<int>
    {
        public abstract IEnumerator<int> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class EmptyType { }
}
