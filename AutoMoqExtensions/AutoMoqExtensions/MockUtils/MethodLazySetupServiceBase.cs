using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.MockUtils
{
    internal abstract class MethodLazySetupServiceBase : MethodSetupServiceBase
    {
        protected MethodLazySetupServiceBase(IAutoMock mock, MethodInfo method,ISpecimenContext context,
            string? customTrackingPath = null) 
                : base(mock, method, context, customTrackingPath)
        {
        }

        protected override void HandleNonGenericFunction(Expression methodInvocationLambda, Type returnType)
        {
            SetupWithInvocationFunc(methodInvocationLambda, returnType);
        }
    }
}
