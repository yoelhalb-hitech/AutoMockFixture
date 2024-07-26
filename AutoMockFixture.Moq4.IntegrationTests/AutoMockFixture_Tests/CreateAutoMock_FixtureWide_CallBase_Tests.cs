
using AutoMockFixture.FixtureUtils;
using AutoMockFixture.NUnit3;

namespace AutoMockFixture.Tests.AutoMockFixture_Tests;

internal class CreateAutoMock_FixtureWide_CallBase_Tests
{
    [Test]
    public void Test_AutoMock_WithCallBase_Is_CallBase()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture() { CallBase = true };

        // Act
        var obj = fixture.CreateAutoMock<InternalSimpleTestClass>();
        var mock = AutoMock.Get(obj);

        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<InternalSimpleTestClass>();

        mock.Should().NotBeNull();
        mock.Should().BeOfType<AutoMock<InternalSimpleTestClass>>();
        mock!.CallBase.Should().BeTrue();
    }

    [Test]
    public void Test_AutoMock_Abstract_WithCallBase_Is_CallBase()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture() { CallBase = true };
        // Act
        var obj = fixture.CreateAutoMock<InternalAbstractMethodTestClass>();
        var mock = AutoMock.Get(obj);

        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<InternalAbstractMethodTestClass>();

        mock.Should().NotBeNull();
        mock.Should().BeOfType<AutoMock<InternalAbstractMethodTestClass>>();
        mock!.CallBase.Should().BeTrue();
    }

    [Test]
    public void Test_AutoMock_Interface_WithCallBase_Is_CallBase()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture() { CallBase = true };
        // Act
        var obj = fixture.CreateAutoMock<InternalReadOnlyTestInterface>();
        var mock = AutoMock.Get(obj);

        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<InternalReadOnlyTestInterface>();

        mock.Should().NotBeNull();
        mock.Should().BeOfType<AutoMock<InternalReadOnlyTestInterface>>();
        mock!.CallBase.Should().BeTrue();
    }

    [Test]
    public void Test_AutoMock_WithCallBase_CallsCtor()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture() { CallBase = true };
        // Act
        var obj = fixture.CreateAutoMock<WithCtorArgsTestClass>();
        var mock = AutoMock.Get(obj);

        // Assert

        // We need first to verify that it is an AutoMock, otherwise it is no biggy...
        mock.Should().NotBeNull();
        mock.Should().BeOfType<AutoMock<WithCtorArgsTestClass>>();

        obj!.TestCtorArg.Should().NotBeNull();
        obj.TestCtorArgProp.Should().NotBeNull();
        obj.TestCtorArgVirtualProp.Should().NotBeNull();
    }

    [Test]
    public void Test_AutoMock_WithCallBase_SetsUpPropertiesWithPrivateSetter()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture() { CallBase = true };
        // Act
        var obj = fixture.CreateAutoMock<WithCtorArgsTestClass>();
        var mock = AutoMock.Get(obj);

        // Assert

        // We need first to verify that it is an AutoMock, otherwise it is no biggy...
        mock.Should().NotBeNull();
        mock.Should().BeOfType<AutoMock<WithCtorArgsTestClass>>();

        obj!.TestCtorArgPrivateProp.Should().NotBeNull();
        obj.TestCtorArgVirtualPrivateProp.Should().NotBeNull();
    }

    [Test]
    public void Test_AutoMock_WithCallBase_CallsBaseDefaultCtor()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture() { CallBase = true };
        // Act
        var obj = fixture.CreateAutoMock<WithCtorNoArgsTestClass>();
        var mock = AutoMock.Get(obj);

        // Assert

        // We need first to verify that it is an AutoMock, otherwise it is no biggy...
        mock.Should().NotBeNull();
        mock.Should().BeOfType<AutoMock<WithCtorNoArgsTestClass>>();

        obj!.TestCtor.Should().Be(25);
    }

    [Test]
    public void Test_AutoMock_WithCallBase_DoesNotSetup_ReadOnlyProps()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture() { CallBase = true };
        // Act
        var obj = fixture.CreateAutoMock<WithCtorArgsTestClass>();
        var mock = AutoMock.Get(obj);

        // Assert

        // We need first to verify that it is an AutoMock, otherwise it is no biggy...
        mock.Should().NotBeNull();
        mock.Should().BeOfType<AutoMock<WithCtorArgsTestClass>>();

        obj!.TestClassPropGet.Should().BeNull();
    }

    [Test]
    public void Test_AutoMock_WithCallBase_SetsUp_NonDefaultReadOnlyProps_WhenInterface()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture() { CallBase = true };
        // Act
        var obj = fixture.CreateAutoMock<ITestInterface>();

        // Assert
        obj.Should().NotBeNull();
        obj!.TestProp.Should().NotBeNull();
    }

    [Test]
    public void Test_AutoMock_WithCallBase_SetsUp_ReadOnlyProps_WhenAbstract()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture() { CallBase = true };
        // Act
        var obj = fixture.CreateAutoMock<InternalAbstractReadonlyPropertyClass>();

        // Assert
        obj.Should().NotBeNull();
        obj!.InternalTest.Should().NotBeNull();
    }

    [Test]
    public void Test_AutoMock_WithCallBase_SetsUp_ReadWriteProps() // Via the AutoProperties command
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture() { CallBase = true };
        // Act
        var obj = fixture.CreateAutoMock<InternalSimpleTestClass>();
        var mock = AutoMock.Get(obj);

        // Assert

        // We need first to verify that it is an AutoMock, otherwise it is no biggy...
        mock.Should().NotBeNull();
        mock.Should().BeOfType<AutoMock<InternalSimpleTestClass>>();

        obj!.InternalTest.Should().NotBeNull();
    }

    [Test]
    public void Test_AutoMock_WithCallBase_SetsUp_ReadWriteFields() // Via the AutoProperties command
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture() { CallBase = true };
        // Act
        var obj = fixture.CreateAutoMock<InternalTestFields>();
        obj.Should().NotBeNull();

        var mock = AutoMock.Get(obj);

        // Assert

        // We need first to verify that it is an AutoMock, otherwise it is no biggy...
        mock.Should().NotBeNull();
        mock.Should().BeOfType<AutoMock<InternalTestFields>>();

        obj!.InternalTest.Should().NotBeNull();
    }

    [Test]
    public void Test_AutoMock_WithCallBase_DoesNotSetup_NonAbstractMethods()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture() { CallBase = true };
        // Act
        var obj = fixture.CreateAutoMock<InternalAbstractSimpleTestClass>();
        obj.Should().NotBeNull();

        var mock = AutoMock.Get(obj);

        // Assert

        // We need first to verify that it is an AutoMock, otherwise it is no biggy...
        mock.Should().NotBeNull();
        mock.Should().BeOfType<AutoMock<InternalAbstractSimpleTestClass>>();

        obj!.NonAbstractMethod().Should().BeNull();
    }

    [Test]
    public void Test_AutoMock_WithCallBase_SetsUp_AbstractMethods()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture() { CallBase = true };
        // Act
        var obj = fixture.CreateAutoMock<InternalTestMethods>();

        // Assert
        obj.Should().NotBeNull();
        obj!.InternalTestMethod().Should().NotBeNull();
    }

    [Test]
    public void Test_AutoMock_WithCallBase_CallsBase_NonAbstractMethods()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture() { CallBase = true };
        // Act
        var obj = fixture.CreateAutoMock<InternalAbstractSimpleTestClass>();
        obj.Should().NotBeNull();
        var mock = AutoMock.Get(obj);

        // Assert

        // We need first to verify that it is an AutoMock, otherwise it is no biggy...
        mock.Should().NotBeNull();
        mock.Should().BeOfType<AutoMock<InternalAbstractSimpleTestClass>>();

        obj!.NonAbstractWithValueMethod().Should().Be(10);
    }

    [Test]
    [TestCase<UnitFixture>]
    [TestCase<IntegrationFixture>]
    public void Test_AutoMock_WithFixtureWideCallBase_CallsBase_BugRepro<TFixture>()
            where TFixture : AutoMockFixtureBase, new()
    {
        // Arrange
        var fixture = new TFixture();
        fixture.AutoMockTypeControl.AlwaysAutoMockTypes.Add(typeof(InternalSimpleTestClass));
        fixture.CallBase = true;
        // Act
        var obj = fixture.Create<WithCtorArgsTestClass>();

        // Assert
        obj!.Should().NotBeNull();
        obj!.TestCtorArg.Should().BeAutoMock();
        AutoMock.Get(obj.TestCtorArg)!.CallBase.Should().BeTrue();
    }

    [Test]
    [TestCase<UnitFixture>]
    [TestCase<IntegrationFixture>]
    public async Task TestAsync_AutoMock_WithFixtureWideCallBase_CallsBase_BugRepro<TFixture>()
            where TFixture : AutoMockFixtureBase, new()
    {
        // Arrange
        var fixture = new TFixture();
        fixture.AutoMockTypeControl.AlwaysAutoMockTypes.Add(typeof(InternalSimpleTestClass));
        fixture.CallBase = true;
        // Act
        var obj = await fixture.CreateAsync<WithCtorArgsTestClass>().ConfigureAwait(false);

        // Assert
        obj!.Should().NotBeNull();
        obj!.TestCtorArg.Should().BeAutoMock();
        AutoMock.Get(obj.TestCtorArg)!.CallBase.Should().BeTrue();
    }

    [Test]
    [TestCase<UnitFixture>]
    [TestCase<IntegrationFixture>]
    public void Test_UnitFixture_DoesNotChangeOnSelf_BugRepro<TFixture>() where TFixture : AutoMockFixtureBase, new()
    {
        // Arrange
        var fixture = new TFixture();

        // Act
        var f = fixture.Create<TFixture>() as TFixture; // The bug was that UnitFixture/IntegrationFixture were not directly registered and therefore went through AutoMockDependencyBuilder which was setting up their properties

        // Assert
        f.Should().NotBeNull();
        f.Should().BeSameAs(fixture);
        fixture.CallBase.Should().BeNull();
    }
}
