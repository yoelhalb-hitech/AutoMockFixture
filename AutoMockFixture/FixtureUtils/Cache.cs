using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;
using AutoMockFixture.FixtureUtils.Specifications;

namespace AutoMockFixture.FixtureUtils;

internal class Cache
{
    public Cache(IAutoMockFixture fixture)
    {
        Fixture = fixture;
    }

    // NOTE:
    // We are doing it this way instead of the AutoFixture way of using a FixedBuilder,
    //      because we want to be able to do it lazy, as we do it also:
    //      1) based on the attributes and also for 2) different types of AutoMock etc. etc.
    public List<IRequestSpecification> CacheSpecifications { get; } = new();
    public Dictionary<TrackerWithFixture, object?> CacheDictionary { get; } = new();
    public IAutoMockFixture Fixture { get; }

    public (bool HasValue, object? Value) Get(object request)
    {
        if (request is not TrackerWithFixture tracker || !ReferenceEquals(tracker.Fixture, Fixture)) return (false, null);

        if (CacheDictionary.ContainsKey(tracker)) return (true, CacheDictionary[tracker]);

        // CAUTION: It can also be null not just true/false, so that's why we cannot do just `.FirstOrDefault()`
        if (CacheDictionary.Any(c => IsRequestEquals(tracker, c.Key)))
        {
            var existing = CacheDictionary.First(c => IsRequestEquals(tracker, c.Key));
            return (true, existing.Value);
        }

        return (false, null);
    }

    public void AddIfNeeded(object request, object? specimen)
    {
        if (request is not TrackerWithFixture tracker || !ReferenceEquals(tracker.Fixture, Fixture)) return;
        if (request is AutoMockRequest) return; // `AutoMockRequest` is just a forwarder so we will deal with it by the actual request

        if (!CacheSpecifications.Any(s => s.IsSatisfiedBy(tracker))) return;

        var existing = Get(tracker);
        if(existing.HasValue && object.Equals(existing.Value, specimen)) return;

        if(existing.HasValue) throw new Exception("A different object is already in cache");

        CacheDictionary[tracker] = specimen;
    }

    protected internal bool IsRequestEquals(TrackerWithFixture request, TrackerWithFixture inCache)
    {
        if(!ReferenceEquals(request.Fixture, inCache.Fixture)) return false;
        if (request is AutoMockRequest) return false; // `AutoMockRequest` is just a forwarder so we will deal with it by the actual request
        if (new ForceAutoMockSpecification(Fixture.AutoMockHelpers).IsSatisfiedBy(request)) return false; // Will be transformed to AutoMock so we will deal there

        if (request is AutoMockDirectRequest directRequest) return directRequest.IsRequestEquals(inCache);

        var start = request.StartTracker;
        var mockDepend = start.MockDependencies;

        // At this point it's either AutoMockDependeciesRequest or NonAutoMockRequest and we don't have to be concerned with AutoMocking
        if (mockDepend != inCache.StartTracker.MockDependencies) return false;

        // The default callBase (when `StartTracker.MockShouldCallBase` is null) is true for non `MockDependencies`
        //      because we try to emulate the real object as much as possible even in the case of a mock
        // while for `MockDependencies` it is false (unless the SUT is abstract but we anyway excluded that case already)
        var defaultCallBase = !mockDepend;

        // Verify the start tracker callbase status as it will affect the dependencies
        // Since we are not dealing with an AutoMock possibility we don't care on the object itself
        // Similarly we don't care on the difference for MockDependencies between the SUT and dependecies
        // This is due the fact that callbase is only relevent for mocks...
        if ((start.MockShouldCallBase ?? defaultCallBase) != (inCache.StartTracker.MockShouldCallBase ?? defaultCallBase)) return false;

        return request.Request == inCache.Request;
    }
}
