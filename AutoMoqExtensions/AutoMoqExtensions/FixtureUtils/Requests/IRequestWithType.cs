using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Requests
{
    internal interface IRequestWithType : ITracker
    {
        public Type Request { get; }
    }
}
