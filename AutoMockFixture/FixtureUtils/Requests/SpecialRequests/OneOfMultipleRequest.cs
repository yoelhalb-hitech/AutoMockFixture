
namespace AutoMockFixture.FixtureUtils.Requests.SpecialRequests;

internal abstract record OneOfMultipleRequest : InnerRequest
{
    public OneOfMultipleRequest(Type request, IRequestWithType outerRequest, int index) : base(request, outerRequest)
    {
        Index = index;
    }

    public int Index { get; }
}
