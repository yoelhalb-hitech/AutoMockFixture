using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Requests
{
    internal class PropertyRequest : BaseTracker, IEquatable<PropertyRequest>
    {
        public PropertyRequest(Type declaringType, PropertyInfo propertyInfo, ITracker? tracker) : base(tracker)
        {
            DeclaringType = declaringType;
            PropertyInfo = propertyInfo;
        }

        public virtual Type DeclaringType { get; }
        public virtual PropertyInfo PropertyInfo { get; }

        public override string InstancePath => "." + PropertyInfo.Name;

        public override bool Equals(BaseTracker obj) 
                        => obj is PropertyRequest other && this.Equals(other);

        public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), DeclaringType, PropertyInfo);

        public virtual bool Equals(PropertyRequest other)
            => // base.Equals((BaseTracker)other) &&
            this.DeclaringType == other.DeclaringType && this.PropertyInfo == other.PropertyInfo;       
    }
}
