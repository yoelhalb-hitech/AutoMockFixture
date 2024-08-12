using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;
using AutoMockFixture.FixtureUtils.Specifications;

namespace AutoMockFixture.FixtureUtils.Builders.MainBuilders;

internal class ForceAutoMockBuilder : ISpecimenBuilder
{
    public ForceAutoMockBuilder(IAutoMockHelpers autoMockHelpers)
    {
        AutoMockHelpers = autoMockHelpers;
        ForceAutoMockSpecification = new ForceAutoMockSpecification(autoMockHelpers);
    }

    public IAutoMockHelpers AutoMockHelpers { get; }
    public ForceAutoMockSpecification ForceAutoMockSpecification { get; }

    public object Create(object request, ISpecimenContext context)
    {
        if(!ForceAutoMockSpecification.IsSatisfiedBy(request)) return new NoSpecimen();

        try
        {
            var fixtureTracker = (request as IFixtureTracker)!;
            // We don't want to end up in recursion...
            if (fixtureTracker.GetParentsOnCurrentLevel().Any(t => t.GetType() == typeof(AutoMockRequest))) return new NoSpecimen();

            var type = (request as IRequestWithType)!.Request;

            var isMock = AutoMockHelpers.IsAutoMock(type)
                            || AutoMockHelpers.MockRequestSpecification.IsSatisfiedBy(type);

            var typeToUse = !isMock ? type
                            : AutoMockHelpers.IsAutoMock(type)
                            ? AutoMockHelpers.GetMockedType(type)!
                            : type.GenericTypeArguments.First(); // Probably IMock<> or Mock<>


            if(!AutoMockHelpers.IsAutoMockAllowed(typeToUse)) return new NoSpecimen();

            var autoMockRequest = new AutoMockRequest(typeToUse, fixtureTracker)
            {
                MockShouldCallBase = fixtureTracker.MockShouldCallBase, // Forward any explicit `MockShouldCallBase` on the request
            };

            var start = fixtureTracker.StartTracker;
            var startType = (start as IRequestWithType)?.Request ?? type;

            // We might arrive here because of some creation logic and not because the user asked for a non mock
            // When a user requests `AutoMockDependenciesRequest` then even if he specifies `CallBase = false` he probably refers to the dependencies and not to the SUT
            // But if he asked for `NonAutoMockRequest` then he meant situations like this
            var userAskedForNonMock = fixtureTracker.StartTracker.MockShouldCallBase != false
                && (fixtureTracker.StartTracker is NonAutoMockRequest
                || (fixtureTracker.StartTracker is AutoMockDependenciesRequest && fixtureTracker.Path == string.Empty));

            // In general we want to set to CallBase when abstract etc. because we want to emulate the original object as much as we can
            if (userAskedForNonMock && autoMockRequest.MockShouldCallBase is null) autoMockRequest.MockShouldCallBase = true;

            var result = context.Resolve(autoMockRequest); // The AutoMockDirectRequest commands handles correctly the `NonAutoMock` dependecies so no worries

            object? autoMock = AutoMockHelpers.GetFromObj(result);
            if (autoMock is null && isMock) return new NoSpecimen();

            var retValue = isMock ? autoMock : result is IAutoMock mock ? mock.GetMocked() : result;
            fixtureTracker.SetResult(retValue, this);

            return retValue!;
        }
        catch
        {
            return new NoSpecimen();
        }
    }
}
