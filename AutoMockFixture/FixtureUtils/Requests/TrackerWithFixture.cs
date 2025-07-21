using System.Diagnostics.CodeAnalysis;
using static AutoMockFixture.FixtureUtils.Requests.IFixtureTracker;

namespace AutoMockFixture.FixtureUtils.Requests;

/// <summary>
/// For use with objects that don't have a start tracker, and as a base for IFixtureTracker
/// </summary>
internal abstract record TrackerWithFixture : BaseTracker, IFixtureTracker, IRequestWithType
{
    [SetsRequiredMembers]
    public TrackerWithFixture(Type request, ITracker tracker) : base(tracker)
    {
        Request = request;
        if (tracker is null) throw new Exception("Either tracker or fixture must be provided");
        Fixture = tracker.StartTracker.Fixture;
    }

    [SetsRequiredMembers]
    public TrackerWithFixture(Type request, IAutoMockFixture fixture) : base((ITracker?)null)
    {
        Fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        Request = request;
    }

    public required virtual Type Request { get; init; }

    public virtual IAutoMockFixture Fixture { get; }

    public override string InstancePath => "";

    public bool? MockShouldCallBase { get; set; }

    public abstract bool MockDependencies { get; }

    public event EventHandler<UpdateData>? DataUpdated;

    public override void UpdateResult()
    {
        var currentMocks = this.allMocks ?? new List<WeakReference<IAutoMock>>();
        var currentPaths = this.childrensPaths ?? new Dictionary<string, List<WeakReference?>>();

        base.UpdateResult();

        var childrensPaths = this.childrensPaths?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
                                ?? new Dictionary<string, List<WeakReference?>>();

        // Assuming that all paths and mocks are add only
        var newMocks = this.allMocks?.Except(currentMocks).ToList() ?? new List<WeakReference<IAutoMock>>();
        var newPaths = childrensPaths.Keys.Except(currentPaths.Keys).ToList();

        var modifiedPaths = currentPaths
            .Select(c => new KeyValuePair<string, List<WeakReference?>>(c.Key, childrensPaths[c.Key].Except(c.Value).ToList()))
            .Where(kvp => kvp.Value.Any())
            .Union(childrensPaths.Where(c => newPaths.Contains(c.Key)))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        DataUpdated?.Invoke(this, new UpdateData { AutoMocks = newMocks, Paths = modifiedPaths });
    }
}
