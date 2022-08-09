using AutoFixture;
using AutoMoqExtensions.AutoMockUtils;
using AutoMoqExtensions.FixtureUtils.Requests;
using AutoMoqExtensions.FixtureUtils.Requests.HelperRequests.AutoMock;
using AutoMoqExtensions.FixtureUtils.Requests.MainRequests;
using AutoMoqExtensions.FixtureUtils.Specifications;
using System.Reflection;

namespace AutoMoqExtensions.FixtureUtils.Commands;

internal class AutoMockDependenciesAutoPropertiesHandlerCommand : ISpecimenCommand
{
    public AutoMockDependenciesAutoPropertiesHandlerCommand(AutoMockFixture fixture)
    {
        Fixture = fixture;
    }

    public AutoMockFixture Fixture { get; }

    public virtual void Execute(object specimen, ISpecimenContext context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        Fixture.ProcessingTrackerDict.TryGetValue(specimen, out var existingTracker);
      
        // Private setters is normally the job of the called but if not callbase we have to do it
        var command = new AutoMockAutoPropertiesCommand(Fixture) 
        { 
            IncludePrivateSetters = existingTracker?.StartTracker.MockShouldCallbase != true
        };
        
        command.Execute(specimen, context);
    }
}