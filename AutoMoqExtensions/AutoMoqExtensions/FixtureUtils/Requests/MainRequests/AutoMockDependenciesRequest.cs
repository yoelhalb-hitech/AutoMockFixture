
namespace AutoMoqExtensions.FixtureUtils.Requests.MainRequests;

internal class AutoMockDependenciesRequest : TrackerWithFixture, IFixtureTracker, IRequestWithType
{
    public AutoMockDependenciesRequest(Type request, ITracker tracker) : base(tracker.StartTracker.Fixture, tracker)
    {
        Request = request;
        if (tracker is null) throw new Exception("Either tracker or fixture must be provided");
    }

    public AutoMockDependenciesRequest(Type request, AutoMockFixture fixture) : base(fixture, null)
    {
        Request = request;
    }

    public virtual Type Request { get; }

    public override string InstancePath => "";
    public override bool MockDependencies => true;

    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Request);

    public override bool IsRequestEquals(ITracker other)
        => other is AutoMockDependenciesRequest request
                && request.Request == Request && base.IsRequestEquals(other);
}
