using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;

namespace AutoMockFixture.FixtureUtils.Builders.SpecialBuilders;

/// <summary>
/// For last case when all other builders didn't work
/// </summary>
internal class LastResortBuilder : ISpecimenBuilder
{
    public LastResortBuilder(IAutoMockHelpers autoMockHelpers)
    {
        AutoMockHelpers = autoMockHelpers ?? throw new ArgumentNullException(nameof(autoMockHelpers));
    }

    public IAutoMockHelpers AutoMockHelpers { get; }

    public object Create(object request, ISpecimenContext context)
    {
        var newRequest = request switch
        {
            AutoMockRequest autoMockRequest => new AutoMockDependenciesRequest(autoMockRequest.Request, autoMockRequest),
            AutoMockDependenciesRequest autoMockDependenciesRequest => new NonAutoMockRequest(autoMockDependenciesRequest.Request, autoMockDependenciesRequest),
            IRequestWithType typedRequest => typedRequest.Request,
            _ => request,
        };

        if (newRequest is ITracker tracker)
        {
            var newType = newRequest.GetType();
            if (tracker.GetParentsOnCurrentLevel().Any(t => t.GetType() == newType))
            {
                // We are in recursion, this can happen since NoAutoMock builder can try to make a Mock and then come back here

                if (newRequest is IRequestWithType requestWithType) newRequest = requestWithType.Request;
                else throw new Exception("Unable to fulfill request");
            }
        }

        if (newRequest is Type type && AutoMockHelpers.IsAutoMock(type)) return new NoSpecimen(); // AutoMock should not be handled as a type only as one of the dedicated requests

        var specimen = context.Resolve(newRequest);

        // A specimen can only be the same as the request if the specimen is also a type
        // otherwise the recursion handler got stuck
        if (object.ReferenceEquals(specimen, newRequest) && newRequest is not Type)
            throw new Exception("Unable to create object");

        if (specimen is ITracker) return new NoSpecimen();// We obviously got back our own request (maybe a few levels deep)

        var requestTracker = request as ITracker;

        // We might get an AutoMock via the relay
        var autoMock = AutoMockHelpers.GetFromObj(specimen);
        if (autoMock is not null)
        {
            var newTracker = autoMock.Tracker as BaseTracker;
            if (newTracker is not null && requestTracker is not null) newTracker.SetParent(requestTracker);
        }
        if (requestTracker is not null) requestTracker.SetResult(specimen, this);

        return specimen;
    }
}
