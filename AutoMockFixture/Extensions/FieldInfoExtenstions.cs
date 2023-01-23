using System.Reflection;

namespace AutoMockFixture.Extensions;

internal static class FieldInfoExtenstions
{
    public static bool IsInternal(this FieldInfo fieldInfo) => fieldInfo.IsAssembly || fieldInfo.IsFamilyOrAssembly;
    public static bool IsPublicOrInternal(this FieldInfo fieldInfo) => fieldInfo.IsPublic || fieldInfo.IsInternal();
}
