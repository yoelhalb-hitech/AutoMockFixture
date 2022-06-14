using AutoMoqExtensions.AutoMockUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Requests
{
    internal class AutoMockRequest : BaseTracker, IEquatable<AutoMockRequest>
    {
        public AutoMockRequest(Type request, ITracker? tracker) : base(tracker)
        {
            Request = request;            
        }

        public virtual Type Request { get; }

        public override string InstancePath => "";

        public override bool Equals(BaseTracker other) => other is AutoMockRequest r && this.Equals(r);

        public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Request);
        public bool Equals(AutoMockRequest other) => base.Equals((BaseTracker)other) // Force the correct overload
                                                            && other.Request == Request;
    }
}
