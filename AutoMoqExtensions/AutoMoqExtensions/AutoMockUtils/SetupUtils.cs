using AutoMoqExtensions.Expressions;
using AutoMoqExtensions.VerifyInfo;
using Moq;
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
    public void SetupInternal(LambdaExpression originalExpression, Expression<Action<T>> expression, Times? times = null)
    {
        var method = basicExpression.GetMethod(originalExpression);
        SetupActionInternal(method, expression, times);
    }

    public void SetupActionInternal(MethodInfo method, Expression<Action<T>> expression, Times? times = null)
    {
        if (method.IsSpecialName) // Assumming property set
        {
            var compiled = expression.Compile();
            AutoMock.SetupSet(compiled);
            if (times.HasValue) AutoMock.VerifyList.Add(new VerifySetInfo<T>(compiled, times.Value));
        }

        AutoMock.Setup(expression);
        if (times.HasValue) AutoMock.VerifyList.Add(new VerifyActionInfo<T>(expression, times.Value));            
    }

    public void SetupInternal<TResult>(LambdaExpression originalExpression, Expression<Func<T, TResult>> expression, Times? times = null)
    {
        var method = basicExpression.GetMethod(originalExpression);
        SetupFuncInternal(method, expression, times);
    }
    public void SetupFuncFromLambda<TResult>(MethodInfo method, LambdaExpression expression, Times? times = null)
    {
        SetupFuncInternal(method, (Expression<Func<T, TResult>>)expression, times);
    }

    // Cannot use default parameters as null can be sometinmes a valid result
    public void SetupFuncInternal<TResult>(MethodInfo method, Expression<Func<T, TResult>> expression, Times? times = null)
    {
        if (method.IsSpecialName) // Assumming property get
        {
            AutoMock.SetupGet(expression);
            if (times.HasValue) AutoMock.VerifyList.Add(new VerifyGetInfo<T, TResult>(expression, times.Value));               
        }

        AutoMock.Setup(expression);
        if (times.HasValue) AutoMock.VerifyList.Add(new VerifyFuncInfo<T, TResult>(expression, times.Value));            
    }

    public void SetupFuncWithResult<TResult>(MethodInfo method, Expression<Func<T, TResult>> expression, TResult result, Times? times = null)
    {
        if (method.IsSpecialName) // Assumming property get
        {
            AutoMock.SetupGet(expression).Returns(result);
            if (times.HasValue) AutoMock.VerifyList.Add(new VerifyGetInfo<T, TResult>(expression, times.Value));
        }

        AutoMock.Setup(expression).Returns(result);
        if (times.HasValue) AutoMock.VerifyList.Add(new VerifyFuncInfo<T, TResult>(expression, times.Value));
    }
    

    public AutoMock<T> AutoMock { get; }

    public MethodInfo GetSetupFuncInternal(Type type)
        => this.GetType().GetMethod(nameof(SetupFuncFromLambda), AutoMoqExtensions.Extensions.TypeExtensions.AllBindings)
        .MakeGenericMethod(type);

    // Doing this way it because of issues with overload resolution
    public void SetupInternal<TAnon, TResult>(MethodInfo method, TAnon paramData, TResult result, Times? times) where TAnon : class
    {
        var paramTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
        var expr = basicExpression.GetExpression(method, paramData, paramTypes);

        SetupFuncWithResult<TResult>(method, (Expression<Func<T, TResult>>)expr, result, times);
    }

    // Doing this way it because of issues with overload resolution
    public void SetupInternal<TAnon>(MethodInfo method, TAnon paramData, Times? times) where TAnon : class
    {
        var paramTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
        var expr = basicExpression.GetExpression(method, paramData, paramTypes);

        if (method.ReturnType == typeof(void))
        {
            SetupActionInternal(method, (Expression<Action<T>>)expr, times);
        }
        else
        {
            GetSetupFuncInternal(method.ReturnType).Invoke(this, new object?[] { method, expr, times });

        }
    }
}
