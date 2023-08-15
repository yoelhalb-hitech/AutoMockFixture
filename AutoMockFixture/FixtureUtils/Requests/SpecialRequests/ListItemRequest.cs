
namespace AutoMockFixture.FixtureUtils.Requests.SpecialRequests;

internal record ListItemRequest : OneOfMultipleRequest
{
    public ListItemRequest(Type request, IRequestWithType outerRequest, int index) : base(request, outerRequest, index)
    {
    }

    public override string InstancePath => $"[{Index}]";

    public override bool IsRequestEquals(ITracker other) => other is ListItemRequest && base.IsRequestEquals(other);
}
