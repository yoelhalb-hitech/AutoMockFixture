using AutoMoqExtensions.AutoMockUtils;
using AutoMoqExtensions.FixtureUtils.Requests.MainRequests;
using AutoMoqExtensions.FixtureUtils.Specifications;
using Moq;

namespace AutoMoqExtensions.FixtureUtils.Builders.MainBuilders;

internal class NonAutoMockBuilder : ISpecimenBuilder
{        
    public NonAutoMockBuilder(ISpecimenBuilder builder)
    {
        Builder = builder ?? throw new ArgumentNullException(nameof(builder));
    }

    public ISpecimenBuilder Builder { get; }

    public object? Create(object request, ISpecimenContext context)
    {
        if (request is not NonAutoMockRequest nonMockRequest)
            return new NoSpecimen();

        var isCreatable = !nonMockRequest.Request.IsAbstract && !nonMockRequest.Request.IsInterface;
        var isMock = AutoMockHelpers.IsAutoMock(nonMockRequest.Request)
            || (typeof(Mock).IsAssignableFrom(nonMockRequest.Request) && nonMockRequest.Request.IsGenericType);
        if (isMock || !isCreatable)
        {
            // Remember that the request might be IMock<> 
            var inner = !isMock
                            ? nonMockRequest.Request
                            : AutoMockHelpers.IsAutoMock(nonMockRequest.Request)
                                ? AutoMockHelpers.GetMockedType(nonMockRequest.Request)!
                                : nonMockRequest.Request.GenericTypeArguments.First();
                       
            var automockRequest = new AutoMockRequest(inner, nonMockRequest) 
            {
                MockShouldCallbase = !isMock || nonMockRequest.MockShouldCallbase == true
            };

            // TODO... we have to not mock the depedencies
            var result = context.Resolve(automockRequest);

            object? autoMock = AutoMockHelpers.GetFromObj(result);
            if (autoMock is null && isMock) autoMock = new NoSpecimen();

            var retValue = isMock ? autoMock : result;
            nonMockRequest.SetResult(retValue);
            return retValue;
        }

        // Send all types that we want to leave for AutoFixture to it
        if (!AutoMockHelpers.IsAutoMockAllowed(nonMockRequest.Request) || typeof(System.Delegate).IsAssignableFrom(nonMockRequest.Request))
        {
            // Note that IEnumerable etc. should already be handled in the special builders
            var result = context.Resolve(nonMockRequest.Request);
            nonMockRequest.SetResult(result);
            return result;
        }

        var specimen = Builder.Create(request, context);
        if (specimen is NoSpecimen || specimen is OmitSpecimen)
            return specimen;

        nonMockRequest.SetResult(specimen);

        return specimen;
    }
}
