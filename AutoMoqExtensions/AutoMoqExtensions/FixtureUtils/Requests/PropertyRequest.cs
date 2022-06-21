using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Requests
{
    internal class PropertyRequest : BaseTracker
    {
        public PropertyRequest(Type declaringType, PropertyInfo propertyInfo, ITracker? tracker) : base(tracker)
        {
            DeclaringType = declaringType;
            PropertyInfo = propertyInfo;
        }

        public virtual Type DeclaringType { get; }
        public virtual PropertyInfo PropertyInfo { get; }

        public override string InstancePath => "." + PropertyInfo.Name;

        public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), DeclaringType, PropertyInfo);

        public override bool IsRequestEquals(ITracker other)
            => other is PropertyRequest request 
                    && this.DeclaringType == request.DeclaringType 
                    && this.PropertyInfo == request.PropertyInfo;
    }
}
