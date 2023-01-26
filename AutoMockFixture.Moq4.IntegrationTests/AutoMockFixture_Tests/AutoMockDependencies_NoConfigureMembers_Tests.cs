
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
        AutoMock.IsAutoMock(obj).Should().BeFalse();
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
        AutoMock.IsAutoMock(obj).Should().BeFalse();

        obj!.TestCtorArg.Should().NotBeNull();
        AutoMock.IsAutoMock(obj.TestCtorArg).Should().BeTrue();

        obj.TestCtorArgProp.Should().NotBeNull();
        AutoMock.IsAutoMock(obj.TestCtorArgProp).Should().BeTrue();

        obj.TestCtorArgPrivateProp.Should().NotBeNull();
        AutoMock.IsAutoMock(obj.TestCtorArgPrivateProp).Should().BeTrue();

        obj.TestCtorArgVirtualProp.Should().NotBeNull();
        AutoMock.IsAutoMock(obj.TestCtorArgVirtualProp).Should().BeTrue();

        obj.TestCtorArgVirtualPrivateProp.Should().NotBeNull();
        AutoMock.IsAutoMock(obj.TestCtorArgVirtualPrivateProp).Should().BeTrue();
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
