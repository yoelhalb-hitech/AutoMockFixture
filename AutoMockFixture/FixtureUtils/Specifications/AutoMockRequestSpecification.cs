using AutoMoqExtensions.FixtureUtils.Requests.MainRequests;

namespace AutoMoqExtensions.FixtureUtils.Specifications;

internal class AutoMockRequestSpecification : IRequestSpecification
{
    private readonly AutoMockableSpecification autoMockableSpecification = new();
    public bool IsSatisfiedBy(object request)
    {
        var mockRequest = request as AutoMockRequest;
        if (mockRequest is null) return false;

        return autoMockableSpecification.IsSatisfiedBy(mockRequest.Request);
    }
}
