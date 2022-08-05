﻿
using AutoMoqExtensions.FixtureUtils.Requests;
using AutoMoqExtensions.FixtureUtils.Requests.HelperRequests.AutoMock;
using AutoMoqExtensions.FixtureUtils.Requests.HelperRequests.NonAutoMock;
using AutoMoqExtensions.FixtureUtils.Requests.MainRequests;
using Moq;
using System.Reflection;

namespace AutoMoqExtensions.MockUtils;

internal class MethodSetupService
{
    private readonly IAutoMock mock;
    private readonly Type mockedType;
    private readonly MethodInfo method;
    private readonly ISpecimenContext context;
    private readonly ITracker? tracker;
    private readonly bool noMockDependencies;

    public MethodSetupService(IAutoMock mock, Type mockedType, MethodInfo method, ISpecimenContext context)
    {
        this.mock = mock;
        this.mockedType = mockedType;
        this.method = method;
        this.context = context;
        this.tracker = mock.Tracker;
        this.noMockDependencies = mock.Tracker is AutoMockDirectRequest directRequest
                                                            && directRequest.NoMockDependencies == true;
    }

    public void Setup()
    {
        var returnType = method.ReturnType;
        var methodInvocationLambda = new ExpressionGenerator(mockedType, method, context, tracker)
                                                    .MakeMethodInvocationLambda();

        if (methodInvocationLambda is null) return;

        // TODO... this is all based on the assumption that the method can return the same object everytime
        if (method.DeclaringType == typeof(object)) // Overriding .Equals etc. can cause problems
        {
            SetupHelpers.SetupCallbaseMethod(mockedType, returnType, mock, methodInvocationLambda);              
        }
        else if (method.IsVoid())
        {
            SetupHelpers.SetupVoidMethod(mockedType, mock, methodInvocationLambda);                
        }
        else if (!method.ReturnType.ContainsGenericParameters)
        {
            Logger.LogInfo("\t\t\tBefore return: " + method.ReturnType.Name);
            var request = noMockDependencies
                                    ? new ReturnRequest(mockedType, method, method.ReturnType, tracker)
                                    : new AutoMockReturnRequest(mockedType, method, method.ReturnType, tracker);
            var returnValue = context.Resolve(request);

            Logger.LogInfo("\t\t\tResolved return: " + returnValue.GetType().Name);                
            SetupHelpers.SetupMethodWithResult(mockedType, returnType, mock, methodInvocationLambda, returnValue);            
        }
        else
        {
            Logger.LogInfo("\t\t\tBefore return: " + method.ReturnType.Name);
            var invocationFunc = new InvocationFunc(HandleInvocationFunc); // TODO... we have to handle recursion, RecursionGuard won't work for that...           

            var genericArgs = returnType.GetGenericArguments().Select(a => MatcherGenerator.GetGenericMatcher(a)).ToArray();
            var newReturnType = returnType.IsGenericParameter ? MatcherGenerator.GetGenericMatcher(returnType) : returnType.GetGenericTypeDefinition().MakeGenericType(genericArgs);

            SetupHelpers.SetupMethodWithGenericResult(mockedType, newReturnType, mock, methodInvocationLambda, invocationFunc);
        }
    }

    private Dictionary<MethodInfo, object?> resultDict = new Dictionary<MethodInfo, object?>();
    private object lockObject = new object(); // Not static as it is only local to the object

    private object? HandleInvocationFunc(IInvocation invocation)
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

    private object? GenerateResult(MethodInfo method)
    {
        var trackingPath = method.GetTrackingPath();

        Logger.LogInfo("In generate result - Is generic definition: " + method.IsGenericMethodDefinition);

        if (mock.MethodsNotSetup.ContainsKey(trackingPath))
                throw mock.MethodsNotSetup[trackingPath].Exception 
                ?? new Exception("Method not setup but without an exception, shouldn't arrive here");

        Logger.LogInfo("\t\tResolving return: " + method.ReturnType.FullName);
        
        var request = noMockDependencies 
                             ? new ReturnRequest(mockedType, method, method.ReturnType, tracker)
                             : new AutoMockReturnRequest(mockedType, method, method.ReturnType, tracker);
        Logger.LogInfo("\t\tResolving return for containing path: " + request.Path);

        try
        {
            var result = context.Resolve(request);        

            if (mock.MethodsSetup.ContainsKey(trackingPath)) throw new Exception("Method was already setup");
            mock.MethodsSetup.Add(trackingPath, method);

            return result;
        }
        catch (Exception ex)
        {
            mock.MethodsNotSetup[trackingPath] = new CannotSetupMethodException(CannotSetupMethodException.CannotSetupReason.Exception, ex);
            throw;
        }            
    }
}
