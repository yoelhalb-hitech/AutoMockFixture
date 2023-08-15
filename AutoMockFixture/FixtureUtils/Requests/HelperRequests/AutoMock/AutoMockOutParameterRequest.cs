using AutoMockFixture.FixtureUtils.Requests.HelperRequests.NonAutoMock;

namespace AutoMockFixture.FixtureUtils.Requests.HelperRequests.AutoMock;

internal record AutoMockOutParameterRequest : OutParameterRequest, IAutoMockRequest
{
    public AutoMockOutParameterRequest(Type declaringType, MethodInfo methodInfo,
            ParameterInfo parameterInfo, Type parameterType, ITracker? tracker)
        : base(declaringType, methodInfo, parameterInfo, parameterType, tracker)
    {
    }

    public override bool IsRequestEquals(ITracker other)
        => other is AutoMockOutParameterRequest && base.IsRequestEquals(other);
}
