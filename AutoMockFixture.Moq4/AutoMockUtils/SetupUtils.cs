using AutoMockFixture.Moq4.Expressions;
using AutoMockFixture.Moq4.VerifyInfo;
using SequelPay.DotNetPowerExtensions.Reflection;
using Moq.Language.Flow;
using System.Reflection;

namespace AutoMockFixture.Moq4.AutoMockUtils;

internal class SetupUtils<T> where T : class
{
    public SetupUtils(AutoMock<T> autoMock)
    {
        AutoMock = autoMock;
    }
    private readonly BasicExpressionBuilder<T> basicExpression = new();
    public MethodInfo GetMethod(string methodName)
    {
        if (typeof(T).GetAllMethodTrackingPaths().ContainsKey(methodName))
            return typeof(T).GetAllMethodTrackingPaths()[methodName].ReflectionInfo;

        if (typeof(T).GetAllPropertyTrackingPaths().ContainsKey(methodName))
            return typeof(T).GetAllPropertyTrackingPaths()[methodName].ReflectionInfo.GetMethod ?? throw new MissingMethodException(methodName);

        if (typeof(T).GetAllEventTrackingPaths().ContainsKey(methodName))
            return typeof(T).GetAllEventTrackingPaths()[methodName].ReflectionInfo.AddMethod!;

        var methodWithPrefix = "." + methodName;
        var possibleMatchingMethods = typeof(T).GetAllMethodTrackingPaths().Where(m => m.Key.EndsWith(methodWithPrefix));

        if (possibleMatchingMethods.Any() && !possibleMatchingMethods.Skip(1).Any())
                return possibleMatchingMethods.First().Value.ReflectionInfo;
        else if(possibleMatchingMethods.Any()) throw new AmbiguousMatchException($"Found multiple candidates `{string.Join(",",possibleMatchingMethods.Select(m => m.Key))}`");

        var possibleMatchingProperties = typeof(T).GetAllPropertyTrackingPaths().Where(m => m.Key.EndsWith(methodWithPrefix));

        if (possibleMatchingProperties.Any() && !possibleMatchingProperties.Skip(1).Any()
                                    && possibleMatchingProperties.First().Value.ReflectionInfo.GetMethod is not null)
                return possibleMatchingProperties.First().Value.ReflectionInfo.GetMethod!;
        else if(possibleMatchingProperties.Any()) throw new AmbiguousMatchException($"Found multiple candidates `{string.Join(",", possibleMatchingProperties.Select(m => m.Key))}`");

        var possibleMatchingEvents = typeof(T).GetAllEventTrackingPaths().Where(m => m.Key.EndsWith(methodWithPrefix));

        if (possibleMatchingEvents.Any() && !possibleMatchingEvents.Skip(1).Any())
                return possibleMatchingEvents.First().Value.ReflectionInfo.AddMethod!;
        else if (possibleMatchingEvents.Any()) throw new AmbiguousMatchException($"Found multiple candidates `{string.Join(",", possibleMatchingEvents.Select(m => m.Key))}`");


        throw new MissingMethodException(methodName);
    }

    public ISetup<T> SetupInternal(LambdaExpression originalExpression, Expression<Action<T>> expression, Times? times = null)
    {
        return SetupActionInternal(expression, times);
    }

    public ISetup<T> SetupActionInternal(Expression<Action<T>> expression, Times? times = null)
    {
        var setup = ((Mock<T>)AutoMock).Setup(expression);
        if (times.HasValue) AutoMock.VerifyList.Add(new VerifyActionInfo<T>(expression, times.Value));

        return setup;
    }

    public ISetup<T, TResult> SetupInternal<TResult>(LambdaExpression originalExpression, Expression<Func<T, TResult>> expression, Times? times = null)
    {
        var method = typeof(Delegate).IsAssignableFrom(typeof(T)) || originalExpression.Body.NodeType == ExpressionType.MemberAccess
                        ? null
                        : basicExpression.GetMethod(originalExpression);
        return SetupFuncInternal(method, expression, times);
    }
    public ISetup<T, TResult> SetupFuncFromLambda<TResult>(MethodInfo method, LambdaExpression expression, Times? times = null)
    {
        return SetupFuncInternal(method, (Expression<Func<T, TResult>>)expression, times);
    }

    // Cannot use default parameters as null can be sometinmes a valid result
    public ISetup<T, TResult> SetupFuncInternal<TResult>(MethodInfo? method, Expression<Func<T, TResult>> expression, Times? times = null)
    {
        if (method?.IsSpecialName == true) // Assumming property get
        {
            AutoMock.SetupGet(expression);
            if (times.HasValue) AutoMock.VerifyList.Add(new VerifyGetInfo<T, TResult>(expression, times.Value));
        }

        var setup = ((Mock<T>)AutoMock).Setup(expression);
        if (times.HasValue) AutoMock.VerifyList.Add(new VerifyFuncInfo<T, TResult>(expression, times.Value));

        return setup;
    }

    public IReturnsResult<T> SetupFuncWithResult<TResult>(MethodInfo method, Expression<Func<T, TResult>> expression, TResult result, Times? times = null)
    {
        if(!method.ReturnType.IsAssignableFrom(typeof(TResult)))
            throw new ArgumentException($"Provided return value does not match method return type of {method.ReturnType.Name}");

        if (method.IsSpecialName) // Assumming property get
        {
            AutoMock.SetupGet(expression).Returns(result);
            if (times.HasValue) AutoMock.VerifyList.Add(new VerifyGetInfo<T, TResult>(expression, times.Value));
        }

        var setup = ((Mock<T>)AutoMock).Setup(expression).Returns(result);
        if (times.HasValue) AutoMock.VerifyList.Add(new VerifyFuncInfo<T, TResult>(expression, times.Value));

        return setup;
    }


    public AutoMock<T> AutoMock { get; }

    public MethodInfo GetSetupFuncInternal(Type type)
        => GetType().GetMethod(nameof(SetupFuncFromLambda), BindingFlagsExtensions.AllBindings)!
        .MakeGenericMethod(type)!;

    public MethodInfo GetCorrectMethod(MethodInfo method)
    {
        if (!method.IsExplicitImplementation()) return method;

        // Moq bug workaround
        // Moq has an issue setting up the explicit method but no issue setting up the original method, so let's swap it
        var typeDetailInfo = method.DeclaringType!.GetTypeDetailInfo();
        var explicitMethod = typeDetailInfo.ExplicitMethodDetails.FirstOrDefault(m => m.ReflectionInfo.IsEqual(method))
                            ?? typeDetailInfo.ExplicitPropertyDetails.FirstOrDefault(m => m.GetMethod?.ReflectionInfo.IsEqual(method) == true)?.GetMethod
                            ?? typeDetailInfo.ExplicitPropertyDetails.FirstOrDefault(m => m.SetMethod?.ReflectionInfo.IsEqual(method) == true)?.SetMethod
                            // No need to check base private since we are dealing with the method declaring type...
                            ?? typeDetailInfo.ExplicitEventDetails.FirstOrDefault(m => m.AddMethod.ReflectionInfo.IsEqual(method) == true)?.AddMethod
                            ?? typeDetailInfo.ExplicitEventDetails.FirstOrDefault(m => m.RemoveMethod.ReflectionInfo.IsEqual(method) == true)?.RemoveMethod;

        var iface = explicitMethod!.ExplicitInterface!;
        return iface.GetMethod(explicitMethod.Name, BindingFlagsExtensions.AllBindings)!;
    }

    // Doing this way it because of issues with overload resolution
    public IReturnsResult<T> SetupInternal<TAnon, TResult>(MethodInfo method, TAnon paramData, TResult result, Times? times) where TAnon : class
    {
        method = GetCorrectMethod(method);

        if (!method.ReturnType.IsAssignableFrom(typeof(TResult)))
            throw new ArgumentException($"Provided return value does not match method return type of {method.ReturnType.Name}");

        var paramTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
        var expr = basicExpression.GetExpression(method, paramData, paramTypes);

        return SetupFuncWithResult(method, (Expression<Func<T, TResult>>)expr, result, times);
    }

    // Doing this way it because of issues with overload resolution
    public void SetupInternal<TAnon>(MethodInfo method, TAnon paramData, Times? times, bool callBase = false) where TAnon : class
    {
        method = GetCorrectMethod(method);

        var paramTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
        var expr = basicExpression.GetExpression(method, paramData, paramTypes);

        if (method.ReturnType == typeof(void))
        {
            var setup = SetupActionInternal((Expression<Action<T>>)expr, times);
            if (callBase) setup.CallBase();
        }
        else
        {
            var setup = GetSetupFuncInternal(method.ReturnType)!.Invoke(this, new object?[] { method, expr, times })!;
            if (callBase) setup.GetType().GetMethod(nameof(global::Moq.Language.ICallBase.CallBase))!.Invoke(setup, new object[] { });
        }
    }
}
