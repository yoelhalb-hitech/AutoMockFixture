
namespace AutoMockFixture.AutoMockUtils;

internal interface ISetupService
{
    public void Setup();
    public void SetupWithResult(Func<object?> resultFunc);
}
