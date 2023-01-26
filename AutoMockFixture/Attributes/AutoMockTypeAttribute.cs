
namespace AutoMockFixture;

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
public class AutoMockTypeAttribute : Attribute
{
    public AutoMockTypes AutoMockType { get; }

    public AutoMockTypeAttribute(AutoMockTypes autoMockType)
    {
        this.AutoMockType = autoMockType;
    }
}
