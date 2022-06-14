using Moq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions
{
    public partial class AutoMock<T>
    {
        #region MethodInfo
        IAutoMock IAutoMock.Setup(MethodInfo method, Times times) 
                => Setup(method, times);
        IAutoMock IAutoMock.Setup<TAnon>(MethodInfo method, TAnon paramData) where TAnon : class 
                => Setup(method, paramData);
        // Doing TAnon : class to avoid overload resolution issues
        IAutoMock IAutoMock.Setup<TAnon>(MethodInfo method, TAnon paramData, Times times) where TAnon : class
                => Setup(method, paramData, times);

        IAutoMock IAutoMock.Setup<TAnon, TResult>(MethodInfo method, TAnon paramData, TResult result) where TAnon : class
                => Setup<TAnon, TResult>(method, paramData, result);
        // Doing TAnon : class to avoid overload resolution issues
        IAutoMock IAutoMock.Setup<TAnon, TResult>(MethodInfo method, TAnon paramData, TResult result, Times times) where TAnon : class
                => Setup<TAnon, TResult>(method, paramData, result, times);

        #endregion

        #region string

        IAutoMock IAutoMock.Setup(string methodName) => Setup(methodName);
        IAutoMock IAutoMock.Setup(string methodName, Times times) => Setup(methodName, times);

        // Doing TAnon : class to avoid overload resolution issues
        IAutoMock IAutoMock.Setup<TAnon>(string methodName, TAnon paramData) where TAnon : class => Setup(methodName, paramData);

        // Doing TAnon : class to avoid overload resolution issues
        IAutoMock IAutoMock.Setup<TAnon>(string methodName, TAnon paramData, Times times) where TAnon : class
                => Setup(methodName, paramData, times);

        // Doing TAnon : class to avoid overload resolution issues


        IAutoMock IAutoMock.Setup<TAnon, TResult>(string methodName, TAnon paramData, TResult result) where TAnon : class
                => Setup(methodName, paramData, result);
        IAutoMock IAutoMock.Setup<TAnon, TResult>(string methodName, TAnon paramData, TResult result, Times times) where TAnon : class
                => Setup(methodName, paramData, result, times);

        #endregion
    }
}
