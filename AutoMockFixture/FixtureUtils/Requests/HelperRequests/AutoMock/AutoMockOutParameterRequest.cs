using AutoMockFixture.FixtureUtils.Requests.HelperRequests.NonAutoMock;
using SequelPay.DotNetPowerExtensions.Reflection;

namespace AutoMockFixture.FixtureUtils.Requests.HelperRequests.AutoMock;

internal record AutoMockOutParameterRequest : OutParameterRequest, IAutoMockRequest
{
    public AutoMockOutParameterRequest(Type declaringType, MethodDetail methodInfo,
            ParameterInfo parameterInfo, Type parameterType, ITracker? tracker)
        : base(declaringType, methodInfo, parameterInfo, parameterType, tracker)
    {
    }
}
