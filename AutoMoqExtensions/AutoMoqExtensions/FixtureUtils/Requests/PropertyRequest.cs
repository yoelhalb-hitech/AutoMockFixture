using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Requests
{
    internal class PropertyRequest : IEquatable<PropertyRequest>
    {
        public PropertyRequest(Type declaringType, PropertyInfo propertyInfo)
        {
            DeclaringType = declaringType;
            PropertyInfo = propertyInfo;
        }

        public Type DeclaringType { get; }
        public PropertyInfo PropertyInfo { get; }

        public override bool Equals(object obj) 
            => obj is PropertyRequest other ? this.Equals(other) : base.Equals(obj);

        public override int GetHashCode() => HashCode.Combine(DeclaringType, PropertyInfo);

        public virtual bool Equals(PropertyRequest other)
            => other is not null &&this.DeclaringType == other.DeclaringType && this.PropertyInfo == other.PropertyInfo;       
    }
}
