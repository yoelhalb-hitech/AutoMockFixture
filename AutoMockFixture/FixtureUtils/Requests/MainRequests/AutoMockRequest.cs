using System.Diagnostics.CodeAnalysis;

namespace AutoMockFixture.FixtureUtils.Requests.MainRequests;

internal record AutoMockRequest : TrackerWithFixture, IAutoMockRequest, IDisposable, IFixtureTracker, IRequestWithType
{
    [SetsRequiredMembers]
    public AutoMockRequest(Type request, ITracker tracker) : base(tracker.StartTracker.Fixture, tracker)
    {
        Request = request;
        if (tracker is null) throw new Exception("Either tracker or fixture must be provided");
    }

    [SetsRequiredMembers]
    public AutoMockRequest(Type request, IAutoMockFixture fixture) : base(fixture, null)
    {
        Request = request;
    }


    public required virtual Type Request { get; init; }
    public virtual bool BypassChecks { get; set; }

    public override string InstancePath => "";
    public override bool MockDependencies => StartTracker is null || StartTracker is AutoMockRequest ? true : StartTracker.MockDependencies; // Avoid stack overflow

    public override bool IsRequestEquals(ITracker other)
        => other is AutoMockRequest request
            && request.Request == Request && request.BypassChecks == BypassChecks
            && base.IsRequestEquals(other);

    public void Dispose() => SetCompleted((ISpecimenBuilder?)null);
}
