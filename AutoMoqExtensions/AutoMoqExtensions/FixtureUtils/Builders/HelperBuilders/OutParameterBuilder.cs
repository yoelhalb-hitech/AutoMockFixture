using AutoMoqExtensions.FixtureUtils.Requests;
using AutoMoqExtensions.FixtureUtils.Requests.HelperRequests.NonAutoMock;
using AutoMoqExtensions.FixtureUtils.Requests.MainRequests;
using AutoMoqExtensions.FixtureUtils.Specifications;

namespace AutoMoqExtensions.FixtureUtils.Builders.HelperBuilders;

internal class OutParameterBuilder : ISpecimenBuilder
{
    private static readonly AutoMockableSpecification autoMockableSpecification = new();

    public object? Create(object request, ISpecimenContext context)
    {
        if (request is not OutParameterRequest outRequest) return new NoSpecimen();

        // Out param types can be different than the ParameterInfo.ParameterType
        var type = outRequest.ParameterType;
        if (!autoMockableSpecification.IsSatisfiedBy(type) || !outRequest.ShouldAutoMock)
        {
            object newRequest = outRequest.IsInAutoMockChain || outRequest.IsInAutoMockDepnedencyChain
                                    ? new AutoMockDependenciesRequest(type, outRequest)
                                    : new NonAutoMockRequest(type, outRequest);
            var result = context.Resolve(newRequest);
            outRequest.SetResult(result);
            return result;
        }

        var specimen = context.Resolve(new AutoMockRequest(type, outRequest));

        if (specimen is NoSpecimen || specimen is OmitSpecimen || specimen is null)
        {
            outRequest.SetResult(specimen);
            return specimen;
        }

        outRequest.SetCompleted();

        return specimen;
    }
}
