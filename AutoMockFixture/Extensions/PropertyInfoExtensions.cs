using System.Reflection;

namespace AutoMockFixture.Extensions;

internal static class PropertyInfoExtensions
{
    internal static bool IsOverridable(this PropertyInfo property)
        => (property.GetMethod is not null && property.GetMethod.IsOverridable()) || property.SetMethod.IsOverridable();

    internal static bool HasGetAndSet(this PropertyInfo property, bool includeBasePrivate)
        => property.GetMethod is not null
        && (property.SetMethod is not null || (includeBasePrivate && property.GetWritablePropertyInfo() is not null));

    internal static PropertyInfo? GetWritablePropertyInfo(this PropertyInfo property)
    {
        if (property.SetMethod is not null) return property;

        // It is still possible that the original decleration of the property has a private set

        // `prop.DeclaringType` or `prop.GetMethod.DeclaringType` will only return the class it was overridden in
        //      but we cannot just traverse the graph and just see if the property exists,
        //          as it might have been shadowed, and we are dealing with the shadow

        var declaringType = property.GetMethod.GetBaseDefinition().DeclaringType;
        if (property.ReflectedType == declaringType) return null;

        var declaringProperty = declaringType.GetProperty(property.Name, Extensions.TypeExtensions.AllBindings);

        return declaringProperty?.SetMethod is not null ? declaringProperty : null;
    }

    internal static MethodInfo[] GetMethods(this PropertyInfo property)
        => property.GetAccessors(true);

    internal static bool IsExplicitImplementation(this PropertyInfo property)
        => property.Name.Contains(".") && property.GetMethods().All(m => m.IsExplicitImplementation());

    internal static string GetTrackingPath(this PropertyInfo property)
            => property.Name;
}
