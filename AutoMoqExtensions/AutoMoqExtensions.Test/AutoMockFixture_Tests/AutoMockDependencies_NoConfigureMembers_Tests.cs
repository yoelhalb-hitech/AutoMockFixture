
using AutoMoqExtensions.AutoMockUtils;

namespace AutoMoqExtensions.Test.AutoMockFixture_Tests;

internal class AutoMockDependencies_NoConfigureMembers_Tests
{
    [Test]
    public void Test_NoConfigureMembers_MainObject_NotAutoMocked()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture(true);
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<WithCtorArgsTestClass>();
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeOfType<WithCtorArgsTestClass>();
        AutoMockHelpers.GetAutoMock(obj).Should().BeNull();
    }

    [Test]
    public void Test_NoConfigureMembers_CtorArguments_AutoMocked()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture(true);
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<WithCtorArgsTestClass>();
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeOfType<WithCtorArgsTestClass>();
        AutoMockHelpers.GetAutoMock(obj).Should().BeNull();

        obj.TestCtorArg.Should().NotBeNull();
        AutoMockHelpers.GetAutoMock(obj.TestCtorArg).Should().NotBeNull();

        obj.TestCtorArgProp.Should().NotBeNull();
        AutoMockHelpers.GetAutoMock(obj.TestCtorArgProp).Should().NotBeNull();

        obj.TestCtorArgPrivateProp.Should().NotBeNull();
        AutoMockHelpers.GetAutoMock(obj.TestCtorArgPrivateProp).Should().NotBeNull();

        obj.TestCtorArgVirtualProp.Should().NotBeNull();
        AutoMockHelpers.GetAutoMock(obj.TestCtorArgVirtualProp).Should().NotBeNull();

        obj.TestCtorArgVirtualPrivateProp.Should().NotBeNull();
        AutoMockHelpers.GetAutoMock(obj.TestCtorArgVirtualPrivateProp).Should().NotBeNull();
    }

    [Test]
    public void Test_NoConfigureMembers_Properties_AutoMocked()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<WithCtorArgsTestClass>();
        // Assert
        obj.TestClassProp.Should().NotBeNull();
        obj.TestClassProp!.InternalTest.Should().BeNull();
        AutoMockHelpers.GetAutoMock(obj.TestClassProp).Should().NotBeNull();
    }

    [Test]
    public void Test_NoConfigureMembers_PropertiesPrivateSetter_AutoMocked_WhenNotCallBase()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<WithCtorArgsTestClass>(callBase: false);
        // Assert
        obj.TestClassPropWithPrivateSet.Should().NotBeNull();
        obj.TestClassPropWithPrivateSet!.InternalTest.Should().BeNull();
        AutoMockHelpers.GetAutoMock(obj.TestClassPropWithPrivateSet).Should().NotBeNull();

        obj.TestClassPrivateNonVirtualProp.Should().NotBeNull();
        obj.TestClassPrivateNonVirtualProp!.InternalTest.Should().BeNull();
        AutoMockHelpers.GetAutoMock(obj.TestClassPrivateNonVirtualProp).Should().NotBeNull();
    }

    [Test]
    public void Test_NoConfigureMembers_PropertiesPrivateSetter_NotAutoMocked_WhenCallBase()
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
    public void Test_NoConfigureMembers_Fields_AutoMocked()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<WithCtorArgsTestClass>();
        // Assert
        obj.TestClassField.Should().NotBeNull();
        obj.TestClassField!.InternalTest.Should().BeNull();
        AutoMockHelpers.GetAutoMock(obj.TestClassField).Should().NotBeNull();
    }
}
