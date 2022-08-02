using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Requests
{
    internal class AutoMockReturnRequest : ReturnRequest, IAutoMockRequest
    {
        public AutoMockReturnRequest(Type declaringType, MethodInfo methodInfo, Type returnType, ITracker? tracker) 
            : base(declaringType, methodInfo, returnType, tracker)
        {
        }

        public override bool IsRequestEquals(ITracker other)
            => other is AutoMockReturnRequest && base.IsRequestEquals(other);
    }
}
