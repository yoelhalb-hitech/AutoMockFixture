using AutoFixture.AutoMoq;
using AutoMockFixture.Moq4.Expressions;
using static AutoMockFixture.Moq4.Expressions.Visitors;

namespace AutoMockFixture.Moq4;

public partial class AutoMock<T>
{
    #region Utils

    private readonly ActionExpressionBuilder<T> actionExpression = new();
    private readonly FuncExpressionBuilder<T> funcExpression = new();

    private AutoMock<T> SetupInternal(LambdaExpression originalExpression, Expression<Action<T>> expression, Times? times = null)
    {
        setupUtils.SetupInternal(originalExpression, expression, times);
        return this;
    }
    private AutoMock<T> SetupInternal<TResult>(LambdaExpression originalExpression, Expression<Func<T, TResult>> expression, Times? times = null)
    {
        setupUtils.SetupInternal(originalExpression, expression, times);
        return this;
    }

    private AutoMock<T> SetupInternal<TAnon, TResult>(MethodInfo method, TAnon paramData, TResult result, Times? times) where TAnon : class
    {
        setupUtils.SetupInternal(method, paramData, result, times);
        return this;
    }

    private AutoMock<T> SetupInternal<TAnon>(MethodInfo method, TAnon paramData, Times? times) where TAnon : class
    {
        setupUtils.SetupInternal(method, paramData, times);
        return this;
    }

    private MethodInfo GetMethodInternal(string methodName) => setupUtils.GetMethod(methodName);

    #endregion

    #region MethodInfo
    public AutoMock<T> Setup(MethodInfo method) => Setup(method, new { });
    public AutoMock<T> Setup(MethodInfo method, Times times) => Setup(method, new { }, times);
    public AutoMock<T> Setup<TAnon>(MethodInfo method, TAnon paramData) where TAnon : class
        => SetupInternal(method, paramData, null);

    // Doing TAnon : class to avoid overload resolution issues
    public AutoMock<T> Setup<TAnon>(MethodInfo method, TAnon paramData, Times times) where TAnon : class
        => SetupInternal(method, paramData, times);

    public AutoMock<T> Setup<TAnon, TResult>(MethodInfo method, TAnon paramData, TResult result) where TAnon : class
         => SetupInternal(method, paramData, result, null);
    // Doing TAnon : class to avoid overload resolution issues
    public AutoMock<T> Setup<TAnon, TResult>(MethodInfo method, TAnon paramData, TResult result, Times times) where TAnon : class
            => SetupInternal(method, paramData, result, times);

    #endregion

    #region string

    public AutoMock<T> Setup(string methodName) => SetupInternal(GetMethodInternal(methodName), new { }, null);
    public AutoMock<T> Setup(string methodName, Times times) => SetupInternal(GetMethodInternal(methodName), new { }, times);

    // Doing TAnon : class to avoid overload resolution issues
    public AutoMock<T> Setup<TAnon>(string methodName, TAnon paramData) where TAnon : class
        => SetupInternal(GetMethodInternal(methodName), paramData, null);

    // Doing TAnon : class to avoid overload resolution issues
    public AutoMock<T> Setup<TAnon>(string methodName, TAnon paramData, Times times) where TAnon : class
        => SetupInternal(GetMethodInternal(methodName), paramData, times);

    // Doing TAnon : class to avoid overload resolution issues


    public AutoMock<T> Setup<TAnon, TResult>(string methodName, TAnon paramData, TResult result) where TAnon : class
            => SetupInternal(GetMethodInternal(methodName), paramData, result, null);
    public AutoMock<T> Setup<TAnon, TResult>(string methodName, TAnon paramData, TResult result, Times times) where TAnon : class
            => SetupInternal(GetMethodInternal(methodName), paramData, result, times);

    #endregion

    /// <summary>
    /// Sets up with Moq and optionaly adds verification
    /// NOTE: This changes all default values to <see cref="It.IsAny{TValue}"/>,
    /// to match the default values use <see cref="It.Is{TValue}(Expression{Func{TValue, bool}})"/>
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    public new Moq.Language.Flow.ISetup<T> Setup(Expression<Action<T>> expression)
    {
        var modifiedExpr = (LambdaExpression)new TopVisitor().Visit(expression);
        return setupUtils.SetupInternal(modifiedExpr, (Expression<Action<T>>)modifiedExpr, null);
    }

    /// <summary>
    /// Sets up with Moq and optionaly adds verification
    /// NOTE: This changes all default values to <see cref="It.IsAny{TValue}"/>,
    /// to match the default values use <see cref="It.Is{TValue}(Expression{Func{TValue, bool}})"/>
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="times"></param>
    /// <returns></returns>
    public AutoMock<T> Setup(Expression<Action<T>> expression, Times times)
    {
        var modifiedExpr = (LambdaExpression)new TopVisitor().Visit(expression);
        return SetupInternal(modifiedExpr, (Expression<Action<T>>)modifiedExpr, times);
    }

    /// <summary>
    /// Sets up with Moq and optionaly adds verification
    /// NOTE: This changes all default values to <see cref="It.IsAny{TValue}"/>,
    /// to match the default values use <see cref="It.Is{TValue}(Expression{Func{TValue, bool}})"/>
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="setupAction"></param>
    /// <param name="times"></param>
    /// <returns></returns>
    public AutoMock<T> Setup(Expression<Action<T>> expression, Action<Moq.Language.Flow.ISetup<T>> setupAction, Times? times = null)
    {
        var modifiedExpr = (LambdaExpression)new TopVisitor().Visit(expression);
        var setup = setupUtils.SetupInternal(modifiedExpr, (Expression<Action<T>>)modifiedExpr, times);

        setupAction(setup);
        return this;
    }

    /// <summary>
    /// Sets up with Moq and optionaly adds verification
    /// NOTE: This changes all default values to <see cref="It.IsAny{TValue}"/>,
    /// to match the default values use <see cref="It.Is{TValue}(Expression{Func{TValue, bool}})"/>
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="expression"></param>
    /// <param name="times"></param>
    /// <returns></returns>
    public new Moq.Language.Flow.ISetup<T, TResult> Setup<TResult>(Expression<Func<T, TResult>> expression)
    {
        var modifiedExpr = (LambdaExpression)new TopVisitor().Visit(expression);
        return setupUtils.SetupInternal(modifiedExpr, (Expression<Func<T, TResult>>)modifiedExpr, null);
    }

    /// <summary>
    /// Sets up with Moq and optionaly adds verification
    /// NOTE: This changes all default values to <see cref="It.IsAny{TValue}"/>,
    /// to match the default values use <see cref="It.Is{TValue}(Expression{Func{TValue, bool}})"/>
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="expression"></param>
    /// <param name="times"></param>
    /// <returns></returns>
    public AutoMock<T> Setup<TResult>(Expression<Func<T, TResult>> expression, Times times)
    {
        var modifiedExpr = (LambdaExpression)new TopVisitor().Visit(expression);
        return SetupInternal(modifiedExpr, (Expression<Func<T, TResult>>)modifiedExpr, times);
    }

    /// <summary>
    /// Sets up with Moq and optionaly adds verification
    /// NOTE: This changes all default values to <see cref="It.IsAny{TValue}"/>,
    /// to match the default values use <see cref="It.Is{TValue}(Expression{Func{TValue, bool}})"/>
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="expression"></param>
    /// <param name="result"></param>
    /// <param name="times"></param>
    /// <returns></returns>
    public AutoMock<T> Setup<TResult>(Expression<Func<T, TResult>> expression, TResult result, Times? times = null)
    {
        var modifiedExpr = (LambdaExpression)new TopVisitor().Visit(expression);
        var setup = setupUtils.SetupInternal(modifiedExpr, (Expression<Func<T, TResult>>)modifiedExpr, times);
        setup.Returns(result);

        return this;
    }

    /// <summary>
    /// Sets up with Moq and optionaly adds verification
    /// NOTE: This changes all default values to <see cref="It.IsAny{TValue}"/>,
    /// to match the default values use <see cref="It.Is{TValue}(Expression{Func{TValue, bool}})"/>
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="expression"></param>
    /// <param name="fixture"></param>
    /// <param name="times"></param>
    /// <returns></returns>
    public AutoMock<T> Setup<TResult>(Expression<Func<T, TResult>> expression, IFixture fixture, Times? times = null)
    {
        var modifiedExpr = (LambdaExpression)new TopVisitor().Visit(expression);
        var setup = setupUtils.SetupInternal(modifiedExpr, (Expression<Func<T, TResult>>)modifiedExpr, times);
        setup.ReturnsUsingFixture(fixture);

        return this;
    }

    /// <summary>
    /// Sets up with Moq and optionaly adds verification
    /// NOTE: This changes all default values to <see cref="It.IsAny{TValue}"/>,
    /// to match the default values use <see cref="It.Is{TValue}(Expression{Func{TValue, bool}})"/>
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="expression"></param>
    /// <param name="setupAction"></param>
    /// <param name="times"></param>
    /// <returns></returns>
    public AutoMock<T> Setup<TResult>(Expression<Func<T, TResult>> expression, Action<Moq.Language.Flow.ISetup<T, TResult>> setupAction, Times? times = null)
    {
        var modifiedExpr = (LambdaExpression)new TopVisitor().Visit(expression);
        var setup = setupUtils.SetupInternal(modifiedExpr, (Expression<Func<T, TResult>>)modifiedExpr, times);

        setupAction(setup);
        return this;
    }

    #region SetupAction

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the overload that takes a string/nameof")]
    public AutoMock<T> Setup(Expression<Func<T, Action>> expression, Times? times = null)
    {
        var expr = actionExpression.GetExpression(expression, new { });
        return SetupInternal(expression, expr, times);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the overload that takes a string/nameof")]
    public AutoMock<T> Setup<TParam>(Expression<Func<T, Action<TParam>>> expression, Times? times = null)
    {
        var expr = actionExpression.GetExpression(expression, new { });
        return SetupInternal(expression, expr, times);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the overload that takes a string/nameof")]
    public AutoMock<T> Setup<TParam1, TParam2>(Expression<Func<T, Action<TParam1, TParam2>>> expression, Times? times = null)
    {
        var expr = actionExpression.GetExpression(expression, new { });
        return SetupInternal(expression, expr, times);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the overload that takes a string/nameof")]
    public AutoMock<T> Setup<TParam1, TParam2, TAnon>(
        Expression<Func<T, Action<TParam1, TParam2>>> expression, TAnon paramData, Times? times = null)
        where TAnon : class // Doing TAnon : class to avoid overload resolution issues
    {
        var expr = actionExpression.GetExpression(expression, paramData);
        return SetupInternal(expression, expr, times);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the overload that takes a string/nameof")]
    public AutoMock<T> Setup<TParam1, TParam2, TParam3>(
                Expression<Func<T, Action<TParam1, TParam2, TParam3>>> expression, Times? times = null)
    {
        var expr = actionExpression.GetExpression(expression, new { });
        return SetupInternal(expression, expr, times);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the overload that takes a string/nameof")]
    public AutoMock<T> Setup<TParam1, TParam2, TParam3, TAnon>(
        Expression<Func<T, Action<TParam1, TParam2, TParam3>>> expression, TAnon paramData, Times? times = null)
        where TAnon : class // Doing TAnon : class to avoid overload resolution issues
    {
        var expr = actionExpression.GetExpression(expression, paramData);
        return SetupInternal(expression, expr, times);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the overload that takes a string/nameof")]
    public AutoMock<T> Setup<TParam1, TParam2, TParam3, TParam4>(
        Expression<Func<T, Action<TParam1, TParam2, TParam3, TParam4>>> expression, Times? times = null)
    {
        var expr = actionExpression.GetExpression(expression, new { });
        return SetupInternal(expression, expr, times);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the overload that takes a string/nameof")]
    public AutoMock<T> Setup<TParam1, TParam2, TParam3, TParam4, TAnon>(
        Expression<Func<T, Action<TParam1, TParam2, TParam3, TParam4>>> expression, TAnon paramData, Times? times = null)
        where TAnon : class // Doing TAnon : class to avoid overload resolution issues
    {
        var expr = actionExpression.GetExpression(expression, paramData);
        return SetupInternal(expression, expr, times);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the overload that takes a string/nameof")]
    public AutoMock<T> Setup<TParam1, TParam2, TParam3, TParam4, TParam5>(
        Expression<Func<T, Action<TParam1, TParam2, TParam3, TParam4, TParam5>>> expression, Times? times = null)
    {
        var expr = actionExpression.GetExpression(expression, new { });
        return SetupInternal(expression, expr, times);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the overload that takes a string/nameof")]
    public AutoMock<T> Setup<TParam1, TParam2, TParam3, TParam4, TParam5, TAnon>(
        Expression<Func<T, Action<TParam1, TParam2, TParam3, TParam4, TParam5>>> expression,
        TAnon paramData, Times? times = null)
        where TAnon : class // Doing TAnon : class to avoid overload resolution issues
    {
        var expr = actionExpression.GetExpression(expression, paramData);
        return SetupInternal(expression, expr, times);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the overload that takes a string/nameof")]
    public AutoMock<T> Setup<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>(
Expression<Func<T, Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>>> expression, Times? times = null)
    {
        var expr = actionExpression.GetExpression(expression, new { });
        return SetupInternal(expression, expr, times);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the overload that takes a string/nameof")]
    public AutoMock<T> Setup<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TAnon>(
        Expression<Func<T, Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>>> expression,
        TAnon paramData, Times? times = null)
        where TAnon : class // Doing TAnon : class to avoid overload resolution issues
    {
        var expr = actionExpression.GetExpression(expression, paramData);
        return SetupInternal(expression, expr, times);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the overload that takes a string/nameof")]
    public AutoMock<T> Setup<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7>(
Expression<Func<T, Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7>>> expression, Times? times = null)
    {
        var expr = actionExpression.GetExpression(expression, new { });
        return SetupInternal(expression, expr, times);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the overload that takes a string/nameof")]
    public AutoMock<T> Setup<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TAnon>(
        Expression<Func<T, Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7>>> expression,
        TAnon paramData, Times? times = null)
        where TAnon : class // Doing TAnon : class to avoid overload resolution issues
    {
        var expr = actionExpression.GetExpression(expression, paramData);
        return SetupInternal(expression, expr, times);
    }

    #endregion
    #region SetupFunc

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the overload that takes a string/nameof")]
    public AutoMock<T> Setup<TResult>(Expression<Func<T, Func<TResult>>> expression, Times? times = null)
    {
        var expr = funcExpression.GetExpression(expression, new { });
        return SetupInternal(expression, expr, times);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the overload that takes a string/nameof")]
    public AutoMock<T> Setup<TParam, TResult>(Expression<Func<T, Func<TParam, TResult>>> expression, Times? times = null)
    {
        var expr = funcExpression.GetExpression(expression, new { });
        return SetupInternal(expression, expr, times);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the overload that takes a string/nameof")]
    public AutoMock<T> Setup<TParam1, TParam2, TResult>(Expression<Func<T, Func<TParam1, TParam2, TResult>>> expression, Times? times = null)
    {
        var expr = funcExpression.GetExpression(expression, new { });
        return SetupInternal(expression, expr, times);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the overload that takes a string/nameof")]
    public AutoMock<T> Setup<TParam1, TParam2, TResult, TAnon>(
        Expression<Func<T, Func<TParam1, TParam2, TResult>>> expression, TAnon paramData, Times? times = null)
        where TAnon : class // Doing TAnon : class to avoid overload resolution issues
    {
        var expr = funcExpression.GetExpression(expression, paramData);
        return SetupInternal(expression, expr, times);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the overload that takes a string/nameof")]
    public AutoMock<T> Setup<TParam1, TParam2, TParam3, TResult>(
                Expression<Func<T, Func<TParam1, TParam2, TParam3, TResult>>> expression, Times? times = null)
    {
        var expr = funcExpression.GetExpression(expression, new { });
        return SetupInternal(expression, expr, times);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the overload that takes a string/nameof")]
    public AutoMock<T> Setup<TParam1, TParam2, TParam3, TResult, TAnon>(
        Expression<Func<T, Func<TParam1, TParam2, TParam3, TResult>>> expression, TAnon paramData, Times? times = null)
        where TAnon : class // Doing TAnon : class to avoid overload resolution issues
    {
        var expr = funcExpression.GetExpression(expression, paramData);
        return SetupInternal(expression, expr, times);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the overload that takes a string/nameof")]
    public AutoMock<T> Setup<TParam1, TParam2, TParam3, TParam4, TResult>(
        Expression<Func<T, Func<TParam1, TParam2, TParam3, TParam4, TResult>>> expression, Times? times = null)
    {
        var expr = funcExpression.GetExpression(expression, new { });
        return SetupInternal(expression, expr, times);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the overload that takes a string/nameof")]
    public AutoMock<T> Setup<TParam1, TParam2, TParam3, TParam4, TResult, TAnon>(
        Expression<Func<T, Func<TParam1, TParam2, TParam3, TParam4, TResult>>> expression, TAnon paramData, Times? times = null)
        where TAnon : class // Doing TAnon : class to avoid overload resolution issues
    {
        var expr = funcExpression.GetExpression(expression, paramData);
        return SetupInternal(expression, expr, times);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the overload that takes a string/nameof")]
    public AutoMock<T> Setup<TParam1, TParam2, TParam3, TParam4, TResult, TParam5>(
        Expression<Func<T, Func<TParam1, TParam2, TParam3, TParam4, TResult, TParam5>>> expression, Times? times = null)
    {
        var expr = funcExpression.GetExpression(expression, new { });
        return SetupInternal(expression, expr, times);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the overload that takes a string/nameof")]
    public AutoMock<T> Setup<TParam1, TParam2, TParam3, TParam4, TParam5, TResult, TAnon>(
        Expression<Func<T, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TResult>>> expression,
        TAnon paramData, Times? times = null)
        where TAnon : class // Doing TAnon : class to avoid overload resolution issues
    {
        var expr = funcExpression.GetExpression(expression, paramData);
        return SetupInternal(expression, expr, times);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the overload that takes a string/nameof")]
    public AutoMock<T> Setup<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TResult>(
Expression<Func<T, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>>> expression, Times? times = null)
    {
        var expr = funcExpression.GetExpression(expression, new { });
        return SetupInternal(expression, expr, times);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the overload that takes a string/nameof")]
    public AutoMock<T> Setup<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TResult, TAnon>(
        Expression<Func<T, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>>> expression,
        TAnon paramData, Times? times = null)
        where TAnon : class // Doing TAnon : class to avoid overload resolution issues
    {
        var expr = funcExpression.GetExpression(expression, paramData);
        return SetupInternal(expression, expr, times);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the overload that takes a string/nameof")]
    public AutoMock<T> Setup<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TResult>(
Expression<Func<T, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TResult>>> expression, Times? times = null)
    {
        var expr = funcExpression.GetExpression(expression, new { });
        return SetupInternal(expression, expr, times);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the overload that takes a string/nameof")]
    public AutoMock<T> Setup<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TResult, TAnon>(
        Expression<Func<T, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TResult>>> expression,
        TAnon paramData, Times? times = null)
        where TAnon : class // Doing TAnon : class to avoid overload resolution issues
    {
        var expr = funcExpression.GetExpression(expression, paramData);
        return SetupInternal(expression, expr, times);
    }

    #endregion
}
