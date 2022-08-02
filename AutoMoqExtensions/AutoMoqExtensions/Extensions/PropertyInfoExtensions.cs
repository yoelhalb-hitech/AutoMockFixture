using System.Reflection;

namespace AutoMoqExtensions.Extensions;

internal static class PropertyInfoExtensions
{
    internal static bool IsOverridable(this PropertyInfo property)
        => (property.GetMethod is not null && property.GetMethod.IsOverridable()) || property.SetMethod.IsOverridable();

    internal static bool HasGetAndSet(this PropertyInfo property)
        => property.GetMethod is not null && property.SetMethod is not null;

    internal static MethodInfo[] GetMethods(this PropertyInfo property)
        => property.GetAccessors(true);

    internal static string GetTrackingPath(this PropertyInfo property)
            => property.Name;
}
