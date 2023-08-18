
using AutoMockFixture.FixtureUtils;

namespace AutoMockFixture;

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
public class AutoMockTypeControlAttribute : Attribute
{
	public AutoMockTypeControlAttribute(Type[] alwaysMockTypes, Type[] neverMockTypes)
	{
        AlwaysMockTypes = alwaysMockTypes;
        NeverMockTypes = neverMockTypes;
    }

    public AutoMockTypeControl AutoMockTypeControl => new AutoMockTypeControl
    {
        AlwaysAutoMockTypes = AlwaysMockTypes.ToList(),
        NeverAutoMockTypes = NeverMockTypes.ToList(),
    };

    public Type[] AlwaysMockTypes { get; set; }
    public Type[] NeverMockTypes { get; set; }
}
