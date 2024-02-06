using AutoMockFixture.FixtureUtils.Requests.MainRequests;
using AutoMockFixture.FixtureUtils.Specifications;

namespace AutoMockFixture.FixtureUtils.Commands;

internal class AutoMockAutoPropertiesHandlerCommand : ISpecimenCommand
{
    private static readonly DelegateSpecification delegateSpecification = new DelegateSpecification();

    public AutoMockAutoPropertiesHandlerCommand(IAutoMockHelpers autoMockHelpers)
    {
        AutoMockHelpers = autoMockHelpers ?? throw new ArgumentNullException(nameof(autoMockHelpers));
    }

    public IAutoMockHelpers AutoMockHelpers { get; }

    public virtual void Execute(object specimen, ISpecimenContext context)
    {
        if (context is null) throw new ArgumentNullException(nameof(context));

        var mock = AutoMockHelpers.GetFromObj(specimen);
        if (mock is null) return;

        if (delegateSpecification.IsSatisfiedBy(mock.GetInnerType())) return;

        var directTracker = mock.Tracker as AutoMockDirectRequest;

        var specification = new IgnoreProxyMembersSpecification();
        var fixture = mock.Fixture;

        var command = !directTracker?.StartTracker.MockDependencies ?? true
            ? new CustomAutoPropertiesCommand(specification, fixture)
            : new AutoMockAutoPropertiesCommand(specification, fixture);

        // Private setters is normally the job of the class code but if not callBase we have to do it
        command.IncludePrivateSetters = !mock.CallBase;
        // Private getters is normally not relevent outside the class code and the caller code has to handle it
        //      but if not callBase we have to do it in case some method is setup with callBase and will run into issues (because the other methods are not callBase)
        command.IncludePrivateOrMissingGetter = !mock.CallBase;

        command.Execute(mock.GetMocked(), context);
    }
}