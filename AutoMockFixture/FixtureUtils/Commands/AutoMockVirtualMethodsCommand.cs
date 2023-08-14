
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
        if (context == null) throw new ArgumentNullException(nameof(context));

        try
        {
            var mock = autoMockHelpers.GetFromObj(specimen);
            if (mock is null) return;

            if (mock.Tracker is AutoMockDirectRequest request && request.NoConfigureMemebrs) return;

            var setupService = new MockSetupService(mock, context, setupServiceFactory, autoMockHelpers);
            setupService.Setup();
        }
        catch { }
    }
}
