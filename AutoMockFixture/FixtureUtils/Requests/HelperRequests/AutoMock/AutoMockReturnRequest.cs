using AutoMockFixture.FixtureUtils.Requests.HelperRequests.NonAutoMock;

namespace AutoMockFixture.FixtureUtils.Requests.HelperRequests.AutoMock;

internal class AutoMockReturnRequest : ReturnRequest, IAutoMockRequest
{
    public AutoMockReturnRequest(Type declaringType, MethodInfo methodInfo, Type returnType,
                ITracker? tracker, string? customTrackingPath = null)
        : base(declaringType, methodInfo, returnType, tracker)
    {
        CustomTrackingPath = customTrackingPath;
    }

    public string? CustomTrackingPath { get; } // For example when setting up a readonly property via setup method

    public override string InstancePath
                => CustomTrackingPath is not null ?  "." + CustomTrackingPath : base.InstancePath;

    public override bool IsRequestEquals(ITracker other)
        => other is AutoMockReturnRequest && base.IsRequestEquals(other);
}
