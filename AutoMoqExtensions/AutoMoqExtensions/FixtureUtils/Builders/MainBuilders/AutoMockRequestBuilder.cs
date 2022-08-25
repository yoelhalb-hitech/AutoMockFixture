using AutoMoqExtensions.AutoMockUtils;
using AutoMoqExtensions.FixtureUtils.Requests;
using AutoMoqExtensions.FixtureUtils.Requests.MainRequests;
using AutoMoqExtensions.FixtureUtils.Specifications;

namespace AutoMoqExtensions.FixtureUtils.FixtureUtils.Builders.MainBuilders;

internal class AutoMockRequestBuilder : ISpecimenBuilder
{
    private static readonly AutoMockableSpecification autoMockableSpecification = new();

    public object? Create(object request, ISpecimenContext context)
    {            
        if (request is not AutoMockRequest mockRequest)
                    return new NoSpecimen();

        if (!mockRequest.BypassChecks && !autoMockableSpecification.IsSatisfiedBy(mockRequest.Request))
        {
            try
            {
                IRequestWithType altRequest = !mockRequest.StartTracker.MockDependencies
                        ? new NonAutoMockRequest(mockRequest.Request, mockRequest)
                        : new AutoMockDependenciesRequest(mockRequest.Request, mockRequest);
                var altResult = context.Resolve(altRequest);

                if(altResult is not NoSpecimen)
                {
                    mockRequest.SetResult(altResult);
                    return altResult;
                }
            }
            catch{}

            // Handle it here so we should be able to set the result
            var otherResult = context.Resolve(mockRequest.Request);
               
            mockRequest.SetResult(otherResult);
            return otherResult;
        }

        var type = mockRequest.Request;
        if (!AutoMockHelpers.IsAutoMock(type)) type = AutoMockHelpers.GetAutoMockType(type);

        var directRequest = new AutoMockDirectRequest(type, mockRequest) 
        { 
            MockShouldCallbase = mockRequest.MockShouldCallbase,
        };

     
        var specimen = context.Resolve(directRequest);
        if (specimen is null)
        {
            mockRequest.SetResult(null);
            return specimen;
        }

        var t = specimen.GetType();
        if (specimen is NoSpecimen || specimen is OmitSpecimen || !AutoMockHelpers.IsAutoMock(t) || t != type)
        {
            // Try to unwrap it and see if we can get anything
            var unwrapResult = context.Resolve(AutoMockHelpers.GetMockedType(type));

            mockRequest.SetResult(unwrapResult);
            return unwrapResult;
        }

        var result = AutoMockHelpers.GetFromObj(specimen)!.GetMocked();
        mockRequest.SetCompleted(); // Result was set by the AutoMockPostprocessor

        return result;
    }
}
