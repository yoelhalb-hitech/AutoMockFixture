using AutoMoqExtensions.Expressions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions
{
    // TODO... add analyzer for:
    // 1) TAnon to ensure the properties are actually parameters of the method and the correct type
    // 2) method name as string that it is correct and there is only one overload
    // 3) return tyhpe is correct for the method provided
    // TODO... maybe split it in partial classes for readability

    // TODO... hanlde out and ref methods
    public interface IAutoMock 
    {
        bool CallBase { get; set; }
        void EnsureMocked();
        Type GetInnerType();
        object GetMocked();
    }
    public class AutoMock<T> : Mock<T>, IAutoMock where T : class
    {        
        public List<IVerifyInfo<T>> VerifyList { get; } = new List<IVerifyInfo<T>>();
        private T? mocked;
        public Type GetInnerType() => typeof(T);
        public void EnsureMocked()
        {
            if (mocked is null)
                mocked = this.Object;
        }
        object IAutoMock.GetMocked() => GetMocked();
        public T GetMocked()
        {
            EnsureMocked();

            return mocked!;
        }

        public static implicit operator T(AutoMock<T> m) => m.GetMocked();

        public AutoMock(MockBehavior behavior) : base(behavior) { }
        public AutoMock(params object[] args) : base(args) { }
        public AutoMock(MockBehavior behavior, params object[] args) : base(behavior, args) { }
        public AutoMock() : base() { }

        public new void VerifyAll()
        {
            VerifyList.ForEach(v => v.Verify(this));// TODO... maybe we should catch everything and rethrow as aggregate exception
            base.VerifyAll();
        }

        private readonly BasicExpressionBuilder<T> basicExpression = new();
        private readonly ActionExpressionBuilder<T> actionExpression = new();
        private readonly FuncExpressionBuilder<T> funcExpression = new();

        private AutoMock<T> SetupInternal(LambdaExpression originalExpression, Expression<Action<T>> expression, Times? times = null)
        {
            var method = basicExpression.GetMethod(originalExpression);
            return SetupActionInternal(method, expression, times);
        }

        private AutoMock<T> SetupActionInternal(MethodInfo method, Expression<Action<T>> expression, Times? times = null)
        {            
            if (method.IsSpecialName) // Assumming property set
            {
                var compiled = expression.Compile();
                base.SetupSet(compiled);
                if (times.HasValue) VerifyList.Add(new VerifySetInfo<T>(compiled, times.Value));
                return this;
            }

            base.Setup(expression);
            if (times.HasValue) VerifyList.Add(new VerifyActionInfo<T>(expression, times.Value));
            return this;
        }

        private AutoMock<T> SetupInternal<TResult>(LambdaExpression originalExpression, Expression<Func<T, TResult>> expression, Times? times = null)
        {
            var method = basicExpression.GetMethod(originalExpression);
            return SetupFuncInternal(method, expression, times);
        }
        private AutoMock<T> SetupFuncFromLambda<TResult>(MethodInfo method, LambdaExpression expression, Times? times = null)
        {
            return SetupFuncInternal(method, (Expression<Func<T, TResult>>)expression, times);
        }

        // Cannot use default parameters as null can be sometinmes a valid result
        private AutoMock<T> SetupFuncInternal<TResult>(MethodInfo method, Expression<Func<T, TResult>> expression, Times? times = null)
        {          
            if (method.IsSpecialName) // Assumming property get
            {
                base.SetupGet(expression);
                if (times.HasValue) VerifyList.Add(new VerifyGetInfo<T, TResult>(expression, times.Value));
                return this;
            }

            base.Setup(expression);
            if (times.HasValue) VerifyList.Add(new VerifyFuncInfo<T, TResult>(expression, times.Value));
            return this;
        }
        
        private AutoMock<T> SetupFuncWithResult<TResult>(MethodInfo method, Expression<Func<T, TResult>> expression, TResult result, Times? times = null)
        {
            if (method.IsSpecialName) // Assumming property get
            {
                base.SetupGet(expression).Returns(result);
                if (times.HasValue) VerifyList.Add(new VerifyGetInfo<T, TResult>(expression, times.Value));
                return this;
            }

            base.Setup(expression).Returns(result);
            if (times.HasValue) VerifyList.Add(new VerifyFuncInfo<T, TResult>(expression, times.Value));
            return this;
        }

        private BindingFlags allInstance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        private MethodInfo GetSetupFuncInternal(Type type)
            => this.GetType().GetMethod(nameof(SetupFuncFromLambda), allInstance)
            .MakeGenericMethod(type);

        public AutoMock<T> Setup(MethodInfo method, Times times) => Setup(method, new { }, times);
      
        private MethodInfo GetMethod(string methodName) => typeof(T).GetMethod(methodName, allInstance);

        public AutoMock<T> Setup(string methodName) => SetupInternal(GetMethod(methodName), new { }, null);
        public AutoMock<T> Setup(string methodName, Times times) => SetupInternal(GetMethod(methodName), new { }, times);

        // Doing TAnon : class to avoid overload resolution issues
        public AutoMock<T> Setup<TAnon>(string methodName, TAnon paramData) where TAnon : class
            => SetupInternal(GetMethod(methodName), paramData, null);

        // Doing TAnon : class to avoid overload resolution issues
        public AutoMock<T> Setup<TAnon>(string methodName, TAnon paramData, Times times) where TAnon : class
            => SetupInternal(GetMethod(methodName), paramData, times);

        // Doing TAnon : class to avoid overload resolution issues
        public AutoMock<T> Setup<TAnon>(MethodInfo method, TAnon paramData) where TAnon : class
            => SetupInternal(method, paramData, null);

        // Doing TAnon : class to avoid overload resolution issues
        public AutoMock<T> Setup<TAnon>(MethodInfo method, TAnon paramData, Times times) where TAnon : class
            => SetupInternal(method, paramData, times);
        // Doing this way it because of issues with overload resolution
        private AutoMock<T> SetupInternal<TAnon>(MethodInfo method, TAnon paramData, Times? times) where TAnon : class
        {
            var paramTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
            var expr = basicExpression.GetExpression(method, paramData, paramTypes);
            if (method.ReturnType == typeof(void)) return SetupActionInternal(method, (Expression<Action<T>>)expr, times);

            GetSetupFuncInternal(method.ReturnType).Invoke(this, new object?[] { method, expr, times });
            return this;
        }

        public AutoMock<T> Setup<TAnon, TResult>(string methodName, TAnon paramData, TResult result) where TAnon : class
                => SetupInternal(GetMethod(methodName), paramData, result, null);
        public AutoMock<T> Setup<TAnon, TResult>(string methodName, TAnon paramData, TResult result, Times times) where TAnon : class
                => SetupInternal(GetMethod(methodName), paramData, result, times);

        public AutoMock<T> Setup<TAnon, TResult>(MethodInfo method, TAnon paramData, TResult result) where TAnon : class
                => SetupInternal(method, paramData, result, null);
        // Doing TAnon : class to avoid overload resolution issues
        public AutoMock<T> Setup<TAnon, TResult>(MethodInfo method, TAnon paramData, TResult result, Times times) where TAnon : class
                => SetupInternal(method, paramData, result, times);
        // Doing this way it because of issues with overload resolution
        private AutoMock<T> SetupInternal<TAnon, TResult>(MethodInfo method, TAnon paramData, TResult result, Times? times) where TAnon : class
        {
            var paramTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
            var expr = basicExpression.GetExpression(method, paramData, paramTypes);

            return SetupFuncWithResult<TResult>(method, (Expression<Func<T, TResult>>)expr, result, times);
        }

        #region SetupAction

        public AutoMock<T> Setup(Expression<Func<T, Action>> expression, Times? times = null)
        {
            var expr = actionExpression.GetExpression(expression, new { });
            return SetupInternal(expression, expr, times);
        }

        public AutoMock<T> Setup<TParam>(Expression<Func<T, Action<TParam>>> expression, Times? times = null)
        {
            var expr = actionExpression.GetExpression(expression, new { });
            return SetupInternal(expression, expr, times);
        }


        public AutoMock<T> Setup<TParam1, TParam2>(Expression<Func<T, Action<TParam1, TParam2>>> expression, Times? times = null)
        {
            var expr = actionExpression.GetExpression(expression, new { });
            return SetupInternal(expression, expr, times);
        }

        public AutoMock<T> Setup<TParam1, TParam2, TAnon>(
            Expression<Func<T, Action<TParam1, TParam2>>> expression, TAnon paramData, Times? times = null)
            where TAnon : class // Doing TAnon : class to avoid overload resolution issues
        {
            var expr = actionExpression.GetExpression(expression, paramData);
            return SetupInternal(expression, expr, times);
        }

        public AutoMock<T> Setup<TParam1, TParam2, TParam3>(
                    Expression<Func<T, Action<TParam1, TParam2, TParam3>>> expression, Times? times = null)
        {
            var expr = actionExpression.GetExpression(expression, new { });
            return SetupInternal(expression, expr, times);
        }

        public AutoMock<T> Setup<TParam1, TParam2, TParam3, TAnon>(
            Expression<Func<T, Action<TParam1, TParam2, TParam3>>> expression, TAnon paramData, Times? times = null)
            where TAnon : class // Doing TAnon : class to avoid overload resolution issues
        {
            var expr = actionExpression.GetExpression(expression, paramData);
            return SetupInternal(expression, expr, times);
        }

        public AutoMock<T> Setup<TParam1, TParam2, TParam3, TParam4>(
            Expression<Func<T, Action<TParam1, TParam2, TParam3, TParam4>>> expression, Times? times = null)
        {
            var expr = actionExpression.GetExpression(expression, new { });
            return SetupInternal(expression, expr, times);
        }

        public AutoMock<T> Setup<TParam1, TParam2, TParam3,TParam4, TAnon>(
            Expression<Func<T, Action<TParam1, TParam2, TParam3, TParam4>>> expression, TAnon paramData, Times? times = null)
            where TAnon : class // Doing TAnon : class to avoid overload resolution issues
        {
            var expr = actionExpression.GetExpression(expression, paramData);
            return SetupInternal(expression, expr, times);
        }

        public AutoMock<T> Setup<TParam1, TParam2, TParam3, TParam4, TParam5>(
            Expression<Func<T, Action<TParam1, TParam2, TParam3, TParam4, TParam5>>> expression, Times? times = null)
        {
            var expr = actionExpression.GetExpression(expression, new { });
            return SetupInternal(expression, expr, times);
        }

        public AutoMock<T> Setup<TParam1, TParam2, TParam3, TParam4, TParam5, TAnon>(
            Expression<Func<T, Action<TParam1, TParam2, TParam3, TParam4, TParam5>>> expression,
            TAnon paramData, Times? times = null)
            where TAnon : class // Doing TAnon : class to avoid overload resolution issues
        {
            var expr = actionExpression.GetExpression(expression, paramData);
            return SetupInternal(expression, expr, times);
        }

        public AutoMock<T> Setup<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>(
    Expression<Func<T, Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>>> expression, Times? times = null)
        {
            var expr = actionExpression.GetExpression(expression, new { });
            return SetupInternal(expression, expr, times);
        }

        public AutoMock<T> Setup<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TAnon>(
            Expression<Func<T, Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>>> expression,
            TAnon paramData, Times? times = null)
            where TAnon : class // Doing TAnon : class to avoid overload resolution issues
        {
            var expr = actionExpression.GetExpression(expression, paramData);
            return SetupInternal(expression, expr, times);
        }

        public AutoMock<T> Setup<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7>(
Expression<Func<T, Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7>>> expression, Times? times = null)
        {
            var expr = actionExpression.GetExpression(expression, new { });
            return SetupInternal(expression, expr, times);
        }

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

        public AutoMock<T> Setup<TResult>(Expression<Func<T, Func<TResult>>> expression, Times? times = null)
        {
            var expr = funcExpression.GetExpression(expression, new { });
            return SetupInternal(expression, expr, times);
        }

        public AutoMock<T> Setup<TParam, TResult>(Expression<Func<T, Func<TParam, TResult>>> expression, Times? times = null)
        {
            var expr = funcExpression.GetExpression(expression, new { });
            return SetupInternal(expression, expr, times);
        }


        public AutoMock<T> Setup<TParam1, TParam2, TResult>(Expression<Func<T, Func<TParam1, TParam2, TResult>>> expression, Times? times = null)
        {
            var expr = funcExpression.GetExpression(expression, new { });
            return SetupInternal(expression, expr, times);
        }

        public AutoMock<T> Setup<TParam1, TParam2, TResult, TAnon>(
            Expression<Func<T, Func<TParam1, TParam2, TResult>>> expression, TAnon paramData, Times? times = null)
            where TAnon : class // Doing TAnon : class to avoid overload resolution issues
        {
            var expr = funcExpression.GetExpression(expression, paramData);
            return SetupInternal(expression, expr, times);
        }

        public AutoMock<T> Setup<TParam1, TParam2, TParam3, TResult>(
                    Expression<Func<T, Func<TParam1, TParam2, TParam3, TResult>>> expression, Times? times = null)
        {
            var expr = funcExpression.GetExpression(expression, new { });
            return SetupInternal(expression, expr, times);
        }

        public AutoMock<T> Setup<TParam1, TParam2, TParam3, TResult, TAnon>(
            Expression<Func<T, Func<TParam1, TParam2, TParam3, TResult>>> expression, TAnon paramData, Times? times = null)
            where TAnon : class // Doing TAnon : class to avoid overload resolution issues
        {
            var expr = funcExpression.GetExpression(expression, paramData);
            return SetupInternal(expression, expr, times);
        }

        public AutoMock<T> Setup<TParam1, TParam2, TParam3, TParam4, TResult>(
            Expression<Func<T, Func<TParam1, TParam2, TParam3, TParam4, TResult>>> expression, Times? times = null)
        {
            var expr = funcExpression.GetExpression(expression, new { });
            return SetupInternal(expression, expr, times);
        }

        public AutoMock<T> Setup<TParam1, TParam2, TParam3, TParam4, TResult, TAnon>(
            Expression<Func<T, Func<TParam1, TParam2, TParam3, TParam4, TResult>>> expression, TAnon paramData, Times? times = null)
            where TAnon : class // Doing TAnon : class to avoid overload resolution issues
        {
            var expr = funcExpression.GetExpression(expression, paramData);
            return SetupInternal(expression, expr, times);
        }

        public AutoMock<T> Setup<TParam1, TParam2, TParam3, TParam4, TResult, TParam5>(
            Expression<Func<T, Func<TParam1, TParam2, TParam3, TParam4, TResult, TParam5>>> expression, Times? times = null)
        {
            var expr = funcExpression.GetExpression(expression, new { });
            return SetupInternal(expression, expr, times);
        }

        public AutoMock<T> Setup<TParam1, TParam2, TParam3, TParam4, TParam5, TResult, TAnon>(
            Expression<Func<T, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TResult>>> expression,
            TAnon paramData, Times? times = null)
            where TAnon : class // Doing TAnon : class to avoid overload resolution issues
        {
            var expr = funcExpression.GetExpression(expression, paramData);
            return SetupInternal(expression, expr, times);
        }

        public AutoMock<T> Setup<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TResult>(
    Expression<Func<T, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>>> expression, Times? times = null)
        {
            var expr = funcExpression.GetExpression(expression, new { });
            return SetupInternal(expression, expr, times);
        }

        public AutoMock<T> Setup<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TResult, TAnon>(
            Expression<Func<T, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>>> expression,
            TAnon paramData, Times? times = null)
            where TAnon : class // Doing TAnon : class to avoid overload resolution issues
        {
            var expr = funcExpression.GetExpression(expression, paramData);
            return SetupInternal(expression, expr, times);
        }

        public AutoMock<T> Setup<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TResult>(
Expression<Func<T, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TResult>>> expression, Times? times = null)
        {
            var expr = funcExpression.GetExpression(expression, new { });
            return SetupInternal(expression, expr, times);
        }

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
}
