using Castle.DynamicProxy;

namespace AutoMockFixture.Moq4.FixtureUtils.Commands;

internal class AutoMockInitCommand : ISpecimenCommand
{
    public void Execute(object specimen, ISpecimenContext context)
    {
        if (specimen is null || specimen is not IAutoMock mock || specimen is not Mock m) return;

        m.DefaultValue = DefaultValue.Empty; // When we want a value we will set it up ourselves with AutoMock
    }
}
