using AutoMockFixture.AutoMockUtils;
using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;

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

        if (dependencyRequest.Request.IsAbstract || dependencyRequest.Request.IsInterface)
        {
            // Can't leave it for the relay as we want the dependencies mocked correctly
            var result = TryAutoMock(dependencyRequest, context);

            dependencyRequest.SetResult(result, this);

            return result;
        }

        if (AutoMockHelpers.IsAutoMock(dependencyRequest.Request) || AutoMockHelpers.MockRequestSpecification.IsSatisfiedBy(dependencyRequest.Request))
        {
            var inner = AutoMockHelpers.IsAutoMock(dependencyRequest.Request)
                                ? AutoMockHelpers.GetMockedType(dependencyRequest.Request)!
                                : dependencyRequest.Request.GenericTypeArguments.First();

            var result = TryAutoMock(dependencyRequest, context, inner);

            object? autoMock = AutoMockHelpers.GetFromObj(result);
            if (autoMock is null) autoMock = new NoSpecimen();

            dependencyRequest.SetResult(autoMock, this);
            return autoMock;
        }

        if (!AutoMockHelpers.IsAutoMockAllowed(dependencyRequest.Request)
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
            if (specimen is NoSpecimen || specimen is OmitSpecimen) return TryAutoMock(dependencyRequest, context);

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
            return TryAutoMock(dependencyRequest, context);
        }
    }

    private object? TryAutoMock(AutoMockDependenciesRequest dependencyRequest, ISpecimenContext context, Type? type = null)
    {
        var requestedType = type ?? dependencyRequest.Request;

        // We don't want to end up in recursion...
        if (dependencyRequest.GetParentsOnCurrentLevel().Any(t => t.GetType() == typeof(AutoMockRequest))
                        || !AutoMockHelpers.IsAutoMockAllowed(requestedType))
            return new NoSpecimen();

        try
        {
            var autoMockRequest = new AutoMockRequest(requestedType, dependencyRequest)
            {
                // We want MockShouldCallbase so to get ctor dependencies and also because AutoMockDependencies is a SUT
                // It should automatically revert to the MockShouldCallbase on the StartTracker for the dependecies

                MockShouldCallbase = true
            };

            var autoMockResult = context.Resolve(autoMockRequest);

            return autoMockResult;
        }
        catch
        {
            return new NoSpecimen();
        }
    }
}
