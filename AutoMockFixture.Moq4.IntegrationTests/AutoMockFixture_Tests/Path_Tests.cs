using System.ComponentModel;

namespace AutoMockFixture.Tests.AutoMockFixture_Tests;

internal class Path_Tests
{
    #region Utils
    public enum AutoMockType
    {
        NonAutoMock,
        AutoMock,
        AutoMockDependencies,
    }
    public static object[][] Types => new object[][]
    {
        new object[] { AutoMockType.NonAutoMock },
        new object[] { AutoMockType.AutoMock },
        new object[] { AutoMockType.AutoMockDependencies },
    };
    private T? GetObj<T>(AbstractAutoMockFixture fixture, AutoMockType type, bool? callBase = null) where T : class
    {
        return type switch
        {
            AutoMockType.NonAutoMock => fixture.CreateNonAutoMock<T>(callBase),
            AutoMockType.AutoMock => fixture.CreateAutoMock<T>(callBase),
            AutoMockType.AutoMockDependencies => fixture.CreateWithAutoMockDependencies<T>(callBase),
            _ => throw new InvalidEnumArgumentException(),
        };
    }
    #endregion

    [Test]
    [TestCaseSource(nameof(Types))]
    public void Test_MainObject(AutoMockType type)
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.MethodSetupType = MethodSetupTypes.Eager;

        var obj = GetObj<InternalSimpleTestClass>(fixture, type);

        obj!.Should().NotBeNull();
        var paths = fixture.GetPaths(obj!);

        fixture.GetAt(obj!, "").First().Should().Be(obj);
    }

    [Test]
    [TestCaseSource(nameof(Types))]
    public void Test_AskingForAutoMock_WorksCorrectly(AutoMockType type)
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.MethodSetupType = MethodSetupTypes.Eager;

        var obj = GetObj<AutoMock<InternalSimpleTestClass>>(fixture, type);

        obj!.Should().NotBeNull();
        obj.Should().BeAutoMock();

        var paths = fixture.GetPaths(obj!);

        fixture.GetAt(obj!, "").First().Should().Be(obj!.Object);
    }

    [Test]
    [TestCaseSource(nameof(Types))]
    public void Test_ReadWriteProperty(AutoMockType type)
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.MethodSetupType = MethodSetupTypes.Eager;

        var obj = GetObj<InternalSimpleTestClass>(fixture, type);

        obj.Should().NotBeNull();
        var paths = fixture.GetPaths(obj!);

        paths.Should().Contain(".InternalTest");
        fixture.GetAt(obj!, ".InternalTest").First().Should().Be(obj!.InternalTest);
    }

    [Test]
    [TestCase(AutoMockType.AutoMock)]
    public void Test_ReadOnlyProperty(AutoMockType type)
    {
        var fixture = new AbstractAutoMockFixture();

        var obj = GetObj<InternalReadonlyPropertyClass>(fixture, type);
        obj.Should().NotBeNull();

        var f = obj!.InternalTest; // We need first to invoke the property is it might be lazy
        var paths = fixture.GetPaths(obj);

        paths.Should().Contain(".InternalTest");
        fixture.GetAt(obj!, ".InternalTest").First().Should().Be(obj!.InternalTest);
    }

    [Test]
    [TestCase(AutoMockType.AutoMock)]
    public void Test_Method(AutoMockType type)
    {
        var fixture = new AbstractAutoMockFixture();

        var obj = GetObj<InternalTestMethods>(fixture, type);
        obj.Should().NotBeNull();

        var f = obj!.InternalTestMethod(); // We need first to invoke the property as it might be lazy
        var paths = fixture.GetPaths(obj);

        paths.Should().Contain(".InternalTestMethod");
    }

    public class TestDelegate
    {
        public virtual Func<object>? Delegate { get; set; }
    }

    [Test]
    [TestCase(AutoMockType.AutoMock)]
    public void Test_Delegate(AutoMockType type)
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.MethodSetupType = MethodSetupTypes.LazySame;

        var obj = GetObj<TestDelegate>(fixture, type);
        obj.Should().NotBeNull();

        obj!.Delegate.Should().NotBeNull();
        obj.Delegate!().Should().NotBeNull();
        obj.Delegate.Invoke().Should().NotBeNull();

        var paths = fixture.GetPaths(obj);

        paths.Should().Contain(".Delegate.Invoke");
        fixture.GetAt(obj, ".Delegate.Invoke").First().Should().Be(obj.Delegate!()!);
    }

    internal interface IExplicit
    {
        int Test();
        void TestWithOut(out object? obj);
    }
    internal class Explicit : IExplicit
    {
        int IExplicit.Test() { return 0; }
        void IExplicit.TestWithOut(out object? obj) { obj = null; }
    }

    public class CtorAgrs(int i, string s)
    {
        public int ArgI { get; } = i;
        public string ArgS { get; } = s;
    }

    [Test]
    [TestCase(AutoMockType.AutoMock)]
    [TestCase(AutoMockType.NonAutoMock)]
    [TestCase(AutoMockType.AutoMockDependencies)]
    public void Test_CtorArgs(AutoMockType type)
    {
        using var fixture = new AbstractAutoMockFixture();

        var obj = GetObj<CtorAgrs>(fixture, type, true);
        obj.Should().NotBeNull();

        var paths = fixture.GetPaths(obj!);
        paths.Should().Contain("..ctor->i");
        paths.Should().Contain("..ctor->s");

        fixture.GetAt(obj!, "..ctor->i").First().Should().Be(obj!.ArgI);
        fixture.GetAt(obj!, "..ctor->s").First().Should().Be(obj!.ArgS);
    }

    [Test]
    [TestCase(AutoMockType.AutoMock)]
    public void Test_Explicit(AutoMockType type)
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.MethodSetupType = MethodSetupTypes.Eager;

        var obj = GetObj<Explicit>(fixture, type);
        obj.Should().NotBeNull();

        var paths = fixture.GetPaths(obj!);

        paths.Should().Contain(".:AutoMockFixture.Tests.AutoMockFixture_Tests.Path_Tests+IExplicit.TestWithOut->obj");
        paths.Should().Contain(".:AutoMockFixture.Tests.AutoMockFixture_Tests.Path_Tests+IExplicit.Test");

        fixture.GetAt(obj!, ".:AutoMockFixture.Tests.AutoMockFixture_Tests.Path_Tests+IExplicit.TestWithOut->obj").First().Should().NotBeNull();
        fixture.GetAt(obj!, ".:AutoMockFixture.Tests.AutoMockFixture_Tests.Path_Tests+IExplicit.Test").First()
                    .Should().NotBeNull().And.BeOfType<int>();
    }
}
