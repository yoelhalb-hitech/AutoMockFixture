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

        // Private setters is normally the job of the called but if not callbase we have to do it
        command.IncludePrivateSetters = !mock.CallBase;

        command.Execute(mock.GetMocked(), context);
    }
}