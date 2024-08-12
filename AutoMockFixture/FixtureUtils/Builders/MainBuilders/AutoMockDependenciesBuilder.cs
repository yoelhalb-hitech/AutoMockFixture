using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;
using AutoMockFixture.FixtureUtils.Specifications;

namespace AutoMockFixture.FixtureUtils.Builders.MainBuilders;

internal class AutoMockDependenciesBuilder : ISpecimenBuilder
{
    public AutoMockDependenciesBuilder(ISpecimenBuilder builder, IAutoMockHelpers autoMockHelpers)
    {
        Builder = builder ?? throw new ArgumentNullException(nameof(builder));
        AutoMockHelpers = autoMockHelpers ?? throw new ArgumentNullException(nameof(autoMockHelpers));
    }

    public ISpecimenBuilder Builder { get; }
    public IAutoMockHelpers AutoMockHelpers { get; }

    public object? Create(object request, ISpecimenContext context)
    {
        if (request is not AutoMockDependenciesRequest dependencyRequest)
            return new NoSpecimen();

        if (new ForceAutoMockSpecification(AutoMockHelpers).IsSatisfiedBy(dependencyRequest)) return new NoSpecimen();

        if (!AutoMockHelpers.IsAllowed(dependencyRequest.Request)
            || typeof(System.Delegate).IsAssignableFrom(dependencyRequest.Request))
        {
            // Note that IEnumerable etc. should already be handled in the special builders
            var result = context.Resolve(dependencyRequest.Request);
            dependencyRequest.SetResult(result, this);
            return result;
        }

        try
        {
            var specimen = Builder.Create(request, context);
            if (specimen is NoSpecimen || specimen is OmitSpecimen) return specimen;

            if (specimen is null)
            {
                dependencyRequest.SetResult(specimen, this);
                return specimen;
            }

            if (specimen.GetType() != dependencyRequest.Request)
            {
                var result = new NoSpecimen();
                dependencyRequest.SetResult(result, this);
                return result;
            }

            dependencyRequest.SetResult(specimen, this);
            return specimen;
        }
        catch
        {
            return new NoSpecimen();
        }
    }
}
