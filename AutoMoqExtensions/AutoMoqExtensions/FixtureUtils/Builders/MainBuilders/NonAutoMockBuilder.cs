using AutoMoqExtensions.AutoMockUtils;
using AutoMoqExtensions.FixtureUtils.Requests.MainRequests;
using AutoMoqExtensions.FixtureUtils.Specifications;

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
