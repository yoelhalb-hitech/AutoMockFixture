using AutoFixture.Kernel;
using AutoMockFixture.Moq4.AutoMockUtils;
using Moq;

namespace AutoMockFixture.Moq4.FixtureUtils.Commands;

internal class AutoMockClearInvocationsCommand : ISpecimenCommand
{
    public AutoMockClearInvocationsCommand(AutoMockHelpers autoMockHelpers)
    {
        AutoMockHelpers = autoMockHelpers;
    }

    public AutoMockHelpers AutoMockHelpers { get; }

    public void Execute(object specimen, ISpecimenContext context)
    {
        var m = AutoMockHelpers.GetFromObj(specimen);
        if (m is null || m is not Mock mock) return;

        mock.Invocations.Clear(); // This way we will remove for example counts for property set etc.
    }
}
