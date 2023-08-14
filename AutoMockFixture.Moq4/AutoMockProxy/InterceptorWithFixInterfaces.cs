using DotNetPowerExtensions.Reflection;
using Moq;
using Moq.Protected;
using System;
using System.Diagnostics;

namespace AutoMockFixture.Moq4.AutoMockProxy;

/// <summary>
/// This will fix the fooling Moq issues:
/// 1) If an inherited interface was added as an additional interface and the method is not virtual it will still call the proxy overriden method
///         This only happens when using late binding (such as when doing generic or dynamic) as by default the compiler binds it to the base
/// 2) Moq does not handle correctly default interface implementations for events
/// 3) Moq does not handle correctly the case of default interface implementation when the original interface is abstract and only an inherited interface implements it
/// </summary>
internal class InterceptorWithFixInterfaces : Castle.DynamicProxy.IInterceptor
{
    private readonly Castle.DynamicProxy.IInterceptor originalinterceptor;
    private readonly Type? originalType;

    public InterceptorWithFixInterfaces(Castle.DynamicProxy.IInterceptor originalinterceptor, Type? originalType = null)
    {
        this.originalinterceptor = originalinterceptor;
        this.originalType = originalType;
    }

    public void Intercept(Castle.DynamicProxy.IInvocation invocation)
    {
        var proxyType = invocation.Proxy.GetType();
        if (invocation.Method.IsExplicitImplementation() || (invocation.Method.DeclaringType != proxyType && invocation.Method.DeclaringType?.IsInterface != true))
        {
            originalinterceptor.Intercept(invocation);
            return;
        }

        var m = invocation.TargetType?.GetMethod(invocation.Method.Name, BindingFlagsExtensions.AllBindings);
        // .GetMethod might end up with an ambiguas match if two methods have the same name, while .GetMethods can return also base shadowed methods so we have to do it this way
        if (m is null || (m.IsVirtual && !m.IsFinal) || proxyType.GetMethods(BindingFlagsExtensions.AllBindings).All(me => me.IsEqual(m) || !me.IsSignatureEqual(m)))
        {
            if(m is null) invocation = GetFixedInvocationForDefaultImplmentation(invocation);
            originalinterceptor.Intercept(invocation);
            return;
        }

        // We are with a reimplmented method but base is not virtual so we have to call base
        var method = m;
        if (m.IsGenericMethodDefinition) method = m.MakeGenericMethod(invocation.GenericArguments);

        try
        {
            invocation.ReturnValue = m.Invoke(invocation.InvocationTarget, invocation.Arguments); // If it's non virtual we send it back to the original method
        }
        catch(TargetInvocationException ex) // Reflection rethrows any errors as `TargetInvocationException` so get the inner exception
        {
            throw ex.InnerException ?? ex;
        }
    }

    private Castle.DynamicProxy.IInvocation? GetFixedInvocationForEvent(Castle.DynamicProxy.IInvocation invocation)
    {
        var detailInfo = (invocation.TargetType ?? originalType)!.GetTypeDetailInfo();
        //Remember that e.AddMethod.Name has the short name while the refletionInfo has the explicit name
        var evtMethod = detailInfo.ExplicitEventDetails.Select(e => new[] { e.AddMethod, e.RemoveMethod }.FirstOrDefault(m => m.Name == invocation.Method.Name)).FirstOrDefault(m => m is not null);

        if (evtMethod is null || evtMethod.ReflectionInfo == invocation.Method) return null;

        //return new InvocationWrapper(invocation, new MethodWrapper(evtMethod.ReflectionInfo) );

        var t = new AutoMock<TypeInfo>() { CallBase = true };
        t.SetTarget(invocation.Method.DeclaringType!.GetTypeInfo());
        t.Protected().Setup<TypeAttributes>("GetAttributeFlagsImpl").Returns(0); // If it is an interface then Moq tries to match the interface map method to ours which won't work, so fake it

        var originalMethod = invocation.Method;

        var md = new MethodWrapper(originalMethod);
        md.MethodAttributes = invocation.Method.Attributes & ~MethodAttributes.Abstract; // Since the original declaration method should never be callable we don't have to worry faking it as non abstract...
        md.ParentType = t.Object;

        return new InvocationWrapper(invocation, md, evtMethod.ReflectionInfo);
    }

    private Castle.DynamicProxy.IInvocation? GetFixedInvocationForOriginalBaseAbstract(Castle.DynamicProxy.IInvocation invocation)
    {
        var detailInfo = originalType?.GetTypeDetailInfo();

        var methodToUse = detailInfo?.ExplicitMethodDetails
                                        .Union(detailInfo.ExplicitPropertyDetails.SelectMany(p => new[] { p.SetMethod, p.GetMethod, p.BasePrivateGetMethod, p.BasePrivateSetMethod }))
                                        .FirstOrDefault(m => m?.Name == invocation.Method.Name);
        if (methodToUse is null) return null;

        var t = new AutoMock<TypeInfo>() { CallBase = true };
        t.SetTarget(invocation.Method.DeclaringType!.GetTypeInfo());
        t.Protected().Setup<TypeAttributes>("GetAttributeFlagsImpl").Returns(0); // If it is an interface then Moq tries to match the interface map method to ours which won't work, so fake it

        var originalMethod = invocation.Method;

        var md = new MethodWrapper(originalMethod);
        md.MethodAttributes = invocation.Method.Attributes & ~MethodAttributes.Abstract; // Since the original declaration method should never be callable we don't have to worry faking it as non abstract...
        md.ParentType = t.Object;

        return new InvocationWrapper(invocation, md, methodToUse.ReflectionInfo);
    }
    private Castle.DynamicProxy.IInvocation GetFixedInvocationForDefaultImplmentation(Castle.DynamicProxy.IInvocation invocation)
    {
        if (!invocation.Method.IsAbstract || invocation.Method.DeclaringType?.IsInterface != true) return invocation;

        Castle.DynamicProxy.IInvocation? invocationToReturn = null;
        if((invocation.TargetType is not null || originalType is not null)
                                            && new[] { "add_", "remove_" }.Any(s => invocation.Method.Name.StartsWith(s)))
        {
            invocationToReturn = GetFixedInvocationForEvent(invocation);
        }

        if (invocationToReturn is null && invocation.TargetType is null && originalType is not null)
        {
            // Moq has a bug with a default implemented event that was implmented in a sub interface but not in the original, in this case we will have to fake it

            invocationToReturn = GetFixedInvocationForOriginalBaseAbstract(invocation);
        }

        return invocationToReturn ?? invocation;
    }
}
