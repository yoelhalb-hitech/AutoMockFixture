
namespace AutoMoqExtensions.Test.AutoMockFixture_Tests;

internal class AutoMockDependencies_Tests
{
    [Test]
    public void Test_MainObject_NotAutoMocked()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<WithCtorArgsTestClass>();
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeOfType<WithCtorArgsTestClass>();
        AutoMockUtils.AutoMockHelpers.GetAutoMock(obj).Should().BeNull();
    }

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
    }

    [Test]
    public void Test_Properties_AutoMocked()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<WithCtorArgsTestClass>();
        // Assert
        obj.TestClassProp.Should().NotBeNull();
        obj.TestClassProp!.InternalTest.Should().NotBeNull();
        AutoMockUtils.AutoMockHelpers.GetAutoMock(obj.TestClassProp).Should().NotBeNull();
    }

    [Test]
    public void Test_PropertiesPrivateSetter_AutoMocked_WhenNotCallBase()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<WithCtorArgsTestClass>(callBase: false);
        // Assert
        obj.TestClassPropWithPrivateSet.Should().NotBeNull();
        obj.TestClassPropWithPrivateSet!.InternalTest.Should().NotBeNull();
        AutoMockUtils.AutoMockHelpers.GetAutoMock(obj.TestClassPropWithPrivateSet).Should().NotBeNull();

        obj.TestClassPrivateNonVirtualProp.Should().NotBeNull();
        obj.TestClassPrivateNonVirtualProp!.InternalTest.Should().NotBeNull();
        AutoMockUtils.AutoMockHelpers.GetAutoMock(obj.TestClassPrivateNonVirtualProp).Should().NotBeNull();
    }

    [Test]
    public void Test_PropertiesPrivateSetter_NotAutoMocked_WhenCallBase()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<WithCtorArgsTestClass>(callBase: true);
        // Assert
        obj.TestClassPropWithPrivateSet.Should().BeNull();
        obj.TestClassPrivateNonVirtualProp.Should().BeNull();
    }

    [Test]
    public void Test_Fields_AutoMocked()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<WithCtorArgsTestClass>();
        // Assert
        obj.TestClassField.Should().NotBeNull();
        obj.TestClassField!.InternalTest.Should().NotBeNull();
        AutoMockUtils.AutoMockHelpers.GetAutoMock(obj.TestClassField).Should().NotBeNull();
    }
}
