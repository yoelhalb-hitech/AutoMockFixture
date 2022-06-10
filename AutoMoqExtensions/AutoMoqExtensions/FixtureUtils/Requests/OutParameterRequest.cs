using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Requests
{
    internal class OutParameterRequest : IEquatable<OutParameterRequest>
    {
        public OutParameterRequest(Type declaringType, MethodInfo methodInfo, ParameterInfo parameterInfo, Type parameterType)
        {
            DeclaringType = declaringType;
            MethodInfo = methodInfo;
            ParameterInfo = parameterInfo;
            ParameterType = parameterType;
        }

        public Type DeclaringType { get; }
        public MethodInfo MethodInfo { get; }
        public ParameterInfo ParameterInfo { get; }
        public Type ParameterType { get; }

        public override bool Equals(object obj) 
            => obj is OutParameterRequest other ? this.Equals(other) : base.Equals(obj);

        public override int GetHashCode() => HashCode.Combine(DeclaringType, MethodInfo, ParameterInfo, ParameterType);

        public virtual bool Equals(OutParameterRequest other)
            => other is not null && this.DeclaringType == other.DeclaringType
            && this.MethodInfo == other.MethodInfo      
            && this.ParameterInfo == other.ParameterInfo && this.ParameterType == other.ParameterType;       
    }
}
