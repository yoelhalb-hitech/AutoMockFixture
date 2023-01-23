using AutoMoqExtensions.Expressions;
using AutoMoqExtensions.VerifyInfo;
using Moq;
using Moq.Language.Flow;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoMoqExtensions.AutoMockUtils;

internal class SetupUtils<T> where T : class
{
    public SetupUtils(AutoMock<T> autoMock)
    {
        AutoMock = autoMock;
    }
    private readonly BasicExpressionBuilder<T> basicExpression = new();
    public MethodInfo GetMethod(string methodName) => typeof(T).GetMethod(methodName, AutoMoqExtensions.Extensions.TypeExtensions.AllBindings);
    public ISetup<T> SetupInternal(LambdaExpression originalExpression, Expression<Action<T>> expression, Times? times = null)
    {        
        return SetupActionInternal(expression, times);
    }

    public ISetup<T> SetupActionInternal(Expression<Action<T>> expression, Times? times = null)
    {
        var setup = AutoMock.Setup(expression);
        if (times.HasValue) AutoMock.VerifyList.Add(new VerifyActionInfo<T>(expression, times.Value));

        return setup;
    }

    public ISetup<T, TResult> SetupInternal<TResult>(LambdaExpression originalExpression, Expression<Func<T, TResult>> expression, Times? times = null)
    {
        var method = typeof(System.Delegate).IsAssignableFrom(typeof(T)) ? null : basicExpression.GetMethod(originalExpression);
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

        var setup = AutoMock.Setup(expression);
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

        var setup = AutoMock.Setup(expression).Returns(result);
        if (times.HasValue) AutoMock.VerifyList.Add(new VerifyFuncInfo<T, TResult>(expression, times.Value));

        return setup;
    }
    

    public AutoMock<T> AutoMock { get; }

    public MethodInfo GetSetupFuncInternal(Type type)
        => this.GetType().GetMethod(nameof(SetupFuncFromLambda), AutoMoqExtensions.Extensions.TypeExtensions.AllBindings)
        .MakeGenericMethod(type);

    // Doing this way it because of issues with overload resolution
    public IReturnsResult<T> SetupInternal<TAnon, TResult>(MethodInfo method, TAnon paramData, TResult result, Times? times) where TAnon : class
    {
        var paramTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
        var expr = basicExpression.GetExpression(method, paramData, paramTypes);

        return SetupFuncWithResult<TResult>(method, (Expression<Func<T, TResult>>)expr, result, times);
    }

    // Doing this way it because of issues with overload resolution
    public void SetupInternal<TAnon>(MethodInfo method, TAnon paramData, Times? times, bool callbase = false) where TAnon : class
    {
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
            if (callbase) setup.GetType().GetMethod(nameof(Moq.Language.ICallBase.CallBase)).Invoke(setup, new object[] { });
        }
    }
}
