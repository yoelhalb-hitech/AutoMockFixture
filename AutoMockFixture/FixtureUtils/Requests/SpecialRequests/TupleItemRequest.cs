
namespace AutoMockFixture.FixtureUtils.Requests.SpecialRequests;

// TODO... now assumming that we won't overload by tuple (not tuple lentgh and not tuple item types)
internal record TupleItemRequest : OneOfMultipleRequest
{
    public TupleItemRequest(Type request, IRequestWithType outerRequest, int index) : base(request, outerRequest, index)
    {
    }

    public override string InstancePath => $"({"".PadLeft(Index, ',')})";

    public override bool IsRequestEquals(ITracker other) => other is TupleItemRequest && base.IsRequestEquals(other);
}
