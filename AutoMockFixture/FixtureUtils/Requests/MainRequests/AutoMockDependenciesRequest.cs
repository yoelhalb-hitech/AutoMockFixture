using System.Diagnostics.CodeAnalysis;

namespace AutoMockFixture.FixtureUtils.Requests.MainRequests;

internal record AutoMockDependenciesRequest : TrackerWithFixture, IFixtureTracker, IRequestWithType
{
    [SetsRequiredMembers]
    public AutoMockDependenciesRequest(Type request, ITracker tracker) : base(request, tracker) { }

    [SetsRequiredMembers]
    public AutoMockDependenciesRequest(Type request, IAutoMockFixture fixture) : base(request, fixture) { }

    public override bool MockDependencies => StartTracker is null || StartTracker is AutoMockDependenciesRequest ? true : StartTracker.MockDependencies; // Avoid stack overflow
}
