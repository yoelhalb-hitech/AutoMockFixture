using Castle.DynamicProxy;
using System;
using System.Reflection;

namespace AutoMoqExtensions.AutoMockUtils
{   
    internal class AutoMockProxyGenerator : ProxyGenerator
    {
        object? target;
        public AutoMockProxyGenerator(object? target)
        {
            this.target = target;
        }

        public override object CreateClassProxy(Type classToProxy, Type[] additionalInterfacesToProxy, 
            ProxyGenerationOptions options, object[] constructorArguments, params IInterceptor[] interceptors)
        {
            // Rememeber that Moq uses the generator as static, so we have to ensure that the target is valid
            if (target is not null && classToProxy.IsAssignableFrom(target.GetType()))
                return CreateClassProxyWithTarget(classToProxy, additionalInterfacesToProxy, 
                            target, options, constructorArguments, interceptors);

            return base.CreateClassProxy(classToProxy, additionalInterfacesToProxy, 
                            options, constructorArguments, interceptors);
        }
    }
}
