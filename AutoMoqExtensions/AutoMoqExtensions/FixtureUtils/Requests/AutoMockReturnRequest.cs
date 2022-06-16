using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Requests
{
    internal class AutoMockReturnRequest : ReturnRequest, IEquatable<AutoMockReturnRequest>, IAutoMockRequest
    {
        public AutoMockReturnRequest(Type declaringType, MethodInfo methodInfo, Type returnType, ITracker? tracker) 
            : base(declaringType, methodInfo, returnType, tracker)
        {
        }

        public override bool Equals(ReturnRequest other)
            => other is AutoMockReturnRequest r && this.Equals(r);

        public virtual bool Equals(AutoMockReturnRequest other)
            => base.Equals((ReturnRequest)other); // Force the correct overload
    }
}
