using AutoMoqExtensions.FixtureUtils.Requests;
using AutoMoqExtensions.FixtureUtils.Requests.HelperRequests.NonAutoMock;
using AutoMoqExtensions.FixtureUtils.Requests.MainRequests;
using AutoMoqExtensions.FixtureUtils.Specifications;

namespace AutoMoqExtensions.FixtureUtils.Builders.HelperBuilders;

internal class FieldBuilder : ISpecimenBuilder
{
    private static readonly AutoMockableSpecification autoMockableSpecification = new();
    
    public object? Create(object request, ISpecimenContext context)
    {
        if (request is not FieldRequest fieldRequest) return new NoSpecimen();

        var type = fieldRequest.FieldInfo.FieldType;

        if (!autoMockableSpecification.IsSatisfiedBy(type) || !fieldRequest.ShouldAutoMock)
        {
            object newRequest = fieldRequest.IsInAutoMockChain || fieldRequest.IsInAutoMockDepnedencyChain
                                    ? new AutoMockDependenciesRequest(type, fieldRequest)
                                    : new NonAutoMockRequest(type, fieldRequest);
            var result = context.Resolve(newRequest);
            fieldRequest.SetResult(result);
            return result;
        }

        var specimen = context.Resolve(new AutoMockRequest(type, fieldRequest));

        if (specimen is NoSpecimen || specimen is OmitSpecimen || specimen is null)
        {
            fieldRequest.SetResult(specimen);
            return specimen;
        }

        fieldRequest.SetCompleted();

        return specimen;
    }
}
