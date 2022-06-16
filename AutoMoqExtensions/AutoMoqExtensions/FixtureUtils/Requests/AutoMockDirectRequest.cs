using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Requests
{
    internal class AutoMockDirectRequest : BaseTracker, 
                IEquatable<AutoMockDirectRequest>, IAutoMockRequest, IFixtureTracker, IDisposable
    {
        public AutoMockDirectRequest(Type request, ITracker tracker) : base(tracker)
        {
            Request = request;
            if (tracker is not null) Fixture = tracker.StartTracker.Fixture;
            else throw new Exception("Either tracker or fixture must be provided");
        }

        public AutoMockDirectRequest(Type request, AutoMockFixture fixture) : base(null)
        {
            Request = request;
            Fixture = fixture;
        }

        public virtual Type Request { get; }

        public override string InstancePath => "";
        public AutoMockFixture Fixture { get; }


        public override bool Equals(object obj)
            => obj is AutoMockDirectRequest other ? this.Equals(other) : base.Equals(obj);

        public override int GetHashCode() => HashCode.Combine(Request);
        public bool Equals(AutoMockDirectRequest other) => other is not null && other.Request == Request;

        public void Dispose() => SetCompleted();
    }
}
