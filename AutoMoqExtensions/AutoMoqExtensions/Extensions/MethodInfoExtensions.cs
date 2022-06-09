using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.Extensions
{
    internal static class MethodInfoExtensions
    {
        public static bool IsInternal(this MethodInfo methodInfo) => methodInfo.IsAssembly || methodInfo.IsFamilyOrAssembly || methodInfo.IsFamilyAndAssembly;
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
    }
}
