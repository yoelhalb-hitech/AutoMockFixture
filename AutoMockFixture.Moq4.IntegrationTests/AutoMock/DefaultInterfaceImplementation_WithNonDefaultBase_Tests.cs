using AutoMockFixture.NUnit3;

namespace AutoMockFixture.Moq4.IntegrationTests.AutoMock;

internal class DefaultInterfaceImplementation_WithNonDefaultBase_Tests
{
    public interface ITest
    {
        public abstract string TestProp { get; }
        public abstract string TestMethod();
    }
    public interface IDefault : ITest
    {
        string ITest.TestProp => "TestProp";
        string ITest.TestMethod() => "TestMethod";
    }

    public interface IDefault2 : IDefault
    {
        string ITest.TestProp => "TestProp2";
        string ITest.TestMethod() => "TestMethod2";
    }

    public interface IDefault3 : IDefault2
    {
        string ITest.TestProp => "TestProp3";
        string ITest.TestMethod() => "TestMethod3";
    }

    public interface IDefaultEmpty : IDefault3
    {
    }

    public class DefaultNoOverride : IDefault
    {
    }
    public class DefaultNoOverrideSub : DefaultNoOverride { }
    public class DefaultNoOverrideSubWithIDefault : DefaultNoOverride, IDefault { }

    public class DefaultNoOverride2 : IDefault2
    {
    }
    public class DefaultNoOverrideSub2 : DefaultNoOverride2 { }
    public class DefaultNoOverrideSubWithIDefault2 : DefaultNoOverride2, IDefault2 { }

    public class DefaultNoOverride3 : IDefault3
    {
    }
    public class DefaultNoOverrideSub3 : DefaultNoOverride3 { }
    public class DefaultNoOverrideSubWithIDefault3 : DefaultNoOverride3, IDefault3 { }

    public class DefaultWithOverride : IDefault
    {
        public virtual string TestProp => "TestPropFromClass";
        public virtual string TestMethod() => "TestMethodFromClass";
    }
    public class DefaultWithOverrideSub : DefaultWithOverride { }
    public class DefaultWithOverrideSubWithIDefault : DefaultWithOverride, IDefault { }

    public class DefaultWithOverride2 : IDefault2
    {
        public virtual string TestProp => "TestPropFromClass";
        public virtual string TestMethod() => "TestMethodFromClass";
    }
    public class DefaultWithOverrideSub2 : DefaultWithOverride2 { }
    public class DefaultWithOverrideSubWithIDefault2 : DefaultWithOverride2, IDefault2 { }

    public class DefaultWithOverride3 : IDefault3
    {
        public virtual string TestProp => "TestPropFromClass";
        public virtual string TestMethod() => "TestMethodFromClass";
    }
    public class DefaultWithOverrideSub3 : DefaultWithOverride3 { }
    public class DefaultWithOverrideSubWithIDefault3 : DefaultWithOverride3, IDefault3 { }


    public class DefaultWithExplicit : IDefault
    {
        string ITest.TestProp => "TestPropFromClass";
        string ITest.TestMethod() => "TestMethodFromClass";
    }
    public class DefaultWithExplicitSub : DefaultWithExplicit { }
    public class DefaultWithExplicitSubWithIDefault : DefaultWithExplicit, IDefault { }

    public class DefaultWithExplicit2 : IDefault2
    {
        string ITest.TestProp => "TestPropFromClass";
        string ITest.TestMethod() => "TestMethodFromClass";
    }
    public class DefaultWithExplicitSub2 : DefaultWithExplicit2 { }
    public class DefaultWithExplicitSubWithIDefault2 : DefaultWithExplicit2, IDefault2 { }

    public class DefaultWithExplicit3 : IDefault3
    {
        string ITest.TestProp => "TestPropFromClass";
        string ITest.TestMethod() => "TestMethodFromClass";
    }
    public class DefaultWithExplicitSub3 : DefaultWithExplicit3 { }
    public class DefaultWithExplicitSubWithIDefault3 : DefaultWithExplicit3, IDefault3 { }


    [Test]
    [TestCase<IDefault>]
    [TestCase<IDefault2>]
    [TestCase<IDefault3>]
    [TestCase<IDefaultEmpty>]
    [TestCase<DefaultNoOverride>]
    [TestCase<DefaultNoOverrideSub>]
    [TestCase<DefaultNoOverrideSubWithIDefault>]
    [TestCase<DefaultNoOverride2>]
    [TestCase<DefaultNoOverrideSub2>]
    [TestCase<DefaultNoOverrideSubWithIDefault2>]
    [TestCase<DefaultNoOverride3>]
    [TestCase<DefaultNoOverrideSub3>]
    [TestCase<DefaultNoOverrideSubWithIDefault3>]
    [TestCase<DefaultWithOverride>]
    [TestCase<DefaultWithOverrideSub>]
    [TestCase<DefaultWithOverrideSubWithIDefault>]
    [TestCase<DefaultWithOverride2>]
    [TestCase<DefaultWithOverrideSub2>]
    [TestCase<DefaultWithOverrideSubWithIDefault2>]
    [TestCase<DefaultWithOverride3>]
    [TestCase<DefaultWithOverrideSub3>]
    [TestCase<DefaultWithOverrideSubWithIDefault3>]
    [TestCase<DefaultWithExplicit>]
    [TestCase<DefaultWithExplicitSub>]
    [TestCase<DefaultWithExplicitSubWithIDefault>]
    [TestCase<DefaultWithExplicit2>]
    [TestCase<DefaultWithExplicitSub2>]
    [TestCase<DefaultWithExplicitSubWithIDefault2>]
    [TestCase<DefaultWithExplicit3>]
    [TestCase<DefaultWithExplicitSub3>]
    [TestCase<DefaultWithExplicitSubWithIDefault3>]
    public void Test_DefaultInterfaceImplementation_NoImplementation_Setup_WithCallBase<T>() where T : class, IDefault
    {
        var m = new AutoMock<IDefault>() { CallBase = true };
        m.Setup(m => m.TestProp).Returns("test");
        m.Setup(m => m.TestMethod()).Returns("test");

        m.Object.TestProp.Should().Be("test");
        m.Object.TestMethod().Should().Be("test");
    }

    [Test]
    [TestCase<IDefault>]
    [TestCase<IDefault2>]
    [TestCase<IDefault3>]
    [TestCase<IDefaultEmpty>]
    [TestCase<DefaultNoOverride>]
    [TestCase<DefaultNoOverrideSub>]
    [TestCase<DefaultNoOverrideSubWithIDefault>]
    [TestCase<DefaultNoOverride2>]
    [TestCase<DefaultNoOverrideSub2>]
    [TestCase<DefaultNoOverrideSubWithIDefault2>]
    [TestCase<DefaultNoOverride3>]
    [TestCase<DefaultNoOverrideSub3>]
    [TestCase<DefaultNoOverrideSubWithIDefault3>]
    [TestCase<DefaultWithOverride>]
    [TestCase<DefaultWithOverrideSub>]
    [TestCase<DefaultWithOverrideSubWithIDefault>]
    [TestCase<DefaultWithOverride2>]
    [TestCase<DefaultWithOverrideSub2>]
    [TestCase<DefaultWithOverrideSubWithIDefault2>]
    [TestCase<DefaultWithOverride3>]
    [TestCase<DefaultWithOverrideSub3>]
    [TestCase<DefaultWithOverrideSubWithIDefault3>]
    [TestCase<DefaultWithExplicit>]
    [TestCase<DefaultWithExplicitSub>]
    [TestCase<DefaultWithExplicitSubWithIDefault>]
    [TestCase<DefaultWithExplicit2>]
    [TestCase<DefaultWithExplicitSub2>]
    [TestCase<DefaultWithExplicitSubWithIDefault2>]
    [TestCase<DefaultWithExplicit3>]
    [TestCase<DefaultWithExplicitSub3>]
    [TestCase<DefaultWithExplicitSubWithIDefault3>]
    public void Test_DefaultInterfaceImplementation_NoImplementation_Setup_NoCallBase<T>() where T : class, IDefault
    {
        var m = new AutoMock<T>() { CallBase = false };
        m.As<ITest>(); // This is needed if explicit???... //TODO... if it is needed then we should put it in the AutoMock code (can remove it then from the init command)

        // TODO... we fix things by adding to the interface but then Moq expects it in the GetInterfaceMap...

        m.Setup(m => m.TestProp).Returns("test");
        m.Setup(m => m.TestMethod()).Returns("test");

        m.Object.TestProp.Should().Be("test");
        m.Object.TestMethod().Should().Be("test");
    }

    [Test]
    [TestCase<IDefault>("TestProp", "TestMethod")]
    [TestCase<IDefault2>("TestProp2", "TestMethod2")]
    [TestCase<IDefault3>("TestProp3", "TestMethod3")]
    [TestCase<IDefaultEmpty>("TestProp3", "TestMethod3")]
    [TestCase<DefaultNoOverride>("TestProp", "TestMethod")]
    [TestCase<DefaultNoOverrideSub>("TestProp", "TestMethod")]
    [TestCase<DefaultNoOverrideSubWithIDefault>("TestProp", "TestMethod")]
    [TestCase<DefaultNoOverride2>("TestProp2", "TestMethod2")]
    [TestCase<DefaultNoOverrideSub2>("TestProp2", "TestMethod2")]
    [TestCase<DefaultNoOverrideSubWithIDefault2>("TestProp2", "TestMethod2")]
    [TestCase<DefaultNoOverride3>("TestProp3", "TestMethod3")]
    [TestCase<DefaultNoOverrideSub3>("TestProp3", "TestMethod3")]
    [TestCase<DefaultNoOverrideSubWithIDefault3>("TestProp3", "TestMethod3")]
    [TestCase<DefaultWithOverride>("TestPropFromClass", "TestMethodFromClass")]
    [TestCase<DefaultWithOverrideSub>("TestPropFromClass", "TestMethodFromClass")]
    [TestCase<DefaultWithOverrideSubWithIDefault>("TestPropFromClass", "TestMethodFromClass")]
    [TestCase<DefaultWithOverride2>("TestPropFromClass", "TestMethodFromClass")]
    [TestCase<DefaultWithOverrideSub2>("TestPropFromClass", "TestMethodFromClass")]
    [TestCase<DefaultWithOverrideSubWithIDefault2>("TestPropFromClass", "TestMethodFromClass")]
    [TestCase<DefaultWithOverride3>("TestPropFromClass", "TestMethodFromClass")]
    [TestCase<DefaultWithOverrideSub3>("TestPropFromClass", "TestMethodFromClass")]
    [TestCase<DefaultWithOverrideSubWithIDefault3>("TestPropFromClass", "TestMethodFromClass")]
    [TestCase<DefaultWithExplicit>("TestPropFromClass", "TestMethodFromClass")]
    [TestCase<DefaultWithExplicitSub>("TestPropFromClass", "TestMethodFromClass")]
    [TestCase<DefaultWithExplicitSubWithIDefault>("TestPropFromClass", "TestMethodFromClass")]
    [TestCase<DefaultWithExplicit2>("TestPropFromClass", "TestMethodFromClass")]
    [TestCase<DefaultWithExplicitSub2>("TestPropFromClass", "TestMethodFromClass")]
    [TestCase<DefaultWithExplicitSubWithIDefault2>("TestPropFromClass", "TestMethodFromClass")]
    [TestCase<DefaultWithExplicit3>("TestPropFromClass", "TestMethodFromClass")]
    [TestCase<DefaultWithExplicitSub3>("TestPropFromClass", "TestMethodFromClass")]
    [TestCase<DefaultWithExplicitSubWithIDefault3>("TestPropFromClass", "TestMethodFromClass")]
    public void Test_DefaultInterfaceImplementation_NoImplementation_NoSetup_CallbaseTrue<T>(string? prop, string? method)
                                                                            where T : class, IDefault
    {
        var m = new AutoMock<T>() { CallBase = true };
        m.As<IDefault>();

        m.Object.TestProp.Should().Be(prop);
        m.Object.TestMethod().Should().Be(method);
    }

    [Test]
    [TestCase<IDefault>("TestProp", "TestMethod")]
    [TestCase<IDefault2>("TestProp2", "TestMethod2")]
    [TestCase<IDefault3>("TestProp3", "TestMethod3")]
    [TestCase<IDefaultEmpty>("TestProp3", "TestMethod3")]
    [TestCase<DefaultNoOverride>("TestProp", "TestMethod")]
    [TestCase<DefaultNoOverrideSub>("TestProp", "TestMethod")]
    [TestCase<DefaultNoOverrideSubWithIDefault>("TestProp", "TestMethod")]
    [TestCase<DefaultNoOverride2>("TestProp2", "TestMethod2")]
    [TestCase<DefaultNoOverrideSub2>("TestProp2", "TestMethod2")]
    [TestCase<DefaultNoOverrideSubWithIDefault2>("TestProp2", "TestMethod2")]
    [TestCase<DefaultNoOverride3>("TestProp3", "TestMethod3")]
    [TestCase<DefaultNoOverrideSub3>("TestProp3", "TestMethod3")]
    [TestCase<DefaultNoOverrideSubWithIDefault3>("TestProp3", "TestMethod3")]
    [TestCase<DefaultWithOverride>("TestPropFromClass", "TestMethodFromClass")]
    [TestCase<DefaultWithOverrideSub>("TestPropFromClass", "TestMethodFromClass")]
    [TestCase<DefaultWithOverrideSubWithIDefault>("TestPropFromClass", "TestMethodFromClass")]
    [TestCase<DefaultWithOverride2>("TestPropFromClass", "TestMethodFromClass")]
    [TestCase<DefaultWithOverrideSub2>("TestPropFromClass", "TestMethodFromClass")]
    [TestCase<DefaultWithOverrideSubWithIDefault2>("TestPropFromClass", "TestMethodFromClass")]
    [TestCase<DefaultWithOverride3>("TestPropFromClass", "TestMethodFromClass")]
    [TestCase<DefaultWithOverrideSub3>("TestPropFromClass", "TestMethodFromClass")]
    [TestCase<DefaultWithOverrideSubWithIDefault3>("TestPropFromClass", "TestMethodFromClass")]
    [TestCase<DefaultWithExplicit>("TestPropFromClass", "TestMethodFromClass")]
    [TestCase<DefaultWithExplicitSub>("TestPropFromClass", "TestMethodFromClass")]
    [TestCase<DefaultWithExplicitSubWithIDefault>("TestPropFromClass", "TestMethodFromClass")]
    [TestCase<DefaultWithExplicit2>("TestPropFromClass", "TestMethodFromClass")]
    [TestCase<DefaultWithExplicitSub2>("TestPropFromClass", "TestMethodFromClass")]
    [TestCase<DefaultWithExplicitSubWithIDefault2>("TestPropFromClass", "TestMethodFromClass")]
    [TestCase<DefaultWithExplicit3>("TestPropFromClass", "TestMethodFromClass")]
    [TestCase<DefaultWithExplicitSub3>("TestPropFromClass", "TestMethodFromClass")]
    [TestCase<DefaultWithExplicitSubWithIDefault3>("TestPropFromClass", "TestMethodFromClass")]
    public void Test_DefaultInterfaceImplementation_NoImplementation_NoSetup_CallbaseCall<T>(string? prop, string? method)
                                                                        where T : class, IDefault
    {
        var m = new AutoMock<T>() { CallBase = false };
        m.As<IDefault>();
        m.Setup(m => m.TestProp).CallBase();
        m.Setup(m => m.TestMethod()).CallBase();

        m.Object.TestProp.Should().Be(prop);
        m.Object.TestMethod().Should().Be(method);
    }

    [TestCase<IDefault>]
    [TestCase<IDefault2>]
    [TestCase<IDefault3>]
    [TestCase<IDefaultEmpty>]
    [TestCase<DefaultNoOverride>]
    [TestCase<DefaultNoOverrideSub>]
    [TestCase<DefaultNoOverrideSubWithIDefault>]
    [TestCase<DefaultNoOverride2>]
    [TestCase<DefaultNoOverrideSub2>]
    [TestCase<DefaultNoOverrideSubWithIDefault2>]
    [TestCase<DefaultNoOverride3>]
    [TestCase<DefaultNoOverrideSub3>]
    [TestCase<DefaultNoOverrideSubWithIDefault3>]
    [TestCase<DefaultWithOverride>]
    [TestCase<DefaultWithOverrideSub>]
    [TestCase<DefaultWithOverrideSubWithIDefault>]
    [TestCase<DefaultWithOverride2>]
    [TestCase<DefaultWithOverrideSub2>]
    [TestCase<DefaultWithOverrideSubWithIDefault2>]
    [TestCase<DefaultWithOverride3>]
    [TestCase<DefaultWithOverrideSub3>]
    [TestCase<DefaultWithOverrideSubWithIDefault3>]
    [TestCase<DefaultWithExplicit>]
    [TestCase<DefaultWithExplicitSub>]
    [TestCase<DefaultWithExplicitSubWithIDefault>]
    [TestCase<DefaultWithExplicit2>]
    [TestCase<DefaultWithExplicitSub2>]
    [TestCase<DefaultWithExplicitSubWithIDefault2>]
    [TestCase<DefaultWithExplicit3>]
    [TestCase<DefaultWithExplicitSub3>]
    [TestCase<DefaultWithExplicitSubWithIDefault3>]
    public void Test_DefaultInterfaceImplementation_NoImplementation_NoSetup_NoCallbase<T>() where T : class, IDefault
    {
        var m = new AutoMock<T>() { CallBase = false };
        m.As<IDefault>();

        m.Object.TestProp.Should().BeNull();
        m.Object.TestMethod().Should().BeNull();
    }
}
