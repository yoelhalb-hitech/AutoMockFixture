using Moq;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoMoqExtensions.Expressions;

internal class BasicExpressionBuilder<T>
{
    public MethodInfo GetMethod(LambdaExpression expression) =>
(((expression.Body as UnaryExpression)?.Operand as MethodCallExpression)?.Object as ConstantExpression)?
        .Value as MethodInfo 
        ?? (expression.Body as MethodCallExpression)?.Method
        ?? throw new Exception("Method not found on object");


    public LambdaExpression GetExpression<TAnon>(
            LambdaExpression expression, TAnon paramData, Type[] types)
    {
        var method = GetMethod(expression);
        return GetExpression(method, paramData, types);
    }

    public LambdaExpression GetExpression<TAnon>(
            MethodInfo method, TAnon paramData, Type[] types)
    {            
        var methodParams = GetParams(method, paramData, types);
        return GetLambdaExpression(method, methodParams);
    }

    private IEnumerable<Expression> GetParams<TAnon>(MethodInfo method, TAnon paramData, Type[] types)
    {
        var methodParams = method.GetParameters();

        var paramDataFields = typeof(TAnon).GetProperties(BindingFlags.Instance | BindingFlags.Public);

        return methodParams.Select((p, i) =>
        {
            var paramFieldData = paramDataFields.FirstOrDefault(p => methodParams[i].Name == p.Name);
            return paramFieldData is not null
                ? (Expression)Expression.Constant(paramFieldData.GetValue(paramData))
                : Expression.Call(typeof(It), nameof(It.IsAny), new Type[] { types[i] });
        });
    }

    private LambdaExpression GetLambdaExpression(MethodInfo method, IEnumerable<Expression> arguments)
    {
        var actionCall = Expression.Call(Expression.Parameter(typeof(T)), method, arguments);
        return (Expression.Lambda(actionCall, Expression.Parameter(typeof(T))));
    }
}
