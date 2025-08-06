using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.SpecialRequests;
using SequelPay.DotNetPowerExtensions.Reflection;

namespace AutoMockFixture.FixtureUtils.Commands;

internal class AutoMockPopulateEnumerableCommand : ISpecimenCommand
{
    private readonly IAutoMockHelpers autoMockHelpers;
    private readonly SetupServiceFactoryBase setupServiceFactory;

    public AutoMockPopulateEnumerableCommand(IAutoMockHelpers autoMockHelpers,
            SetupServiceFactoryBase setupServiceFactory, int repeat)
    {
        this.autoMockHelpers = autoMockHelpers;
        this.setupServiceFactory = setupServiceFactory;
        Repeat = repeat;
    }

    public int Repeat { get; }

    public void Execute(object specimen, ISpecimenContext context)
    {
        if (context is null) throw new ArgumentNullException(nameof(context));

        try
        {
            var mock = autoMockHelpers.GetFromObj(specimen);
            if (mock?.CallBase is not false || mock.Tracker is not IRequestWithType originalRequest) return;

            var obj = (mock as IAutoMock)?.GetMocked() ?? specimen;
            if (obj is not System.Collections.IEnumerable) return;

            var enumerableIface = obj.GetType().GetInterfaces().FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            if (enumerableIface is null) return; // Only an enumerable, not a generic enumerable so we can't help him

            var innerType = enumerableIface.GetInnerTypes().First();

            // Remember that an Ienumerator has to be a new one each time
            var resultFunc = GenerateResult(context, originalRequest, innerType);
            if (resultFunc is null) return;

            var returnTypeMatcherFunc = (Type t) => !t.IsGenericType || t.GetInnerTypes().First().IsAssignableFrom(innerType);
            var methods = GetMethods(obj.GetType().BaseType!, returnTypeMatcherFunc); // We can only setup on base type methods (which is the original object) as otheriwse the lambda expression throws

            foreach (var method in methods)
            {
                try
                {
                    setupServiceFactory.GetMethodSetup(mock, method, context).SetupWithResult(resultFunc);
                    mock.MethodsSetup.Add(method.GetTrackingPath(), method.ReflectionInfo);
                }
                catch { }
            }
        }
        catch { }
    }

    private Func<object?>? GenerateResult(ISpecimenContext context, IRequestWithType originalRequest, Type innerType)
    {
        // TODO... for lazy mode this should be done lazy...
        var inners = GetRepeatedInnerSpecimens(originalRequest, innerType, context)
                        .Where(o => o is not NoSpecimen and not OmitSpecimen);
        if (!inners.Any()) return null;

        // We need the typed version otheriwse it will just be IEnumerable<object>
        var typedInners = typeof(Enumerable).GetMethod(nameof(Enumerable.OfType))!
                                .MakeGenericMethod(innerType)
                                .Invoke(null, [inners]);
        var listInners = typeof(Enumerable).GetMethod(nameof(Enumerable.ToList))!
                                .MakeGenericMethod(innerType)
                                .Invoke(null, [typedInners])!;

        var returnTypeMatcherFunc = (Type t) => t.IsGenericType && innerType.IsAssignableFrom(t.GetInnerTypes().First());
        var innersEnumerator = GetMethods(listInners.GetType(), returnTypeMatcherFunc).FirstOrDefault();
        if (innersEnumerator is null) return null;

        return () => innersEnumerator.ReflectionInfo.Invoke(listInners, Array.Empty<object>());
    }

    private IEnumerable<MethodDetail> GetMethods(Type type, Func<Type, bool> returnTypeMatcherFunc)
    {
        var typeDetail = type.GetTypeDetailInfo();
        var methods = GetMethods(typeDetail, returnTypeMatcherFunc);

        var baseDetails = typeDetail.BaseType; // Interesting that for the mock of AbstractList<> (in EnumerableTests.cs) so far it didn't have correctly the base methods
        while (baseDetails is not null)
        {
            methods = methods.Union(GetMethods(baseDetails, returnTypeMatcherFunc));

            baseDetails = baseDetails.BaseType;
        }

        return methods.OfType<MethodDetail>();

        static IEnumerable<MethodDetail> GetMethods(TypeDetailInfo typeDetailInfo, Func<Type, bool> returnTypeFunc)
            => typeDetailInfo.MethodDetails
                .Where(md => md.Name == nameof(System.Collections.IEnumerable.GetEnumerator)
                                && returnTypeFunc(md.ReturnType))
                .Union(typeDetailInfo.ExplicitMethodDetails
                            .Where(md => md.Name == nameof(System.Collections.IEnumerable.GetEnumerator)
                                && returnTypeFunc(md.ReturnType)));
    }

    protected virtual IEnumerable<object> GetRepeatedInnerSpecimens(IRequestWithType originalRequest, Type innerType, ISpecimenContext context)
    {
        for (int i = 0; i < Repeat; i++)
        {
            var newRequest = new ListItemRequest(innerType, originalRequest, i);
            var inner = context.Resolve(newRequest);

            yield return inner; // Return first even on nospeciman otherwise we won't detect it
            if (inner is NoSpecimen || inner is OmitSpecimen) yield break; // optimization
        }
    }
}
