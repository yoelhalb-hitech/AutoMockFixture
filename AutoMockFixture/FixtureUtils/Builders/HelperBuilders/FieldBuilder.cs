using AutoMoqExtensions.FixtureUtils.Requests.HelperRequests.NonAutoMock;

namespace AutoMoqExtensions.FixtureUtils.Builders.HelperBuilders;

internal class FieldBuilder : HelperBuilderBase<FieldRequest>
{
    protected override object GetFullRequest(FieldRequest request) => request.FieldInfo;

    protected override Type GetRequest(FieldRequest request) => request.FieldInfo.FieldType;
}
