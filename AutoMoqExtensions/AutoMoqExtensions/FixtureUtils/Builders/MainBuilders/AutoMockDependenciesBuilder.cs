using AutoMoqExtensions.AutoMockUtils;
using AutoMoqExtensions.FixtureUtils.Requests;
using AutoMoqExtensions.FixtureUtils.Requests.MainRequests;
using AutoMoqExtensions.FixtureUtils.Specifications;
using Moq;

namespace AutoMoqExtensions.FixtureUtils.Builders.MainBuilders;

internal class AutoMockDependenciesBuilder : ISpecimenBuilder
{
    public AutoMockDependenciesBuilder(ISpecimenBuilder builder)
    {
        Builder = builder ?? throw new ArgumentNullException(nameof(builder));
    }

    public ISpecimenBuilder Builder { get; }

    public object? Create(object request, ISpecimenContext context)
    {
        if (request is not AutoMockDependenciesRequest dependencyRequest)
            return new NoSpecimen();

        if (dependencyRequest.Request.IsAbstract || dependencyRequest.Request.IsInterface)           
            return TryAutoMock(dependencyRequest, context);

        if (!AutoMockHelpers.IsAutoMockAllowed(dependencyRequest.Request) 
            || typeof(System.Delegate).IsAssignableFrom(dependencyRequest.Request))
        {
            // Note that IEnumerable etc. should already be handled in the special builders
            var result = context.Resolve(dependencyRequest.Request);
            dependencyRequest.SetResult(result);
            return result;
        }

        if(AutoMockHelpers.IsAutoMock(dependencyRequest.Request) || typeof(Mock).IsAssignableFrom(dependencyRequest.Request))
        {
            var inner = AutoMockHelpers.IsAutoMock(dependencyRequest.Request) 
                    ? AutoMockHelpers.GetMockedType(dependencyRequest.Request)!
                    : dependencyRequest.Request.IsGenericType 
                        ? dependencyRequest.Request.GenericTypeArguments.First()
                        : typeof(object);
            var automockRequest = new AutoMockRequest(inner, dependencyRequest) { MockShouldCallbase = true };

            var result = context.Resolve(automockRequest);
            dependencyRequest.SetResult(result);
            return result;
        }

        try
        {
            var specimen = Builder.Create(request, context);
            if (specimen is NoSpecimen || specimen is OmitSpecimen) return TryAutoMock(dependencyRequest, context);
            
            if (specimen is null)
            {
                dependencyRequest.SetResult(specimen);
                return specimen;
            }

            if (specimen.GetType() != dependencyRequest.Request)
            {
                var result = new NoSpecimen();
                dependencyRequest.SetResult(result);
                return result;
            }

            dependencyRequest.SetResult(specimen);
            return specimen;
        }
        catch
        {
            return TryAutoMock(dependencyRequest, context);
        }
    }

    private object? TryAutoMock(AutoMockDependenciesRequest dependencyRequest, ISpecimenContext context)
    {
        //If it's not the start request then it arrives here only if it isn't a valid AutoMock
        if (!Object.ReferenceEquals(dependencyRequest, dependencyRequest.StartTracker)  
                        || !AutoMockHelpers.IsAutoMockAllowed(dependencyRequest.Request))
            return new NoSpecimen();

        // Can't leave it for the relay as we want the dependencies mocked correctly
        try
        {
            // We want MockShouldCallbase so to get dependencies
            // It should automatically revert to the MockShouldCallbase on the StartTracker for the next objects
            // TODO... add tests                
            var autoMockRequest = new AutoMockRequest(dependencyRequest.Request, dependencyRequest) { MockShouldCallbase = true };
            var autoMockResult = context.Resolve(autoMockRequest);
            dependencyRequest.SetResult(autoMockResult);
            return autoMockResult;
        }
        catch
        {
            return new NoSpecimen();
        }
    }
}
