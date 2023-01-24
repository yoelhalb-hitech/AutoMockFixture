using AutoMockFixture.AutoMockUtils;
using Moq;

namespace AutoMockFixture.FixtureUtils.Commands;

internal class AutoMockClearInvocationsCommand : ISpecimenCommand
{
    public void Execute(object specimen, ISpecimenContext context)
    {
        var mock = AutoMockHelpers.GetFromObj(specimen);
        if (mock is null) return;

        (mock as Mock)?.Invocations.Clear(); // This way we will remove for example counts for property set etc.
    }
}
