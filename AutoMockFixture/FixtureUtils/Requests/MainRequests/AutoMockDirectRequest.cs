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
    public bool ShouldCallBase() => !GetInner().IsDelegate() // Moq does not allow to callbase for delegates
                    && (MockShouldCallBase // This is the only place where we concern ourselves with the request explicit `MockShouldCallBase` as in general we should follow what the user requested
                                ?? StartTracker.MockShouldCallBase
                                ?? StartTracker.Fixture.CallBase // TBH: This should be the same as the `StartTracker`
                                ?? !MockDependencies);

    public void Dispose() => SetCompleted((ISpecimenBuilder?)null);
}
