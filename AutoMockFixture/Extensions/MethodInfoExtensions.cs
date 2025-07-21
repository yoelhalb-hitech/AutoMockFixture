using SequelPay.DotNetPowerExtensions.Reflection;
using System.Collections.Concurrent;
using System.Reflection;

namespace AutoMockFixture.Extensions;

internal static class MethodInfoExtensions
{
    internal static bool IsRelevant(this MethodInfo method)
        => method.ReflectedType?.Namespace == nameof(System)
            || method.ReflectedType?.Namespace?.StartsWith(nameof(System) + ".") == true
            || method.ReflectedType?.Namespace == nameof(Microsoft)
            || method.ReflectedType?.Namespace?.StartsWith(nameof(Microsoft) + ".") == true ? method.IsPublic : method.IsPublicOrInternal();

    internal static bool IsSealed(this MethodInfo method) => !method.IsOverridable();

    internal static bool HasOutParameters(this MethodInfo method) => method.GetParameters().Any(p => p.IsOut);

    // "out" parameters are also considered "byref", so we have to filter these out
    internal static bool HasRefParameters(this MethodInfo method) => method.GetParameters()
                     .Any(p => p.ParameterType.IsByRef && !p.IsOut);

    internal static bool HasOverloads(this MethodInfo method) => method.HasOverloadsInternal(false);

    internal static bool HasOverloadSameCount(this MethodInfo method) => method.HasOverloadsInternal(true);

    private static bool HasOverloadsInternal(this MethodInfo method, bool sameCount)
        => method.ReflectedType?.GetAllMethods().Any(m => m.Name == method.Name // CATUION: Use `RefelectedType` as `DeclaringType` methods will be different for a subclass
                    && (!sameCount || m.GetParameters().Length == method.GetParameters().Length)
                    && (!method.IsGenericMethod || !m.IsGenericMethod
                            || method.GetGenericMethodDefinition() != m.GetGenericMethodDefinition()) // CATUION: Use `GetGenericMethodDefinition` on both sides of the equality since `ReflectedType` for `GetGenericMethodDefinition` might be the base class
                    && m != method) ?? false;

    internal static ConcurrentDictionary<(MethodInfo, bool), string> methodPathCache = new();

    internal static string GetTrackingPath(this MethodDetail methodDetail)
        => methodDetail.ExplicitInterfaceReflectionInfo?.GetTrackingPath(true)
                    ?? methodDetail.ReflectionInfo.GetTrackingPath(false);

    internal static string GetTrackingPath(this MethodInfo method, bool isExplicit)
        => methodPathCache.GetOrAdd((method, isExplicit), (_) =>
        {
            var str = method.Name;
            if (isExplicit) str = ":" + method.DeclaringType.FullName + "." + str;

            if (method.IsGenericMethod && !method.IsGenericMethodDefinition)
            {
                // TODO... do we have to be concerned that there will be two different types with the same name?...
                str += "<" + String.Join(",", method.GetGenericArguments().Select(m => m.ToGenericTypeString())) + ">";
            }
            else if (method.IsGenericMethodDefinition)
            {
                str += $"`{method.GetGenericArguments().Length}";
            }
            // TODO... out and ref or in are considered different overloads than if none
            var hasOverloads = method.HasOverloads();
            var hasSameCount = hasOverloads && method.HasOverloadSameCount();

            return str + hasOverloads switch
            {
                false => "",
                true when !hasSameCount => "(`" + method.GetParameters().Length + ")",
                _ => "(" + String.Join(",", method.GetParameters().Select(p => p.ParameterType.ToGenericTypeString())) + ")",
            };
        });
}
