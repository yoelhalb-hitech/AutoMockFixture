using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Requests
{
    internal class ConstructorArgumentRequest : IEquatable<ConstructorArgumentRequest>
    {
        public ConstructorArgumentRequest(Type declaringType, ParameterInfo parameterInfo)
        {
            DeclaringType = declaringType;
            ParameterInfo = parameterInfo;
        }

        public Type DeclaringType { get; }
        public ParameterInfo ParameterInfo { get; }

        public override bool Equals(object obj) 
            => obj is ConstructorArgumentRequest other ? this.Equals(other) : base.Equals(obj);

        public override int GetHashCode() => HashCode.Combine(DeclaringType, ParameterInfo);

        public bool Equals(ConstructorArgumentRequest other)
            => other is not null &&this.DeclaringType == other.DeclaringType && this.ParameterInfo == other.ParameterInfo;       
    }
}
