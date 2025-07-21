using SequelPay.DotNetPowerExtensions.Reflection;
using SequelPay.DotNetPowerExtensions.Reflection.Common;

namespace AutoMockFixture.Moq4.AutoMockProxy;

internal class DefaultInterfaceForClassSetupHandler : IProxyTypeSetupHandler
{
    private static MethodDetail[] GetDefaultInterfaceMethods(Type classToProxy)
        => classToProxy.GetTypeDetailInfo().ExplicitMethodDetails
                            // The issue is only by default interface methods not reimplemented (either in the class or in another interface)
                            .Where(m => m.ReflectionInfo.DeclaringType?.IsInterface == true && m.DeclarationType == DeclarationTypes.Decleration)
                        .Concat(GetDefaultProperties(classToProxy).SelectMany(p => new[] { p.GetMethod ?? p.BasePrivateGetMethod, p.SetMethod ?? p.BasePrivateSetMethod }))
                        .Concat(GetDefaultEvents(classToProxy).SelectMany(e => new[] { e.AddMethod, e.RemoveMethod }))
                        .OfType<MethodDetail>()
                        .ToArray();


    private static PropertyDetail[] GetDefaultProperties(Type classToProxy)
        => classToProxy.GetTypeDetailInfo().ExplicitPropertyDetails
                // The issue is only by default interface properties not reimplemented
                .Where(p => p.ReflectionInfo.DeclaringType?.IsInterface == true && p.DeclarationType == DeclarationTypes.Decleration)
                .ToArray();
    private static EventDetail[] GetDefaultEvents(Type classToProxy)
        => classToProxy.GetTypeDetailInfo().ExplicitEventDetails.Where(e => e.ReflectionInfo.DeclaringType?.IsInterface == true && e.DeclarationType == DeclarationTypes.Decleration).ToArray(); // Explicit event has always an issue with SetupAdd/SetupRemove

    public void Setup(Type originalType, AutoMock<TypeInfo> mock)
    {
        if (originalType.IsInterface) return;

        // Moq has an issue on class with default interface method that it callsbase (even for non callBase) and any setup is always ignored
        // Another issue is for SetupSet etc. since it considers it to be sealed

        var defaultMethods = GetDefaultInterfaceMethods(originalType).Select(m => m.ReflectionInfo).ToArray();

        if (!defaultMethods.Any()) return;

        var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;

        mock
            .Setup(t => t.GetMethods(It.Is<BindingFlags>(bf => (bf & (BindingFlags.NonPublic | BindingFlags.Instance)) == bindingFlags)))
            .Returns(originalType!.GetMethods(bindingFlags).Concat(defaultMethods).ToArray());

        var defaultProperties = GetDefaultProperties(originalType).Select(m => m.ReflectionInfo).ToArray();
        mock
            .Setup(t => t.GetProperties(It.Is<BindingFlags>(bf => (bf & (BindingFlags.NonPublic | BindingFlags.Instance)) == bindingFlags)))
            .Returns(originalType!.GetProperties(bindingFlags).Concat(defaultProperties).ToArray());

        var defaultEvents = GetDefaultEvents(originalType).Select(m => m.ReflectionInfo).ToArray();
        mock
            .Setup(t => t.GetEvents(It.Is<BindingFlags>(bf => (bf & (BindingFlags.NonPublic | BindingFlags.Instance)) == bindingFlags)))
            .Returns(originalType!.GetEvents(bindingFlags).Concat(defaultEvents).ToArray());
    }
}
