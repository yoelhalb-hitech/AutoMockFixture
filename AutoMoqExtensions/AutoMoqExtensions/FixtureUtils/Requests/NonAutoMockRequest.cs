namespace AutoMoqExtensions.FixtureUtils.Requests;

internal class NonAutoMockRequest : BaseTracker, IRequestWithType
{
    public NonAutoMockRequest(Type request, ITracker? tracker) : base(tracker)
    {
        Request = request;
    }
    
    public override string InstancePath => "";

    public Type Request { get; }

    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Request);

    public override bool IsRequestEquals(ITracker other)
        => other is NonAutoMockRequest otherRequest && otherRequest.Request == Request;
}
