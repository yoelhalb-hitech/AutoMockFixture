using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Requests
{
    internal class AutoMockDependenciesRequest : IEquatable<AutoMockDependenciesRequest>
    {
        public AutoMockDependenciesRequest(Type request)
        {
            Request = request;
        }

        public Type Request { get; }
        public override bool Equals(object obj) 
            => obj is AutoMockDependenciesRequest other ? this.Equals(other) : base.Equals(obj);

        public override int GetHashCode() => HashCode.Combine(Request);
        public bool Equals(AutoMockDependenciesRequest other) => other is not null && other.Request == Request;
    }
}
