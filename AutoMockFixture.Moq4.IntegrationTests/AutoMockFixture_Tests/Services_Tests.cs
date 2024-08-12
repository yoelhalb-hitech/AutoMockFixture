using AutoMockFixture.NUnit3;
using SequelPay.DotNetPowerExtensions;
using static AutoMockFixture.Moq4.IntegrationTests.AutoMockFixture_Tests.Services_Tests;

namespace AutoMockFixture.Moq4.IntegrationTests.AutoMockFixture_Tests;

internal class Services_Tests
{
    [TransientBase] public interface IfaceTransientWithAttribute { }
    public interface IfaceTransientNoAttribute { }

    [TransientBase] public class BaseTransientWithAttribute { }
    public class BaseTransientNoAttribute { }

    [Transient<IfaceTransientWithAttribute, BaseTransientWithAttribute>]
    public class SubTransientAttribute : BaseTransientWithAttribute, IfaceTransientWithAttribute { }

    [Transient<IfaceTransientNoAttribute, BaseTransientNoAttribute>]
    public class SubTransientNoAttribute : BaseTransientNoAttribute, IfaceTransientNoAttribute { }


    [ScopedBase] public interface IfaceScopedWithAttribute { }
    public interface IfaceScopedNoAttribute { }

    [ScopedBase] public class BaseScopedWithAttribute { }
    public class BaseScopedNoAttribute { }

    [Scoped<IfaceScopedWithAttribute, BaseScopedWithAttribute>]
    public class SubScopedAttribute : BaseScopedWithAttribute, IfaceScopedWithAttribute { }

    [Scoped<IfaceScopedNoAttribute, BaseScopedNoAttribute>]
    public class SubScopedNoAttribute : BaseScopedNoAttribute, IfaceScopedNoAttribute { }

    [SingletonBase] public interface IfaceSingletonWithAttribute { }
    public interface IfaceSingletonNoAttribute { }


    [SingletonBase] public class BaseSingletonWithAttribute { }
    public class BaseSingletonNoAttribute { }

    [Singleton<IfaceSingletonWithAttribute, BaseSingletonWithAttribute>]
    public class SubSingletonAttribute : BaseSingletonWithAttribute, IfaceSingletonWithAttribute { }

    [Singleton<IfaceSingletonNoAttribute, BaseSingletonNoAttribute>]
    public class SubSingletonNoAttribute : BaseSingletonNoAttribute, IfaceSingletonNoAttribute { }

    [Test]
    [TestCase<IfaceTransientWithAttribute, BaseTransientWithAttribute, SubTransientAttribute>]
    [TestCase<IfaceTransientNoAttribute, BaseTransientNoAttribute, SubTransientNoAttribute>]
    [TestCase<IfaceScopedWithAttribute, BaseScopedWithAttribute, SubScopedAttribute>]
    [TestCase<IfaceScopedNoAttribute, BaseScopedNoAttribute, SubScopedNoAttribute>]
    [TestCase<IfaceSingletonWithAttribute, BaseSingletonWithAttribute, SubSingletonAttribute>]
    [TestCase<IfaceSingletonNoAttribute, BaseSingletonNoAttribute, SubSingletonNoAttribute>]
    public void Test_IntegrationFixture_UsesServices<TIFaceType, TBaseType, TTExpected>()
        where TIFaceType : class
        where TBaseType : class
        where TTExpected : TBaseType, TIFaceType
    {
        using var fixture = new IntegrationFixture();

        var t1 = fixture.Create<TIFaceType>();
        var t2 = fixture.Create<TBaseType>();

        var t3 = fixture.CreateAutoMock<TIFaceType>();
        var t4 = fixture.CreateAutoMock<TBaseType>();

        var t5 = fixture.CreateNonAutoMock<TIFaceType>();
        var t6 = fixture.CreateNonAutoMock<TBaseType>();

        var t7 = fixture.CreateWithAutoMockDependencies<TIFaceType>();
        var t8 = fixture.CreateWithAutoMockDependencies<TBaseType>();

        new object[] { t1!, t2!, t5!, t6!, t7!, t8! }.Should().AllBeOfType<TTExpected>().And.AllNonAutoMock();

        new object[] { t3!, t4! }.Should().AllBeAssignableTo<TTExpected>().And.AllAutoMock();
    }

    [Test]
    [TestCase<IfaceTransientWithAttribute, BaseTransientWithAttribute, SubTransientAttribute>]
    [TestCase<IfaceTransientNoAttribute, BaseTransientNoAttribute, SubTransientNoAttribute>]
    [TestCase<IfaceScopedWithAttribute, BaseScopedWithAttribute, SubScopedAttribute>]
    [TestCase<IfaceScopedNoAttribute, BaseScopedNoAttribute, SubScopedNoAttribute>]
    [TestCase<IfaceSingletonWithAttribute, BaseSingletonWithAttribute, SubSingletonAttribute>]
    [TestCase<IfaceSingletonNoAttribute, BaseSingletonNoAttribute, SubSingletonNoAttribute>]
    public void Test_UnitFixture_NotUsingServices<TIFaceType, TBaseType, TTExpected>()
    where TIFaceType : class
    where TBaseType : class
    where TTExpected : TBaseType, TIFaceType
    {
        using var fixture = new UnitFixture();

        var t1 = fixture.Create<TIFaceType>();
        var t2 = fixture.Create<TBaseType>();

        var t3 = fixture.CreateAutoMock<TIFaceType>();
        var t4 = fixture.CreateAutoMock<TBaseType>();

        var t5 = fixture.CreateNonAutoMock<TIFaceType>();
        var t6 = fixture.CreateNonAutoMock<TBaseType>();

        var t7 = fixture.CreateWithAutoMockDependencies<TIFaceType>();
        var t8 = fixture.CreateWithAutoMockDependencies<TBaseType>();

        new object[] { t1!, t2!, t5!, t6!, t7!, t8! }.Should().AllNotBeAssignableTo<object, TTExpected>();

        new object[] { t3!, t4! }.Should().AllNotBeAssignableTo<object, TTExpected>().And.AllAutoMock();
    }

    [Test]
    [TestCase<IfaceTransientWithAttribute, BaseTransientWithAttribute, SubTransientAttribute>]
    [TestCase<IfaceTransientNoAttribute, BaseTransientNoAttribute, SubTransientNoAttribute>]
    [TestCase<IfaceScopedWithAttribute, BaseScopedWithAttribute, SubScopedAttribute>]
    [TestCase<IfaceScopedNoAttribute, BaseScopedNoAttribute, SubScopedNoAttribute>]
    [TestCase<IfaceSingletonWithAttribute, BaseSingletonWithAttribute, SubSingletonAttribute>]
    [TestCase<IfaceSingletonNoAttribute, BaseSingletonNoAttribute, SubSingletonNoAttribute>]
    public void Test_UnitFixture_NotUsingServices_WhenExplicitFalse<TIFaceType, TBaseType, TTExpected>()
    where TIFaceType : class
    where TBaseType : class
    where TTExpected : TBaseType, TIFaceType
    {
        using var fixture = new IntegrationFixture();
        fixture.AutoTransformBySericeAttributes = false;

        var t1 = fixture.Create<TIFaceType>();
        var t2 = fixture.Create<TBaseType>();

        var t3 = fixture.CreateAutoMock<TIFaceType>();
        var t4 = fixture.CreateAutoMock<TBaseType>();

        var t5 = fixture.CreateNonAutoMock<TIFaceType>();
        var t6 = fixture.CreateNonAutoMock<TBaseType>();

        var t7 = fixture.CreateWithAutoMockDependencies<TIFaceType>();
        var t8 = fixture.CreateWithAutoMockDependencies<TBaseType>();

        new object[] { t1!, t2!, t5!, t6!, t7!, t8! }.Should().AllNotBeAssignableTo<object, TTExpected>();

        new object[] { t3!, t4! }.Should().AllNotBeAssignableTo<object, TTExpected>().And.AllAutoMock();
    }

    [Test]
    [TestCase<IfaceTransientWithAttribute, BaseTransientWithAttribute, SubTransientAttribute>]
    [TestCase<IfaceTransientNoAttribute, BaseTransientNoAttribute, SubTransientNoAttribute>]
    [TestCase<IfaceScopedWithAttribute, BaseScopedWithAttribute, SubScopedAttribute>]
    [TestCase<IfaceScopedNoAttribute, BaseScopedNoAttribute, SubScopedNoAttribute>]
    [TestCase<IfaceSingletonWithAttribute, BaseSingletonWithAttribute, SubSingletonAttribute>]
    [TestCase<IfaceSingletonNoAttribute, BaseSingletonNoAttribute, SubSingletonNoAttribute>]
    public void Test_UnitFixture_UsesServicesWhenExplicitTrue<TIFaceType, TBaseType, TTExpected>()
    where TIFaceType : class
    where TBaseType : class
    where TTExpected : TBaseType, TIFaceType
    {
        using var fixture = new IntegrationFixture();
        fixture.AutoTransformBySericeAttributes = true;

        var t1 = fixture.Create<TIFaceType>();
        var t2 = fixture.Create<TBaseType>();

        var t3 = fixture.CreateAutoMock<TIFaceType>();
        var t4 = fixture.CreateAutoMock<TBaseType>();

        var t5 = fixture.CreateNonAutoMock<TIFaceType>();
        var t6 = fixture.CreateNonAutoMock<TBaseType>();

        var t7 = fixture.CreateWithAutoMockDependencies<TIFaceType>();
        var t8 = fixture.CreateWithAutoMockDependencies<TBaseType>();

        new object[] { t1!, t2!, t5!, t6!, t7!, t8! }.Should().AllBeOfType<TTExpected>().And.AllNonAutoMock();

        new object[] { t3!, t4! }.Should().AllBeAssignableTo<TTExpected>().And.AllAutoMock();
    }
}
