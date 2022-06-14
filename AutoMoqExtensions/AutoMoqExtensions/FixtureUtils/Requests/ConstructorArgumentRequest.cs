using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Requests
{
    internal class ConstructorArgumentRequest : BaseTracker, IEquatable<ConstructorArgumentRequest>
    {
        public ConstructorArgumentRequest(Type declaringType, ParameterInfo parameterInfo, ITracker? tracker)
            : base(tracker)
        {
            DeclaringType = declaringType;
            ParameterInfo = parameterInfo;
        }

        public virtual Type DeclaringType { get; }
        public virtual ParameterInfo ParameterInfo { get; }

        public override string InstancePath => "->" + ParameterInfo.Name;

        public override bool Equals(object obj) 
            => obj is ConstructorArgumentRequest other ? this.Equals(other) : base.Equals(obj);

        public override int GetHashCode() => HashCode.Combine(DeclaringType, ParameterInfo);

        public virtual bool Equals(ConstructorArgumentRequest other)
            => other is not null &&this.DeclaringType == other.DeclaringType && this.ParameterInfo == other.ParameterInfo;       
    }
}
