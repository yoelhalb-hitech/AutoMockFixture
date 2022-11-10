namespace AutoMoqExtensions.FixtureUtils.Requests.SpecialRequests;
internal class InnerRequest : BaseTracker
{
    public InnerRequest(Type request, IRequestWithType outerRequest) : base(outerRequest)
    {
        Request = request;
        OuterRequest = outerRequest;
    }


    public Type Request { get; }
    public IRequestWithType OuterRequest { get; }

    public override string InstancePath => ""; // TODO... is this right for task and delegate?

    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Request, OuterRequest);
    public override bool IsRequestEquals(ITracker other)
        => base.IsRequestEquals(other) && other is InnerRequest otherRequest
            && otherRequest.Request == Request && otherRequest.OuterRequest == OuterRequest;
}
