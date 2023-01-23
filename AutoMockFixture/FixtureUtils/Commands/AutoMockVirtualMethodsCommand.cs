using AutoMockFixture.AutoMockUtils;
using AutoMockFixture.MockUtils;

namespace AutoMockFixture.FixtureUtils.Commands;

internal class AutoMockVirtualMethodsCommand : ISpecimenCommand
{
    private readonly MethodSetupServiceFactory setupServiceFactory;

    public AutoMockVirtualMethodsCommand(MethodSetupServiceFactory setupServiceFactory)
    {
        this.setupServiceFactory = setupServiceFactory;
    }

    public void Execute(object specimen, ISpecimenContext context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        try
        {
            var mock = AutoMockHelpers.GetFromObj(specimen);
            if (mock is null) return;

            var setupService = new MockSetupService(mock, context, setupServiceFactory);
            setupService.Setup();
        }
        catch { }
    }
}
