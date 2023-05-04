using AutoMockFixture.AutoMockUtils;

namespace AutoMockFixture.Tests.AutoMockFixture_Tests;

internal class NonAutoMock_Tests
{

    [Test]
    public void Test_CtorArguments_NotAutoMocked()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateNonAutoMock<WithCtorArgsTestClass>();
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeOfType<WithCtorArgsTestClass>();
        AutoMock.IsAutoMock(obj).Should().BeFalse();

        obj.TestCtorArg.Should().NotBeNull();
        obj.TestCtorArg!.InternalTest.Should().NotBeNull();
        AutoMock.IsAutoMock(obj.TestCtorArg).Should().BeFalse();
    }

    [Test]
    public void Test_MainObject_AutoMocked_WhenAutoMock()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateNonAutoMock<AutoMock<WithCtorArgsTestClass>>();
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<AutoMock<WithCtorArgsTestClass>>();
        obj.GetMocked().Should().NotBeNull();
    }

    [Test]
    public void Test_MainObject_AutoMocked_WhenAbstract()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateNonAutoMock<InternalAbstractMethodTestClass>();
        // Assert
        obj.Should().NotBeNull();
        AutoMock.IsAutoMock(obj).Should().BeTrue();
    }

    [Test]
    public void Test_MainObject_AutoMocked_WhenInterface()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateNonAutoMock<ITestInterface>();
        // Assert
        obj.Should().NotBeNull();
        AutoMock.IsAutoMock(obj).Should().BeTrue();

        obj.TestProp.Should().NotBeNull();
        obj.TestMethod().Should().NotBeNull();
    }
}
