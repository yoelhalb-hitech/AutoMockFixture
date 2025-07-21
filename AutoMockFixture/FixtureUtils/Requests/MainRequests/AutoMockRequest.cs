using System.Diagnostics.CodeAnalysis;

namespace AutoMockFixture.FixtureUtils.Requests.MainRequests;

internal record AutoMockRequest : TrackerWithFixture, IAutoMockRequest, IDisposable, IFixtureTracker, IRequestWithType
{
    [SetsRequiredMembers]
    public AutoMockRequest(Type request, ITracker tracker) : base(request, tracker) { }

    [SetsRequiredMembers]
    public AutoMockRequest(Type request, IAutoMockFixture fixture) : base(request, fixture) { }


    public virtual bool BypassChecks { get; set; }

    public override bool MockDependencies => StartTracker is null || StartTracker is AutoMockRequest ? true : StartTracker.MockDependencies; // Avoid stack overflow

    public override bool IsRequestEquals(ITracker other)
        => other is AutoMockRequest request
            && request.Request == Request && request.BypassChecks == BypassChecks
            && base.IsRequestEquals(other);

    public void Dispose() => SetCompleted((ISpecimenBuilder?)null);
}
