
namespace AutoMockFixture;

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
public class NonAutoMockAttribute : AutoMockTypeAttribute
{
    public NonAutoMockAttribute() : base(AutoMockTypes.NonAutoMock) { }
}
