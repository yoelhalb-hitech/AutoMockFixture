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

        internal static IEnumerable<PropertyInfo> GetAllProperties(this Type type)
        {
            IEnumerable<PropertyInfo> result = type.GetProperties(AllBindings);

            // If "type" is an interface, "GetProperties" does not return methods declared on other interfaces extended by "type".
            if (type.GetTypeInfo().IsInterface)
                result = result.Concat(type.GetInterfaces().SelectMany(x => x.GetProperties(AllBindings)));

            return result;
        }

        internal static IEnumerable<MethodInfo> GetAllMethods(this Type type)
        {
            IEnumerable<MethodInfo> result = type.GetMethods(AllBindings);

            // If "type" is an interface, "GetMethods" does not return methods declared on other interfaces extended by "type".
            if (type.GetTypeInfo().IsInterface)
                result = result.Concat(type.GetInterfaces().SelectMany(x => x.GetMethods(AllBindings)));

            return result;
        }

        internal static IEnumerable<FieldInfo> GetAllFields(this Type type)
        {
            IEnumerable<FieldInfo> result = type.GetFields(AllBindings);

            // If "type" is an interface, "GetProperties" does not return methods declared on other interfaces extended by "type".
            if (type.GetTypeInfo().IsInterface)
                result = result.Concat(type.GetInterfaces().SelectMany(x => x.GetFields(AllBindings)));

            return result;
        }

        internal static bool IsDelegate(this Type type)
        {
            return typeof(MulticastDelegate).IsAssignableFrom(type.GetTypeInfo().BaseType);
        }
    }
}
