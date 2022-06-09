using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Requests
{
    internal class AutoMockConstructorArgumentRequest : ConstructorArgumentRequest
    {
        public AutoMockConstructorArgumentRequest(Type declaringType, ParameterInfo parameterInfo) 
            : base(declaringType, parameterInfo)
        {
        }
    }
}
