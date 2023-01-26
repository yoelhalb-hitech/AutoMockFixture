
namespace AutoMockFixture;

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
public class AutoMockAttribute : AutoMockTypeAttribute
{
    public AutoMockAttribute() : base(AutoMockTypes.AutoMock) { }
}