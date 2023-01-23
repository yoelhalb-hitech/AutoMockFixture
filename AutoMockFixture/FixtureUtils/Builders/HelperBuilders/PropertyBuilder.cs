using AutoMoqExtensions.FixtureUtils.Requests.HelperRequests.NonAutoMock;

namespace AutoMoqExtensions.FixtureUtils.Builders.HelperBuilders;
internal class PropertyBuilder : HelperBuilderBase<PropertyRequest>
{
    protected override object GetFullRequest(PropertyRequest request) => request.PropertyInfo;
    protected override Type GetRequest(PropertyRequest request) => request.PropertyInfo.PropertyType;
}
