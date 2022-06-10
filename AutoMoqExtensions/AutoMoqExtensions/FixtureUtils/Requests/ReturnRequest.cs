using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Requests
{
    internal class ReturnRequest : IEquatable<ReturnRequest>
    {
        public ReturnRequest(Type declaringType, MethodInfo methodInfo)
        {
            DeclaringType = declaringType;
            MethodInfo = methodInfo;
        }

        public Type DeclaringType { get; }
        public MethodInfo MethodInfo { get; }

        public override bool Equals(object obj) 
            => obj is ReturnRequest other ? this.Equals(other) : base.Equals(obj);

        public override int GetHashCode() => HashCode.Combine(DeclaringType, MethodInfo);

        public virtual bool Equals(ReturnRequest other)
            => other is not null && this.DeclaringType == other.DeclaringType && this.MethodInfo == other.MethodInfo;       
    }
}
