using AutoFixture.AutoMoq;
using AutoFixture.Kernel;
using AutoMoqExtensions.AutoMockUtils;
using AutoMoqExtensions.Extensions;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Commands
{
    // TODO... we need to handle internal methods
    internal class AutoMockVirtualMethodsCommand : ISpecimenCommand
    {
        private static readonly DelegateSpecification DelegateSpecification = new DelegateSpecification();

        public void Execute(object specimen, ISpecimenContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            try
            {
                var mock = AutoMockHelpers.GetFromObj(specimen);
                if (mock is null) return;

                var mockedType = mock.GetInnerType();
                var methods = GetConfigurableMethods(mockedType);

                foreach (var method in methods)
                {
                    var returnType = method.ReturnType;
                    var methodInvocationLambda = MakeMethodInvocationLambda(mockedType, method, context);

                    if (methodInvocationLambda == null) continue;

                    if (method.IsVoid())
                    {
                        try
                        {
                            GetType()
                                .GetMethod(nameof(SetupVoidMethod), BindingFlags.NonPublic | BindingFlags.Static)
                                .MakeGenericMethod(mockedType)
                                .Invoke(this, new object[] { mock, methodInvocationLambda });
                        }
                        catch { }
                    }
                    else
                    {
                        try
                        {
                            GetType()
                                .GetMethod(nameof(SetupMethod), BindingFlags.NonPublic | BindingFlags.Static)
                                .MakeGenericMethod(mockedType, returnType)
                                .Invoke(this, new object[] { mock, methodInvocationLambda, context });
                        }
                        catch { }
                    }

                }
            }
            catch { }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "This method is invoked through reflection.")]
        private static void SetupVoidMethod<TMock>(AutoMock<TMock> mock, Expression<Action<TMock>> methodCallExpression)
    where TMock : class
        {
            mock.Setup(methodCallExpression);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "This method is invoked through reflection.")]
        private static void SetupMethod<TMock, TResult>(
            AutoMock<TMock> mock, Expression<Func<TMock, TResult>> methodCallExpression, ISpecimenContext context)
            where TMock : class
        {
            mock.Setup(methodCallExpression)
#pragma warning disable CS8603 // Possible null reference return.
                .Returns(() => GetReturn<TResult>(context));
#pragma warning restore CS8603 // Possible null reference return.
        }

        private static TResult? GetReturn<TResult>(ISpecimenContext context)
        {
            var specimen = ResolveReturn<TResult>(context);

            // check if specimen is null but member is non-nullable value type
            if (specimen == null && default(TResult) != null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    "Tried to setup a member with a return type of {0}, but null was found instead.", typeof(TResult)));
            }

            // check if specimen can be safely converted to TResult
            if (specimen != null && specimen is not TResult
                    && (specimen is not IAutoMock mock || !typeof(TResult).IsAssignableFrom(mock.GetInnerType())))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                        "Tried to setup a member with a return type of {0}, but an instance of {1} was found instead.",
                        typeof(TResult), specimen.GetType()));
            }

            return (TResult?)specimen;
        }

        private static object ResolveReturn<TResult>(ISpecimenContext context)
        {
            var type = typeof(TResult);
            if (!AutoMockHelpers.IsAutoMockAllowed(type))
            {
                return context.Resolve(type);
            }

            try
            {
                var mockType = AutoMockHelpers.GetAutoMockType(type);
                var specimen = context.Resolve(mockType);

                if (specimen is NoSpecimen || specimen is OmitSpecimen
                    || specimen is null || specimen is not IAutoMock mock || mock.GetInnerType() != type)
                    return context.Resolve(type);

                return mock.GetMocked();
            }
            catch
            {

                return context.Resolve(type);
            }
        }

        private static IEnumerable<MethodInfo> GetConfigurableMethods(Type type)
        {
            // If "type" is a delegate, return "Invoke" method only and skip the rest of the methods.
            var methods = DelegateSpecification.IsSatisfiedBy(type)
                ? new[] { type.GetTypeInfo().GetMethod("Invoke") }
                : type.GetAllMethods().Where(m => m.IsPublicOrInternal());

            // Skip properties that have both getters and setters to not interfere
            // with other post-processors in the chain that initialize them later.
            methods = SkipWritablePropertyGetters(type, methods);

            return methods.Where(m => CanBeConfigured(type, m));
        }

        private static IEnumerable<MethodInfo> SkipWritablePropertyGetters(Type type, IEnumerable<MethodInfo> methods)
        {
            var getterMethods = type.GetAllProperties()
                .Where(p => p.GetGetMethod() != null && p.GetSetMethod() != null)
                .Select(p => p.GetGetMethod());

            return methods.Except(getterMethods);
        }


        private static bool CanBeConfigured(Type type, MethodInfo method)
            => (type.IsInterface || method.IsOverridable()) &&
                   !method.IsGenericMethod &&
                   !method.HasRefParameters() &&
                    (!method.IsVoid() || method.HasOutParameters()); // No point in setting up a void method...

        private static Expression? MakeMethodInvocationLambda(Type mockedType, MethodInfo method,
                                                      ISpecimenContext context)
        {
            var lambdaParam = Expression.Parameter(mockedType, "x");

            var methodCallParams = method.GetParameters()
                                         .Select(param => MakeParameterExpression(param, context))
                                         .ToList();

            if (methodCallParams.Any(exp => exp == null))
                return null;

            Expression methodCall;
            if (DelegateSpecification.IsSatisfiedBy(mockedType))
            {
                // e.g. "x(It.IsAny<string>(), out parameter)"
                methodCall = Expression.Invoke(lambdaParam, methodCallParams);
            }
            else
            {
                // e.g. "x.Method(It.IsAny<string>(), out parameter)"
                methodCall = Expression.Call(lambdaParam, method, methodCallParams);
            }

            // e.g. "x => x.Method(It.IsAny<string>(), out parameter)"
            // or "x => x(It.IsAny<string>(), out parameter)"
            return Expression.Lambda(methodCall, lambdaParam);
        }

        // TODO... add generic support using IsAnyType?
        private static Expression? MakeParameterExpression(ParameterInfo parameter, ISpecimenContext context)
        {
            // check if parameter is an "out" parameter
            if (parameter.IsOut)
            {
                // gets the type corresponding to this "byref" type
                // e.g., the underlying type of "System.String&" is "System.String"
                var underlyingType = parameter.ParameterType.GetElementType();

                // resolve the "out" param from the context
                object variable = context.Resolve(underlyingType);

                if (variable is OmitSpecimen)
                    return null;

                return Expression.Constant(variable, underlyingType);
            }
            else
            {
                // for any non-out parameter, invoke "It.IsAny<T>()"
                var isAnyMethod = typeof(It).GetMethod(nameof(It.IsAny)).MakeGenericMethod(parameter.ParameterType);

                return Expression.Call(isAnyMethod);
            }
        }

    }
}
