
using DotNetPowerExtensions.Reflection;

namespace AutoMockFixture.Moq4.IntegrationTests.AutoMock;

internal class ExplicitInterfaceImplementation_Tests
{
    public interface ITest
    {
        public int TestProp { get; }
        public int TestMethod();
    }

    public class Test : ITest
    {
        int ITest.TestProp { get => 10; }

        int ITest.TestMethod() => 20;
    }

    [Test]
    public void Test_WorksWithDefaultCallBase_NoSetup()
    {
        var autoMock = new AutoMock<Test>() { CallBase = true };

        var itest = autoMock.Object as ITest;
        itest.TestProp.Should().Be(10);
        itest.TestMethod().Should().Be(20);
    }

    [Test]
    public void Test_WorksWithDefaultCallBase_WithSetup_UsingAs()
    {
        var autoMock = new AutoMock<Test>() { CallBase = true };
        var asItest = autoMock.As<ITest>();
        asItest.Setup(i => i.TestProp).Returns(50);
        asItest.Setup(i => i.TestMethod()).Returns(100);

        var itest = autoMock.Object as ITest;
        itest.TestProp.Should().Be(50);
        itest.TestMethod().Should().Be(100);
    }

    [Test]
    public void Test_WorksWithDefaultCallBase_WithSetup_UsingAs_AfterCreating()
    {
        var autoMock = new AutoMock<Test>() { CallBase = true };

        var itest = autoMock.Object as ITest;

        var asItest = autoMock.As<ITest>();
        asItest.Setup(i => i.TestProp).Returns(50);
        asItest.Setup(i => i.TestMethod()).Returns(100);

        itest.TestProp.Should().Be(50);
        itest.TestMethod().Should().Be(100);
    }

    [Test]
    public void Test_WorksWithDefaultCallBase_WithSetup_UsingReflection()
    {
        var autoMock = new AutoMock<Test>() { CallBase = true };
        autoMock.Setup(typeof(ITest).GetProperty(nameof(ITest.TestProp), BindingFlagsExtensions.AllBindings)!.GetMethod!, new { }, 50);
        autoMock.Setup(typeof(ITest).GetMethod(nameof(ITest.TestMethod), BindingFlagsExtensions.AllBindings)!, new { }, 100);

        var itest = autoMock.Object as ITest;
        itest.TestProp.Should().Be(50);
        itest.TestMethod().Should().Be(100);
    }

    [Test]
    public void Test_WorksWithDefaultNonCallBase_NoSetup()
    {
        var autoMock = new AutoMock<Test>() { CallBase = false };

        var itest = autoMock.Object as ITest;
        itest.TestProp.Should().Be(0);
        itest.TestMethod().Should().Be(0);
    }

    [Test]
    public void Test_WorksWithDefaultNonCallBase_WithSetup_UsingAs()
    {
        var autoMock = new AutoMock<Test>() { CallBase = false };
        var asItest = autoMock.As<ITest>();
        asItest.Setup(i => i.TestProp).Returns(50);
        asItest.Setup(i => i.TestMethod()).Returns(100);

        var itest = autoMock.Object as ITest;
        itest.TestProp.Should().Be(50);
        itest.TestMethod().Should().Be(100);
    }

    [Test]
    public void Test_WorksWithDefaultNonCallBase_WithSetup_UsingAs_AfterCreating()
    {
        var autoMock = new AutoMock<Test>() { CallBase = false };
        var itest = autoMock.Object as ITest;

        var asItest = autoMock.As<ITest>();
        asItest.Setup(i => i.TestProp).Returns(50);
        asItest.Setup(i => i.TestMethod()).Returns(100);

        itest.TestProp.Should().Be(50);
        itest.TestMethod().Should().Be(100);
    }

    [Test]
    public void Test_WorksWithDefaultNonCallBase_WithSetup_UsingRefelction()
    {
        var autoMock = new AutoMock<Test>() { CallBase = false };
        autoMock.Setup(typeof(ITest).GetProperty(nameof(ITest.TestProp), BindingFlagsExtensions.AllBindings)!.GetMethod!, new { }, 50);
        autoMock.Setup(typeof(ITest).GetMethod(nameof(ITest.TestMethod), BindingFlagsExtensions.AllBindings)!, new { }, 100);

        var itest = autoMock.Object as ITest;
        itest.TestProp.Should().Be(50);
        itest.TestMethod().Should().Be(100);
    }

    [Test]
    public void Test_WorksWithDefaultNonCallBase_WithSetupForCallBase_UsingAs()
    {
        var autoMock = new AutoMock<Test>() { CallBase = false };
        var asItest = autoMock.As<ITest>();
        asItest.Setup(i => i.TestProp).CallBase();
        asItest.Setup(i => i.TestMethod()).CallBase();

        var itest = autoMock.Object as ITest;
        itest.TestProp.Should().Be(10);
        itest.TestMethod().Should().Be(20);
    }

    [Test]
    public void Test_WorksWithDefaultNonCallBase_WithSetupForCallBase_UsingAs_AfterCreating()
    {
        var autoMock = new AutoMock<Test>() { CallBase = false };
        var itest = autoMock.Object as ITest;

        var asItest = autoMock.As<ITest>();
        asItest.Setup(i => i.TestProp).CallBase();
        asItest.Setup(i => i.TestMethod()).CallBase();

        itest.TestProp.Should().Be(10);
        itest.TestMethod().Should().Be(20);
    }
}
