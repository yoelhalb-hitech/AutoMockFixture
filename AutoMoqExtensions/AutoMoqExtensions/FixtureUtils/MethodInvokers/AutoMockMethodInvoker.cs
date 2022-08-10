using AutoMoqExtensions.FixtureUtils.MethodQueries;
using AutoMoqExtensions.FixtureUtils.Requests.HelperRequests.AutoMock;
using AutoMoqExtensions.FixtureUtils.Requests.HelperRequests.NonAutoMock;
using AutoMoqExtensions.FixtureUtils.Requests.MainRequests;
using System.Reflection;

namespace AutoMoqExtensions.FixtureUtils.MethodInvokers;

internal class AutoMockMethodInvoker : ISpecimenBuilder
{    
    public AutoMockMethodInvoker(IMethodQuery query)
    {
        this.Query = query ?? throw new ArgumentNullException(nameof(query));
    }

    public IMethodQuery Query { get; }

    public object Create(object request, ISpecimenContext context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        if (request is not AutoMockDirectRequest mockRequest) return new NoSpecimen();


        Logger.LogInfo("In autmock ctor arg - type is " + mockRequest.Request.FullName);

        var recursionContext = context as RecursionContext;

        if (recursionContext is not null)
        {
            // Not doing this in a special builder, to ensure that no matter what happens we do it on recursion
            if (recursionContext.BuilderCache.ContainsKey(mockRequest.Request))
                return recursionContext.BuilderCache[mockRequest.Request]; // We are in recursion
        }

        Logger.LogInfo("In ctor arg - creating new");

        var mock = (IAutoMock)Activator.CreateInstance(mockRequest.Request);        

        if (mock.GetInnerType().IsDelegate() 
                || (mockRequest.MockShouldCallbase != true && mockRequest.StartTracker.MockShouldCallbase != true))
        {
            ((ISetCallBase)mock).ForceSetCallbase(false);
            
            mock.EnsureMocked();
            return mock;
        }

        if (recursionContext is not null)
        {
            recursionContext.BuilderCache[mockRequest.Request] = mock;

            Logger.LogInfo("In ctor arg - context cache now contains: " + string.Join(",", recursionContext.BuilderCache.Keys.Select(k => k.FullName)));
        }

        var methods = Query.SelectMethods(mockRequest.Request);

        // We need a default ctor for EnsureMocked()
        // When `CallBase = false` AutoMock will create us a default ctor
        // However we don't want to do it if there is a default ctor as in this case Automock will oevrride the default ctor
        if (methods.All(m => !m.Parameters.Any())) mock.CallBase = false;

        try
        {
            mock.EnsureMocked();

            // We have to do it before calling a constuctur, otherwise if the ctor is setting a virtual property with private setter it will not work
            ((ISetCallBase)mock).ForceSetCallbase(true);

            if (!methods.Any()) // Just in case...
            {
                return mock;
            }

            foreach (var ci in methods)
            {
                var paramValues = (from pi in ci.Parameters select ResolveParamater(mockRequest, pi, context)).ToList();

                if (paramValues.All(IsValueValid))
                {
                    if (ci is not CustomConstructorMethod cm) throw new Exception("Should not arrive here");

                    cm.Invoke(paramValues.ToArray(), mock.GetMocked());

                    return mock;
                }                       
            }

            return new NoSpecimen();
        }
        finally
        {
            if (recursionContext is not null) recursionContext.BuilderCache.Remove(mockRequest.Request);
        }
    }

    protected virtual object ResolveParamater(AutoMockDirectRequest mockRequest, ParameterInfo pi, ISpecimenContext context)
    {            
        var argsRequest = mockRequest.NoMockDependencies == true
                ? new ConstructorArgumentRequest(mockRequest.Request, pi, mockRequest)
                : new AutoMockConstructorArgumentRequest(mockRequest.Request, pi, mockRequest);

        Logger.LogInfo("\t\t\t\t\t\tBefore args: " + pi.Name);
        var result = context.Resolve(argsRequest);
        Logger.LogInfo("\t\t\t\t\t\tAfter args: " + result.GetType().FullName);

        return result;
    }

    protected virtual bool IsValueValid(object value) => !(value is NoSpecimen) && !(value is OmitSpecimen);
}
