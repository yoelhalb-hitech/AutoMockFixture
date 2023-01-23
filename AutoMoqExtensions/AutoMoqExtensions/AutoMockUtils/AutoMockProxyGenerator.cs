using Castle.DynamicProxy;
using System.Reflection;
using Moq;
using Moq.Protected;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace AutoMoqExtensions.AutoMockUtils;

internal class AutoMockProxyGenerator : ProxyGenerator
{
    private static ProxyGenerator originalProxyGenerator = new ProxyGenerator();
    // This contains the caches so we will have it static
    private static DefaultProxyBuilder nonCallbaseProxyBuilder = new DefaultProxyBuilder();

    internal object? Target { get; }
    internal bool? Callbase { get; }

    public AutoMockProxyGenerator(object? target, bool callbase) : base(nonCallbaseProxyBuilder)
    {
        this.Target = target;
        this.Callbase = callbase;
    }
    public AutoMockProxyGenerator() : base(nonCallbaseProxyBuilder){}

    private void Validate(Type classToProxy, Type[] additionalInterfacesToProxy, ProxyGenerationOptions options)
    {
        if (classToProxy is null) throw new ArgumentNullException(nameof(classToProxy));
        if (options is null) throw new ArgumentNullException(nameof(options));
        if (!classToProxy.IsClass) throw new ArgumentException("'classToProxy' must be a class", nameof(classToProxy));

        CheckNotGenericTypeDefinition(classToProxy, nameof(classToProxy));
        CheckNotGenericTypeDefinitions(additionalInterfacesToProxy, nameof(additionalInterfacesToProxy));
    }

    public override object CreateInterfaceProxyWithoutTarget(Type interfaceToProxy, Type[] additionalInterfacesToProxy, ProxyGenerationOptions options, params IInterceptor[] interceptors)
    {
        // Rememeber that Moq uses the generator as static, so we have to ensure that the target is valid
        if (Target is not null && interfaceToProxy.IsAssignableFrom(Target.GetType()))
            return base.CreateInterfaceProxyWithTarget(interfaceToProxy, additionalInterfacesToProxy, Target, options, interceptors);

        return base.CreateInterfaceProxyWithoutTarget(interfaceToProxy, additionalInterfacesToProxy, options, interceptors);
    }

    public override object CreateClassProxy(Type classToProxy, Type[] additionalInterfacesToProxy, 
        ProxyGenerationOptions options, object[] constructorArguments, params IInterceptor[] interceptors)
    {
        // Rememeber that Moq uses the generator as static, so we have to ensure that the target is valid
        if (Target is not null && classToProxy.IsAssignableFrom(Target.GetType()))
            return originalProxyGenerator.CreateClassProxyWithTarget(classToProxy, additionalInterfacesToProxy, 
                        Target, options, constructorArguments, interceptors);

        // In Moq they use two types of proxies
        //      1) for mock (which always has IMocked)
        //      2) for recording which doesn't need to callbase (and might have issues if we don't supply the ctor args and there is no defualt ctor)
        var imockedType = classToProxy.IsClass ? typeof(IMocked<>).MakeGenericType(classToProxy) : null;

        if (typeof(Type) == classToProxy || 
            (this.Callbase != false && imockedType is not null && additionalInterfacesToProxy.Contains(imockedType))) 
                return originalProxyGenerator.CreateClassProxy(classToProxy, additionalInterfacesToProxy, 
                        options, constructorArguments, interceptors);

        var typeToUse = GetTypeToUse(classToProxy);

        var proxyType = CreateClassProxyType(typeToUse, additionalInterfacesToProxy, options);

        var arguments = BuildArgumentListForClassProxy(options, interceptors);
        return CreateClassProxyInstance(proxyType, arguments, classToProxy, constructorArguments);
    }

    private static Dictionary<Type, Type> typesToUseDict = new Dictionary<Type, Type>();
    private static Type GetTypeToUse(Type type)
    {
        if (typesToUseDict.ContainsKey(type)) return typesToUseDict[type];

        var mockType = CreateMockType(type);
        typesToUseDict[type] = mockType;

        return mockType;
    }
    private static Type CreateMockType(Type classToProxy)
    {
        var mockType = new AutoMock<TypeInfo>();

        mockType.SetTarget((TypeInfo)classToProxy);
        mockType.CallBase = true;

        // Needed for Castle.Core < 5.0
        // We need to explictly return it, as otherwise it won't intercept calls when accessed via GetTypeInfo() even though it is proxied
        mockType.As<IReflectableType>().Setup(t => t.GetTypeInfo()).Returns(() => mockType.Object);

        Expression<Func<BindingFlags, bool>> bindingExpr = f => (f & BindingFlags.Public) == BindingFlags.Public
                            && (f & BindingFlags.Instance) == BindingFlags.Instance;
        Func<BindingFlags, ConstructorInfo[]> ctorsFunc = f =>
                (f & BindingFlags.Public) != BindingFlags.Public || (f & BindingFlags.Instance) != BindingFlags.Instance
                                    ? classToProxy.GetConstructors(f)
                                    : classToProxy.GetConstructors(f)
                                        .Except(new ConstructorInfo[] { classToProxy.GetConstructor(f, null, new Type[] { }, null) })
                                        .Union(new[] { emptyConstuctor }).ToArray();

        mockType.Setup(t => t.GetConstructors(It.Is<BindingFlags>(bindingExpr)))
            .Returns(ctorsFunc);

        mockType.Protected()
            .Setup<ConstructorInfo>("GetConstructorImpl",
                    new object[] { It.Is<BindingFlags>(bindingExpr), ItExpr.IsAny<Binder>(),
                                        ItExpr.IsAny<CallingConventions>(), ItExpr.Is<Type[]>(t => !t.Any()),
                                        ItExpr.IsAny<ParameterModifier[]>() })
            .Returns(() => emptyConstuctor);

        return mockType.GetMocked();
    }

    static AutoMockProxyGenerator()
    {
        emptyConstuctor = EmitDefaultConstructor();
    }
    private static ConstructorInfo emptyConstuctor;
    private static ConstructorInfo EmitDefaultConstructor()
    {
        var name = "D" + Guid.NewGuid().ToString("N");
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(name), AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
        var tb = moduleBuilder.DefineType(name,
                TypeAttributes.Public |
                TypeAttributes.Class |
                TypeAttributes.AutoClass |
                TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit |
                TypeAttributes.AutoLayout,
                null, null);
        ConstructorBuilder constructor = tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
        tb.CreateTypeInfo();

        return constructor;
    }
}
