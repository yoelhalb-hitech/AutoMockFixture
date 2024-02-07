using AutoMockFixture.Moq4.AutoMockUtils;
using DotNetPowerExtensions.Reflection;
using System.Diagnostics.CodeAnalysis;

namespace AutoMockFixture.Moq4.IntegrationTests.AutoMockFixture_Tests;

internal class PropertiesFieldsSetup_Tests
{
    public class TestClass
    {
        public string? PropWithPrivateSetter { get; private set; }
        public string? PropWithPrivateGetter { private get; set; }
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used via refelection")]
        private string? PrivateProperty { get; set; }
        private string? PrivateField;
    }

    [Test]
    public void Test_NotSettingUpPrivateSetter_WhenCallBase()
    {
        var fixture = new AbstractAutoMockFixture();

        var result = fixture.CreateAutoMock<TestClass>(callBase: true);
        result.Should().NotBeNull();

        AutoMockHelpers.GetAutoMock(result).Should().NotBeNull();

        result!.PropWithPrivateSetter.Should().BeNull();
        typeof(TestClass).GetProperty("PrivateProperty", BindingFlagsExtensions.AllBindings)!.GetValue(result).Should().BeNull();
        typeof(TestClass).GetField("PrivateField", BindingFlagsExtensions.AllBindings)!.GetValue(result).Should().BeNull();
    }

    [Test]
    public void Test_SettingUpPrivateSetter_WhenNonCallBase()
    {
        var fixture = new AbstractAutoMockFixture();

        var result = fixture.CreateAutoMock<TestClass>(callBase: false);
        result.Should().NotBeNull();

        AutoMockHelpers.GetAutoMock(result).Should().NotBeNull();

        result!.PropWithPrivateSetter.Should().NotBeNull();
    }

    [Test]
    public void Test_NotSettingUpPrivateGetter([Values(true, false)] bool callBase)
    {
        var fixture = new AbstractAutoMockFixture();

        var result = fixture.CreateAutoMock<TestClass>(callBase: callBase);
        result.Should().NotBeNull();

        AutoMockHelpers.GetAutoMock(result).Should().NotBeNull();

        typeof(TestClass).GetProperty(nameof(TestClass.PropWithPrivateGetter))!.GetValue(result).Should().BeNull();
        typeof(TestClass).GetProperty("PrivateProperty", BindingFlagsExtensions.AllBindings)!.GetValue(result).Should().BeNull();
        typeof(TestClass).GetField("PrivateField", BindingFlagsExtensions.AllBindings)!.GetValue(result).Should().BeNull();
    }

    [Test]
    public void Test_SettingUpPrivateGetter_WhenNonCallBase_WhenTypeSpecified()
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.TypesToSetupPrivateGetters.Add(typeof(TestClass));

        var result = fixture.CreateAutoMock<TestClass>(callBase: false);
        result.Should().NotBeNull();

        AutoMockHelpers.GetAutoMock(result).Should().NotBeNull();

        typeof(TestClass).GetProperty(nameof(TestClass.PropWithPrivateGetter))!.GetValue(result).Should().NotBeNull();
        typeof(TestClass).GetProperty("PrivateProperty", BindingFlagsExtensions.AllBindings)!.GetValue(result).Should().NotBeNull();
        typeof(TestClass).GetField("PrivateField", BindingFlagsExtensions.AllBindings)!.GetValue(result).Should().NotBeNull();
    }

    [Test]
    public void Test_NotSettingUpPrivateGetter_WhenCallBase_WhenTypeSpecified()
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.TypesToSetupPrivateGetters.Add(typeof(TestClass));

        var result = fixture.CreateAutoMock<TestClass>(callBase: true);
        result.Should().NotBeNull();

        AutoMockHelpers.GetAutoMock(result).Should().NotBeNull();

        typeof(TestClass).GetProperty(nameof(TestClass.PropWithPrivateGetter))!.GetValue(result).Should().BeNull();
        typeof(TestClass).GetProperty("PrivateProperty", BindingFlagsExtensions.AllBindings)!.GetValue(result).Should().BeNull();
        typeof(TestClass).GetField("PrivateField", BindingFlagsExtensions.AllBindings)!.GetValue(result).Should().BeNull();
    }
}
