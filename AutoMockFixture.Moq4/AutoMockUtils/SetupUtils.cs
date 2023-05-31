using AutoMockFixture.Moq4.Expressions;
using AutoMockFixture.Moq4.VerifyInfo;
using DotNetPowerExtensions.Reflection;
using Moq.Language.Flow;

namespace AutoMockFixture.Moq4.AutoMockUtils;

internal class SetupUtils<T> where T : class
{
    public SetupUtils(AutoMock<T> autoMock)
    {
        AutoMock = autoMock;
    }
    private readonly BasicExpressionBuilder<T> basicExpression = new();
    public MethodInfo GetMethod(string methodName) => typeof(T).GetMethod(methodName, BindingFlagsExtensions.AllBindings)
                                                      ?? typeof(T).GetProperty(methodName, BindingFlagsExtensions.AllBindings)?.GetMethod
                                                      ?? typeof(T).GetInterfaces().Select(i => i.GetMethod(methodName, BindingFlagsExtensions.AllBindings)).SingleOrDefault()
                                                      ?? typeof(T).GetInterfaces().Select(i => i.GetProperty(methodName, BindingFlagsExtensions.AllBindings)).SingleOrDefault()?.GetMethod
                                                      ?? throw new MissingMethodException(methodName);
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
        => GetType().GetMethod(nameof(SetupFuncFromLambda), BindingFlagsExtensions.AllBindings)
        .MakeGenericMethod(type);

    public MethodInfo GetCorrectMethod(MethodInfo method)
    {
        if (!method.IsExplicitImplementation()) return method;

        // Moq bug workaround
        // Moq has an issue setting up the explicit method but no issue setting up the original method, so let's swap it
        var typeDetailInfo = method.DeclaringType.GetTypeDetailInfo();
        var explicitMethod = typeDetailInfo.ExplicitMethodDetails.FirstOrDefault(m => m.ReflectionInfo.IsEqual(method))
                            ?? typeDetailInfo.ExplicitPropertyDetails.FirstOrDefault(m => m.GetMethod?.ReflectionInfo.IsEqual(method) == true).GetMethod
                            ?? typeDetailInfo.ExplicitPropertyDetails.FirstOrDefault(m => m.SetMethod?.ReflectionInfo.IsEqual(method) == true).SetMethod
                            // No need to check base private since we are dealing with the method declaring type...
                            ?? typeDetailInfo.ExplicitEventDetails.FirstOrDefault(m => m.AddMethod.ReflectionInfo.IsEqual(method) == true).AddMethod
                            ?? typeDetailInfo.ExplicitEventDetails.FirstOrDefault(m => m.RemoveMethod.ReflectionInfo.IsEqual(method) == true).RemoveMethod;

        var iface = explicitMethod.ExplicitInterface!;
        return iface.GetMethod(explicitMethod.Name, BindingFlagsExtensions.AllBindings);
    }

    // Doing this way it because of issues with overload resolution
    public IReturnsResult<T> SetupInternal<TAnon, TResult>(MethodInfo method, TAnon paramData, TResult result, Times? times) where TAnon : class
    {
        method = GetCorrectMethod(method);

        var paramTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
        var expr = basicExpression.GetExpression(method, paramData, paramTypes);

        return SetupFuncWithResult(method, (Expression<Func<T, TResult>>)expr, result, times);
    }

    // Doing this way it because of issues with overload resolution
    public void SetupInternal<TAnon>(MethodInfo method, TAnon paramData, Times? times, bool callbase = false) where TAnon : class
    {
        method = GetCorrectMethod(method);

        var paramTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
        var expr = basicExpression.GetExpression(method, paramData, paramTypes);

        if (method.ReturnType == typeof(void))
        {
            var setup = SetupActionInternal((Expression<Action<T>>)expr, times);
            if (callbase) setup.CallBase();
        }
        else
        {
            var setup = GetSetupFuncInternal(method.ReturnType).Invoke(this, new object?[] { method, expr, times });
            if (callbase) setup.GetType().GetMethod(nameof(global::Moq.Language.ICallBase.CallBase)).Invoke(setup, new object[] { });
        }
    }
}
