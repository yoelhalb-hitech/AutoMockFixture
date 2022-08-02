using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Requests
{
    internal class AutoMockFieldRequest : FieldRequest, IAutoMockRequest
    {
        public AutoMockFieldRequest(Type declaringType, FieldInfo fieldInfo, ITracker? tracker) 
            : base(declaringType, fieldInfo, tracker)
        {
        }

        public override bool IsRequestEquals(ITracker other) 
            => other is AutoMockFieldRequest && base.IsRequestEquals(other);
    }
}
