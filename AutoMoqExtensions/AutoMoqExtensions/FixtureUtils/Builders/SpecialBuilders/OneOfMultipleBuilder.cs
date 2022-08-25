using AutoMoqExtensions.FixtureUtils.Requests.MainRequests;
using AutoMoqExtensions.FixtureUtils.Requests.SpecialRequests;

namespace AutoMoqExtensions.FixtureUtils.Builders.SpecialBuilders;

internal class OneOfMultipleBuilder : ISpecimenBuilder
{
    public object? Create(object request, ISpecimenContext context)
    {
        if (request is not OneOfMultipleRequest itemRequest) return new NoSpecimen();

        var type = itemRequest.Request;

        object newRequest = itemRequest.AutoMock == true
                                ? new AutoMockRequest(type, itemRequest)
                                : itemRequest.StartTracker.MockDependencies
                                    ? new AutoMockDependenciesRequest(type, itemRequest)
                                    : new NonAutoMockRequest(type, itemRequest);

        var specimen = context.Resolve(newRequest);

        if (specimen is NoSpecimen || specimen is OmitSpecimen || specimen is null)
        {
            itemRequest.SetResult(specimen);
            return new NoSpecimen(); // Let the system handle it
        }
        
        itemRequest.SetCompleted();

        return specimen;       
    }
}
