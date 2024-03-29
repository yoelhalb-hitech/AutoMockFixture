﻿
using Moq;

namespace AutoMockFixture.Moq4.MockUtils;

public class MatcherGenerator
{
    private static Dictionary<string, Type> matcherDict = new Dictionary<string, Type>();
    private static object lockObject = new object(); // TODO... Maybe we can improve that by locking on unique things such as interned strings

    /// <summary>
    /// Gets a matcher for <see cref="ParameterInfo"/> that is potentially a generic type
    /// </summary>
    /// <param name="parameterInfo">The potentialy generic parameter</param>
    /// <returns>A <see cref="Type"/> instance of the correct matcher</returns>
    public static Type GetGenericMatcher(ParameterInfo parameterInfo)
    {
        return GetMatcherForParameterInternal(parameterInfo.ParameterType).type;
    }

    /// <summary>
    /// Gets a matcher for a <see cref="Type"/> object that is potentially generic
    /// </summary>
    /// <param name="parameterInfo">The potentialy generic parameter</param>
    /// <returns>A <see cref="Type"/> instance of the correct matcher</returns>
    public static Type GetGenericMatcher(Type genericType)
    {
        if (!genericType.IsGenericParameter) return GetMatcherForParameterInternal(genericType).type;

        var isValueType = (genericType.GenericParameterAttributes &
        GenericParameterAttributes.SpecialConstraintMask & GenericParameterAttributes.NotNullableValueTypeConstraint) != 0;
        var constraints = genericType.GetGenericParameterConstraints();

        if (!constraints.Any()) return isValueType ? typeof(It.IsValueType) : typeof(It.IsAnyType);

        else
        {
            // https://stackoverflow.com/a/59144369/640195
            if (isValueType) constraints = constraints.Union(new[] { typeof(ValueType) }).ToArray();
            return GetOrAdd(constraints);
        }
    }

    private static(Type type, bool constructed) GetMatcherForParameterInternal(Type parameterType)
    {
        if (parameterType.IsArray)
        {
            var recurse = GetMatcherForParameterInternal(parameterType.GetElementType()!);
            if (recurse.constructed) return (recurse.Item1.MakeArrayType(), true);
            return (parameterType, false);
        }

        if (parameterType.IsGenericType)
        {
            var parameters = parameterType.GetGenericArguments(); // `GetGenericArguments()` returns the actual types if specified, otherwise the stubs
            var recurses = parameters.Select(p => GetMatcherForParameterInternal(p)).ToList();
            if (!recurses.Any(r => r.constructed)) return (parameterType, false);

            return (parameterType.GetGenericTypeDefinition()
                                    .MakeGenericType(recurses.Select(r => r.Item1).ToArray()), true);
        }

        if (parameterType.IsGenericParameter)
        {
            return (GetGenericMatcher(parameterType), true);
        }

        return (parameterType, false);
    }

    private static Type GetOrAdd(IEnumerable<Type> types)
    {
        var tag = types.GetTagForTypes();
        if (matcherDict.ContainsKey(tag)) return matcherDict[tag];
        lock (lockObject)
        {
            if (matcherDict.ContainsKey(tag)) return matcherDict[tag];

            var newType = CreateTypeMatcher(types);

            matcherDict[tag] = newType;
            return newType;
        }
    }

    private static Type CreateTypeMatcher(IEnumerable<Type> types)
    {
        var name = "D" + Guid.NewGuid().ToString("N"); // Making sure that the name is unique but staring with a alpha letter
        var parent = types.FirstOrDefault(c => !c.IsInterface);
        var interfaces = types.Where(c => c.IsInterface).ToArray();

        var emitter = new TypeMatcherEmitter(name, parent, interfaces);
        return emitter.EmitTypeMatcher();
    }
}
