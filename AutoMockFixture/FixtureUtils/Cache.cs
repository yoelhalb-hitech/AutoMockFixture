using AutoMockFixture.FixtureUtils.Requests;

namespace AutoMockFixture.FixtureUtils;

internal class Cache
{
    // NOTE:
    // We are doing it this way instead of the AutoFixture way of using a FixedBuilder,
    //      because we want to be able to it lazy, as we do it also:
    //      1) based on the attributes and also for 2) different types of AutoMock etc. etc.
    public List<IRequestSpecification> CacheSpecifications { get; } = new List<IRequestSpecification>();
    public Dictionary<object, object?> CacheDictionary { get; } = new Dictionary<object, object?>();

    public (bool HasValue, object? Value) Get(object request)
    {
        if (CacheDictionary.ContainsKey(request)) return (true, CacheDictionary[request]);

        // TODO... we need to consider that even if the tracker.MockShouldCallBase is false,
        //  still the AutoMockMethodInvoker will still count it as true, but here it will be considered false by IsRequestEquals
        // Note that it can also be null not just true/false
        if (request is ITracker tracker && CacheDictionary.Any(c => (c.Key as ITracker)?.IsRequestEquals(tracker) == true))
        {
            var existing = CacheDictionary.First(c => (c.Key as ITracker)?.IsRequestEquals(tracker) == true);
            return (true, existing.Value);
        }

        return (false, null);
    }

    public void AddIfNeeded(object request, object? specimen)
    {
        if (!CacheSpecifications.Any(s => s.IsSatisfiedBy(request))) return;

        var existing = Get(request);
        if(existing.HasValue && object.Equals(existing.Value, specimen)) return;

        if(existing.HasValue) throw new Exception("A different object is already in cache");

        CacheDictionary[request] = specimen;
    }
}
