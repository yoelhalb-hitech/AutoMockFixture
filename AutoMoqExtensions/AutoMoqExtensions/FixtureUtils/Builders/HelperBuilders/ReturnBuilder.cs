using AutoMoqExtensions.FixtureUtils.Requests;
using AutoMoqExtensions.FixtureUtils.Requests.HelperRequests.NonAutoMock;
using AutoMoqExtensions.FixtureUtils.Requests.MainRequests;
using AutoMoqExtensions.FixtureUtils.Specifications;

namespace AutoMoqExtensions.FixtureUtils.Builders.HelperBuilders;

internal class ReturnBuilder : ISpecimenBuilder
{
    private static readonly AutoMockableSpecification autoMockableSpecification = new();

    public object? Create(object request, ISpecimenContext context)
    {
        if (request is not ReturnRequest returnRequest) return new NoSpecimen();

        var type = returnRequest.ReturnType;
        if (!autoMockableSpecification.IsSatisfiedBy(type) || !returnRequest.ShouldAutoMock)
        {
            object newRequest = returnRequest.IsInAutoMockChain || returnRequest.IsInAutoMockDepnedencyChain
                                    ? new AutoMockDependenciesRequest(type, returnRequest)
                                    : new NonAutoMockRequest(type, returnRequest);
            var result = context.Resolve(newRequest);
            returnRequest.SetResult(result);
            return result;
        }

        var specimen = context.Resolve(new AutoMockRequest(type, returnRequest));

        if (specimen is NoSpecimen || specimen is OmitSpecimen || specimen is null)
        {
            returnRequest.SetResult(specimen);
            return specimen;
        }

        returnRequest.SetCompleted();

        return specimen;
    }
}
