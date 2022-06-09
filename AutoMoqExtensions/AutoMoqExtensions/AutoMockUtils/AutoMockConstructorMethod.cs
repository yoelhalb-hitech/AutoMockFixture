using AutoFixture.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AutoMoqExtensions.AutoMockUtils
{
    internal class AutoMockConstructorMethod : IMethod
    {
        private readonly ConstructorInfo ctor;

        internal AutoMockConstructorMethod(ConstructorInfo ctor, ParameterInfo[] paramInfos)
        {
            this.ctor = ctor ?? throw new ArgumentNullException(nameof(ctor));
            this.Parameters = paramInfos ?? throw new ArgumentNullException(nameof(paramInfos));
        }

        public IEnumerable<ParameterInfo> Parameters { get; }

        public object Invoke(IEnumerable<object> parameters)
        {
            var paramsArray = new object[] { parameters.ToArray() };
            return this.ctor.Invoke(paramsArray);
        }
        
    }
}
