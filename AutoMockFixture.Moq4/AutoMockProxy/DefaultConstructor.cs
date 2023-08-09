
using System.Reflection.Emit;

namespace AutoMockFixture.Moq4.AutoMockProxy;

internal class DefaultConstructorService
{
    static DefaultConstructorService()
    {
        emptyConstuctor = EmitDefaultConstructor();
    }

    public static ConstructorInfo GetDefaultConstructor() => emptyConstuctor;
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
