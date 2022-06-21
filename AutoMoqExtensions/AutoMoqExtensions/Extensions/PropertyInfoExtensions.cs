using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AutoMoqExtensions.Extensions
{
    internal static class PropertyInfoExtensions
    {
        internal static bool IsOverridable(this PropertyInfo property)
            => (property.GetMethod is not null && property.GetMethod.IsOverridable()) || property.SetMethod.IsOverridable();

        internal static bool HasGetAndSet(this PropertyInfo property)
            => property.GetMethod is not null && property.SetMethod is not null;        

        internal static string GetTrackingPath(this PropertyInfo property)
                => property.Name;
    }
}
