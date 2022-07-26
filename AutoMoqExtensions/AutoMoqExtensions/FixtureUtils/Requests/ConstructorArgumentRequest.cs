using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Requests
{
    internal class ConstructorArgumentRequest : BaseTracker
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

        public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), DeclaringType, ParameterInfo);

        public override bool IsRequestEquals(ITracker other)
            =>other is ConstructorArgumentRequest argumentRequest
                && this.DeclaringType == argumentRequest.DeclaringType 
                && this.ParameterInfo == argumentRequest.ParameterInfo
                && base.IsRequestEquals(other);       
    }
}
