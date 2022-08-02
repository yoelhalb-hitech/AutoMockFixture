using AutoMoqExtensions.AutoMockUtils;
using AutoMoqExtensions.FixtureUtils.Requests;
using AutoMoqExtensions.FixtureUtils.Requests.MainRequests;

namespace AutoMoqExtensions.FixtureUtils.Builders.SpecialBuilders;

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

        return result;
    }

    internal class TypeControlHelper
    {
        private readonly AutoMockFixture fixture;
        private readonly IRequestWithType? typedRequest;
        private readonly Type type;
        private readonly ISpecimenContext context;

        public TypeControlHelper(AutoMockFixture fixture, IRequestWithType? typedRequest,
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
            var autoMock = AutoMockHelpers.GetFromObj(specimen);
            if (autoMock is not null)
            {
                var tracker = autoMock.Tracker as BaseTracker;
                if (tracker is not null && typedRequest is not null) tracker.SetParent(typedRequest);
            }
            if (typedRequest is not null) typedRequest.SetResult(specimen);

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
                autoMockRequest.NoMockDependencies = typedRequest is not AutoMockDependenciesRequest;

                return autoMockRequest;
            }

            if (autoMockTypeControl.NeverAutoMockTypes.Contains(type))
            {
                if (typedRequest is not AutoMockRequest) return null;

                if (typedRequest is AutoMockRequest mockRequest && mockRequest.NoMockDependencies == true)
                    return new NonAutoMockRequest(type, mockRequest);

                return typedRequest is not null
                            ? new AutoMockDependenciesRequest(type, typedRequest)
                            : new AutoMockDependenciesRequest(type, fixture);
            }

            return null;
        }
    }
}

