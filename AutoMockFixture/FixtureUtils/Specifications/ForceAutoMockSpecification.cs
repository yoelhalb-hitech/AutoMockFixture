using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;

namespace AutoMockFixture.FixtureUtils.Specifications;

internal class ForceAutoMockSpecification : IRequestSpecification
{
    public ForceAutoMockSpecification(IAutoMockHelpers autoMockHelpers)
    {
        AutoMockHelpers = autoMockHelpers;
    }

    public IAutoMockHelpers AutoMockHelpers { get; }

    public bool IsSatisfiedBy(object request)
    {
        if (request is not AutoMockDependenciesRequest && request is not NonAutoMockRequest) // We specifically want to avoid `AutoMockRequest` and `AutMockDirectRequest`
            return false;

        var type = (request as IRequestWithType)!.Request;

        if(type.IsAbstract || type.IsInterface) return true;

        var isMock = AutoMockHelpers.IsAutoMock(type)
                        || AutoMockHelpers.MockRequestSpecification.IsSatisfiedBy(type);

        return isMock;
    }
}
