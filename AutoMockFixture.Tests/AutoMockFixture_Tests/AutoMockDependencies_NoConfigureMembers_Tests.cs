using AutoMockFixture.AutoMockUtils;

namespace AutoMockFixture.Tests.AutoMockFixture_Tests;

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
    public void Test_NoConfigureMembers_Properties_NotSet()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture(true);
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<WithCtorArgsTestClass>();
        // Assert
        obj.TestClassProp.Should().BeNull();
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void Test_NoConfigureMembers_PropertiesPrivateSetter_NotSet(bool callbase)
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture(true);
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<WithCtorArgsTestClass>(callbase);
        // Assert
        obj.TestClassPropWithPrivateSet.Should().BeNull();
        obj.TestClassPrivateNonVirtualProp.Should().BeNull();
    }

    [Test]
    public void Test_NoConfigureMembers_Fields_NotSet()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture(true);
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<WithCtorArgsTestClass>();
        // Assert
        obj.TestClassField.Should().BeNull();
    }
}
