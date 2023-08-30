
using AutoMockFixture.FixtureUtils.Requests;

namespace AutoMockFixture.FixtureUtils.Builders.HelperBuilders;

internal class CacheBuilder : ISpecimenBuilder
{
    public CacheBuilder(Cache cache)
    {
        Cache = cache;
    }

    public Cache Cache { get; }

    public object? Create(object request, ISpecimenContext context)
    {
        var existing = Cache.Get(request);
        if (existing.HasValue)
        {
            if (request is ITracker tracker) tracker.SetResult(existing.Value, this);

            return existing.Value;
        }

        return new NoSpecimen();
    }
}
