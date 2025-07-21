using System.Diagnostics.CodeAnalysis;

namespace AutoMockFixture.FixtureUtils.Requests.MainRequests;

internal record NonAutoMockRequest : TrackerWithFixture, IRequestWithType, IFixtureTracker
{
    [SetsRequiredMembers]
    public NonAutoMockRequest(Type request, ITracker tracker) : base(request, tracker) { }

    [SetsRequiredMembers]
    public NonAutoMockRequest(Type request, IAutoMockFixture fixture) : base(request, fixture) { }

    public override bool MockDependencies => StartTracker is null || StartTracker is NonAutoMockRequest ? false : StartTracker.MockDependencies; // Avoid stack overflow
}
