namespace AutoMoqExtensions.FixtureUtils.Requests.SpecialRequests;
internal abstract class OneOfMultipleRequest : BaseTracker
{
    public OneOfMultipleRequest(Type request, int index, bool? autoMock, ITracker? tracker) : base(tracker)
    {
        Request = request;
        Index = index;
        AutoMock = autoMock;
    }


    public Type Request { get; }
    public int Index { get; }
    public bool? AutoMock { get; }
    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Request, Index, AutoMock);
    public override bool IsRequestEquals(ITracker other)
        => base.IsRequestEquals(other) && other is OneOfMultipleRequest otherRequest && otherRequest.Request == Request
            && otherRequest.Index == Index && otherRequest.AutoMock == AutoMock;
}
