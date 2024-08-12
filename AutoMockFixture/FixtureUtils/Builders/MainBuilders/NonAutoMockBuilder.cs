using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;
using AutoMockFixture.FixtureUtils.Specifications;

namespace AutoMockFixture.FixtureUtils.Builders.MainBuilders;

internal class NonAutoMockBuilder : ISpecimenBuilder
{
    public NonAutoMockBuilder(ISpecimenBuilder builder, IAutoMockHelpers autoMockHelpers)
    {
        Builder = builder ?? throw new ArgumentNullException(nameof(builder));
        AutoMockHelpers = autoMockHelpers ?? throw new ArgumentNullException(nameof(autoMockHelpers));
    }

    public ISpecimenBuilder Builder { get; }
    public IAutoMockHelpers AutoMockHelpers { get; }

    public object? Create(object request, ISpecimenContext context)
    {
        if (request is not NonAutoMockRequest nonMockRequest) return new NoSpecimen();
        if (new ForceAutoMockSpecification(AutoMockHelpers).IsSatisfiedBy(nonMockRequest)) return new NoSpecimen();

        // Send all types that we want to leave for AutoFixture to it
        if (!AutoMockHelpers.IsAllowed(nonMockRequest.Request) || typeof(System.Delegate).IsAssignableFrom(nonMockRequest.Request))
        {
            // Note that IEnumerable etc. should already be handled in the special builders
            var result = context.Resolve(nonMockRequest.Request);
            nonMockRequest.SetResult(result, this);
            return result;
        }

        var specimen = Builder.Create(request, context);
        if (specimen is NoSpecimen || specimen is OmitSpecimen)
            return specimen;

        nonMockRequest.SetResult(specimen, this);

        return specimen;
    }
}
