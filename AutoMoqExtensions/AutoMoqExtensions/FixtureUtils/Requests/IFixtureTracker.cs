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
        public bool? MockShouldCallbase { get; }
        /// <summary>
        /// Compare two <see cref="IFixtureTracker"/> objects to see if an object in the chain (but not necessarily they themselves) are to be considered equal requests
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsStartTrackerEquals(IFixtureTracker other);
    }
}
