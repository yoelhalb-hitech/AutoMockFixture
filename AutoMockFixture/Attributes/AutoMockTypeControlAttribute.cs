
using AutoMockFixture.FixtureUtils;

namespace AutoMockFixture;

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
public class AutoMockTypeControlAttribute : Attribute
{
	public AutoMockTypeControlAttribute(AutoMockTypeControl autoMockTypeControl)
	{
        AutoMockTypeControl = autoMockTypeControl;
    }

    public AutoMockTypeControl AutoMockTypeControl { get; set; }
}
