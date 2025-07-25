using AutoMockFixture.FixtureUtils.Customizations;

namespace AutoMockFixture.Moq4.IntegrationTests.AutoMockFixture_Tests;

public class ConstructorArgument_Tests
{
    public class CtorArgs(int i, string s)
    {
        public int ArgI { get; } = i;
        public string ArgS { get; } = s;
    }

    [Test]
    public void Test_ConstructorArgument()
    {
        using var fixture = new AbstractAutoMockFixture();
        fixture.Customize(new ConstructorArgumentCustomization(new ConstructorArgumentValue("TestValue")));
        fixture.Customize(new ConstructorArgumentCustomization(new ConstructorArgumentValue(25)));

        var obj = fixture.CreateNonAutoMock<CtorArgs>();

        obj!.ArgI.Should().Be(25);
        obj!.ArgS.Should().Be("TestValue");
    }

    [Test]
    public void Test_ConstructorArgument_ForAutoMock()
    {
        using var fixture = new AbstractAutoMockFixture();
        fixture.Customize(new ConstructorArgumentCustomization(new ConstructorArgumentValue("TestValue")));
        fixture.Customize(new ConstructorArgumentCustomization(new ConstructorArgumentValue(25)));

        var obj = fixture.CreateAutoMock<CtorArgs>(true); // Callbase false would call a new parameterless ctor

        obj!.ArgI.Should().Be(25);
        obj!.ArgS.Should().Be("TestValue");
    }

    [Test]
    public void Test_ConstructorArgument_ByName()
    {
        using var fixture = new AbstractAutoMockFixture();
        fixture.Customize(new ConstructorArgumentCustomization(new ConstructorArgumentValue("TestValue")));
        fixture.Customize(new ConstructorArgumentCustomization(new ConstructorArgumentValue(null!, 25, "i123")));

        var obj = fixture.CreateNonAutoMock<CtorArgs>();

        obj!.ArgI.Should().NotBe(25);
        obj!.ArgS.Should().Be("TestValue");
    }

    [Test]
    public void Test_ConstructorArgument_ByPath()
    {
        using var fixture = new AbstractAutoMockFixture();
        fixture.Customize(new ConstructorArgumentCustomization(new ConstructorArgumentValue("TestValue", "..ctor->x")));
        fixture.Customize(new ConstructorArgumentCustomization(new ConstructorArgumentValue(25, "..ctor->i")));

        var obj = fixture.CreateNonAutoMock<CtorArgs>();

        obj!.ArgI.Should().Be(25);
        obj!.ArgS.Should().NotBe("TestValue");
    }

    [Test]
    public void Test_ConstructorArgument_ByCtorType()
    {
        using var fixture = new AbstractAutoMockFixture();
        fixture.Customize(new ConstructorArgumentCustomization(new ConstructorArgumentValue(typeof(string), "TestValue")));
        fixture.Customize(new ConstructorArgumentCustomization(new ConstructorArgumentValue(typeof(CtorArgs), 25)));

        var obj = fixture.CreateNonAutoMock<CtorArgs>();

        obj!.ArgI.Should().Be(25);
        obj!.ArgS.Should().NotBe("TestValue");
    }
}
