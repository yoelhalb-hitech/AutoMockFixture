
namespace AutoMockFixture.Moq4.AutoMockProxy;

internal interface IProxyTypeSetupHandler
{
    void Setup(Type originalType, AutoMock<TypeInfo> mock);
}
