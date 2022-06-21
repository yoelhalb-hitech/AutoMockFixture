using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Requests
{
    internal class AutoMockOutParameterRequest : OutParameterRequest, IAutoMockRequest
    {
        public AutoMockOutParameterRequest(Type declaringType, MethodInfo methodInfo, 
                ParameterInfo parameterInfo, Type parameterType, ITracker? tracker) 
            : base(declaringType, methodInfo, parameterInfo, parameterType, tracker)
        {
        }

        public override bool IsRequestEquals(ITracker other) 
            => other is AutoMockOutParameterRequest && base.IsRequestEquals(other);
    }
}
