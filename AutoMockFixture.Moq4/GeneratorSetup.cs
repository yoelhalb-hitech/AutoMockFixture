using AutoMockFixture.Moq4.AutoMockProxy;
using Castle.DynamicProxy;

namespace AutoMockFixture.Moq4;

// CAUTION: This cannot be in AutoMock<T> as it would copy the originalProxyGenerator for each T,
//      however when the static ctor runs for a given T the originalProxyGenerator might have been modified by another AutoMock<T>
//      and then later when the ResetGenerator runs it might reset it incorrectly
internal static class GeneratorSetup
{
    static GeneratorSetup()
    {
        var castleProxyFactoryType = typeof(Moq.CastleProxyFactory);
        generatorFieldInfo = castleProxyFactoryType.GetField("generator", BindingFlags.NonPublic | BindingFlags.Instance)!;
        originalProxyGenerator = (ProxyGenerator)generatorFieldInfo.GetValue(Moq.ProxyFactory.Instance)!;
    }

    private static FieldInfo generatorFieldInfo { get; set; }
    private static ProxyGenerator originalProxyGenerator { get; set; }

    public static void SetupGenerator(object? target, bool callBase)
        => generatorFieldInfo.SetValue(Moq.ProxyFactory.Instance, new AutoMockProxyGenerator(target, callBase));

    public static void ResetGenerator()
        => generatorFieldInfo.SetValue(Moq.ProxyFactory.Instance, originalProxyGenerator);
}
