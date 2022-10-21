
using System;
using System.Linq;
using static AutoMoqExtensions.FixtureUtils.Requests.IFixtureTracker;

namespace AutoMoqExtensions.FixtureUtils.Requests;

/// <summary>
/// For use with objects that don't have a start tracker, and as a base for IFixtureTracker
/// </summary>
internal abstract class TrackerWithFixture : BaseTracker, IFixtureTracker
{
    public TrackerWithFixture(AutoMockFixture fixture, ITracker? tracker = null) : base(tracker)
    {
        Fixture = fixture;
    }

    public virtual AutoMockFixture Fixture { get; }

    public override string InstancePath => "";

    public bool? MockShouldCallbase { get; set; }

    public abstract bool MockDependencies { get; }

    public override bool IsRequestEquals(ITracker other)
        => base.IsRequestEquals(other)
                && other is TrackerWithFixture tracker
                && IsFixtureTrackerEquals(tracker);

    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Fixture, MockShouldCallbase, MockDependencies);

    public virtual bool IsStartTrackerEquals(IFixtureTracker other) => IsFixtureTrackerEquals(other);

    protected virtual bool IsFixtureTrackerEquals(IFixtureTracker other)
                => Object.ReferenceEquals(other.Fixture, Fixture)
                    && other.MockDependencies == MockDependencies
                    && (other.MockShouldCallbase ?? other.StartTracker.MockShouldCallbase) == (MockShouldCallbase ?? StartTracker.MockShouldCallbase);
    
    public event EventHandler<UpdateData>? DataUpdated;

    public override void UpdateResult()
    {
        var currentMocks = this.allMocks ?? new List<IAutoMock>();
        var currentPaths = this.childrensPaths ?? new Dictionary<string, List<object?>>();

        base.UpdateResult();

        var childrensPaths = this.childrensPaths.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ?? new Dictionary<string, List<object?>>();

        // Assuming that all paths and mocks are add only
        var newMocks = this.allMocks?.Except(currentMocks).ToList() ?? new List<IAutoMock>();
        var newPaths = childrensPaths.Keys.Except(currentPaths.Keys).ToList();

        var modifiedPaths = currentPaths
            .Select(c => new KeyValuePair<string, List<object?>>(c.Key, childrensPaths[c.Key].Except(c.Value).ToList()))
            .Where(kvp => kvp.Value.Any())            
            .Union(childrensPaths.Where(c => newPaths.Contains(c.Key)))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        DataUpdated?.Invoke(this, new UpdateData { AutoMocks = newMocks, Paths = modifiedPaths });
    }

    public override void SetResult(object? result)
    {
        base.SetResult(result);

        Fixture.Cache.AddIfNeeded(this, result);
    }
}
