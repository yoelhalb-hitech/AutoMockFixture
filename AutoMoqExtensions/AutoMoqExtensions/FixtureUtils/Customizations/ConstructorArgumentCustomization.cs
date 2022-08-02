
using AutoFixture;

namespace AutoMoqExtensions.FixtureUtils.Customizations;

public class ConstructorArgumentValue
{
    public ConstructorArgumentValue(object? value, string? path = null)
    {
        Value = value;
        Path = path;
    }

    public object? Value { get; }
    public string? Path { get; }
}
public class ConstructorArgumentCustomization : ICustomization
{
    public ConstructorArgumentValue ConstructorArgumentValue { get; }

    public ConstructorArgumentCustomization(ConstructorArgumentValue constructorArgumentValue)
    {
        ConstructorArgumentValue = constructorArgumentValue;
    }

    public void Customize(IFixture fixture)
    {
        if (fixture is not AutoMockFixture mockFixture) throw new ArgumentException($"Expected {nameof(fixture)} to be `{nameof(AutoMockFixture)}`");

        mockFixture.ConstructorArgumentValues.Add(ConstructorArgumentValue);
    }
}
