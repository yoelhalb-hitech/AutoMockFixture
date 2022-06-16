using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Requests
{
    internal class AutoMockOutParameterRequest
        : OutParameterRequest, IEquatable<AutoMockOutParameterRequest>, IAutoMockRequest
    {
        public AutoMockOutParameterRequest(Type declaringType, MethodInfo methodInfo, 
                ParameterInfo parameterInfo, Type parameterType, ITracker? tracker) 
            : base(declaringType, methodInfo, parameterInfo, parameterType, tracker)
        {
        }

        public override bool Equals(OutParameterRequest other)
            => other is AutoMockOutParameterRequest r && this.Equals(r);

        public virtual bool Equals(AutoMockOutParameterRequest other)
            => base.Equals((OutParameterRequest)other); // Force the correct overload
    }
}
