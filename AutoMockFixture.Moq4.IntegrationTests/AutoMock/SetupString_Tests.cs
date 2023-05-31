
using DotNetPowerExtensions.Reflection;
using System.Reflection;

namespace AutoMockFixture.Moq4.IntegrationTests.AutoMock;

internal class SetupString_Tests
{
    public interface ITest
    {
        public int TestITestProp { get; }
        public int TestITestMethod();
    }

    public class Test : ITest
    {
        public virtual int TestProp { get; set; }
        public virtual int TestMethod() => 1;

        int ITest.TestITestProp { get => 10; }

        int ITest.TestITestMethod() => 20;

        public virtual int TestMethod2() => 30;
        public virtual int TestMethod2(int i) => 1;
    }


    [Test]
    public void Test_SetupString_WhenInClass()
    {
        var autoMock = new AutoMock<Test>();
        Assert.DoesNotThrow(() => autoMock.Setup(nameof(Test.TestProp), new { }, 50));
        Assert.DoesNotThrow(() => autoMock.Setup(nameof(Test.TestMethod), new { }, 100));

        var obj = autoMock.Object;
        obj.TestProp.Should().Be(50);
        obj.TestMethod().Should().Be(100);
    }

    [Test]
    public void Test_SetupString_WhenInClassInterface()
    {
        var autoMock = new AutoMock<Test>();

        Assert.DoesNotThrow(() => autoMock.Setup(nameof(ITest.TestITestProp), new { }, 50));
        Assert.DoesNotThrow(() => autoMock.Setup(nameof(ITest.TestITestMethod), new { }, 100));

        var itest = autoMock.Object as ITest;
        itest.TestITestProp.Should().Be(50);
        itest.TestITestMethod().Should().Be(100);
    }

    public interface ITest2
    {
        public int TestITestProp { get; }
        public int TestITestMethod();
    }

    public class Test2 : ITest, ITest2
    {
        int ITest.TestITestProp => 1;
        int ITest2.TestITestProp => 1;
        int ITest.TestITestMethod() => 1;
        int ITest2.TestITestMethod() => 1;
    }

    [Test]
    public void Test_SetupString_Throws_WhenInMultipleClassInterfaces()
    {
        var autoMock = new AutoMock<Test2>();

        Assert.Throws<InvalidOperationException>(() => autoMock.Setup(nameof(ITest.TestITestProp), new { }, 50), "Sequence contains more than one element");
        Assert.Throws<InvalidOperationException>(() => autoMock.Setup(nameof(ITest.TestITestMethod), new { }, 100), "Sequence contains more than one element");
    }

    [Test]
    public void Test_SetupString_Throws_WhenMultipleMethods()
    {
        var autoMock = new AutoMock<Test>();

        Assert.Throws<AmbiguousMatchException>(() => autoMock.Setup(nameof(Test.TestMethod2), new { }, 50), "Ambiguous match found");
    }

    [Test]
    public void Test_SetupString_Throws_WhenNotFound()
    {
        var autoMock = new AutoMock<Test>();

        Assert.Throws<MissingMethodException>(() => autoMock.Setup("TestMethod3", new { }, 50), "TestMethod3");
    }
}
