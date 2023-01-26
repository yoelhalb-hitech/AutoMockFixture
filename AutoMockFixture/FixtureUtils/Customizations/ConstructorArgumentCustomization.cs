using AutoFixture;

namespace AutoMockFixture.FixtureUtils.Customizations;

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
public class ConstructorArgumentCustomization : IRemovableCustomization
{
    public List<ConstructorArgumentValue> ConstructorArgumentValues { get; } = new List<ConstructorArgumentValue> ();

    public ConstructorArgumentCustomization(ConstructorArgumentValue constructorArgumentValue)
        : this(new []{ constructorArgumentValue })
    {
    }

    public ConstructorArgumentCustomization(IEnumerable<ConstructorArgumentValue> constructorArgumentValues)
    {
        ConstructorArgumentValues.AddRange(constructorArgumentValues);
    }

    public void Customize(IFixture fixture)
    {
        if (fixture is not IAutoMockFixture mockFixture) throw new ArgumentException($"Expected {nameof(fixture)} to be as `{nameof(IAutoMockFixture)}`");

        mockFixture.ConstructorArgumentValues.AddRange(ConstructorArgumentValues);
    }

    public void RemoveCustomization(IFixture fixture)
    {
        if (fixture is not IAutoMockFixture mockFixture) throw new ArgumentException($"Expected {nameof(fixture)} to be an `{nameof(IAutoMockFixture)}`");

        ConstructorArgumentValues.ForEach(v => mockFixture.ConstructorArgumentValues.Remove(v));
    }
}
