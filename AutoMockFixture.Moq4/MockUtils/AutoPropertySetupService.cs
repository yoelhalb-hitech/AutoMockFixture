
namespace AutoMockFixture.Moq4.MockUtils;

internal class AutoPropertySetupService : ISetupService
{
    private readonly Type mockedType;
    private readonly Type propertyType;
    private readonly IAutoMock mock;
    private readonly PropertyInfo prop;
    private readonly object? propValue;

    public AutoPropertySetupService(Type mockedType, Type propertyType, IAutoMock mock, PropertyInfo prop, object? propValue)
	{
        this.mockedType = mockedType;
        this.propertyType = propertyType;
        this.mock = mock;
        this.prop = prop;
        this.propValue = propValue;
    }

    public void Setup()
    {
        SetupHelpers.SetupAutoProperty(mockedType, propertyType, mock, prop, propValue);
    }
}
