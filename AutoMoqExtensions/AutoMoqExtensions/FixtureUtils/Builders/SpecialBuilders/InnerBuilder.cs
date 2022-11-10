using AutoMoqExtensions.AutoMockUtils;
using AutoMoqExtensions.FixtureUtils.Requests;
using AutoMoqExtensions.FixtureUtils.Requests.MainRequests;
using AutoMoqExtensions.FixtureUtils.Requests.SpecialRequests;

namespace AutoMoqExtensions.FixtureUtils.Builders.SpecialBuilders;

internal class InnerBuilder : ISpecimenBuilder
{
    public object? Create(object request, ISpecimenContext context)
    {
        if (request is not InnerRequest innerRequest) return new NoSpecimen();

        var type = innerRequest.Request;

        IRequestWithType newRequest = innerRequest.OuterRequest switch
        {
            { } when AutoMockHelpers.IsAutoMock(type) => new AutoMockDirectRequest(type, innerRequest),
            AutoMockDependenciesRequest => new AutoMockDependenciesRequest(type, innerRequest),
            AutoMockRequest => new AutoMockRequest(type, innerRequest),
            NonAutoMockRequest => new NonAutoMockRequest(type, innerRequest),
            _ => throw new NotSupportedException(),
        };

        var specimen = context.Resolve(newRequest);

        newRequest.SetResult(specimen);

        if (specimen is NoSpecimen || specimen is OmitSpecimen)
        {            
            return new NoSpecimen(); // Let the system handle it
        }

        return specimen;       
    }
}
