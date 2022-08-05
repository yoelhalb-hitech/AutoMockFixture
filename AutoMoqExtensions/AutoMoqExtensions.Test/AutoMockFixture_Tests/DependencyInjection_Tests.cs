
namespace AutoMoqExtensions.Test.AutoMockFixture_Tests;

internal class DependencyInjection_Tests
{       

    [Test]
    public void Test_CtorArguments_AutoMocked()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<WithCtorArgsTestClass>();
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeOfType<WithCtorArgsTestClass>();
        AutoMockUtils.AutoMockHelpers.GetAutoMock(obj).Should().BeNull();

        obj.TestCtorArg.Should().NotBeNull();
        obj.TestCtorArg!.InternalTest.Should().NotBeNull();
        AutoMockUtils.AutoMockHelpers.GetAutoMock(obj.TestCtorArg).Should().NotBeNull();
    }
}
