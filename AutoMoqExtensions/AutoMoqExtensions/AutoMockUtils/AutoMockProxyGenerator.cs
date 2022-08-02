using Castle.DynamicProxy;
using System.Reflection;
using Moq;
using Moq.Protected;
using System.Linq.Expressions;
using System.Globalization;
using System.Reflection.Emit;

namespace AutoMoqExtensions.AutoMockUtils;

internal class AutoMockProxyGenerator : ProxyGenerator
{
    // This contains the cache so we will have it static
    private static DefaultProxyBuilder callbaseProxyBuilder = new DefaultProxyBuilder(); 
    private static DefaultProxyBuilder nonCallbaseProxyBuilder = new DefaultProxyBuilder(); 
    object? target;
    private readonly bool callbase;

    public AutoMockProxyGenerator(object? target, bool callbase) : base(callbase ? callbaseProxyBuilder : nonCallbaseProxyBuilder)
    {
        this.target = target;
        this.callbase = callbase;
    }

    public override object CreateClassProxy(Type classToProxy, Type[] additionalInterfacesToProxy, 
        ProxyGenerationOptions options, object[] constructorArguments, params IInterceptor[] interceptors)
    {
        // Rememeber that Moq uses the generator as static, so we have to ensure that the target is valid
        if (target is not null && classToProxy.IsAssignableFrom(target.GetType()))
            return CreateClassProxyWithTarget(classToProxy, additionalInterfacesToProxy, 
                        target, options, constructorArguments, interceptors);

        if (typeof(Type) == classToProxy || this.callbase) return base.CreateClassProxy(classToProxy, additionalInterfacesToProxy, 
                        options, constructorArguments, interceptors);

        if (classToProxy is null) throw new ArgumentNullException(nameof(classToProxy));
        if (options is null) throw new ArgumentNullException(nameof(options));
        if (!classToProxy.IsClass) throw new ArgumentException("'classToProxy' must be a class", nameof(classToProxy));

        CheckNotGenericTypeDefinition(classToProxy, nameof(classToProxy));
        CheckNotGenericTypeDefinitions(additionalInterfacesToProxy, nameof(additionalInterfacesToProxy));

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

        // Looks like it is not setting up correctly abstract methods, but definetly not protected
        mockType.SetupGet(t => t.Assembly).Returns(classToProxy.Assembly);
        mockType.SetupGet(t => t.Module).Returns(classToProxy.Module);
        mockType.SetupGet(t => t.Namespace).Returns(classToProxy.Namespace);
        mockType.SetupGet(t => t.Name).Returns(classToProxy.Name);
        mockType.SetupGet(t => t.FullName).Returns(classToProxy.FullName);
        mockType.SetupGet(t => t.AssemblyQualifiedName).Returns(classToProxy.AssemblyQualifiedName);
        mockType.SetupGet(t => t.CustomAttributes).Returns(classToProxy.CustomAttributes);
        mockType.SetupGet(t => t.DeclaringType).Returns(classToProxy.DeclaringType);
        mockType.SetupGet(t => t.ReflectedType).Returns(classToProxy.ReflectedType);
        mockType.SetupGet(t => t.UnderlyingSystemType).Returns(classToProxy.UnderlyingSystemType);
        mockType.SetupGet(t => t.GUID).Returns(classToProxy.GUID);
        mockType.SetupGet(t => t.BaseType).Returns(classToProxy.BaseType);
        mockType.Protected().Setup<bool>("IsArrayImpl").Returns((bool)typeof(Type).GetMethod("IsArrayImpl", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(classToProxy, new object[] { }));
        mockType.Protected().Setup<bool>("IsByRefImpl").Returns((bool)typeof(Type).GetMethod("IsByRefImpl", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(classToProxy, new object[] { }));
        mockType.Protected().Setup<bool>("IsPointerImpl").Returns((bool)typeof(Type).GetMethod("IsPointerImpl", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(classToProxy, new object[] { }));
        mockType.Protected().Setup<bool>("HasElementTypeImpl").Returns((bool)typeof(Type).GetMethod("HasElementTypeImpl", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(classToProxy, new object[] { }));
        mockType.Protected().Setup<bool>("IsCOMObjectImpl").Returns((bool)typeof(Type).GetMethod("IsCOMObjectImpl", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(classToProxy, new object[] { }));
        mockType.Protected().Setup<bool>("IsContextfulImpl").Returns((bool)typeof(Type).GetMethod("IsContextfulImpl", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(classToProxy, new object[] { }));
        mockType.Protected().Setup<bool>("IsMarshalByRefImpl").Returns((bool)typeof(Type).GetMethod("IsMarshalByRefImpl", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(classToProxy, new object[] { }));
        mockType.Protected().Setup<bool>("IsPrimitiveImpl").Returns((bool)typeof(Type).GetMethod("IsPrimitiveImpl", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(classToProxy, new object[] { }));
        mockType.Setup(m => m.GetElementType()).Returns(classToProxy.GetElementType());

        mockType.Setup(m => m.GetEvent(It.IsAny<string>(), It.IsAny<BindingFlags>())).Returns((string name, BindingFlags f) => classToProxy.GetEvent(name, f));
        mockType.Setup(m => m.GetEvents(It.IsAny<BindingFlags>())).Returns((BindingFlags f) => classToProxy.GetEvents(f));

        mockType.Setup(m => m.GetField(It.IsAny<string>(), It.IsAny<BindingFlags>())).Returns((string name, BindingFlags f) => classToProxy.GetField(name, f));
        mockType.Setup(m => m.GetFields(It.IsAny<BindingFlags>())).Returns((BindingFlags f) => classToProxy.GetFields(f));
        mockType.Setup(m => m.GetMembers(It.IsAny<BindingFlags>())).Returns((BindingFlags f) => classToProxy.GetMembers(f));
        mockType.Protected().Setup<MethodInfo>("GetMethodImpl",
            new object[] { ItExpr.IsAny<string>(), ItExpr.IsAny<BindingFlags>(), ItExpr.IsAny<Binder>(), ItExpr.IsAny<CallingConventions>(), ItExpr.IsAny<Type[]>(), ItExpr.IsAny<ParameterModifier[]>() })
        .Returns((string name, BindingFlags f, Binder b, CallingConventions c, Type[] t, ParameterModifier[] p) => classToProxy.GetMethod(name, f, b, c, t, p));
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        mockType.Protected().Setup<MethodInfo>("GetMethodImpl",
            new object[] { ItExpr.IsAny<string>(), It.IsAny<int>(), ItExpr.IsAny<BindingFlags>(), ItExpr.IsAny<Binder>(), ItExpr.IsAny<CallingConventions>(), ItExpr.IsAny<Type[]>(), ItExpr.IsAny<ParameterModifier[]>() })
        .Returns((string name, int gc, BindingFlags f, Binder b, CallingConventions c, Type[] t, ParameterModifier[] p) => classToProxy.GetMethod(name, gc, f, b, c, t, p));
#endif
        mockType.Setup(m => m.GetMethods(It.IsAny<BindingFlags>())).Returns((BindingFlags f) => classToProxy.GetMethods(f));
        mockType.Setup(m => m.GetNestedType(It.IsAny<string>(), It.IsAny<BindingFlags>())).Returns((string name, BindingFlags f) => classToProxy.GetNestedType(name, f));
        mockType.Setup(m => m.GetNestedTypes(It.IsAny<BindingFlags>())).Returns((BindingFlags f) => classToProxy.GetNestedTypes(f));
        mockType.Protected().Setup<PropertyInfo>("GetPropertyImpl",
            new object[] { ItExpr.IsAny<string>(), ItExpr.IsAny<BindingFlags>(), ItExpr.IsAny<Binder>(), ItExpr.IsAny<Type>(), ItExpr.IsAny<Type[]>(), ItExpr.IsAny<ParameterModifier[]>() })
        .Returns((string name, BindingFlags f, Binder b, Type t1, Type[] t, ParameterModifier[] p) => classToProxy.GetProperty(name, f, b, t1, t, p));
        mockType.Setup(m => m.GetProperties(It.IsAny<BindingFlags>())).Returns((BindingFlags f) => classToProxy.GetProperties(f));
        mockType.Setup(m => m.InvokeMember(It.IsAny<string>(), It.IsAny<BindingFlags>(), It.IsAny<Binder>(), It.IsAny<object>(), It.IsAny<object[]>(), It.IsAny<ParameterModifier[]>(), It.IsAny<CultureInfo>(), It.IsAny<string[]>()))
        .Returns((string name, BindingFlags f, Binder b, object o, object[] o1, ParameterModifier[] p, CultureInfo ci, string[] s) => classToProxy.InvokeMember(name, f, b, o, o1, p, ci, s));
        mockType.Setup(m => m.GetInterface(It.IsAny<string>(), It.IsAny<bool>())).Returns((string name, bool b) => classToProxy.GetInterface(name, b));
        mockType.Setup(m => m.GetInterfaces()).Returns(classToProxy.GetInterfaces());
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

        var attrs = (TypeAttributes)typeof(Type)
                        .GetMethod("GetAttributeFlagsImpl", BindingFlags.Instance | BindingFlags.NonPublic)
                        .Invoke(classToProxy, new object[] { });
        mockType.Protected().Setup<TypeAttributes>("GetAttributeFlagsImpl").Returns(attrs);

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
