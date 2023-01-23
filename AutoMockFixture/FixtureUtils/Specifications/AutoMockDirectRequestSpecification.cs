using AutoMockFixture.AutoMockUtils;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;

namespace AutoMockFixture.FixtureUtils.Specifications;

internal class AutoMockDirectRequestSpecification : IRequestSpecification
{
    public bool IsSatisfiedBy(object request)
    {
        var mockRequest = request as AutoMockDirectRequest;
        if (mockRequest is null || !AutoMockHelpers.IsAutoMock(mockRequest.Request)) return false;

        // We don't check if it is AutoMockable, this is the job of the one creating a DirectRequest,
        //          we want to be able to try to force cretaing an AutoMock, should all other options fail
        return true;
    }
}
