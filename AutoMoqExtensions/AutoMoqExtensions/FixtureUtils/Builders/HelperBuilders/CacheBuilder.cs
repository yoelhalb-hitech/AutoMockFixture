
namespace AutoMoqExtensions.FixtureUtils.Builders.HelperBuilders;

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
        if(existing.HasValue) return existing.Value;

        return new NoSpecimen();
    }
}
