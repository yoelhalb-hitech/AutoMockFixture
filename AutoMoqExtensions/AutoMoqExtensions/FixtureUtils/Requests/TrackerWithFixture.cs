using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMoqExtensions.FixtureUtils.Requests
{
    /// <summary>
    /// For use with objects that don't have a start tracker, and as a base for IFixtureTracker
    /// </summary>
    internal class TrackerWithFixture : BaseTracker, IFixtureTracker
    {
        public TrackerWithFixture(AutoMockFixture fixture, ITracker? tracker = null) : base(tracker)
        {
            Fixture = fixture;
        }

        public AutoMockFixture Fixture { get; }

        public override string InstancePath => "";

        public override bool IsRequestEquals(ITracker other)
            => other is TrackerWithFixture tracker
                    && Object.ReferenceEquals(tracker.Fixture, Fixture);

        public override void SetResult(object? result)
        {
            base.SetResult(result);

            Fixture.Cache.AddIfNeeded(this, result);
        }
    }
}
