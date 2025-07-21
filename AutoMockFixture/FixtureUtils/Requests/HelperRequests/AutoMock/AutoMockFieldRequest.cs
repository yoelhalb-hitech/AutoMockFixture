using AutoMockFixture.FixtureUtils.Requests.HelperRequests.NonAutoMock;

namespace AutoMockFixture.FixtureUtils.Requests.HelperRequests.AutoMock;

internal record AutoMockFieldRequest : FieldRequest, IAutoMockRequest
{
    public AutoMockFieldRequest(Type declaringType, FieldInfo fieldInfo, ITracker? tracker)
        : base(declaringType, fieldInfo, tracker)
    {
    }
}
