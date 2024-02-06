
namespace AutoMockFixture.Moq4.MockUtils;

internal class ReadWritePropertyLazySetupService : ISetupService
{
    private readonly Type mockedType;
    private readonly Type propertyType;
    private readonly IAutoMock mock;
    private readonly PropertyInfo prop;
    private readonly Func<object?> propValueGenerator;

    public ReadWritePropertyLazySetupService(Type mockedType, Type propertyType, IAutoMock mock, PropertyInfo prop, Func<object?> propValueGenerator)
    {
        this.mockedType = mockedType;
        this.propertyType = propertyType;
        this.mock = mock;
        this.prop = prop;
        this.propValueGenerator = propValueGenerator;
    }

    public void Setup()
    {
        SetupHelpers.SetupLazyReadWriteProperty(mockedType, propertyType, mock, prop, propValueGenerator);
    }
}
