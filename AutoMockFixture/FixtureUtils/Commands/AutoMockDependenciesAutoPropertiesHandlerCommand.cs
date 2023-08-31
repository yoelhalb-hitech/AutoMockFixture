
namespace AutoMockFixture.FixtureUtils.Commands;

internal class AutoMockDependenciesAutoPropertiesHandlerCommand : ISpecimenCommand
{
    public AutoMockDependenciesAutoPropertiesHandlerCommand(IAutoMockFixture fixture)
    {
        Fixture = fixture;
    }

    public IAutoMockFixture Fixture { get; }

    public virtual void Execute(object specimen, ISpecimenContext context)
    {
        if (context is null) throw new ArgumentNullException(nameof(context));

        if (specimen is null) return;

        Fixture.ProcessingTrackerDict.TryGetValue(specimen, out var existingTracker);

        var command = new AutoMockAutoPropertiesCommand(Fixture)
        {
            // Private setters is always the job of the object code, while mocked non callbase doesn't need it (as we custom setup the readonly prop)
            IncludePrivateSetters = false,
        };

        command.Execute(specimen, context);
    }
}