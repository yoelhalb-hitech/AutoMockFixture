using AutoMockFixture.FixtureUtils;
using AutoMockFixture.NUnit3;

namespace AutoMockFixture.Tests.AutoMockFixture_Tests;

internal class Create_UnspecifiedCallBase_Tests
{
    [Test]
    [TestCase<UnitFixture, InternalAbstractMethodTestClass>]
    [TestCase<UnitFixture, ITestInterface>]
    [TestCase<IntegrationFixture, InternalAbstractMethodTestClass>]
    [TestCase<IntegrationFixture, ITestInterface>]
    public void Test_SUT_IsGenerally_CallBase<TFixture, TType>()
        where TFixture : AutoMockFixtureBase, new()
        where TType : class
    {
        // Arrange
        var fixture = new TFixture();

        // Act Assert

        new[]{
            fixture.Create<TType>(),
            fixture.CreateWithAutoMockDependencies<TType>(),
            fixture.CreateNonAutoMock<TType>(),
            fixture.Freeze<TType>(),
        }.Select(x => AutoMock.Get(x)!.CallBase).Should().AllBeTrue();
    }

    [Test]
    [TestCase<UnitFixture, AutoMock<InternalSimpleTestClass>>]
    [TestCase<UnitFixture, AutoMock<InternalAbstractMethodTestClass>>]
    [TestCase<UnitFixture, AutoMock<ITestInterface>>]
    [TestCase<IntegrationFixture, AutoMock<InternalSimpleTestClass>>]
    [TestCase<IntegrationFixture, AutoMock<InternalAbstractMethodTestClass>>]
    [TestCase<IntegrationFixture, AutoMock<ITestInterface>>]
    public void Test_SUT_IsGenerally_CallBase_WhenAutoMockSpecified<TFixture, TType>()
        where TFixture : AutoMockFixtureBase, new()
        where TType : IAutoMock
    {
        // Arrange
        var fixture = new TFixture();

        // Act Assert

        new[]{
            fixture.Create<TType>(),
            fixture.CreateWithAutoMockDependencies<TType>(),
            fixture.CreateNonAutoMock<TType>(),
            fixture.Freeze<TType>(),
        }.Select(x => x!.CallBase).ToArray().Should().AllBeTrue();
    }

    [Test]
    public void Test_UnitFixture_SUT_CreateAutoMock_NotCallBase()
    {
        // Arrange
        var fixture = new UnitFixture();

        // Act
        var byCreateAutoMock = fixture.CreateAutoMock<InternalSimpleTestClass>();
        var byCreateAutoMockWithSpecified = fixture.CreateAutoMock<AutoMock<InternalSimpleTestClass>>();

        var abstractCreateAutoMock = fixture.CreateAutoMock<InternalAbstractMethodTestClass>();
        var abstractCreateAutoMockWithSpecified = fixture.CreateAutoMock<AutoMock<InternalAbstractMethodTestClass>>();

        // Assert

        AutoMock.Get(byCreateAutoMock)!.CallBase.Should().BeFalse();
        byCreateAutoMockWithSpecified!.CallBase.Should().BeFalse();

        AutoMock.Get(abstractCreateAutoMock)!.CallBase.Should().BeFalse();
        abstractCreateAutoMockWithSpecified!.CallBase.Should().BeFalse();
    }

    [Test]
    public void Test_IntegrationFixture_SUT_CreateAutoMock_CallBase()
    {
        // Arrange
        var fixture = new IntegrationFixture();

        // Act
        var byCreateAutoMock = fixture.CreateAutoMock<InternalSimpleTestClass>();
        var byCreateAutoMockWithSpecified = fixture.CreateAutoMock<AutoMock<InternalSimpleTestClass>>();

        var abstractCreateAutoMock = fixture.CreateAutoMock<InternalAbstractMethodTestClass>();
        var abstractCreateAutoMockWithSpecified = fixture.CreateAutoMock<AutoMock<InternalAbstractMethodTestClass>>();

        // Assert

        AutoMock.Get(byCreateAutoMock)!.CallBase.Should().BeTrue();
        byCreateAutoMockWithSpecified!.CallBase.Should().BeTrue();

        AutoMock.Get(abstractCreateAutoMock)!.CallBase.Should().BeTrue();
        abstractCreateAutoMockWithSpecified!.CallBase.Should().BeTrue();
    }

    [Test]
    [TestCase<UnitFixture>]
    [TestCase<IntegrationFixture>]
    public void Test_NonSUT_NonAutoMock_CallBase<TFixture>() where TFixture : AutoMockFixtureBase, new()
    {
        // Arrange
        var fixture = new TFixture();

        // Act

        var abstractCreateNonMock = fixture.CreateNonAutoMock<SUT_WithAbtsractInner>();
        var abstractCreateNonMockWithSpecified = fixture.CreateNonAutoMock<AutoMock<SUT_WithAbtsractInner>>();

        var abstractInnerInnerCreateNonMock = fixture.CreateNonAutoMock<SUT_WithNonAbtsractInner_AbstractInnerInner>();
        var abstractInnerInnerCreateNonMockWithSpecified = fixture.CreateNonAutoMock<AutoMock<SUT_WithNonAbtsractInner_AbstractInnerInner>>();

        // Assert

        AutoMock.Get(abstractCreateNonMock!.TestClassProp)!.CallBase.Should().BeTrue();
        AutoMock.Get(abstractCreateNonMockWithSpecified!.Object.TestClassProp)!.CallBase.Should().BeTrue();

        AutoMock.Get(abstractInnerInnerCreateNonMock!.TestClassProp!.TestClassProp)!.CallBase.Should().BeTrue();
        AutoMock.Get(abstractInnerInnerCreateNonMockWithSpecified!.Object.TestClassProp!.TestClassProp)!.CallBase.Should().BeTrue();
    }

    [Test]
    [TestCase<UnitFixture>]
    [TestCase<IntegrationFixture>]
    public void Test_NonSUT_CreateWithDependencies_NonCallBase<TFixture>() where TFixture : AutoMockFixtureBase, new()
    {
        // Arrange
        var fixture = new TFixture();

        // Act

        var byCreateDependsWithSpecified = fixture.CreateWithAutoMockDependencies<AutoMock<SUT_WithNonAbtsractInner>>();

        var abstractCreateDepends = fixture.CreateWithAutoMockDependencies<SUT_WithAbtsractInner>();
        var abstractCreateDependsWithSpecified = fixture.CreateWithAutoMockDependencies<AutoMock<SUT_WithAbtsractInner>>();

        var abstractInnerInnerCreateDepends = fixture.CreateWithAutoMockDependencies<SUT_WithNonAbtsractInner_AbstractInnerInner>();
        var abstractInnerInnerCreateDependsWithSpecified = fixture.CreateWithAutoMockDependencies<AutoMock<SUT_WithNonAbtsractInner_AbstractInnerInner>>();

        // Assert

        AutoMock.Get(byCreateDependsWithSpecified!.Object.TestClassProp)!.CallBase.Should().BeFalse();
        AutoMock.Get(abstractCreateDepends!.TestClassProp)!.CallBase.Should().BeFalse();
        AutoMock.Get(abstractCreateDependsWithSpecified!.Object.TestClassProp)!.CallBase.Should().BeFalse();

        AutoMock.Get(abstractInnerInnerCreateDepends!.TestClassProp!.TestClassProp)!.CallBase.Should().BeFalse();
        AutoMock.Get(abstractInnerInnerCreateDependsWithSpecified!.Object.TestClassProp!.TestClassProp)!.CallBase.Should().BeFalse();
    }

    [Test]
    public void Test_NonSUT_Create_CreateAutoMock_IntegrationFixture_CallBase()
    {
        // Arrange
        var fixture = new IntegrationFixture();

        // Act

        var abstractCreate = fixture.Create<SUT_WithAbtsractInner>();
        var abstractCreateWithSpecified = fixture.Create<AutoMock<SUT_WithAbtsractInner>>();

        var abstractInnerInnerCreate = fixture.Create<SUT_WithNonAbtsractInner_AbstractInnerInner>();
        var abstractInnerInnerCreateWithSpecified = fixture.Create<AutoMock<SUT_WithNonAbtsractInner_AbstractInnerInner>>();


        var abstractCreateMock = fixture.CreateAutoMock<SUT_WithAbtsractInner>();
        var abstractCreateMockWithSpecified = fixture.CreateAutoMock<AutoMock<SUT_WithAbtsractInner>>();

        var abstractInnerInnerCreateMock = fixture.CreateAutoMock<SUT_WithNonAbtsractInner_AbstractInnerInner>();
        var abstractInnerInnerCreateMockWithSpecified = fixture.CreateAutoMock<AutoMock<SUT_WithNonAbtsractInner_AbstractInnerInner>>();

        // Assert

        AutoMock.Get(abstractCreate!.TestClassProp)!.CallBase.Should().BeTrue();
        AutoMock.Get(abstractCreateWithSpecified!.Object.TestClassProp)!.CallBase.Should().BeTrue();

        AutoMock.Get(abstractInnerInnerCreate!.TestClassProp!.TestClassProp)!.CallBase.Should().BeTrue();
        AutoMock.Get(abstractInnerInnerCreateWithSpecified!.Object.TestClassProp!.TestClassProp)!.CallBase.Should().BeTrue();


        AutoMock.Get(abstractCreateMock!.TestClassProp)!.CallBase.Should().BeTrue();
        AutoMock.Get(abstractCreateMockWithSpecified!.Object.TestClassProp)!.CallBase.Should().BeTrue();

        AutoMock.Get(abstractInnerInnerCreateMock!.TestClassProp!.TestClassProp)!.CallBase.Should().BeTrue();
        AutoMock.Get(abstractInnerInnerCreateMockWithSpecified!.Object.TestClassProp!.TestClassProp)!.CallBase.Should().BeTrue();
    }

    [Test]
    public void Test_NonSUT_Create_CreateAutoMock_UnitFixture_NonCallBase()
    {
        // Arrange
        var fixture = new UnitFixture();

        // Act

        var byCreate = fixture.Create<SUT_WithNonAbtsractInner>();
        var byCreateWithSpecified = fixture.Create<AutoMock<SUT_WithNonAbtsractInner>>();

        var abstractCreate = fixture.Create<SUT_WithAbtsractInner>();
        var abstractCreateWithSpecified = fixture.Create<AutoMock<SUT_WithAbtsractInner>>();

        var abstractInnerInnerCreate = fixture.Create<SUT_WithNonAbtsractInner_AbstractInnerInner>();
        var abstractInnerInnerCreateWithSpecified = fixture.Create<AutoMock<SUT_WithNonAbtsractInner_AbstractInnerInner>>();


        var byCreateMock = fixture.CreateAutoMock<SUT_WithNonAbtsractInner>();
        var byCreateMockWithSpecified = fixture.CreateAutoMock<AutoMock<SUT_WithNonAbtsractInner>>();

        var abstractCreateMock = fixture.CreateAutoMock<SUT_WithAbtsractInner>();
        var abstractCreateMockWithSpecified = fixture.CreateAutoMock<AutoMock<SUT_WithAbtsractInner>>();

        var abstractInnerInnerCreateMock = fixture.CreateAutoMock<SUT_WithNonAbtsractInner_AbstractInnerInner>();
        var abstractInnerInnerCreateMockWithSpecified = fixture.CreateAutoMock<AutoMock<SUT_WithNonAbtsractInner_AbstractInnerInner>>();

        // Assert

        AutoMock.Get(byCreate!.TestClassProp)!.CallBase.Should().BeFalse();
        AutoMock.Get(byCreateWithSpecified!.Object.TestClassProp)!.CallBase.Should().BeFalse();

        AutoMock.Get(abstractCreate!.TestClassProp)!.CallBase.Should().BeFalse();
        AutoMock.Get(abstractCreateWithSpecified!.Object.TestClassProp)!.CallBase.Should().BeFalse();

        AutoMock.Get(abstractInnerInnerCreate!.TestClassProp!.TestClassProp)!.CallBase.Should().BeFalse();
        AutoMock.Get(abstractInnerInnerCreateWithSpecified!.Object.TestClassProp!.TestClassProp)!.CallBase.Should().BeFalse();


        AutoMock.Get(byCreateMock!.TestClassProp)!.CallBase.Should().BeFalse();
        AutoMock.Get(byCreateMockWithSpecified!.Object.TestClassProp)!.CallBase.Should().BeFalse();

        AutoMock.Get(abstractCreateMock!.TestClassProp)!.CallBase.Should().BeFalse();
        AutoMock.Get(abstractCreateMockWithSpecified!.Object.TestClassProp)!.CallBase.Should().BeFalse();

        AutoMock.Get(abstractInnerInnerCreateMock!.TestClassProp!.TestClassProp)!.CallBase.Should().BeFalse();
        AutoMock.Get(abstractInnerInnerCreateMockWithSpecified!.Object.TestClassProp!.TestClassProp)!.CallBase.Should().BeFalse();
    }
}
