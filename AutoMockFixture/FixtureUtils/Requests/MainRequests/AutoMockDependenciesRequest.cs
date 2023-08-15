using System.Diagnostics.CodeAnalysis;

namespace AutoMockFixture.FixtureUtils.Requests.MainRequests;

internal record AutoMockDependenciesRequest : TrackerWithFixture, IFixtureTracker, IRequestWithType
{
    [SetsRequiredMembers]
    public AutoMockDependenciesRequest(Type request, ITracker tracker) : base(tracker.StartTracker.Fixture, tracker)
    {
        Request = request;
        if (tracker is null) throw new Exception("Either tracker or fixture must be provided");
    }

    [SetsRequiredMembers]
    public AutoMockDependenciesRequest(Type request, IAutoMockFixture fixture) : base(fixture, null)
    {
        Request = request;
    }

    public required virtual Type Request { get; init; }

    public override string InstancePath => "";
    public override bool MockDependencies => true;

    public override bool IsRequestEquals(ITracker other)
        => other is AutoMockDependenciesRequest request
                && request.Request == Request && base.IsRequestEquals(other);
}
