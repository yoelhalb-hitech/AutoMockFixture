using AutoFixture;

namespace AutoMockFixture.FixtureUtils.Customizations;

public class ConstructorArgumentValue
{
    /// <param name="value"><inheritdoc cref="Value" path="/summary" /></param>
    /// <param name="path"><inheritdoc cref="Path" path="/summary" /></param>
    public ConstructorArgumentValue(object? value, string? path = null)
    {
        Value = value;
        Path = path;
    }

    /// <param name="ctorType"><inheritdoc cref="ConstructorType" path="/summary" /></param>
    /// <param name="value"><inheritdoc cref="Value" path="/summary" /></param>
    /// <param name="argName"><inheritdoc cref="ArgumentName" path="/summary" /></param>
    /// <param name="path"><inheritdoc cref="Path" path="/summary" /></param>
    public ConstructorArgumentValue(Type ctorType, object? value, string? argName = null, string? path = null)
    {
        ConstructorType = ctorType;
        Value = value;
        ArgumentName = argName;
        Path = path;
    }

    /// <summary>
    /// The argument value to be used for the constructor argument
    /// </summary>
    public object? Value { get; }
    /// <summary>
    /// The name of the argument, if provided then only the constructor argument with this name will be set
    /// </summary>
    public string? ArgumentName { get; }
    /// <summary>
    /// Using the path system to the constructor argument, if it is not null then only the constructor argument with this path in the entire object chain will be set
    /// </summary>
    public string? Path { get; }
    /// <summary>
    /// The type that the ctor is constructing and is declared in
    /// </summary>
    public Type? ConstructorType { get; }
}

/// <typeparam name="TCtorType"><inheritdoc cref="ConstructorArgumentValue.ConstructorType" path="/summary" /></typeparam>
/// <param name="value"><inheritdoc cref="ConstructorArgumentValue.Value" path="/summary" /></param>
/// <param name="argName"><inheritdoc cref="ConstructorArgumentValue.ArgumentName" path="/summary" /></param>
/// <param name="path"><inheritdoc cref="ConstructorArgumentValue.Path" path="/summary" /></param>
public class ConstructorArgumentValue<TCtorType>(object? value, string? argName = null, string? path = null)
    : ConstructorArgumentValue(typeof(TCtorType), value, argName, path)
{
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
