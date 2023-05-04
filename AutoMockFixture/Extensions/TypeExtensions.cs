using DotNetPowerExtensions.Reflection;

namespace AutoMockFixture.Extensions;

internal static class TypeExtensions
{
    internal static ConstructorInfo GetDefaultConstructor(this Type type) => type.GetConstructor(Type.EmptyTypes);

    internal static ConstructorInfo GetParamsConstructor(this Type type) => type.GetConstructor(new[] { typeof(object[]) });
    internal static IEnumerable<ConstructorInfo> GetPublicAndProtectedConstructors(this Type type)
        => type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(ctor => !ctor.IsPrivate);

    internal static IEnumerable<PropertyInfo> GetAllProperties(this Type type, bool includeBasePrivate = false)
        => GetAll(type, includeBasePrivate, (t, b) => t.GetProperties(b));
    internal static IEnumerable<PropertyInfo> GetExplicitInterfaceProperties(this Type type)
        => GetAll(type, true, (t, b) => t.GetProperties(b)).Where(p => p.IsExplicitImplementation());

    internal static IEnumerable<MethodInfo> GetAllMethods(this Type type, bool includeBasePrivate = false)
        => GetAll(type, includeBasePrivate, (t, b) => t.GetMethods(b));

    internal static IEnumerable<MethodInfo> GetExplicitInterfaceMethods(this Type type)
        => GetAll(type, true, (t, b) => t.GetMethods(b)).Where(p => p.IsExplicitImplementation());

    internal static IEnumerable<FieldInfo> GetAllFields(this Type type, bool includeBasePrivate = false)
       => GetAll(type, includeBasePrivate, (t, b) => t.GetFields(b));

#if !NET6_0_OR_GREATER
    internal static MethodInfo? GetMethod(this Type type, string name, BindingFlags bindingAttr, Type[] types)
        => type.GetMethod(name, bindingAttr, null, types, null);
#endif

    internal static (MethodInfo InterfaceMethod, MethodInfo TargetMethod)[] ToPairs(this InterfaceMapping mapping)
        => Enumerable.Range(0, mapping.InterfaceMethods.Length).Select(i => (mapping.InterfaceMethods[i], mapping.TargetMethods[i])).ToArray();


    private static Dictionary<Type, MethodInfo[]> defaultInterfaceImplementationsCache = new Dictionary<Type, MethodInfo[]>();

    // An interface-method is either a base (which can be implemented in the same interface) or an overrider in the interface that it is overriden
    // We want to remove all base methods besides the actual used ones
    internal static MethodInfo[] GetUnusableMethods(this Type type, Type iface)
        => type.GetInterfaceMapForInterface(iface).InterfaceMethods.Except(type.GetDefaultInterfaceImplementations()).ToArray();

    internal static MethodInfo[] GetUnusableMethods(this Type type)
        => type.GetInterfaces().SelectMany(i => type.GetUnusableMethods(i)).ToArray();

    internal static MethodInfo[] GetDefaultInterfaceImplementations(this Type type)
    {
        // Here is how it works:
        // 1. For the original declaring interface it will always show the original in the interface-methods while the most specific will show up in target-methods
        // 2. For the overriding interface it will always show the ovrriding method in both interface-methods and target-methods
        // 3. An overriding method will always be explictly implemented
        // Thus a valid usage can either be an overriden (with explicit) with a different class than the interface-method or non explicit
        // Note that a valid overriden can show up in both the base (but a different reflected class than the interface-method)
        //          and also in the overrider, so it's vary hard to find the non valid, rather find the valid and remove the rest
        if(!defaultInterfaceImplementationsCache.ContainsKey(type))
            defaultInterfaceImplementationsCache[type] = type.GetInterfaces()
                                                                .Select(i => type.IsInterface ? type.GetInterfaceMapForInterface(i) : type.GetInterfaceMap(i))
                                                                .SelectMany(m => m.ToPairs()
                                                                                .Where(p => IsValidImplementation(p.InterfaceMethod, p.TargetMethod)
                                                                                        && p.TargetMethod.ReflectedType.IsInterface)
                                                                                .Select(p => p.TargetMethod))
                                                                .ToArray();
        return defaultInterfaceImplementationsCache[type];

        static bool IsValidImplementation(MethodInfo interfaceMethod, MethodInfo targetMethod)
            => !targetMethod.IsAbstract && (!targetMethod.IsExplicitImplementation() || interfaceMethod.ReflectedType != targetMethod.ReflectedType);

    }

    private static IEnumerable<T> GetAll<T>(this Type type, bool includeBasePrivate, Func<Type, BindingFlags, IEnumerable<T>> func)
                                                                                                where T : MemberInfo
    {
        var bindings = BindingFlagsExtensions.AllBindings;
        var isInterface = type.GetTypeInfo().IsInterface;
        //CAUTION: BindingFlags.DeclaredOnly will still give you overrides, so we don't use it because we will end up with multiple copies

        var result = func(type, bindings);

        // If "type" is an interface, "GetProperties/GetMethods" does not return methods declared on other interfaces extended by "type".
        if (isInterface)
            result = result.Concat(type.GetInterfaces().SelectMany(i => func(i, bindings)).Where(x => includeBasePrivate || !isPrivate(x)));
        else if (includeBasePrivate)
            result = result.Concat(type.GetBaseTypes().SelectMany(b => func(b, bindings)).Where(x => isPrivate(x)));

        if (isInterface)
        {
            // We got all the methods from all the interfaces, so we have to remove the overriden ones
            var nonValidMethods = type.GetUnusableMethods();

            result = result.Except(nonValidMethods.Select(m => toT(m)).OfType<T>().ToArray());
        }
        else
        {
            // If type is not an interface then it should have all interface implemented besides default interface implementations
            var interfaceImplementations = type.GetDefaultInterfaceImplementations();
            var items = interfaceImplementations.Select(toT).OfType<T>();
            result = result.Union(items); // Add

            if(includeBasePrivate) // Private with default implementation won't show up in the interface map so we have to look for them
            {
                result = result.Concat(type.GetInterfaces().SelectMany(i => func(i, bindings)).Where(x => isPrivate(x) && !isAbstract(x)));
            }
        }

        return result;

        bool isPrivate(T t) => t switch
        {
            MethodBase m => m.IsPrivate,
            PropertyInfo p => p.GetAllMethods().All(m => m.IsPrivate),
            FieldInfo f => f.IsPrivate,
            EventInfo e => e.AddMethod?.IsPrivate != false && e.RemoveMethod?.IsPrivate != false,
            Type type => type.IsNestedPrivate,
            _ => false,
        };

        bool isAbstract(T t) => t switch
        {
            MethodBase m => m.IsAbstract,
            PropertyInfo p => p.IsAbstract(),
            FieldInfo f => false,
            EventInfo e => e.IsAbstract(),
            Type type => type.IsAbstract,
            _ => false,
        };

        T? toT(MethodBase m)
        {
            var tt = typeof(T);
            if (typeof(MethodBase).IsAssignableFrom(tt)) return m as MemberInfo as T;
            if (typeof(FieldInfo).IsAssignableFrom(tt)) return null;
            if (typeof(PropertyInfo).IsAssignableFrom(tt)) return m.ReflectedType
                                                                                    .GetProperties(BindingFlagsExtensions.AllBindings | BindingFlags.DeclaredOnly)
                                                                                    .FirstOrDefault(p => p.GetAllMethods().Contains(m)) as MemberInfo as T;
            if (typeof(EventInfo).IsAssignableFrom(tt)) return m.ReflectedType
                                                                        .GetEvents(BindingFlagsExtensions.AllBindings | BindingFlags.DeclaredOnly)
                                                                        .FirstOrDefault(e => e.AddMethod == m || e.RemoveMethod == m) as MemberInfo as T;
            if (typeof(Type).IsAssignableFrom(tt)) return m.ReflectedType as object as T;

            return null;
        };
    }

    // Based on  https://stackoverflow.com/a/22824808/640195
    internal static string ToGenericTypeString(this Type t, params Type[] arg)
    {
        if (t.IsGenericParameter || (t.FullName is null && !t.IsNested)) return t.Name;//Generic argument stub
        else if (t.FullName is null && t.IsNested) return ToGenericTypeString(t.DeclaringType) + "." + t.Name;

        bool isGeneric = t.IsGenericType || t.FullName!.IndexOf('`') >= 0;//an array of generic types is not considered a generic type although it still have the genetic notation
        bool isArray = !t.IsGenericType && t.FullName!.IndexOf('`') >= 0;
        Type genericType = t;
        while (genericType.IsNested && genericType.DeclaringType.GetGenericArguments().Count() == t.GetGenericArguments().Count())//Non generic class in a generic class is also considered in Type as being generic
        {
            genericType = genericType.DeclaringType;
        }
        if (!isGeneric) return t.Name.Replace('+', '.');

        var arguments = arg.Any() ? arg : t.GetGenericArguments();//if arg has any then we are in the recursive part, note that we always must take arguments from t, since only t (the last one) will actually have the constructed type arguments and all others will just contain the generic parameters
        string genericTypeName = genericType.Name;
        if (genericType.IsNested)
        {
            var argumentsToPass = arguments.Take(genericType.DeclaringType.GetGenericArguments().Count()).ToArray();//Only the innermost will return the actual object and only from the GetGenericArguments directly on the type, not on the on genericDfintion, and only when all parameters including of the innermost are set
            arguments = arguments.Skip(argumentsToPass.Count()).ToArray();
            genericTypeName = genericType.DeclaringType.ToGenericTypeString(argumentsToPass) + "." + genericType.Name;//Recursive
        }
        if (isArray)
        {
            genericTypeName = t.GetElementType().ToGenericTypeString() + "[]";//this should work even for multidimensional arrays
        }
        if (genericTypeName.IndexOf('`') >= 0)
        {
            genericTypeName = genericTypeName.Substring(0, genericTypeName.IndexOf('`'));
            string genericArgs = string.Join(",", arguments.Select(a => a.ToGenericTypeString()).ToArray());
            //Recursive
            genericTypeName = genericTypeName + "<" + genericArgs + ">";
            if (isArray) genericTypeName += "[]";
        }
        if (t != genericType)
        {
            genericTypeName += t.Name.Replace(genericType.Name, "").Replace('+', '.');
        }
        if (genericTypeName.IndexOf("[") >= 0 && genericTypeName.IndexOf("]") != genericTypeName.IndexOf("[") + 1) genericTypeName = genericTypeName.Substring(0, genericTypeName.IndexOf("["));//For a non generic class nested in a generic class we will still have the type parameters at the end
        return genericTypeName;
    }

    internal static string GetTagForTypes(this IEnumerable<Type> types)
    {
        return string.Join("#", types.OrderBy(t => t.AssemblyQualifiedName).Select(t => t.AssemblyQualifiedName));
    }
}
