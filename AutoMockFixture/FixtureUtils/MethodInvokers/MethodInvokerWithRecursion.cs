using AutoMockFixture.FixtureUtils.MethodQueries;
using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.HelperRequests.AutoMock;
using AutoMockFixture.FixtureUtils.Requests.HelperRequests.NonAutoMock;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;

namespace AutoMockFixture.FixtureUtils.MethodInvokers;

internal class MethodInvokerWithRecursion : ISpecimenBuilder
{
    public MethodInvokerWithRecursion(IMethodQuery query, IAutoMockHelpers autoMockHelpers)
    {
        this.Query = query ?? throw new ArgumentNullException(nameof(query));
        this.AutoMockHelpers = autoMockHelpers ?? throw new ArgumentNullException(nameof(autoMockHelpers));
    }

    public IMethodQuery Query { get; }
    internal IAutoMockHelpers AutoMockHelpers { get; }

    public object Create(object request, ISpecimenContext context)
    {
        if (context is null) throw new ArgumentNullException(nameof(context));

        var requestedType = GetRequestedType(request);
        if(requestedType is null || requestedType.IsInterface
                || requestedType.IsAbstract || requestedType.IsGenericTypeDefinition || AutoMockHelpers.IsAutoMock(requestedType)) return new NoSpecimen();

        var methods = Query.SelectMethods(requestedType);

        object? unformattedObject = null;
        var recursionContext = context as RecursionContext;

        if (recursionContext is not null && methods.Any(m => m is CustomConstructorMethod))
        {
            // Not doing this in a special builder, to ensure that no matter what happens we do it on recursion
            if (recursionContext.BuilderCache.ContainsKey(requestedType))
                return recursionContext.BuilderCache[requestedType]; // We are in recursion

            methods = methods.Where(m => m is CustomConstructorMethod);

            Logger.LogInfo("In ctor arg - creating new");
            Logger.LogInfo("In ctor arg - context cache now contains: " + string.Join(",", recursionContext.BuilderCache.Keys.Select(k => k.FullName)));

            unformattedObject = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(requestedType);
            recursionContext.BuilderCache[requestedType] = unformattedObject;
        }

        try
        {
            foreach (var ci in methods)
            {
                var paramValues = ci.Parameters
                        .Select(pi => ResolveParamater(request, requestedType, pi, context))
                         .ToList();

                if (paramValues.All(IsValueValid))
                {
                    if(unformattedObject is null) return ci.Invoke(paramValues.ToArray());

                    if (ci is not CustomConstructorMethod cm) throw new Exception("Should not arrive here");

                    cm.Invoke(paramValues.ToArray(), unformattedObject);

                    return unformattedObject;
                }
            }

            return new NoSpecimen();
        }
        finally
        {
            if (recursionContext is not null) recursionContext.BuilderCache.Remove(requestedType);
        }
    }

    protected virtual Type? GetRequestedType(object request)
        => request as Type ?? (request as IRequestWithType)?.Request;

    protected virtual object ResolveParamater(object request, Type declaringType,
                                                ParameterInfo pi, ISpecimenContext context)
    {
        object argRequest = (request, context) switch
        {
            (ITracker { StartTracker.MockDependencies: true } tracker, _) => new AutoMockConstructorArgumentRequest(declaringType, pi, tracker),
            (ITracker tracker, _) => new ConstructorArgumentRequest(declaringType, pi, tracker),
            (_, RecursionContext recursionContext) => new NonAutoMockRequest(pi.ParameterType, recursionContext.Fixture),
            (_,_) => pi,
        };

        return context.Resolve(argRequest);
    }


    protected virtual bool IsValueValid(object value) => !(value is NoSpecimen) && !(value is OmitSpecimen);
}
