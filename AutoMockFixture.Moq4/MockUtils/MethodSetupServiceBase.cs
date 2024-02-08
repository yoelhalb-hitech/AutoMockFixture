using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.HelperRequests.AutoMock;
using AutoMockFixture.FixtureUtils.Requests.HelperRequests.NonAutoMock;
using DotNetPowerExtensions.Reflection;
using DotNetPowerExtensions.Reflection.Models;

namespace AutoMockFixture.Moq4.MockUtils;

internal abstract class MethodSetupServiceBase : ISetupService
{
    protected readonly IAutoMock mock;
    protected readonly MethodDetail method;
    protected readonly Type mockedType;
    protected readonly ISpecimenContext context;
    protected readonly ITracker? tracker;
    protected readonly string trackingPath;
    protected readonly bool noMockDependencies;

    public MethodSetupServiceBase(IAutoMock mock, MethodDetail method, ISpecimenContext context, string trackingPath)
    {
        this.mock = mock;
        this.method = method;
        this.context = context;
        this.trackingPath = trackingPath;

        // Don't do mock.GetMocked().GetType() as it has additional properties etc.
        this.mockedType = method.ExplicitInterface ?? mock.GetInnerType();
        this.tracker = mock.Tracker;

        this.noMockDependencies = !mock.Tracker?.StartTracker.MockDependencies ?? false;
    }

    static MethodInfo[] objectMethods = typeof(object).GetMethods();

    public void Setup()
    {
        var returnType = method.ReturnType;
        var methodInvocationLambda = new ExpressionGenerator(mockedType, method, context, tracker)
                                                    .MakeMethodInvocationLambda();

        if (methodInvocationLambda is null) return;

        if (objectMethods.Any(m => m.IsSignatureEqual(method.ReflectionInfo))) // Overriding .Equals etc. can cause problems, note that we cannot base on the DelcaringType if it is overriden
        {
            SetupHelpers.SetupCallBaseMethod(mockedType, returnType, mock, methodInvocationLambda);
            return;
        }
        else if (method.ReflectionInfo.IsVoid())
        {
            SetupHelpers.SetupVoidMethod(mockedType, mock, methodInvocationLambda);
            return;
        }

        Logger.LogInfo("\t\t\tBefore return: " + method.ReturnType.Name);
        // We have to use InvocationFunc for generic
        //  but even for non generic it's better for perfomance not to generate the object until we need it

        if (method.ReturnType.ContainsGenericParameters)
        {
            var genericArgs = returnType.GetGenericArguments().Select(a => MatcherGenerator.GetGenericMatcher(a)).ToArray();
            var returnTypeToUse = returnType.IsGenericParameter ? MatcherGenerator.GetGenericMatcher(returnType) : returnType.GetGenericTypeDefinition().MakeGenericType(genericArgs);

            SetupWithInvocationFunc(methodInvocationLambda, returnTypeToUse);
        }
        else
        {
            HandleNonGenericFunction(methodInvocationLambda, returnType);
        }
    }

    protected abstract void HandleNonGenericFunction(Expression methodInvocationLambda, Type returnType);

    protected void SetupWithInvocationFunc(Expression methodInvocationLambda, Type returnTypeToUse)
    {
        // TODO... we have to handle recursion when not using our recursion approach, RecursionGuard won't work for that...
        var invocationFunc = new InvocationFunc(HandleInvocationFunc);

        SetupHelpers.SetupMethodWithInvocationFunc(mockedType, returnTypeToUse, mock, methodInvocationLambda, invocationFunc);
    }

    protected object? HandleInvocationFuncWithSameResult(IInvocation invocation)
    {
        if (resultDict.ContainsKey(invocation.Method)) return resultDict[invocation.Method];

        lock (resultDict)
        {
            if (resultDict.ContainsKey(invocation.Method)) return resultDict[invocation.Method];
            var result = GenerateResult(invocation.Method);
            resultDict[invocation.Method] = result;

            Logger.LogInfo("Resolved type: " + (result?.GetType().FullName ?? "null"));
            return result;
        }
    }

    protected Dictionary<MethodInfo, object?> resultDict = new Dictionary<MethodInfo, object?>();
    protected object lockObject = new object(); // Not static as it is only local to the object


    protected abstract object? HandleInvocationFunc(IInvocation invocation);

    protected object? GenerateResult(MethodInfo method)
    {
        Logger.LogInfo("In generate result - Is generic definition: " + method.IsGenericMethodDefinition);

        // Can actually happen if this is an explicit interface implementation while there is another method on the class with the same signature
        // In this case the mock will register the method setup with the explicit name but here (when called via explicit) will arrive with the non explicit name
        //if (mock.MethodsNotSetup.ContainsKey(trackingPath))
        //        throw mock.MethodsNotSetup[trackingPath].Exception
        //        ?? new Exception("Method not setup but without an exception, shouldn't arrive here");

        Logger.LogInfo("\t\tResolving return: " + method.ReturnType.FullName);

        var request = noMockDependencies
                             ? new ReturnRequest(mockedType, method, method.ReturnType, tracker, trackingPath)
                             : new AutoMockReturnRequest(mockedType, method, method.ReturnType, tracker, trackingPath);
        Logger.LogInfo("\t\tResolving return for containing path: " + request.Path);

        try
        {
            var result = context.Resolve(request);

            if(method.ReturnType.ContainsGenericParameters)
            {
                if (mock.MethodsSetup.ContainsKey(trackingPath)) throw new Exception("Method was already setup");
                mock.MethodsSetup.Add(trackingPath, method);
            }

            return result;
        }
        catch (Exception ex)
        {
            if (mock.MethodsSetup.ContainsKey(trackingPath)) mock.MethodsSetup.Remove(trackingPath);
            mock.MethodsNotSetup[trackingPath] = new CannotSetupMethodException(CannotSetupMethodException.CannotSetupReason.Exception, ex);
            throw;
        }
    }
}
