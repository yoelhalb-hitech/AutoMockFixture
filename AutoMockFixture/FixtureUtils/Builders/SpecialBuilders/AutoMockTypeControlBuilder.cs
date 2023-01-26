using AutoMockFixture.AutoMockUtils;
using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;

namespace AutoMockFixture.FixtureUtils.Builders.SpecialBuilders;

/// <summary>
/// Builder to build types that have been specified as always automock or never automock
/// </summary>
internal class AutoMockTypeControlBuilder : ISpecimenBuilder
{
    public object? Create(object request, ISpecimenContext context)
    {
        var typedRequest = request as IRequestWithType;
        var type = typedRequest?.Request ?? request as Type;
        if (type is null) return new NoSpecimen();

        var recursionContext = context as RecursionContext;
        var fixture = typedRequest?.StartTracker.Fixture ?? recursionContext?.Fixture;

        if (recursionContext is null && fixture is null) return new NoSpecimen();

        var requestHelper = new TypeControlHelper(fixture!, typedRequest, type, context);

        object? result = new NoSpecimen();
        if (recursionContext is not null && recursionContext.AutoMockTypeControl is not null)
            result = requestHelper.GetResult(recursionContext.AutoMockTypeControl);

        if (result is NoSpecimen && fixture is not null) result = requestHelper.GetResult(fixture.AutoMockTypeControl);

        if (result is not NoSpecimen) typedRequest ?.SetResult(result, this);

        return result;
    }

    internal class TypeControlHelper
    {
        private readonly IAutoMockFixture fixture;
        private readonly IRequestWithType? typedRequest;
        private readonly Type type;
        private readonly ISpecimenContext context;

        public TypeControlHelper(IAutoMockFixture fixture, IRequestWithType? typedRequest,
                                                                        Type type, ISpecimenContext context)
        {            
            this.fixture = fixture;
            this.typedRequest = typedRequest;
            this.type = type;
            this.context = context;
        }

        public object? GetResult(AutoMockTypeControl autoMockTypeControl)
        {           
            object? newRequest = GetRequest(autoMockTypeControl);

            if (newRequest is null) return new NoSpecimen();

            var specimen = context.Resolve(newRequest);

            // We might get an AutoMock via the relay        
            var autoMock = fixture.AutoMockHelpers.GetFromObj(specimen);
            if (autoMock is not null)
            {
                var tracker = autoMock.Tracker as BaseTracker;
                if (tracker is not null && typedRequest is not null) tracker.SetParent(typedRequest);
            }

            return specimen;
        }

        public object? GetRequest(AutoMockTypeControl autoMockTypeControl)
        {
            if (typedRequest is AutoMockDirectRequest) return null; // Not changing anything on a direct request...
                
            if (autoMockTypeControl.AlwaysAutoMockTypes.Contains(type))
            {
                if (typedRequest is AutoMockRequest) return null;

                var autoMockRequest = typedRequest is not null
                                            ? new AutoMockRequest(type, typedRequest)
                                            : new AutoMockRequest(type, fixture);
                autoMockRequest.MockShouldCallbase = false;

                return autoMockRequest;
            }

            if (autoMockTypeControl.NeverAutoMockTypes.Contains(type))
            {
                if (typedRequest is not AutoMockRequest) return null;

                return typedRequest is not null
                            ? typedRequest.StartTracker.MockDependencies 
                                ? new AutoMockDependenciesRequest(type, typedRequest)
                                : new NonAutoMockRequest(type, typedRequest)
                            : new NonAutoMockRequest(type, fixture);
            }

            return null;
        }
    }
}

