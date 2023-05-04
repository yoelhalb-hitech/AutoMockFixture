using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace AutoMockFixture.Moq4.MockUtils;

// TODO... Maybe we can improve perfomance by batching them together in 1 assembly,
//        (my benchmarks showed that until 10k there is a performance benefit, but afterwards it degardes)
// We can do it even intially to round up all generics of the object and have it created in a background thread while executing the other methods
// However in this case I would recommend doing only small batches at once, so we should be able to start setting up as soon as it compiles some

internal class TypeMatcherEmitter
{
    private static ConstructorInfo ignoreAccessCtor = typeof(IgnoresAccessChecksToAttribute).GetConstructor(new Type[] { typeof(string) });
    private static ConstructorInfo typeMatcherCtor = typeof(TypeMatcherAttribute).GetConstructor(new Type[] { });

    private MethodInfo[] methods;
    private Type[] interfaces;
    private readonly string name;
    private readonly Type? parent;

    private string[] AssemblyNames => interfaces.Union(new[] { parent }).Where(t => t is not null)
                                    .Select(t => t!.Assembly.FullName.Split(',').First())
                                    .Distinct().ToArray();
    public TypeMatcherEmitter(string name, Type? parent, Type[] interfaces)
    {
        this.methods = interfaces.SelectMany(i => i.GetAllMethods())
                .Union(parent?.GetAllMethods().Where(m => m.IsAbstract) ?? new MethodInfo[] { }).ToArray();

        this.interfaces = interfaces.Union(new[] { typeof(ITypeMatcher) }).ToArray(); // ITypeMatcher is implemented separately than the others
        this.name = name;
        this.parent = parent;
    }

    public Type EmitTypeMatcher()
    {
        // TODO... this will probably not work if the type is in a strong named assembly if our assembly is not strong named
        var ab = CreateAssemblyBuilder();
        var tb = CreateTypeBuilder(ab);

        CreateMatcherMethod(tb);
        foreach (var method in methods)
        {
            CreateEmptyMethod(tb, method);
        }

        return tb.CreateTypeInfo();
    }

    private AssemblyBuilder CreateAssemblyBuilder()
    {
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(name), AssemblyBuilderAccess.RunAndCollect);

        foreach (var refName in AssemblyNames)
        {
            var builder = new CustomAttributeBuilder(ignoreAccessCtor, new object[] { refName });
            assemblyBuilder.SetCustomAttribute(builder);
        }

        return assemblyBuilder;
    }

    private TypeBuilder CreateTypeBuilder(AssemblyBuilder assemblyBuilder)
    {
        var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
        var tb = moduleBuilder.DefineType(name,
                TypeAttributes.Public |
                TypeAttributes.Class |
                TypeAttributes.AutoClass |
                TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit |
                TypeAttributes.AutoLayout,
                parent, interfaces);
        tb.SetCustomAttribute(new CustomAttributeBuilder(typeMatcherCtor, new object[] { }));

        ConstructorBuilder constructor = tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);

        return tb;
    }

    private void CreateMatcherMethod(TypeBuilder tb)
    {
        var matchesMthdBldr = tb.DefineMethod(nameof(ITypeMatcher.Matches), MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, typeof(bool), new[] { typeof(Type) });

        var matchesIl = matchesMthdBldr.GetILGenerator();
        matchesIl.Emit(OpCodes.Ldc_I4_1);
        matchesIl.Emit(OpCodes.Ret);

        tb.DefineMethodOverride(matchesMthdBldr, typeof(ITypeMatcher).GetMethod(nameof(ITypeMatcher.Matches)));
    }

    private void CreateEmptyMethod(TypeBuilder tb, MethodInfo method) //  We don't need an actual implementation
    {
        var accessMethod = method switch
        {
            { IsPublic: true } => MethodAttributes.Public,
            { IsPrivate: true } => MethodAttributes.Private,
            { IsFamily: true } => MethodAttributes.Family,
            { IsFamilyAndAssembly: true } => MethodAttributes.FamANDAssem,
            { IsFamilyOrAssembly: true } => MethodAttributes.FamORAssem,
            { IsAssembly: true } => MethodAttributes.Assembly,
            _ => MethodAttributes.Public,
        };
        if (method.IsStatic) accessMethod = accessMethod | MethodAttributes.Static;
        if (method.DeclaringType.IsInterface) accessMethod = accessMethod | MethodAttributes.Virtual;

        var mthdBldr = tb.DefineMethod(method.Name, accessMethod | MethodAttributes.HideBySig, method.ReturnType, method.GetParameters().Select(p => p.ParameterType).ToArray());

        var il = mthdBldr.GetILGenerator();
        il.Emit(OpCodes.Nop);
        il.Emit(OpCodes.Newobj, typeof(NotImplementedException));
        il.Emit(OpCodes.Throw);

        if (method.DeclaringType.IsInterface) tb.DefineMethodOverride(mthdBldr, method);
    }
}
