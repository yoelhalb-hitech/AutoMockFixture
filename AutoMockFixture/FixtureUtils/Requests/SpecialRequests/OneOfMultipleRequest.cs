
namespace AutoMockFixture.FixtureUtils.Requests.SpecialRequests;
internal abstract record OneOfMultipleRequest : InnerRequest
{
    public OneOfMultipleRequest(Type request, IRequestWithType outerRequest, int index) : base(request, outerRequest)
    {
        Index = index;
    }

    public int Index { get; }

    public override bool IsRequestEquals(ITracker other)
        => base.IsRequestEquals(other) && other is OneOfMultipleRequest otherRequest && otherRequest.Index == Index;
}
