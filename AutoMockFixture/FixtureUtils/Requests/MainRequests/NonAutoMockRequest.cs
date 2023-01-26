
namespace AutoMockFixture.FixtureUtils.Requests.MainRequests;

internal class NonAutoMockRequest : TrackerWithFixture, IRequestWithType, IFixtureTracker
{
    public NonAutoMockRequest(Type request, ITracker tracker) : base(tracker.StartTracker.Fixture, tracker)
    {
        Request = request;
        if (tracker is null) throw new Exception("Either tracker or fixture must be provided");
    }

    public NonAutoMockRequest(Type request, IAutoMockFixture fixture) : base(fixture, null)
    {
        Request = request;
    }

    public override string InstancePath => "";
    public override bool MockDependencies => false;
    public Type Request { get; }

    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Request);

    public override bool IsRequestEquals(ITracker other)
        => base.IsRequestEquals(other) && other is NonAutoMockRequest otherRequest && otherRequest.Request == Request;
}
