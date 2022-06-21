using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMoqExtensions.FixtureUtils.Requests
{
    public interface IFixtureTracker : ITracker
    {
        public AutoMockFixture Fixture { get; }
        public bool MockShouldCallbase { get; }
    }
}
