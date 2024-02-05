using System.Diagnostics.CodeAnalysis;

namespace AutoMockFixture.FixtureUtils.Requests.MainRequests;

internal record AutoMockDirectRequest : TrackerWithFixture, IRequestWithType, IFixtureTracker, IDisposable
{
    [SetsRequiredMembers]
    public AutoMockDirectRequest(Type request, ITracker tracker) : base(tracker.StartTracker.Fixture, tracker)
    {
        Request = request;
        if (tracker is null) throw new Exception("Either tracker or fixture must be provided");
    }

    [SetsRequiredMembers]
    public AutoMockDirectRequest(Type request, IAutoMockFixture fixture) : base(fixture, null)
    {
        Request = request;
    }

    public required virtual Type Request { get; init; }

    public override string InstancePath => "";

    public override bool MockDependencies => StartTracker is null || StartTracker is AutoMockDirectRequest ? true : StartTracker.MockDependencies; // Avoid stack overflow

    public override bool IsRequestEquals(ITracker other)
        => other is AutoMockDirectRequest request
            && request.Request == Request
            && base.IsRequestEquals(other);

    public void Dispose() => SetCompleted((ISpecimenBuilder?)null);
}
