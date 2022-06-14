using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Requests
{
    internal class AutoMockDirectRequest : BaseTracker, IEquatable<AutoMockDirectRequest>
    {
        public AutoMockDirectRequest(Type request, ITracker? tracker) : base(tracker)
        {
            Request = request;
        }

        public virtual Type Request { get; }

        public override string InstancePath => "";

        public override bool Equals(object obj)
            => obj is AutoMockDirectRequest other ? this.Equals(other) : base.Equals(obj);

        public override int GetHashCode() => HashCode.Combine(Request);
        public bool Equals(AutoMockDirectRequest other) => other is not null && other.Request == Request;
    }
}
