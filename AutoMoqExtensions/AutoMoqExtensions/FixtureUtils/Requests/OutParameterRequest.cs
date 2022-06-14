using AutoMoqExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Requests
{
    internal class OutParameterRequest : BaseTracker, IEquatable<OutParameterRequest>
    {
        public OutParameterRequest(Type declaringType, MethodInfo methodInfo, 
            ParameterInfo parameterInfo, Type parameterType, ITracker? tracker) : base(tracker)
        {
            DeclaringType = declaringType;
            MethodInfo = methodInfo;
            ParameterInfo = parameterInfo;
            ParameterType = parameterType;
        }

        public virtual Type DeclaringType { get; }
        public virtual MethodInfo MethodInfo { get; }
        public virtual ParameterInfo ParameterInfo { get; }
        public virtual Type ParameterType { get; }

        public override string InstancePath => "." + MethodInfo.GetTrackingPath() + "->" + ParameterInfo;

        public override bool Equals(object obj) 
            => obj is OutParameterRequest other ? this.Equals(other) : base.Equals(obj);

        public override int GetHashCode() => HashCode.Combine(DeclaringType, MethodInfo, ParameterInfo, ParameterType);

        public virtual bool Equals(OutParameterRequest other)
            => other is not null && this.DeclaringType == other.DeclaringType
            && this.MethodInfo == other.MethodInfo      
            && this.ParameterInfo == other.ParameterInfo && this.ParameterType == other.ParameterType;       
    }
}
