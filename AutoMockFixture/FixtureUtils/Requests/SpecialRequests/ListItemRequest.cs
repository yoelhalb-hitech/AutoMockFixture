
namespace AutoMockFixture.FixtureUtils.Requests.SpecialRequests;

internal record ListItemRequest : OneOfMultipleRequest
{
    public ListItemRequest(Type request, IRequestWithType outerRequest, int index) : base(request, outerRequest, index)
    {
    }

    public override string InstancePath => $"[{Index}]";
}
