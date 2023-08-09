using AutoMockFixture.Moq4.AutoMockProxy;
using NUnit.Framework.Internal;
using System.Reflection;

namespace AutoMockFixture.Moq4.UnitTests.AutoMockProxy;

internal class DefaultInterfaceForClassSetupHandler_Tests
{
    private static EventHandler? pe;
    interface IWithDefault
    {
        public void TestMethod() { }
        public int TestProp => 20;
        public event EventHandler TestEvent { add => pe += value; remove => pe -= value; }
    }

    class TypeWithDefault : IWithDefault
    {
    }

    [Test]
    public void Test_TypeWithDefault()
    {
        var originalType = typeof(TypeWithDefault);
        var ifaceType = typeof(IWithDefault);

        var mock = new AutoMock<TypeInfo>();
        mock.SetTarget((TypeInfo)originalType);
        mock.CallBase = true;

        var handler = new DefaultInterfaceForClassSetupHandler();
        handler.Setup(originalType, mock);

        var obj = mock.Object;
        obj.GetMethods(BindingFlags.Instance|BindingFlags.NonPublic).Any(m => m == ifaceType.GetMethod(nameof(IWithDefault.TestMethod))).Should().BeTrue();

        obj.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic).Count().Should().Be(1);
        obj.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic).First().Should().BeSameAs(ifaceType.GetProperty(nameof(IWithDefault.TestProp)));

        obj.GetEvents(BindingFlags.Instance | BindingFlags.NonPublic).Count().Should().Be(1);
        obj.GetEvents(BindingFlags.Instance | BindingFlags.NonPublic).First().Should().BeSameAs(ifaceType.GetEvent(nameof(IWithDefault.TestEvent)));
    }
}
