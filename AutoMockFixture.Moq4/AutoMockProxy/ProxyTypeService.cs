using System.Collections.Concurrent;

namespace AutoMockFixture.Moq4.AutoMockProxy;

internal class ProxyTypeService
{
    private static ConcurrentDictionary<Type, ConcurrentDictionary<bool, Type>> typeDict = new();
    public static Type GetProxyType(Type type, bool callbase)
        => typeDict.GetOrAdd(type, new ConcurrentDictionary<bool, Type>())
                    .GetOrAdd(callbase, GetProxyTypeInternal(type, callbase));

    private static Type GetProxyTypeInternal(Type type, bool callbase)
    {
        var handlers = new List<IProxyTypeSetupHandler> { new DefaultInterfaceForClassSetupHandler() };
        if(!callbase) handlers.Add(new DefaultConstructorSetupHandler());

        var proxyFactory = new ProxyTypeFactory(handlers);
        return proxyFactory.CreateProxyType(type);
    }
}
