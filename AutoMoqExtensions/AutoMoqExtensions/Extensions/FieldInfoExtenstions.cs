using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.Extensions
{
    internal static class FieldInfoExtenstions
    {
        public static bool IsInternal(this FieldInfo fieldInfo) => fieldInfo.IsAssembly || fieldInfo.IsFamilyOrAssembly;
        public static bool IsPublicOrInternal(this FieldInfo fieldInfo) => fieldInfo.IsPublic || fieldInfo.IsInternal();

    }
}
