using SequelPay.DotNetPowerExtensions.Reflection;

namespace AutoMockFixture.Extensions;

internal static class EventInfoExtensions
{
    internal static string GetTrackingPath(this EventDetail evtDetail)
            => evtDetail.ExplicitInterfaceReflectionInfo?.GetTrackingPath(true)
                                        ?? evtDetail.ReflectionInfo.GetTrackingPath(false);

    internal static string GetTrackingPath(this EventInfo evt, bool isExplicit)
            => (isExplicit ? ":" + evt.DeclaringType!.FullName + "." : "") + evt.Name;
}
