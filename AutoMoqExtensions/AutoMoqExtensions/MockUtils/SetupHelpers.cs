using Moq;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoMoqExtensions.MockUtils;

internal static class SetupHelpers
{
    // TODO... maybe we can merge this with the AutoMock setup utils


    public static void SetupCallbaseMethod(Type mockedType, Type returnType, IAutoMock mock, Expression methodInvocationLambda)
    {
        GetMethod(nameof(SetupCallbaseMethod))
            .MakeGenericMethod(mockedType, returnType)
            .Invoke(null, new object[] { mock, methodInvocationLambda }); 
    }

    public static void SetupVoidMethod(Type mockedType, IAutoMock mock, Expression methodInvocationLambda)
    {
        GetMethod(nameof(SetupVoidMethod))
            .MakeGenericMethod(mockedType)
            .Invoke(null, new object[] { mock, methodInvocationLambda });
    }

    public static void SetupMethodWithResult(Type mockedType, Type returnType, 
                    IAutoMock mock, Expression methodInvocationLambda, object? returnValue)
    {
        GetMethod(nameof(SetupMethodWithResult))
               .MakeGenericMethod(mockedType, returnType)
               .Invoke(null, new object?[] { mock, methodInvocationLambda, returnValue });
    }
    
    public static void SetupMethodWithInvocationFunc(Type mockedType, Type returnType, 
                    IAutoMock mock, Expression methodInvocationLambda, InvocationFunc invocationFunc)
    {
        GetMethod(nameof(SetupMethodWithInvocationFunc)).MakeGenericMethod(mockedType, returnType)
            .Invoke(null, new object[] { mock, methodInvocationLambda, invocationFunc });
    }

    public static void SetupAutoProperty(Type mockedType, Type propertyType,
        IAutoMock mock, PropertyInfo property, object? initialValue)
    {
        var paramExpr = Expression.Parameter(mockedType);
        var expr = Expression.Lambda(Expression.MakeMemberAccess(paramExpr, property), paramExpr);

        GetMethod(nameof(SetupMethodWithResult))
               .MakeGenericMethod(mockedType, propertyType)
               .Invoke(null, new object?[] { mock, expr, initialValue });
    }

    public static void SetupAutoProperty(Type mockedType, Type propertyType,
            IAutoMock mock, Expression methodInvocationLambda, object? initialValue)
    {
        GetMethod(nameof(SetupMethodWithResult))
               .MakeGenericMethod(mockedType, propertyType)
               .Invoke(null, new object?[] { mock, methodInvocationLambda, initialValue });
    }

    private static MethodInfo GetMethod(string name) => typeof(SetupHelpers)
            .GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static);


#pragma warning disable CA1811 // AvoidUncalledPrivateCode
    private static void SetupVoidMethod<TMock>(AutoMock<TMock> mock, Expression<Action<TMock>> methodCallExpression)
where TMock : class
    {
        mock.Setup(methodCallExpression);
    }
    
    private static void SetupCallbaseMethod<TMock, TResult>(AutoMock<TMock> mock, Expression<Func<TMock, TResult>> methodCallExpression)
where TMock : class
    {
        mock.Setup(methodCallExpression).CallBase();
    }
    
    private static void SetupMethodWithInvocationFunc<TMock, TResult>(
        AutoMock<TMock> mock, Expression<Func<TMock, TResult>> methodCallExpression,
                                InvocationFunc invocationFunc)
        where TMock : class
    {
        mock.Setup(methodCallExpression).Returns(invocationFunc);
    }
    
    private static void SetupMethodWithResult<TMock, TResult>(
        AutoMock<TMock> mock, Expression<Func<TMock, TResult>> methodCallExpression, TResult result)
    where TMock : class
    {
        mock.Setup(methodCallExpression)
#pragma warning disable CS8603 // Possible null reference return.
            .Returns(result);
#pragma warning restore CS8603 // Possible null reference return.
    }

    // https://stackoverflow.com/a/72440782/640195
    public static void SetupAutoProperty<TMock, TProperty>(this AutoMock<TMock> mock,
           Expression<Func<TMock, TProperty>> memberAccessExpr,
           TProperty initialValue) where TMock : class
    {
        var propStates = new List<TProperty>();
        Expression<Action> captureExpression = () => Capture.In(propStates);

        var finalExpression = Expression.Lambda<Action<TMock>>(
                    Expression.Assign(memberAccessExpr.Body, captureExpression.Body),
                                           memberAccessExpr.Parameters);

        mock.SetupSet(finalExpression.Compile());
        mock.SetupGet(memberAccessExpr).Returns(() =>
                   propStates.Any() ? propStates.Last() : initialValue);
    }
#pragma warning restore CA1811 // AvoidUncalledPrivateCode
}
