
namespace AutoMoqExtensions.Test.AutoMockFixture_Tests;

internal class NonAutoMock_Tests
{       

    [Test]
    public void Test_CtorArguments_NotAutoMocked()
    {
        // Arrange
        var fixture = new AutoMockFixture();
        // Act
        var obj = fixture.CreateNonAutoMock<WithCtorArgsTestClass>();
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeOfType<WithCtorArgsTestClass>();
        AutoMockUtils.AutoMockHelpers.GetAutoMock(obj).Should().BeNull();

        obj.TestCtorArg.Should().NotBeNull();
        obj.TestCtorArg!.InternalTest.Should().NotBeNull();
        AutoMockUtils.AutoMockHelpers.GetAutoMock(obj.TestCtorArg).Should().BeNull();
    }
}
