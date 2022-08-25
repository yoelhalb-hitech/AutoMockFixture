using AutoFixture;
using AutoMoqExtensions.AutoMockUtils;
using AutoMoqExtensions.FixtureUtils.Requests;
using AutoMoqExtensions.FixtureUtils.Requests.HelperRequests.AutoMock;
using AutoMoqExtensions.FixtureUtils.Requests.MainRequests;
using AutoMoqExtensions.FixtureUtils.Specifications;
using System.Reflection;

namespace AutoMoqExtensions.FixtureUtils.Commands;

internal class AutoMockAutoPropertiesHandlerCommand : ISpecimenCommand
{
    public virtual void Execute(object specimen, ISpecimenContext context)
    {
        if (context is null) throw new ArgumentNullException(nameof(context));

        var mock = AutoMockHelpers.GetFromObj(specimen);
        if (mock is null) return;

        var directTracker = mock.Tracker as AutoMockDirectRequest;

        var specification = new IgnoreProxyMembersSpecification();
        var fixture = mock.Fixture;

        // Private setters is normally the job of the called but if not callbase we have to do it
        var command = !directTracker?.StartTracker.MockDependencies ?? true
            ? new CustomAutoPropertiesCommand(specification, fixture)
            : new AutoMockAutoPropertiesCommand(specification, fixture) { IncludePrivateSetters = !mock.CallBase };
        
        command.Execute(mock.GetMocked(), context);
    }
}