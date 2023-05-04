using AutoMockFixture.FixtureUtils.MethodQueries;
using AutoMockFixture.FixtureUtils.Requests.HelperRequests.AutoMock;
using AutoMockFixture.FixtureUtils.Requests.HelperRequests.NonAutoMock;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;
using DotNetPowerExtensions.Reflection;
using System.Reflection;

namespace AutoMockFixture.FixtureUtils.MethodInvokers;

internal class AutoMockMethodInvoker : ISpecimenBuilder
{
    public AutoMockMethodInvoker(IMethodQuery query, ISpecimenCommand autoMockInitCommand)
    {
        this.Query = query ?? throw new ArgumentNullException(nameof(query));
        this.AutoMockInitCommand = autoMockInitCommand ?? throw new ArgumentNullException(nameof(autoMockInitCommand));
    }

    public IMethodQuery Query { get; }
    public ISpecimenCommand AutoMockInitCommand { get; }

    private bool ShouldCallBase(IAutoMock mock, AutoMockDirectRequest mockRequest) => !mock.GetInnerType().IsDelegate()
                                && (mockRequest.MockShouldCallbase == true || mockRequest.StartTracker.MockShouldCallbase == true);

    public object Create(object request, ISpecimenContext context)
    {
        if (context is null) throw new ArgumentNullException(nameof(context));

        if (request is not AutoMockDirectRequest mockRequest) return new NoSpecimen();

        Logger.LogInfo("In autmock ctor arg - type is " + mockRequest.Request.FullName);

        // Not doing this in a special builder, to ensure that no matter what happens we do it on recursion
        if (context is RecursionContext recursionContext && recursionContext.BuilderCache.ContainsKey(mockRequest.Request))
            return recursionContext.BuilderCache[mockRequest.Request]; // We are in recursion


        Logger.LogInfo("In ctor arg - creating new");

        var obj = Activator.CreateInstance(mockRequest.Request);
        if (obj is null || obj is not IAutoMock mock) return new NoSpecimen();

        AutoMockInitCommand.Execute(obj, context);

        if (!ShouldCallBase(mock, mockRequest))
        {
            CreateInnerObject(mock, new IMethod[] { }, false);
            return mock;
        }

        return HandleCtorWithRecursion(context, mockRequest, mock);
    }

    private object HandleCtorWithRecursion(ISpecimenContext context, AutoMockDirectRequest mockRequest, IAutoMock mock)
    {
        var recursionContext = context as RecursionContext;

        if (recursionContext is not null)
        {
            recursionContext.BuilderCache[mockRequest.Request] = mock;

            Logger.LogInfo("In ctor arg - context cache now contains: " + string.Join(",", recursionContext.BuilderCache.Keys.Select(k => k.FullName)));
        }

        try
        {
            return HandleCtors(context, mockRequest, mock);
        }
        finally
        {
            if (recursionContext is not null) recursionContext.BuilderCache.Remove(mockRequest.Request);
        }
    }

    private object CreateInnerObject(IAutoMock mock, IEnumerable<IMethod> methods, bool callBase)
    {
        mock.CallBase = callBase;

        // We need a default ctor for EnsureMocked()
        // When `CallBase = false` AutoMock should create us a default ctor
        // However we don't want to do it if there is a default ctor as in this case Automock will override the default ctor
        if (methods.All(m => m.Parameters.Any())) mock.CallBase = false;

        try
        {
            mock.EnsureMocked(); // Automatically calls the default ctor
        }
        finally
        {
            // We have to do it before calling a constructor, otherwise if the ctor is setting a virtual property with private setter it will not work
            ((ISetCallBase)mock).ForceSetCallbase(callBase); // Remember we only call non default ctors if Callbase is true
        }

        return mock.GetMocked();
    }

    private object HandleCtors(ISpecimenContext context, AutoMockDirectRequest mockRequest, IAutoMock mock)
    {
        var methods = Query.SelectMethods(mockRequest.Request);

        var inner = CreateInnerObject(mock, methods, true);

        if (!methods.Any(m => m.Parameters.Any())) return mock;

        foreach (var ci in methods)
        {
            if (ci is not CustomConstructorMethod cm) throw new Exception("Should not arrive here");

            if(HandleCtor(cm, mockRequest, inner, context)) return mock;
        }

        return new NoSpecimen();
    }

    private bool HandleCtor(CustomConstructorMethod cm, AutoMockDirectRequest mockRequest, object inner, ISpecimenContext context)
    {
        var paramValues = (from pi in cm.Parameters select ResolveParamater(mockRequest, pi, context)).ToList();

        if (!paramValues.All(IsValueValid)) return false;

        cm.Invoke(paramValues.ToArray(), inner);

        return true;
    }

    protected virtual object ResolveParamater(AutoMockDirectRequest mockRequest, ParameterInfo pi, ISpecimenContext context)
    {
        var argsRequest = !mockRequest.MockDependencies
                ? new ConstructorArgumentRequest(mockRequest.Request, pi, mockRequest)
                : new AutoMockConstructorArgumentRequest(mockRequest.Request, pi, mockRequest);

        Logger.LogInfo("\t\t\t\t\t\tBefore args: " + pi.Name);
        var result = context.Resolve(argsRequest);
        Logger.LogInfo("\t\t\t\t\t\tAfter args: " + result.GetType().FullName);

        return result;
    }

    protected virtual bool IsValueValid(object value) => !(value is NoSpecimen) && !(value is OmitSpecimen);
}
