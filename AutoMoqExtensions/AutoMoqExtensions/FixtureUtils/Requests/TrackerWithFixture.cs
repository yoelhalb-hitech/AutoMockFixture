
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

    public override void SetResult(object? result)
    {
        base.SetResult(result);

        Fixture.Cache.AddIfNeeded(this, result);
    }
}
