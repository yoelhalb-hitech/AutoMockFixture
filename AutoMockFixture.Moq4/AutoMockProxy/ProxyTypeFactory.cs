
namespace AutoMockFixture.Moq4.AutoMockProxy;

internal class ProxyTypeFactory
{
    private readonly IEnumerable<IProxyTypeSetupHandler> setupHandlers;

    public ProxyTypeFactory(IEnumerable<IProxyTypeSetupHandler> setupHandlers)
	{
        this.setupHandlers = setupHandlers;
    }

    public Type CreateProxyType(Type originalType)
    {
        var mockType = new AutoMock<TypeInfo>();

        mockType.SetTarget((TypeInfo)originalType);
        mockType.CallBase = true;

        // Needed for Castle.Core < 5.0
        // We need to explictly return it, as otherwise it won't intercept calls when accessed via GetTypeInfo() even though it is proxied
        mockType.As<IReflectableType>().Setup(t => t.GetTypeInfo()).Returns(() => mockType.Object);

        foreach (var setupHandler in setupHandlers)
        {
            setupHandler.Setup(originalType, mockType);
        }

        return mockType.GetMocked();
    }
}
