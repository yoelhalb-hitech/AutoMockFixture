using AutoMockFixture.FixtureUtils.Requests.HelperRequests.NonAutoMock;

namespace AutoMockFixture.FixtureUtils.Requests.HelperRequests.AutoMock;

internal record AutoMockConstructorArgumentRequest(Type declaringType, ParameterInfo parameterInfo, ITracker? tracker)
    : ConstructorArgumentRequest(declaringType, parameterInfo, tracker), IAutoMockRequest
{
}
