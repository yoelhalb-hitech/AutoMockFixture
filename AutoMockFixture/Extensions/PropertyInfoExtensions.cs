using SequelPay.DotNetPowerExtensions.Reflection;

namespace AutoMockFixture.Extensions;

internal static class PropertyInfoExtensions
{
    internal static string GetTrackingPath(this PropertyDetail propDetail)
            => propDetail.ExplicitInterfaceReflectionInfo?.GetTrackingPath(true)
                                        ?? propDetail.ReflectionInfo.GetTrackingPath(false);

    internal static string GetTrackingPath(this PropertyInfo property, bool isExplicit)
            => (isExplicit ? ":" + property.DeclaringType.FullName + "." : "") + property.Name;
}
