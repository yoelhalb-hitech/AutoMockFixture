using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.Extensions
{
    internal static class TypeExtensions
    {

        public static BindingFlags AllBindings => BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        internal static ConstructorInfo GetDefaultConstructor(this Type type) => type.GetConstructor(Type.EmptyTypes);

        internal static ConstructorInfo GetParamsConstructor(this Type type) => type.GetConstructor(new[] { typeof(object[]) });
        internal static IEnumerable<ConstructorInfo> GetPublicAndProtectedConstructors(this Type type)
            => type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(ctor => !ctor.IsPrivate);

        private static ConcurrentDictionary<Type, object> typeDefaults =
           new ConcurrentDictionary<Type, object>();
        public static object? GetDefault(this Type t)
        {
            return typeDefaults.GetOrAdd(t, t1 =>
            {
                Func<object?> f = GetDefault<object>;
                return f.Method.GetGenericMethodDefinition().MakeGenericMethod(t1).Invoke(null, null);
            });
        }

        private static T? GetDefault<T>()
        {
            return default(T);
        }

        public static bool IsNullAllowed(this Type t) => t.GetDefault() is null;  // If type.GetDefault() is null then null is allowed, otherwise it's not...

        internal static IEnumerable<PropertyInfo> GetAllProperties(this Type type, bool includeBasePrivate = false)
            => GetAll(type, includeBasePrivate, (t, b) => t.GetProperties(b));
        internal static IEnumerable<MethodInfo> GetAllMethods(this Type type, bool includeBasePrivate = false)
            => GetAll(type, includeBasePrivate, (t, b) => t.GetMethods(b));

        internal static IEnumerable<FieldInfo> GetAllFields(this Type type, bool includeBasePrivate = false)
           => GetAll(type, includeBasePrivate, (t, b) => t.GetFields(b));
        
        private static IEnumerable<T> GetAll<T>(this Type type, bool includeBasePrivate, Func<Type, BindingFlags, IEnumerable<T>> func)
        {
            var bindings = AllBindings;
            var isInterface = type.GetTypeInfo().IsInterface;
            if (includeBasePrivate && !isInterface) bindings |= BindingFlags.DeclaredOnly;

            var result = func(type, bindings);
            
            // If "type" is an interface, "GetProperties" does not return methods declared on other interfaces extended by "type".
            if (isInterface)
                result = result.Concat(type.GetInterfaces().SelectMany(i => func(i, bindings)));
            else if (includeBasePrivate)
            {
                var b = type.BaseType;
                while (b is not null)
                {
                    result = result.Concat(func(b, bindings));
                    b = b.BaseType;
                }
            }
            return result;
        }

        internal static bool IsDelegate(this Type type)
        {
            return typeof(MulticastDelegate).IsAssignableFrom(type.GetTypeInfo().BaseType);
        }

        internal static string GetTagForTypes(this IEnumerable<Type> types)
        {
            return string.Join("#", types.OrderBy(t => t.AssemblyQualifiedName).Select(t => t.AssemblyQualifiedName));
        }
    }
}
