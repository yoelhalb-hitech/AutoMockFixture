using AutoMockFixture.FixtureUtils.Requests.HelperRequests.NonAutoMock;

namespace AutoMockFixture.FixtureUtils.Builders.HelperBuilders;

internal class ReturnBuilder : HelperBuilderBase<ReturnRequest>
{
    protected override object GetFullRequest(ReturnRequest request) => request.ReturnType;

    protected override Type GetRequest(ReturnRequest request) => request.ReturnType;
}
