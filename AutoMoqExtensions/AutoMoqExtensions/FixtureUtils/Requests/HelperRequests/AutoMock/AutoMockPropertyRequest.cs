using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Requests
{
    internal class AutoMockPropertyRequest : PropertyRequest, IAutoMockRequest
    {
        public AutoMockPropertyRequest(Type declaringType, PropertyInfo propertyInfo, ITracker? tracker) 
            : base(declaringType, propertyInfo, tracker)
        {
        }

        public override bool IsRequestEquals(ITracker other) 
            => other is AutoMockPropertyRequest && base.IsRequestEquals(other);
    }
}
