
namespace AutoMockFixture.Tests.AutoMockFixture_Tests;

internal class Internal_Test
{
    [Test]
    public void Test_SetInternalProperties_NonAutoMock()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateNonAutoMock<InternalSimpleTestClass>();
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeOfType<InternalSimpleTestClass>();

        obj!.InternalTest.Should().NotBeNull();
    }

    [Test]
    public void Test_SetInternalProperties_AutoMock()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateAutoMock<InternalSimpleTestClass>();
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<InternalSimpleTestClass>();

        obj!.InternalTest.Should().NotBeNull();
    }
    [Test]
    public void Test_SetInternalProperties_AutoMockDependencies()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<InternalSimpleTestClass>();
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeOfType<InternalSimpleTestClass>();

        obj!.InternalTest.Should().NotBeNull();
    }

    [Test]
    public void Test_SetInternalFields_NonAutoMock()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateNonAutoMock<InternalTestFields>();
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeOfType<InternalTestFields>();

        obj!.InternalTest.Should().NotBeNull();
    }

    [Test]
    public void Test_SetInternalFields_AutoMock()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateAutoMock<InternalTestFields>();
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<InternalTestFields>();

        obj!.InternalTest.Should().NotBeNull();
    }

    [Test]
    public void Test_SetInternalFields_AutoMockDependencies()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<InternalTestFields>();
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeOfType<InternalTestFields>();

        obj!.InternalTest.Should().NotBeNull();
    }

    [Test]
    public void Test_SetsUpInternalMethods_ForAutomock()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateAutoMock<InternalTestMethods>();
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<InternalTestMethods>();

        obj!.InternalTestMethod().Should().NotBeNull();
    }
}
