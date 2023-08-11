using DotNetPowerExtensions.Reflection;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AutoMockFixture.Extensions;

internal static class TypeExtensions
{
    internal static IEnumerable<ConstructorInfo> GetPublicAndProtectedConstructors(this Type type)
        => type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(ctor => !ctor.IsPrivate);

    internal static IEnumerable<MethodInfo> GetAllMethods(this Type type, bool includeBasePrivate = false)
    {
        var typeDetail = type.GetTypeDetailInfo();
        var methodDetails = typeDetail.MethodDetails.Union(typeDetail.ExplicitMethodDetails);

        var propDetails = typeDetail.PropertyDetails.Union(typeDetail.ExplicitPropertyDetails);
        var evtDetails = typeDetail.EventDetails.Union(typeDetail.ExplicitEventDetails);

        if (includeBasePrivate)
        {
            methodDetails = methodDetails.Union(typeDetail.BasePrivateMethodDetails)
                                                                .Union(typeDetail.ShadowedMethodDetails);
            propDetails = propDetails.Union(typeDetail.BasePrivatePropertyDetails)
                                                                .Union(typeDetail.ShadowedPropertyDetails);
            evtDetails = evtDetails.Union(typeDetail.BasePrivateEventDetails)
                                                    .Union(typeDetail.ShadowedEventDetails);
        }

        var allMethods = methodDetails
                    .Union(propDetails.SelectMany(p => new[]
                    {
                        p.GetMethod,
                        p.SetMethod,
                        includeBasePrivate ? p.BasePrivateGetMethod : null,
                        includeBasePrivate ? p.BasePrivateSetMethod : null,
                    }))
                    .Union(evtDetails.SelectMany(e => new[]
                    {
                        e.AddMethod,
                        e.RemoveMethod
                    }));
        return allMethods.Select(md => md?.ReflectionInfo).OfType<MethodInfo>().ToList();
    }

    // Based on  https://stackoverflow.com/a/22824808/640195
    internal static string ToGenericTypeString(this Type t, params Type[] arg)
    {
        // TODO... Maybe CodeDomProvider.GetTypeOutput does the job already? at least we can use it for the primitive types
        if (t is null) return "null";
        else if (t.IsGenericType && !t.IsGenericTypeDefinition && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                                                                                    return t.GetGenericArguments().First().ToGenericTypeString() + "?";
        else if (t.IsGenericType && !t.IsGenericTypeDefinition && t.Namespace == nameof(System) && t.Name.StartsWith(nameof(ValueTuple) + "`"))
                                                        return "(" + string.Join(",", t.GetGenericArguments().Select(a => a.ToGenericTypeString())) + ")";
        else if (t.IsArray) return t.GetElementType()!.ToGenericTypeString() + "[]";//this should work even for multidimensional arrays

        var dict = new Dictionary<Type, string>
        {
            [typeof(string)] = "string",
            [typeof(int)] = "int",
            [typeof(uint)] = "uint",
            [typeof(byte)] = "byte",
            [typeof(sbyte)] = "sbyte",
            [typeof(char)] = "char",
            [typeof(long)] = "long",
            [typeof(ulong)] = "ulong",
            [typeof(short)] = "short",
            [typeof(ushort)] = "ushort",
            [typeof(float)] = "float",
            [typeof(double)] = "double",
            [typeof(bool)] = "bool",
            [typeof(decimal)] = "decimal",
            [typeof(object)] = "object",
            [typeof(nint)] = "nint",
            [typeof(nuint)] = "nuint",
        };
        if(dict.ContainsKey(t)) return dict[t];

        if (t.IsGenericParameter || (t.FullName is null && !t.IsNested)) return t.Name;//Generic argument stub
        else if (t.FullName is null && t.IsNested) return ToGenericTypeString(t.DeclaringType!) + "." + t.Name;

        bool isGeneric = t.IsGenericType || t.FullName!.IndexOf('`') >= 0;//an array of generic types is not considered a generic type although it still have the genetic notation
        bool isArray = !t.IsGenericType && t.FullName!.IndexOf('`') >= 0;
        Type genericType = t;
        while (genericType.IsNested && genericType.DeclaringType!.GetGenericArguments().Count() == t.GetGenericArguments().Count())//Non generic class in a generic class is also considered in Type as being generic
        {
            genericType = genericType.DeclaringType;
        }
        if (!isGeneric) return t.Name.Replace('+', '.');

        var arguments = arg.Any() ? arg : t.GetGenericArguments();//if arg has any then we are in the recursive part, note that we always must take arguments from t, since only t (the last one) will actually have the constructed type arguments and all others will just contain the generic parameters
        string genericTypeName = genericType.Name;
        if (genericType.IsNested)
        {
            var argumentsToPass = arguments.Take(genericType.DeclaringType!.GetGenericArguments().Count()).ToArray();//Only the innermost will return the actual object and only from the GetGenericArguments directly on the type, not on the on genericDfintion, and only when all parameters including of the innermost are set
            arguments = arguments.Skip(argumentsToPass.Count()).ToArray();
            genericTypeName = genericType.DeclaringType.ToGenericTypeString(argumentsToPass) + "." + genericType.Name;//Recursive
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
