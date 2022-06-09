using AutoFixture.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AutoMoqExtensions.AutoMockUtils
{ 
    internal class AutoMockConstructorWrapper : IMethod
    {
        internal AutoMockConstructorWrapper(IMethod method)
        {
            Method = method ?? throw new ArgumentNullException(nameof(method));
        }

        public IEnumerable<ParameterInfo> Parameters => Method.Parameters;
        public IMethod Method { get; }

        public object Invoke(IEnumerable<object> parameters)
        {
            var paramInfos = Parameters.ToArray();

            var paramToUse = parameters.Select((p, i) =>
            {
                if (p is IAutoMock a && !AutoMockHelpers.IsAutoMock(paramInfos[i].ParameterType))
                {
                    return a.GetMocked();
                }

                return p;
            });

            return Method.Invoke(paramToUse);
        }
  
    }
}
