using AutoMockFixture.FixtureUtils;
using AutoMockFixture.NUnit3;

namespace AutoMockFixture.Tests.AutoMockFixture_Tests;

internal class Create_SpecifiedCallBaseFalse_Tests
{
    [Test]
    [TestCase<UnitFixture, InternalAbstractMethodTestClass>]
    [TestCase<UnitFixture, ITestInterface>]
    [TestCase<IntegrationFixture, InternalAbstractMethodTestClass>]
    [TestCase<IntegrationFixture, ITestInterface>]
    public void Test_SUT_IsNonCallBase_WhenNonCallBase<TFixture, TType>()
        where TFixture : AutoMockFixtureBase, new()
        where TType : class
    {
        // Arrange
        var fixture = new TFixture();

        // Act Assert

        new[]{
            fixture.Create<TType>(false),
            fixture.CreateWithAutoMockDependencies<TType>(false),
            fixture.CreateAutoMock<TType>(false),
            fixture.CreateNonAutoMock<TType>(false),
            fixture.Freeze<TType>(false),
        }.Select(x => AutoMock.Get(x)!.CallBase).Should().AllBeFalse();
    }

    [Test]
    [TestCase<UnitFixture, AutoMock<InternalSimpleTestClass>>]
    [TestCase<UnitFixture, AutoMock<InternalAbstractMethodTestClass>>]
    [TestCase<UnitFixture, AutoMock<ITestInterface>>]
    [TestCase<IntegrationFixture, AutoMock<InternalSimpleTestClass>>]
    [TestCase<IntegrationFixture, AutoMock<InternalAbstractMethodTestClass>>]
    [TestCase<IntegrationFixture, AutoMock<ITestInterface>>]
    public void Test_SUT_IsNonCallBase_WhenNonCallBase_WhenAutoMockSpecified<TFixture, TType>()
        where TFixture : AutoMockFixtureBase, new()
        where TType : class, IAutoMock
    {
        // Arrange
        var fixture = new TFixture();

        // Act Assert

        new[]{
            fixture.Create<TType>(false),
            fixture.CreateWithAutoMockDependencies<TType>(false),
            fixture.CreateAutoMock<TType>(false),
            fixture.CreateNonAutoMock<TType>(false),
            fixture.Freeze<TType>(false),
        }.Select(x => x!.CallBase).Should().AllBeFalse();
    }

    [Test]
    [TestCase<UnitFixture>]
    [TestCase<IntegrationFixture>]
    public void Test_NonSUT_NonAutoMock_IsNonCallBase_WhenNonCallBase<TFixture>() where TFixture : AutoMockFixtureBase, new()
    {
        // Arrange
        var fixture = new TFixture();

        // Act

        var abstractCreate = fixture.Create<SUT_WithAbtsractInner>(false);
        var abstractCreateWithSpecified = fixture.Create<AutoMock<SUT_WithAbtsractInner>>(false);

        var abstractInnerInnerCreate = fixture.Create<SUT_WithNonAbtsractInner_AbstractInnerInner>(false);
        var abstractInnerInnerCreateWithSpecified = fixture.Create<AutoMock<SUT_WithNonAbtsractInner_AbstractInnerInner>>(false);


        var abstractCreateMock = fixture.CreateAutoMock<SUT_WithAbtsractInner>(false);
        var abstractCreateMockWithSpecified = fixture.CreateAutoMock<AutoMock<SUT_WithAbtsractInner>>(false);

        var abstractInnerInnerCreateMock = fixture.CreateAutoMock<SUT_WithNonAbtsractInner_AbstractInnerInner>(false);
        var abstractInnerInnerCreateMockWithSpecified = fixture.CreateAutoMock<AutoMock<SUT_WithNonAbtsractInner_AbstractInnerInner>>(false);

        var byCreateDependsWithSpecified = fixture.CreateWithAutoMockDependencies<AutoMock<SUT_WithNonAbtsractInner>>(false);

        var abstractCreateDepends = fixture.CreateWithAutoMockDependencies<SUT_WithAbtsractInner>(false);
        var abstractCreateDependsWithSpecified = fixture.CreateWithAutoMockDependencies<AutoMock<SUT_WithAbtsractInner>>(false);

        var abstractInnerInnerCreateDepends = fixture.CreateWithAutoMockDependencies<SUT_WithNonAbtsractInner_AbstractInnerInner>(false);
        var abstractInnerInnerCreateDependsWithSpecified = fixture.CreateWithAutoMockDependencies<AutoMock<SUT_WithNonAbtsractInner_AbstractInnerInner>>(false);


        var abstractCreateNonMock = fixture.CreateNonAutoMock<SUT_WithAbtsractInner>(false);
        var abstractCreateNonMockWithSpecified = fixture.CreateNonAutoMock<AutoMock<SUT_WithAbtsractInner>>(false);

        var abstractInnerInnerCreateNonMock = fixture.CreateNonAutoMock<SUT_WithNonAbtsractInner_AbstractInnerInner>(false);
        var abstractInnerInnerCreateNonMockWithSpecified = fixture.CreateNonAutoMock<AutoMock<SUT_WithNonAbtsractInner_AbstractInnerInner>>(false);

        // Assert

        AutoMock.Get(abstractCreate!.TestClassProp)!.CallBase.Should().BeFalse();
        AutoMock.Get(abstractCreateWithSpecified!.Object.TestClassProp)!.CallBase.Should().BeFalse();

        AutoMock.Get(abstractInnerInnerCreate!.TestClassProp!.TestClassProp)!.CallBase.Should().BeFalse();
        AutoMock.Get(abstractInnerInnerCreateWithSpecified!.Object.TestClassProp!.TestClassProp)!.CallBase.Should().BeFalse();


        AutoMock.Get(abstractCreateMock!.TestClassProp)!.CallBase.Should().BeFalse();
        AutoMock.Get(abstractCreateMockWithSpecified!.Object.TestClassProp)!.CallBase.Should().BeFalse();

        AutoMock.Get(abstractInnerInnerCreateMock!.TestClassProp!.TestClassProp)!.CallBase.Should().BeFalse();
        AutoMock.Get(abstractInnerInnerCreateMockWithSpecified!.Object.TestClassProp!.TestClassProp)!.CallBase.Should().BeFalse();


        AutoMock.Get(abstractCreateNonMock!.TestClassProp)!.CallBase.Should().BeFalse();
        AutoMock.Get(abstractCreateNonMockWithSpecified!.Object.TestClassProp)!.CallBase.Should().BeFalse();

        AutoMock.Get(abstractInnerInnerCreateNonMock!.TestClassProp!.TestClassProp)!.CallBase.Should().BeFalse();
        AutoMock.Get(abstractInnerInnerCreateNonMockWithSpecified!.Object.TestClassProp!.TestClassProp)!.CallBase.Should().BeFalse();

        AutoMock.Get(byCreateDependsWithSpecified!.Object.TestClassProp)!.CallBase.Should().BeFalse();
        AutoMock.Get(abstractCreateDepends!.TestClassProp)!.CallBase.Should().BeFalse();
        AutoMock.Get(abstractCreateDependsWithSpecified!.Object.TestClassProp)!.CallBase.Should().BeFalse();

        AutoMock.Get(abstractInnerInnerCreateDepends!.TestClassProp!.TestClassProp)!.CallBase.Should().BeFalse();
        AutoMock.Get(abstractInnerInnerCreateDependsWithSpecified!.Object.TestClassProp!.TestClassProp)!.CallBase.Should().BeFalse();
    }
}
