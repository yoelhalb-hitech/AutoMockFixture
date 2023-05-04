using DotNetPowerExtensions.Reflection;
using System.Reflection;

namespace AutoMockFixture.Extensions;

internal static class MethodInfoExtensions
{
    internal static bool IsSealed(this MethodInfo method) => !method.IsOverridable();

    internal static bool HasOutParameters(this MethodInfo method) => method.GetParameters().Any(p => p.IsOut);

    // "out" parameters are also considered "byref", so we have to filter these out
    internal static bool HasRefParameters(this MethodInfo method) => method.GetParameters()
                     .Any(p => p.ParameterType.IsByRef && !p.IsOut);

    internal static bool HasOverloads(this MethodInfo method)
        => method.DeclaringType.GetAllMethods().Any(m => m.Name == method.Name && m != method 
                        && (!method.IsGenericMethod || method.GetGenericMethodDefinition() != m));

    internal static bool HasOverloadSameCount(this MethodInfo method)
        => method.DeclaringType.GetAllMethods().Any(m => m.Name == method.Name
                                                    && m.GetParameters().Length == method.GetParameters().Length
                                                    && (!method.IsGenericMethod || method.GetGenericMethodDefinition() != m)
                                                    && m != method);


    internal static string GetTrackingPath(this MethodInfo method)
    {
        var str = method.Name;

        if (method.IsGenericMethod && !method.IsGenericMethodDefinition)
        {
            // TODO... do we have to be concerned that there will be two different types with the same name?...
            str += "<" + String.Join(",", method.GetGenericArguments().Select(m => m.Name)) + ">";
        }
        else if(method.IsGenericMethodDefinition)
        {
            str += $"`{method.GetGenericArguments().Length}";
        }

        var hasOverloads = method.HasOverloads();
        var hasSameCount = hasOverloads && method.HasOverloadSameCount();

        return str + hasOverloads switch
        {
            false => "",
            true when !hasSameCount => "(`" + method.GetParameters().Length + ")",
            _ => "(" + String.Join(",", method.GetParameters().Select(p => p.ParameterType.Name)) + ")",
        };
    }
}
