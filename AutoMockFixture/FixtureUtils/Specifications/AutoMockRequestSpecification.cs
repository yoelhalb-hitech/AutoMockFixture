using AutoMockFixture.AutoMockUtils;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;

namespace AutoMockFixture.FixtureUtils.Specifications;

internal class AutoMockRequestSpecification : IRequestSpecification
{
    public AutoMockRequestSpecification(IAutoMockHelpers autoMockHelpers)
    {
        AutoMockHelpers = autoMockHelpers;
    }

    private AutoMockableSpecification autoMockableSpecification => new AutoMockableSpecification(AutoMockHelpers);

    public IAutoMockHelpers AutoMockHelpers { get; }

    public bool IsSatisfiedBy(object request)
    {
        var mockRequest = request as AutoMockRequest;
        if (mockRequest is null) return false;

        return autoMockableSpecification.IsSatisfiedBy(mockRequest.Request);
    }
}
