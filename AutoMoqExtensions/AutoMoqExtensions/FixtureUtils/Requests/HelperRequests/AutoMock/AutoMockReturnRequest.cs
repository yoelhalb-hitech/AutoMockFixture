using AutoMoqExtensions.FixtureUtils.Requests.HelperRequests.NonAutoMock;
using System.Reflection;

namespace AutoMoqExtensions.FixtureUtils.Requests.HelperRequests.AutoMock;

internal class AutoMockReturnRequest : ReturnRequest, IAutoMockRequest
{
    public AutoMockReturnRequest(Type declaringType, MethodInfo methodInfo, Type returnType, ITracker? tracker)
        : base(declaringType, methodInfo, returnType, tracker)
    {
    }

    public override bool IsRequestEquals(ITracker other)
        => other is AutoMockReturnRequest && base.IsRequestEquals(other);
}
