
using Castle.Core.Resource;
using FluentAssertions.Collections;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace AutoMockFixture.Tests.AutoMockFixture_Tests;

internal static class InternalSimpleTestClassExtensions
{
    public static InternalSimpleTestClassAssertions Should(this InternalSimpleTestClass instance)
    {
        return new InternalSimpleTestClassAssertions(instance);
    }

    [CustomAssertion]
    private static AndConstraint<GenericCollectionAssertions<InternalSimpleTestClass>> ValidAutoMocksCheck(
                    this GenericCollectionAssertions<InternalSimpleTestClass> assertions, bool callBase)
    {
        using (var scope = new AssertionScope())
        {
            assertions.AllNonNull();
            CollectionAssertions.ExecuteInternal(assertions, item => item.InternalTest is not null,
                                        $"having {nameof(InternalSimpleTestClass.InternalTest)} null");
            assertions.AllHaveAutoMockState(callBase);
        }

        return new AndConstraint<GenericCollectionAssertions<InternalSimpleTestClass>>(assertions);
    }

    [CustomAssertion]
    public static AndConstraint<GenericCollectionAssertions<InternalSimpleTestClass>> AllValidAutoMock(
            this GenericCollectionAssertions<InternalSimpleTestClass> assertions)
        => assertions.ValidAutoMocksCheck(true);

    [CustomAssertion]
    public static AndConstraint<GenericCollectionAssertions<InternalSimpleTestClass>> AllValidNonAutoMock(
            this GenericCollectionAssertions<InternalSimpleTestClass> assertions)
        => assertions.ValidAutoMocksCheck(false);
}

internal class InternalSimpleTestClassAssertions :
    ReferenceTypeExtendedAssertions<InternalSimpleTestClass, InternalSimpleTestClassAssertions>
{
    public InternalSimpleTestClassAssertions(InternalSimpleTestClass instance)
        : base(instance)
    {
    }

    protected override string Identifier => "InternalSimpleTestClass";

    [CustomAssertion]
    private AndConstraint<InternalSimpleTestClassAssertions> Validate(bool autoMock)
    {
        Subject.Should().NotBeNull();
        using (var scope = new AssertionScope(nameof(Subject.InternalTest)))
        {
            Subject.InternalTest.Should().NotBeNull();
        }

        Subject.Should().HaveAutoMockState(autoMock);

        return new AndConstraint<InternalSimpleTestClassAssertions>(this);
    }

    [CustomAssertion]
    public AndConstraint<InternalSimpleTestClassAssertions> BeValidAutoMock() => Validate(true);

    [CustomAssertion]
    public AndConstraint<InternalSimpleTestClassAssertions> BeValidNonAutoMock() => Validate(false);
}

internal class NonAutoMock_Tests
{
    [Test]
    public void Test_CreateNonAutoMock_NotAutoMocked()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateNonAutoMock<WithCtorArgsTestClass>();
        // Assert
        obj.Should().NotBeNull().And.Subject.Should().BeOfType<WithCtorArgsTestClass>();
        obj.Should().NotBeAutoMock();

        VerifyDependecies(obj!);
    }

    private void VerifyDependecies(WithCtorArgsTestClass obj)
    {
        new[] {
            obj!.TestCtorArg,
            obj.TestCtorArg,
            obj.TestCtorArgProp,
            obj.TestCtorArgPrivateProp,
            obj.TestCtorArgVirtualProp,
            obj.TestCtorArgVirtualPrivateProp,
            obj.TestClassProp!,
        }.Should().AllValidNonAutoMock();

        new object[] {
            obj.TestClassPrivateNonVirtualProp!,
            obj.TestClassPropWithPrivateSet!,
            obj.TestClassPropWithProtectedSet!,
            obj.TestClassPropGet!,
        }.Should().AllNull();

        obj.TestClassField.Should().NotBeNull();
    }

    private void VerifyWhenAutoMockNoCB(WithCtorArgsTestClass obj)
    {
        obj!.TestCtorArg.Should().BeNull();

        new[]
        {
            obj.TestCtorArgProp,
            obj.TestCtorArgVirtualProp,
            obj.TestCtorArgVirtualPrivateProp,
            obj.TestClassProp!,
            obj.TestClassPrivateNonVirtualProp!,
            obj.TestClassPropWithPrivateSet!,
            obj.TestClassPropWithProtectedSet!,
        }.Should().AllValidNonAutoMock();


        obj.TestClassPropGet.Should().NotBeNull();
        obj.TestClassField.Should().NotBeNull();
    }

    [Test]
    public void Test_MainObject_AutoMocked_WhenAutoMock_AndNonCallBase()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var result = fixture.CreateNonAutoMock<AutoMock<WithCtorArgsTestClass>>();
        // Assert
        result.Should().NotBeNull();

        var obj = result!.GetMocked();
        obj.Should().NotBeNull();

        VerifyWhenAutoMockNoCB(obj!);
    }

    [Test]
    public void Test_MainObject_AutoMocked_WhenAlwaysAutoMock_AndNonCallBase()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        fixture.AutoMockTypeControl.AlwaysAutoMockTypes.Add(typeof(WithCtorArgsTestClass));

        // Act
        var result = fixture.CreateNonAutoMock<WithCtorArgsTestClass>();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAutoMock();

        VerifyWhenAutoMockNoCB(result!);
    }

    [Test]
    public void Test_MainObject_AutoMocked_WhenAutoMock_AndCallBase()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();

        // Act
        var result = fixture.CreateNonAutoMock<AutoMock<WithCtorArgsTestClass>>(true);

        // Assert
        result.Should().NotBeNull();

        var obj = result!.GetMocked();
        obj.Should().NotBeNull();

        VerifyDependecies(obj!);
    }

    [Test]
    public void Test_MainObject_AutoMocked_WhenAlwaysAutoMock_AndCallBase()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        fixture.AutoMockTypeControl.AlwaysAutoMockTypes.Add(typeof(WithCtorArgsTestClass));

        // Act
        var result = fixture.CreateNonAutoMock<WithCtorArgsTestClass>(true);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAutoMock();

        VerifyDependecies(result!);
    }

    [Test]
    public void Test_MainObject_AutoMocked_WhenAbstract()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateNonAutoMock<InternalAbstractMethodTestClass>();
        // Assert
        obj.Should().NotBeNull().And.Subject.Should().BeAutoMock();
    }

    [Test]
    public void Test_MainObject_AutoMocked_WhenInterface()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateNonAutoMock<ITestInterface>();
        // Assert
        obj.Should().NotBeNull().And.Subject.Should().BeAutoMock();

        obj!.TestProp.Should().NotBeNull();
        obj.TestMethod().Should().NotBeNull();
    }
}
