using AutoMoqExtensions.AutoMockUtils;
using AutoMoqExtensions.FixtureUtils.Requests;
using AutoMoqExtensions.FixtureUtils.Requests.MainRequests;

namespace AutoMoqExtensions.FixtureUtils.Builders.SpecialBuilders;

/// <summary>
/// For last case when all other builders didn't work
/// </summary>
internal class LastResortBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        var newRequest = request switch
        {
            AutoMockRequest autoMockRequest => new AutoMockDependenciesRequest(autoMockRequest.Request, autoMockRequest),
            AutoMockDependenciesRequest autoMockDependenciesRequest => new NonAutoMockRequest(autoMockDependenciesRequest.Request, autoMockDependenciesRequest),
            IRequestWithType typedRequest => typedRequest.Request,
            _ => request,
        };

        var specimen = context.Resolve(newRequest);

        var reqeustTracker = request as ITracker;

        // We might get an AutoMock via the relay        
        var autoMock = AutoMockHelpers.GetFromObj(specimen);
        if (autoMock is not null)
        {
            var tracker = autoMock.Tracker as BaseTracker;
            if (tracker is not null && reqeustTracker is not null) tracker.SetParent(reqeustTracker);
        }
        if (reqeustTracker is not null) reqeustTracker.SetResult(specimen);

        return specimen;
    }
}
