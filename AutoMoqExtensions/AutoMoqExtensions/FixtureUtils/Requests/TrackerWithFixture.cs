using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMoqExtensions.FixtureUtils.Requests
{
    /// <summary>
    /// For use with objects that don't have a start tracker
    /// </summary>
    internal class TrackerWithFixture : BaseTracker, IFixtureTracker
    {
        public TrackerWithFixture(AutoMockFixture fixture) : base(null)
        {
            Fixture = fixture;
        }

        public AutoMockFixture Fixture { get; }

        public override string InstancePath => "";
    }
}
