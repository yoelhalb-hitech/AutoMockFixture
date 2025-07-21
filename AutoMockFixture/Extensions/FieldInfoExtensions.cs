using SequelPay.DotNetPowerExtensions.Reflection;

namespace AutoMockFixture.Extensions;

internal static class FieldInfoExtensions
{
    internal static bool IsRelevant(this FieldInfo field)
    => field.ReflectedType.Namespace.StartsWith(nameof(System) + ".")
        || field.ReflectedType.Namespace.StartsWith(nameof(Microsoft) + ".") ? field.IsPublic : field.IsPublicOrInternal();
}
