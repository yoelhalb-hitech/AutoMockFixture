using AutoMoqExtensions.FixtureUtils.Requests;

namespace AutoMoqExtensions.FixtureUtils.Postprocessors;

internal class OneOfMultiplePostprocessor : ISpecimenBuilder
{
    public object? Create(object request, ISpecimenContext context)
    {
        if (request is not OneOfMultipleRequest itemRequest) return new NoSpecimen();

        var type = itemRequest.Request;

        object newRequest;      
        if (itemRequest.StartTracker.IsInAutoMockChain
                || itemRequest.StartTracker.IsInAutoMockDepnedencyChain)
        {
            if (itemRequest.AutoMock == false) newRequest = new AutoMockDependenciesRequest(type, itemRequest);
            else newRequest = new AutoMockRequest(type, itemRequest);
        }
        else
        {            
            if (itemRequest.AutoMock == true) newRequest = new AutoMockRequest(type, itemRequest);
            else newRequest = new NonAutoMockRequest(type, itemRequest);
        }

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
