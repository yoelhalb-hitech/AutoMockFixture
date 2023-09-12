using AutoMockFixture.FixtureUtils.Commands;
using System.Reflection;

namespace AutoMockFixture.Tests.FixtureUtils.Commands;

file class TestBase
{
    public string? TestProp { get; private set; }
    public int TestPrimitive { get; set; }
}
file class TestSub : TestBase { }
file class TestSubOfSub : TestSub { }

file class CustomAutoPropertiesCommandSub : CustomAutoPropertiesCommand
{
    public CustomAutoPropertiesCommandSub() : base(new AbstractAutoMockFixture())
    {
    }

    public IEnumerable<PropertyInfo> GetAllProperties<T>() where T : new()
    {
        var specimen = new T();

        return GetPropertiesWithSet(specimen).Select(pi => pi.ReflectionInfo);
    }

    public bool NeedsSetupPublic(object specimen, PropertyInfo pi) => base.NeedsSetup(specimen, pi);
}

internal class CustomAutoPropertiesCommand_Tests
{
    [Test]
    public void Test_GetPropertiesWithSet_Returns_BaseProperty_WhenPrivate()
    {
        var props = new CustomAutoPropertiesCommandSub { IncludePrivateSetters = true }.GetAllProperties<TestSub>();

        props.Should().Contain(p => p.Name == nameof(TestBase.TestProp));
    }

    [Test]
    public void Test_GetPropertiesWithSet_Returns_BaseOfBaseProperty_WhenPrivate()
    {
        var props = new CustomAutoPropertiesCommandSub { IncludePrivateSetters = true }.GetAllProperties<TestSubOfSub>();

        props.Should().Contain(p => p.Name == nameof(TestBase.TestProp));
    }

    [Test]
    public void Test_NeedsSetup_WorksCorrectlyWithPrimitive()
    {
        var obj = new TestBase { TestPrimitive = 0 };

        var result = new CustomAutoPropertiesCommandSub().NeedsSetupPublic(obj, typeof(TestBase).GetProperty(nameof(TestBase.TestPrimitive))!);
        result.Should().BeTrue();
    }
}
