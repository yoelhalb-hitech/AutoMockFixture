
using System.Linq.Expressions;

namespace AutoMoqExtensions.Expressions;

internal class FuncExpressionBuilder<T>
{
    private readonly BasicExpressionBuilder<T> expressionBuilder = new();
    private Expression<Func<T, TResult>> GetExpression<TAnon, TResult>(LambdaExpression expression, TAnon paramData, Type[] types)
                => (Expression<Func<T, TResult>>)expressionBuilder.GetExpression(expression, paramData, types);

    public Expression<Func<T, TResult>> GetExpression<TAnon, TResult>(
                Expression<Func<T, Func<TResult>>> expression, TAnon paramData)
                    => GetExpression<TAnon, TResult>(expression, paramData, new Type[]{});

    public Expression<Func<T, TResult>> GetExpression<TParam1, TResult, TAnon>(
                    Expression<Func<T, Func<TParam1, TResult>>> expression, TAnon paramData)
            => GetExpression<TAnon, TResult>(expression, paramData, new[] { typeof(TParam1)});

    public Expression<Func<T, TResult>> GetExpression<TParam1, TParam2, TResult, TAnon>(
                    Expression<Func<T, Func<TParam1, TParam2, TResult>>> expression, TAnon paramData)
                        => GetExpression<TAnon, TResult>(expression, paramData, new[] { typeof(TParam1), typeof(TParam2) });

    public Expression<Func<T, TResult>> GetExpression<TParam1, TParam2, TParam3, TResult, TAnon>(
            Expression<Func<T, Func<TParam1, TParam2, TParam3, TResult>>> expression, TAnon paramData)
                => GetExpression<TAnon, TResult>(expression, paramData, new[] { typeof(TParam1), typeof(TParam2), typeof(TParam3) });
    public Expression<Func<T, TResult>> GetExpression<TParam1, TParam2, TParam3, TParam4, TResult, TAnon>(
            Expression<Func<T, Func<TParam1, TParam2, TParam3, TParam4, TResult>>> expression, TAnon paramData)
        => GetExpression<TAnon, TResult>(expression, paramData, new[] { typeof(TParam1), typeof(TParam2), typeof(TParam3), typeof(TParam4) });

    public Expression<Func<T, TResult>> GetExpression<TParam1, TParam2, TParam3, TParam4, TParam5, TResult, TAnon>(
        Expression<Func<T, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TResult>>> expression, TAnon paramData)
            => GetExpression<TAnon, TResult>(expression, paramData, 
                new[] { typeof(TParam1), typeof(TParam2), typeof(TParam3), typeof(TParam4), typeof(TParam5) });

    public Expression<Func<T, TResult>> GetExpression<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TResult, TAnon>(
            Expression<Func<T, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TResult>>> expression, TAnon paramData)
                => GetExpression<TAnon, TResult>(expression, paramData,
                new[] { typeof(TParam1), typeof(TParam2), typeof(TParam3), typeof(TParam4), typeof(TParam5), typeof(TParam6) });

    public Expression<Func<T, TResult>> GetExpression<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TResult, TAnon>(
    Expression<Func<T, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TResult>>> expression, TAnon paramData)
        => GetExpression<TAnon, TResult>(expression, paramData,
        new[] { typeof(TParam1), typeof(TParam2), typeof(TParam3), typeof(TParam4), typeof(TParam5), typeof(TParam6), typeof(TParam7) });
}
