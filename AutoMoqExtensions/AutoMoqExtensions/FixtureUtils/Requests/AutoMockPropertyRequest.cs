using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Requests
{
    internal class AutoMockPropertyRequest : PropertyRequest, IEquatable<AutoMockPropertyRequest>, IAutoMockRequest
    {
        public AutoMockPropertyRequest(Type declaringType, PropertyInfo propertyInfo, ITracker? tracker) 
            : base(declaringType, propertyInfo, tracker)
        {
        }

        public override bool Equals(PropertyRequest other)
            => other is AutoMockPropertyRequest r && this.Equals(r);

        public virtual bool Equals(AutoMockPropertyRequest other)
            => base.Equals((PropertyRequest)other); // Force the correct overload
    }
}
