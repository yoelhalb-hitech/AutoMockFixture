
namespace AutoMockFixture.FixtureUtils.Commands;

internal class CacheCommand : ISpecimenCommand
{
    public CacheCommand(Cache cache)
    {
        Cache = cache;
    }

    public Cache Cache { get; }

    public void Execute(object specimen, ISpecimenContext context)
    {
        // NOTE: We only need the `CacheCommand` when the request isn't a `BaseTracker` as `BaseTracker` handles it already
        if (specimen is null) return; // Have no way of knowing what type is the request, plus for null it doesn't make a difference caching...

        Cache.AddIfNeeded(specimen.GetType(), specimen);
    }
}
