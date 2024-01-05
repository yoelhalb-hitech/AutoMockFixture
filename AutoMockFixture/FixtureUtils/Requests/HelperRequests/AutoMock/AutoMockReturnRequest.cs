using AutoMockFixture.FixtureUtils.Requests.HelperRequests.NonAutoMock;

namespace AutoMockFixture.FixtureUtils.Requests.HelperRequests.AutoMock;

internal record AutoMockReturnRequest : ReturnRequest, IAutoMockRequest
{
    public AutoMockReturnRequest(Type declaringType, MethodInfo methodInfo, Type returnType,
                ITracker? tracker, string trackingPath)
        : base(declaringType, methodInfo, returnType, tracker, trackingPath)
    {
    }

    public override bool IsRequestEquals(ITracker other)
        => other is AutoMockReturnRequest && base.IsRequestEquals(other);
}
