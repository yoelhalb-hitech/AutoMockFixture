
namespace AutoMockFixture.Moq4.Expressions;

internal class ActionExpressionBuilder<T>
{
    private readonly BasicExpressionBuilder<T> expressionBuilder = new();
    private Expression<Action<T>> GetExpression<TAnon>(LambdaExpression expression, TAnon paramData, Type[] types)
                => (Expression<Action<T>>)expressionBuilder.GetExpression(expression, paramData, types);

    public Expression<Action<T>> GetExpression<TAnon>(Expression<Func<T, Action>> expression, TAnon paramData)
                => GetExpression(expression, paramData, new Type[] { });

    public Expression<Action<T>> GetExpression<TParam1, TAnon>(
            Expression<Func<T, Action<TParam1>>> expression, TAnon paramData)
                => GetExpression(expression, paramData, new[] { typeof(TParam1) });

    public Expression<Action<T>> GetExpression<TParam1, TParam2, TAnon>(
                    Expression<Func<T, Action<TParam1, TParam2>>> expression, TAnon paramData)
                        => GetExpression(expression, paramData, new[] { typeof(TParam1), typeof(TParam2) });

    public Expression<Action<T>> GetExpression<TParam1, TParam2, TParam3, TAnon>(
            Expression<Func<T, Action<TParam1, TParam2, TParam3>>> expression, TAnon paramData)
                => GetExpression(expression, paramData, new[] { typeof(TParam1), typeof(TParam2), typeof(TParam3) });
    public Expression<Action<T>> GetExpression<TParam1, TParam2, TParam3, TParam4, TAnon>(
            Expression<Func<T, Action<TParam1, TParam2, TParam3, TParam4>>> expression, TAnon paramData)
        => GetExpression(expression, paramData, new[] { typeof(TParam1), typeof(TParam2), typeof(TParam3), typeof(TParam4) });

    public Expression<Action<T>> GetExpression<TParam1, TParam2, TParam3, TParam4, TParam5, TAnon>(
        Expression<Func<T, Action<TParam1, TParam2, TParam3, TParam4, TParam5>>> expression, TAnon paramData)
            => GetExpression(expression, paramData,
                new[] { typeof(TParam1), typeof(TParam2), typeof(TParam3), typeof(TParam4), typeof(TParam5) });

    public Expression<Action<T>> GetExpression<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TAnon>(
            Expression<Func<T, Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>>> expression, TAnon paramData)
                => GetExpression(expression, paramData,
                new[] { typeof(TParam1), typeof(TParam2), typeof(TParam3), typeof(TParam4), typeof(TParam5), typeof(TParam6) });

    public Expression<Action<T>> GetExpression<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TAnon>(
    Expression<Func<T, Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7>>> expression, TAnon paramData)
        => GetExpression(expression, paramData,
        new[] { typeof(TParam1), typeof(TParam2), typeof(TParam3), typeof(TParam4), typeof(TParam5), typeof(TParam6), typeof(TParam7) });
}
