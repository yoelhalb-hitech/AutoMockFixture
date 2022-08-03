using AutoMoqExtensions.FixtureUtils.Requests.HelperRequests.NonAutoMock;
using System.Reflection;

namespace AutoMoqExtensions.FixtureUtils.Builders.HelperBuilders;

internal class OutParameterBuilder : HelperBuilderBase<OutParameterRequest>
{
    // Out param types can be different than the ParameterInfo.ParameterType, so we cannot use ParameterInfo
    protected override object GetFullRequest(OutParameterRequest request) => request.ParameterType;
    
    protected override Type GetRequest(OutParameterRequest request) => request.ParameterType;
}
