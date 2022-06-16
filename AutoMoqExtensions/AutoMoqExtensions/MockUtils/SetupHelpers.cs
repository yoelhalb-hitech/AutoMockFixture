using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoMoqExtensions.MockUtils
{
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
        
        public static void SetupMethodWithGenericResult(Type mockedType, Type newReturnType, 
                        IAutoMock mock, Expression methodInvocationLambda, InvocationFunc invocationFunc)
        {
            GetMethod(nameof(SetupMethodWithGenericResult)).MakeGenericMethod(mockedType, newReturnType)
                .Invoke(null, new object[] { mock, methodInvocationLambda, invocationFunc });
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
        
        private static void SetupMethodWithGenericResult<TMock, TResult>(
            AutoMock<TMock> mock, Expression<Func<TMock, TResult>> methodCallExpression,
                                    InvocationFunc invocationFunc)
            where TMock : class
        {
            mock.Setup(methodCallExpression).Returns(invocationFunc);
        }
        
        private static void SetupMethodWithResult<TMock, TResult>(
            AutoMock<TMock> mock, Expression<Func<TMock, TResult>> methodCallExpression,TResult result)
        where TMock : class
        {
            mock.Setup(methodCallExpression)
#pragma warning disable CS8603 // Possible null reference return.
                .Returns(result);
#pragma warning restore CS8603 // Possible null reference return.
        }
#pragma warning restore CA1811 // AvoidUncalledPrivateCode
    }
}
