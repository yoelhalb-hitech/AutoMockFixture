using AutoMoqExtensions.AutoMockUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Requests
{
    internal class AutoMockRequest : TrackerWithFixture, IAutoMockRequest, IDisposable, IFixtureTracker, IRequestWithType
    {
        public AutoMockRequest(Type request, ITracker tracker) : base(tracker.StartTracker.Fixture, tracker)
        {
            Request = request;
            if (tracker is null) throw new Exception("Either tracker or fixture must be provided");
        }

        public AutoMockRequest(Type request, AutoMockFixture fixture) : base(fixture, null)
        {
            Request = request;
        }


        public virtual Type Request { get; }
        public virtual bool BypassChecks { get; set; }
        public virtual bool? NoMockDependencies { get; set; }

        public override string InstancePath => "";


        public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Request, BypassChecks, NoMockDependencies);

        public override bool IsRequestEquals(ITracker other)
            => other is AutoMockRequest request 
                && request.Request == Request && request.BypassChecks == BypassChecks 
                && request.NoMockDependencies == NoMockDependencies
                && base.IsRequestEquals(other);

        public void Dispose() => SetCompleted();
    }
}
