using SequelPay.DotNetPowerExtensions.Reflection;

namespace AutoMockFixture.Extensions;

internal static class FieldInfoExtensions
{
    internal static bool IsRelevant(this FieldInfo field)
    => field.ReflectedType?.Namespace?.StartsWith(nameof(System) + ".") is true
        || field.ReflectedType?.Namespace?.StartsWith(nameof(Microsoft) + ".") is true ? field.IsPublic : field.IsPublicOrInternal();
}
