namespace AutoMoqExtensions.FixtureUtils.Requests.SpecialRequests;
internal abstract class OneOfMultipleRequest : InnerRequest
{
    public OneOfMultipleRequest(Type request, IRequestWithType outerRequest, int index) : base(request, outerRequest)
    {
        Index = index;
    }
   
    public int Index { get; }
    
    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Index);
    public override bool IsRequestEquals(ITracker other)
        => base.IsRequestEquals(other) && other is OneOfMultipleRequest otherRequest && otherRequest.Index == Index;
}
