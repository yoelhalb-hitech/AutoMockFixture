using System.Reflection;

namespace AutoMoqExtensions.Extensions;

internal static class MethodInfoExtensions
{
    public static bool IsInternal(this MethodInfo methodInfo) => methodInfo.IsAssembly || methodInfo.IsFamilyOrAssembly;
    public static bool IsPublicOrInternal(this MethodInfo methodInfo) => methodInfo.IsPublic || methodInfo.IsInternal();


    internal static bool IsOverridable(this MethodInfo method)
    {
        /*
         * From MSDN (http://goo.gl/WvOgYq)
         *
         * To determine if a method is overridable, it is not sufficient to check that IsVirtual is true.
         * For a method to be overridable, IsVirtual must be true and IsFinal must be false.
         *
         * For example, interface implementations are marked as "virtual final".
         * Methods marked with "override sealed" are also marked as "virtual final".
         */

        return method.IsVirtual && !method.IsFinal;
    }

    internal static bool IsSealed(this MethodInfo method) => !method.IsOverridable();
    internal static bool IsVoid(this MethodInfo method) => method.ReturnType == typeof(void);

    internal static bool HasOutParameters(this MethodInfo method) => method.GetParameters().Any(p => p.IsOut);

    // "out" parameters are also considered "byref", so we have to filter these out
    internal static bool HasRefParameters(this MethodInfo method) => method.GetParameters()
                     .Any(p => p.ParameterType.IsByRef && !p.IsOut);

    internal static bool HasOverloads(this MethodInfo method)
        => method.DeclaringType.GetAllMethods().Any(m => m.Name == method.Name && m != method);

    internal static bool HasOverloadSameCount(this MethodInfo method)
        => method.DeclaringType.GetAllMethods().Any(m => m.Name == method.Name
                                                    && m.GetParameters().Length == method.GetParameters().Length
                                                    && m.IsGenericMethod == method.IsGenericMethod
                                                    && (!m.IsGenericMethod 
                                                        || m.GetGenericArguments().Length == method.GetParameters().Length)
                                                    && m != method);
    internal static string GetTrackingPath(this MethodInfo method)
    {
        var str = method.Name;

        var hasOverloads = method.HasOverloads();
        var hasSameCount = hasOverloads && method.HasOverloadSameCount();

        if (method.IsGenericMethod && !method.IsGenericMethodDefinition)
        {
            // TODO... do we have to be concerned that there will be two different types with the same name?...
            str += "<" + String.Join(",", method.GetGenericArguments().Select(m => m.Name)) + ">";
        }
        else if(method.IsGenericMethodDefinition)
        {
            str += hasOverloads switch
            {
                false => "",
                true when hasSameCount => $"<`{method.GetGenericArguments().Length}>",
                _ => $"<`{String.Join(",", method.GetGenericArguments().Select(a => a.Name))}>",
            };
        }

        return str + hasOverloads switch
        {
            false => "",
            true when hasSameCount => "`" + method.GetParameters().Length,
            _ => "(" + String.Join(",", method.GetParameters().Select(p => p.ParameterType.Name)) + ")",
        };
    }
         
}
