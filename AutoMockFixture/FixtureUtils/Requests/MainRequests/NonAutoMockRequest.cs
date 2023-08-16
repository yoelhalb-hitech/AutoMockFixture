using System.Diagnostics.CodeAnalysis;

namespace AutoMockFixture.FixtureUtils.Requests.MainRequests;

internal record NonAutoMockRequest : TrackerWithFixture, IRequestWithType, IFixtureTracker
{
    [SetsRequiredMembers]
    public NonAutoMockRequest(Type request, ITracker tracker) : base(tracker.StartTracker.Fixture, tracker)
    {
        Request = request;
        if (tracker is null) throw new Exception("Either tracker or fixture must be provided");
    }

    [SetsRequiredMembers]
    public NonAutoMockRequest(Type request, IAutoMockFixture fixture) : base(fixture, null)
    {
        Request = request;
    }

    public override string InstancePath => "";
    public override bool MockDependencies => StartTracker is null || StartTracker is NonAutoMockRequest ? false : StartTracker.MockDependencies; // Avoid stack overflow
    public required virtual Type Request { get; init;  }

    public override bool IsRequestEquals(ITracker other)
        => base.IsRequestEquals(other) && other is NonAutoMockRequest otherRequest && otherRequest.Request == Request;
}
