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
    private T? GetObj<T>(AbstractAutoMockFixture fixture, AutoMockType type) where T : class
    {
        return type switch
        {
            AutoMockType.NonAutoMock => fixture.CreateNonAutoMock<T>(),
            AutoMockType.AutoMock => fixture.CreateAutoMock<T>(),
            AutoMockType.AutoMockDependencies => fixture.CreateWithAutoMockDependencies<T>(),
            _ => throw new InvalidEnumArgumentException(),
        };
    }
    #endregion

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

        var f = obj!.InternalTestMethod(); // We need first to invoke the property is it might be lazy
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
    }
}
