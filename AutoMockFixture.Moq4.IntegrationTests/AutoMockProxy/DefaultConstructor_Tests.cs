using Castle.DynamicProxy;
using System.Reflection;

namespace AutoMockFixture.Moq4.IntegrationTests.AutoMockProxy;

public class DefaultConstructor_Tests
{
    public class TypeWithoutDefaultCtor
    {
        public TypeWithoutDefaultCtor(int i) { }
    }

    public class TypeWithDefaultCtor
    {
        public TypeWithDefaultCtor() { throw new Exception(); }
    }

    [Test]
    public void Test_TypeWithoutDefaultCtor_AddsDefaultCtor_OnNonCallbase()
    {
        var mock = new AutoMock<TypeWithoutDefaultCtor>() { CallBase = false };
        Assert.DoesNotThrow(() => mock.GetMocked());
    }

    [Test]
    public void Test_TypeWithoutDefaultCtor_ReplacesDefaultCtor_OnNonCallbase()
    {
        var mock = new AutoMock<TypeWithDefaultCtor>() { CallBase = false };
        Assert.DoesNotThrow(() => mock.GetMocked());
    }


    [Test]
    public void Test_TypeWithoutDefaultCtor_DoesNotAddDefaultCtor_OnCallbase()
    {
        var mock = new AutoMock<TypeWithoutDefaultCtor>() { CallBase = true };
        Assert.Throws<InvalidProxyConstructorArgumentsException>(() => mock.GetMocked());
    }

    [Test]
    public void Test_TypeWithoutDefaultCtor_DoesNotReplaceDefaultCtor_OnCallbase()
    {
        var mock = new AutoMock<TypeWithDefaultCtor>() { CallBase = true };
        Assert.Throws<TargetInvocationException>(() => mock.GetMocked());
    }
}
