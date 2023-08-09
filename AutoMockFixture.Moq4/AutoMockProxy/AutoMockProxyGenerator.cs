using Castle.DynamicProxy;
using DotNetPowerExtensions.Reflection;
using DotNetPowerExtensions.Reflection.Models;

namespace AutoMockFixture.Moq4.AutoMockProxy;

internal class AutoMockProxyGenerator : ProxyGenerator
{
    private static ProxyGenerator originalProxyGenerator = new ProxyGenerator();
    // This contains the caches so we will have it static
    private static DefaultProxyBuilder nonCallbaseProxyBuilder = new DefaultProxyBuilder();

    internal object? Target { get; }
    internal bool? Callbase { get; }
    internal bool isMoq = false;

    public AutoMockProxyGenerator(object? target, bool callbase) : base(nonCallbaseProxyBuilder)
    {
        Target = target;
        Callbase = callbase;
    }
    public AutoMockProxyGenerator() : base(nonCallbaseProxyBuilder)
    {
        isMoq = true;
    }

    public override object CreateInterfaceProxyWithoutTarget(Type interfaceToProxy, Type[] additionalInterfacesToProxy, ProxyGenerationOptions options,
                                                                                                            params Castle.DynamicProxy.IInterceptor[] interceptors)
    {
        // Rememeber that Moq uses the generator as static, so we have to ensure that the target is valid
        if (!isMoq && Target is not null && interfaceToProxy.IsAssignableFrom(Target.GetType()))
            return base.CreateInterfaceProxyWithTarget(interfaceToProxy, additionalInterfacesToProxy, Target, options, interceptors);

        interceptors = interceptors.Select(i => new InterceptorWithFixInterfaces(i, interfaceToProxy)).ToArray();
        return base.CreateInterfaceProxyWithoutTarget(interfaceToProxy, additionalInterfacesToProxy, options, interceptors);
    }

    public override object CreateClassProxy(Type classToProxy, Type[] additionalInterfacesToProxy,
        ProxyGenerationOptions options, object[] constructorArguments, params Castle.DynamicProxy.IInterceptor[] interceptors)
    {
        if (additionalInterfacesToProxy.Any(i => classToProxy.GetInterfaces().Contains(i))) // We actually make sure to include all explictly implemented interfaces in AutoMock.cs
                            interceptors = interceptors.Select(i => new InterceptorWithFixInterfaces(i)).ToArray();

        if (isMoq) return originalProxyGenerator.CreateClassProxy(classToProxy, additionalInterfacesToProxy,
                    options, constructorArguments, interceptors);

        // Rememeber that Moq uses the generator as static, so we have to ensure that the target is valid
        if (Target is not null && classToProxy.IsAssignableFrom(Target.GetType()))
            return originalProxyGenerator.CreateClassProxyWithTarget(classToProxy, additionalInterfacesToProxy,
                        Target, options, constructorArguments, interceptors);

        // In Moq they use two types of proxies
        //      1) for mock (which always has IMocked)
        //      2) for recording which doesn't need to callbase (and might have issues if we don't supply the ctor args and there is no defualt ctor)
        var imockedType = classToProxy.IsClass ? typeof(IMocked<>).MakeGenericType(classToProxy) : null;

        // If callbase is false we want to add/replace the default ctor
        // But also moq has an issue for explicit implementations that it always calls base (unless the method/prop/event is explictly setup, and even that doesn't work for default interface)
        if (typeof(Type) == classToProxy ||
            (Callbase != false && imockedType is not null && additionalInterfacesToProxy.Contains(imockedType)))
        {
            // Moq has an issue with SetupSet/SetupAdd/SetupRemove that it throws for any explicit implmentation
            // Moq also has an issue with default interface implementation (when not overriden) in that it always calls base even when explictly setup
            var typeDetail = classToProxy.GetTypeDetailInfo();
            if (typeDetail.ExplicitMethodDetails.All(m => m.ReflectionInfo.DeclaringType?.IsInterface != true || m.DeclarationType != DeclarationTypes.Decleration)
                && typeDetail.ExplicitPropertyDetails.All(p => p.ReflectionInfo.DeclaringType?.IsInterface != true || p.DeclarationType != DeclarationTypes.Decleration)
                && !typeDetail.ExplicitEventDetails.All(e => e.ReflectionInfo.DeclaringType?.IsInterface != true || e.DeclarationType != DeclarationTypes.Decleration))
            {
                return originalProxyGenerator.CreateClassProxy(classToProxy, additionalInterfacesToProxy,
                    options, constructorArguments, interceptors);
            }
        }

        var typeToUse = ProxyTypeService.GetProxyType(classToProxy, Callbase != false);

        var proxyType = CreateClassProxyType(typeToUse, additionalInterfacesToProxy, options);
        var arguments = BuildArgumentListForClassProxy(options, interceptors);
        return CreateClassProxyInstance(proxyType, arguments, classToProxy, constructorArguments);
    }
}
