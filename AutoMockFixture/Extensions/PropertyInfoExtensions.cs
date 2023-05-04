using System.Reflection;

namespace AutoMockFixture.Extensions;

internal static class PropertyInfoExtensions
{
    internal static string GetTrackingPath(this PropertyInfo property)
            => property.Name;
}
