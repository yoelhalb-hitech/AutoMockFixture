
using AutoMockFixture.FixtureUtils.Requests.MainRequests;

namespace AutoMockFixture.FixtureUtils.Commands;

internal class AutoMockVirtualMethodsCommand : ISpecimenCommand
{
    private readonly IAutoMockHelpers autoMockHelpers;
    private readonly SetupServiceFactoryBase setupServiceFactory;

    public AutoMockVirtualMethodsCommand(IAutoMockHelpers autoMockHelpers, SetupServiceFactoryBase setupServiceFactory)
    {
        this.autoMockHelpers = autoMockHelpers;
        this.setupServiceFactory = setupServiceFactory;
    }

    public void Execute(object specimen, ISpecimenContext context)
    {
        if (context is null) throw new ArgumentNullException(nameof(context));

        try
        {
            var mock = autoMockHelpers.GetFromObj(specimen);
            if (mock is null) return;

            var setupHelper = new MockSetupHelperService(mock, context, setupServiceFactory, autoMockHelpers);
            var setupService = new MockSetupService(mock, setupHelper);
            setupService.Setup();
        }
        catch { }
    }
}
