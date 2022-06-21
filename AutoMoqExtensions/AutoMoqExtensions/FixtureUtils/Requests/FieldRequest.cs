using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Requests
{
    internal class FieldRequest : BaseTracker
    {
        public FieldRequest(Type declaringType, FieldInfo fieldInfo, ITracker? tracker) : base(tracker)
        {
            DeclaringType = declaringType;
            FieldInfo = fieldInfo;
        }

        public virtual Type DeclaringType { get; }
        public virtual FieldInfo FieldInfo { get; }

        public override string InstancePath => "." + FieldInfo.Name;

        public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), DeclaringType, FieldInfo);

        public override bool IsRequestEquals(ITracker other)
         => other is FieldRequest otherRequest 
                && this.DeclaringType == otherRequest.DeclaringType && this.FieldInfo == otherRequest.FieldInfo;
    }
}
