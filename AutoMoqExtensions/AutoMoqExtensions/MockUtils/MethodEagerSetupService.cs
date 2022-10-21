using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.MockUtils
{
    internal class MethodEagerSetupService : MethodSetupServiceBase
    {
        public MethodEagerSetupService(IAutoMock mock, MethodInfo method, ISpecimenContext context,
            string? customTrackingPath = null)
                : base(mock, method, context, customTrackingPath)
        {
        }

        protected override object? HandleInvocationFunc(IInvocation invocation)
            => HandleInvocationFuncWithSameResult(invocation);

        protected override void HandleNonGenericFunction(Expression methodInvocationLambda, Type returnType)
        {
            var result = GenerateResult(this.method);
            SetupHelpers.SetupMethodWithResult(mockedType, returnType, mock, methodInvocationLambda, result);
        }
    }
}
