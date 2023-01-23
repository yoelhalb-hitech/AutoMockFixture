
namespace AutoMockFixture.FixtureUtils.Commands;

public class EmptyCommand : ISpecimenCommand
{
    public void Execute(object specimen, ISpecimenContext context)
    {
    }
}
