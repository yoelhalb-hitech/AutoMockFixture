using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.SpecialRequests;
using DotNetPowerExtensions.Reflection;
using System.Reflection;

namespace AutoMockFixture.FixtureUtils.Builders.SpecialBuilders;

internal class EnumerableBuilder : NonConformingBuilder
{
    public override Type[] SupportedTypes => new Type[] 
    { 
        typeof(IEnumerable<>),
#if NET461_OR_GREATER || NETSTANDARD2_0_OR_GREATER
        typeof(IAsyncEnumerable<>)
#endif
    };

    public override int Repeat => 3;

    protected override InnerRequest GetInnerRequest(Type type, IRequestWithType originalRequest, int index, int argIndex)
        => new ListItemRequest(type, originalRequest, index);

    protected override object GetRepeatedInnerSpecimens(IRequestWithType originalRequest, ISpecimenContext context)
    {
        if (originalRequest.Request == typeof(string)) return new NoSpecimen();

        return base.GetRepeatedInnerSpecimens(originalRequest, context);
    }

    public override object CreateResult(Type requestType, object[][] innerResults)
    {
        var genericType = GetInnerTypes(requestType).First();

        var data = innerResults.Select(x => x.First()); //Assuming that it is only one item per line
        var typedData = (IEnumerable<object>)(typeof(Enumerable).GetMethod(nameof(Enumerable.Cast))
            .MakeGenericMethod(genericType)
            .Invoke(null, new object[] { data }));
        var isNotEnumerable = requestType.GetInterfaces().All(x => x.IsGenericType && x.GetGenericTypeDefinition() != typeof(IEnumerable<>));
        
        var typeToMatch =
#if NET461_OR_GREATER || NETSTANDARD2_0_OR_GREATER
            isNotEnumerable ? typeof(IAsyncEnumerable<>).MakeGenericType(genericType) :
#endif
            typedData.GetType();
        Func<Type, bool> isMatch = t => t.IsAssignableFrom(typeToMatch);

#if NET461_OR_GREATER || NETSTANDARD2_0_OR_GREATER
        if (isNotEnumerable && typeToMatch == requestType)
        {
            return GetType()
                .GetMethod(nameof(CreateAsyncEnumerable), BindingFlags.Instance | BindingFlags.NonPublic)
                .MakeGenericMethod(genericType)
                .Invoke(this, new object[] { typedData });
        }
#endif

        if (isNotEnumerable && isMatch(requestType)) return typedData;

        if (requestType.IsGenericType && requestType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            return typedData;

        if (requestType.IsArray) return typeof(Enumerable).GetMethod(nameof(Enumerable.ToArray))
            .MakeGenericMethod(genericType)
            .Invoke(null, new object[] { typedData });

        if (new[] {typeof(List<>), typeof(IList<>)}.Contains(requestType.GetGenericTypeDefinition())) 
            return typeof(Enumerable).GetMethod(nameof(Enumerable.ToList))
            .MakeGenericMethod(genericType)
            .Invoke(null, new object[] { typedData });

        // TODO...probably better handle direct Dictionary ReadOnlyCollection HashSet and their interfaces etc, and everything in system.collections.generic.


        return CreateType(requestType, genericType, typedData, isMatch) ?? new NoSpecimen();
    }

    private object? CreateType(Type requestType, Type genericType,
                                    IEnumerable<object> typedData, Func<Type, bool> isMatch)
    {
        // TODO... interfaces
        var ctors = requestType.GetConstructors(BindingFlagsExtensions.AllBindings);
        var singleCtors = ctors.Where(x => x.GetParameters().Length == 1);
        var enumerableCtor = singleCtors.FirstOrDefault(x => isMatch(x.GetParameters()[0].ParameterType));
        if (enumerableCtor is not null) return enumerableCtor.Invoke(new object[] { typedData });

        var emptyCtor = ctors.FirstOrDefault(x => x.GetParameters().Length == 0);
        var intCtor = ctors.FirstOrDefault(x => x.GetParameters().Length == 1 
                                                && x.GetParameters().First().ParameterType ==  typeof(int));
        var methods = requestType.GetAllMethods();
        if (emptyCtor is not null || intCtor is not null)
        {
            var enumerableMethod = methods.FirstOrDefault(m => m.GetParameters().Length == 1
                        && isMatch(m.GetParameters()[0].ParameterType));
            if (enumerableMethod is not null)
            {
                var obj = emptyCtor?.Invoke(new object[] { }) ?? intCtor.Invoke(new object[] { typedData.Count() });
                enumerableMethod.Invoke(enumerableMethod.IsStatic ? null : obj, new object[] { typedData });
                return obj;
            }

            var singleItemMethod = methods.FirstOrDefault(m => m.GetParameters().Length == 1
                        && m.GetParameters()[0].ParameterType == genericType);
            if (singleItemMethod is not null)
            {
                var obj = emptyCtor?.Invoke(new object[] { }) ?? intCtor.Invoke(new object[] { typedData.Count() });
                foreach (var item in typedData)
                {
                    singleItemMethod.Invoke(singleItemMethod.IsStatic ? null : obj, new object[] { item });
                }

                return obj;
            }
        }
        // TODO... we can go further and look for a ctor or a method that can take another enumerable descendent as parameter and 
        //      if that one has a method that can accept the data, and we can go further recursviely
        // var typeToBaseOf = isNotEnumerable ? typeToMatch : typeof(IEnumerable<>).MakeGenericType(genericType);
        //var otherCtor = ctors.FirstOrDefault(x => x.GetParameters().Length == 1 && typeToBaseOf.IsAssignableFrom(x.GetParameters()[0].ParameterType));
        //if (otherCtor is not null)
        //{
        //    var obj = otherCtor.Invoke(new object[] { typedData });
        //    return obj;
        //}

        return null;
    }

#if NET461_OR_GREATER || NETSTANDARD2_0_OR_GREATER
    private async IAsyncEnumerable<T> CreateAsyncEnumerable<T>(IEnumerable<T> enumerable)
    {
        await Task.CompletedTask.ConfigureAwait(false);
        foreach (var item in enumerable)
        {
            yield return item;
        }
    }
#endif
}
