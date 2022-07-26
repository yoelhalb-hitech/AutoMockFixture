using AutoMoqExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Requests
{
    internal class OutParameterRequest : BaseTracker
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

        public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), DeclaringType, 
                                                                MethodInfo, ParameterInfo, ParameterType);

        public override bool IsRequestEquals(ITracker other)
            => base.IsRequestEquals(other) 
            && other is OutParameterRequest outRequest && this.DeclaringType == outRequest.DeclaringType
            && this.MethodInfo == outRequest.MethodInfo
            && this.ParameterInfo == outRequest.ParameterInfo && this.ParameterType == outRequest.ParameterType;
    }
}
