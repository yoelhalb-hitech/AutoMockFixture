using System.Collections.Concurrent;

namespace AutoMockFixture.Moq4.AutoMockProxy;

internal class ProxyTypeService
{
    private static ConcurrentDictionary<Type, ConcurrentDictionary<bool, Type>> typeDict = new();
    public static Type GetProxyType(Type type, bool callBase)
        => typeDict.GetOrAdd(type, new ConcurrentDictionary<bool, Type>())
                    .GetOrAdd(callBase, GetProxyTypeInternal(type, callBase));

    private static Type GetProxyTypeInternal(Type type, bool callBase)
    {
        var handlers = new List<IProxyTypeSetupHandler> { new DefaultInterfaceForClassSetupHandler() };
        if(!callBase) handlers.Add(new DefaultConstructorSetupHandler());

        var proxyFactory = new ProxyTypeFactory(handlers);
        return proxyFactory.CreateProxyType(type);
    }
}
