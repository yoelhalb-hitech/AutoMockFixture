using AutoMockFixture.Moq4.AutoMockProxy;
using System.Reflection;

namespace AutoMockFixture.Moq4.UnitTests.AutoMockProxy;

internal class DefaultConstructorSetupHandler_Tests
{
    class TypeWithoutDefaultCtor
    {
        public TypeWithoutDefaultCtor(int i) { }
    }

    class TypeWithDefaultCtor
    {
        public TypeWithDefaultCtor() { }
    }

    [Test]
    public void Test_TypeWithoutDefaultCtor()
    {
        var originalType = typeof(TypeWithoutDefaultCtor);

        var mock = new AutoMock<TypeInfo>();
        mock.SetTarget((TypeInfo)originalType);
        mock.CallBase = true;

        var handler = new DefaultConstructorSetupHandler();
        handler.Setup(originalType, mock);

        var obj = mock.Object;

        obj.GetConstructor(new Type[] { }).Should().NotBeNull();
        obj.GetConstructors().Count().Should().Be(2);
    }

    [Test]
    public void Test_TypeWithDefaultCtor()
    {
        var originalType = typeof(TypeWithDefaultCtor);

        var mock = new AutoMock<TypeInfo>();
        mock.SetTarget((TypeInfo)originalType);
        mock.CallBase = true;

        var handler = new DefaultConstructorSetupHandler();
        handler.Setup(originalType, mock);

        var obj = mock.Object;

        obj.GetConstructor(new Type[] { }).Should().NotBeNull();
        obj.GetConstructor(new Type[] { }).Should().NotBeSameAs(originalType.GetConstructor(new Type[] { }));

        obj.GetConstructors().Count().Should().Be(1);
        obj.GetConstructors().First().Should().NotBeSameAs(originalType.GetConstructor(new Type[] { }));
    }
}
