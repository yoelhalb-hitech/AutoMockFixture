using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Requests
{
    internal class AutoMockFieldRequest : FieldRequest, IEquatable<AutoMockFieldRequest>, IAutoMockRequest
    {
        public AutoMockFieldRequest(Type declaringType, FieldInfo fieldInfo, ITracker? tracker) 
            : base(declaringType, fieldInfo, tracker)
        {
        }

        public override bool Equals(FieldRequest other)
            => other is AutoMockFieldRequest r && this.Equals(r);

        public virtual bool Equals(AutoMockFieldRequest other)
            => base.Equals((FieldRequest)other); // Force the correct overload
    }
}
