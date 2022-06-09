using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Requests
{
    internal class AutoMockRequest : IEquatable<AutoMockRequest>
    {
        public AutoMockRequest(Type request)
        {
            Request = request;
        }

        public Type Request { get; }
        public override bool Equals(object obj)
            => obj is AutoMockRequest other ? this.Equals(other) : base.Equals(obj);

        public override int GetHashCode() => HashCode.Combine(Request);
        public bool Equals(AutoMockRequest other) => other is not null && other.Request == Request;
    }
}
