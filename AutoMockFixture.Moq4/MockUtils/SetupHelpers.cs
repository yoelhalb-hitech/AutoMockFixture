
namespace AutoMockFixture.Moq4.MockUtils;

internal static class SetupHelpers
{
    // TODO... maybe we can merge this with the AutoMock setup utils

    private static Mock GetMock(IAutoMock mock, Type mockedType)
        => mock.GetInnerType() == mockedType ? (Mock)mock : (Mock)typeof(Mock).GetMethod(nameof(Mock.As))!.MakeGenericMethod(mockedType)!.Invoke(mock, new Type[]{})!;

    public static void SetupCallbaseMethod(Type mockedType, Type returnType, IAutoMock mock, Expression methodInvocationLambda)
    {
        GetMethod(nameof(SetupCallbaseMethod))
            .MakeGenericMethod(mockedType, returnType)
            .Invoke(null, new object[] { GetMock(mock, mockedType), methodInvocationLambda });
    }

    public static void SetupVoidMethod(Type mockedType, IAutoMock mock, Expression methodInvocationLambda)
    {
        GetMethod(nameof(SetupVoidMethod))
            .MakeGenericMethod(mockedType)
            .Invoke(null, new object[] { GetMock(mock, mockedType), methodInvocationLambda });
    }

    public static void SetupMethodWithResult(Type mockedType, Type returnType,
                    IAutoMock mock, Expression methodInvocationLambda, object? returnValue)
    {
        GetMethod(nameof(SetupMethodWithResult))
               .MakeGenericMethod(mockedType, returnType)
               .Invoke(null, new object?[] { GetMock(mock, mockedType), methodInvocationLambda, returnValue });
    }

    public static void SetupMethodWithInvocationFunc(Type mockedType, Type returnType,
                    IAutoMock mock, Expression methodInvocationLambda, InvocationFunc invocationFunc)
    {
        GetMethod(nameof(SetupMethodWithInvocationFunc)).MakeGenericMethod(mockedType, returnType)
            .Invoke(null, new object[] { GetMock(mock, mockedType), methodInvocationLambda, invocationFunc });
    }

    public static void SetupLazyReadWriteProperty(Type mockedType, Type propertyType,
        IAutoMock mock, PropertyInfo property, Func<object?> valueGenerator)
    {
        GetMethod(nameof(SetupLazyReadWriteProperty))
               .MakeGenericMethod(mockedType, propertyType)
               .Invoke(null, new object?[] { GetMock(mock, mockedType), property, valueGenerator });
    }

    private static MethodInfo GetMethod(string name) => typeof(SetupHelpers)
            .GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static)!;


#pragma warning disable CA1811 // AvoidUncalledPrivateCode
    private static void SetupVoidMethod<TMock>(Mock<TMock> mock, Expression<Action<TMock>> methodCallExpression)
where TMock : class
    {
        mock.Setup(methodCallExpression);
    }

    private static void SetupCallbaseMethod<TMock, TResult>(Mock<TMock> mock, Expression<Func<TMock, TResult>> methodCallExpression)
where TMock : class
    {
        mock.Setup(methodCallExpression).CallBase();
    }

    private static void SetupMethodWithInvocationFunc<TMock, TResult>(
        Mock<TMock> mock, Expression<Func<TMock, TResult>> methodCallExpression,
                                InvocationFunc invocationFunc)
        where TMock : class
    {
        mock.Setup(methodCallExpression).Returns(invocationFunc);
    }

    private static void SetupMethodWithResult<TMock, TResult>(
        Mock<TMock> mock, Expression<Func<TMock, TResult>> methodCallExpression, TResult result)
    where TMock : class
    {
        mock.Setup(methodCallExpression)
#pragma warning disable CS8603 // Possible null reference return.
            .Returns(result);
#pragma warning restore CS8603 // Possible null reference return.
    }

    // https://stackoverflow.com/a/72440782/640195
    private static void SetupLazyReadWriteProperty<TMock, TProperty>(this Mock<TMock> mock,
           PropertyInfo property,
           Func<object> valueGenerator) where TMock : class
    {
        TProperty? propState = default;
        var matchCapture = new CaptureMatch<TProperty>(x => propState = x);

        TProperty? propGenerated = default;
        var isAssigned = false;
        var isEvaluated = false;
        var lockObject = new object();

        Expression<Action> captureExpression = () => Capture.With(matchCapture);

        var paramExpr = Expression.Parameter(typeof(TMock));
        var getExpr = Expression.Lambda<Func<TMock, TProperty>>(Expression.Call(paramExpr, property.GetMethod), paramExpr);

        var setExpression = Expression.Lambda<Action<TMock>>(
                                Expression.Call(paramExpr, property.SetMethod, captureExpression.Body),
                                           paramExpr);

        var propFunc = () =>
        {
            if (isEvaluated) return propGenerated;

            lock (lockObject)
            {
                if (isEvaluated) return propGenerated;

                propGenerated = (TProperty)valueGenerator();
                isEvaluated = true;

                return propGenerated;
            }
        };

        // Cannot use `SetupSet` as Moq will try to create a new mock from the mocked type, which might not have a default ctor
        mock.Setup(setExpression).Callback((TProperty p) => { isAssigned = true; });
        mock.Setup(getExpr).Returns(() => isAssigned ? propState! : propFunc());
    }
#pragma warning restore CA1811 // AvoidUncalledPrivateCode
}
