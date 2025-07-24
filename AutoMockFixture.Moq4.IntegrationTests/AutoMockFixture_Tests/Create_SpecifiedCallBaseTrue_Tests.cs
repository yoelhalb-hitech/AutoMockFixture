using AutoMockFixture.FixtureUtils;
using AutoMockFixture.NUnit3;

namespace AutoMockFixture.Tests.AutoMockFixture_Tests;

internal class Create_SpecifiedCallBaseTrue_Tests
{
    [Test]
    [TestCase<UnitFixture, InternalAbstractMethodTestClass>]
    [TestCase<UnitFixture, ITestInterface>]
    [TestCase<IntegrationFixture, InternalAbstractMethodTestClass>]
    [TestCase<IntegrationFixture, ITestInterface>]
    public void Test_SUT_IsCallBase_WhenCallBase<TFixture, TType>()
        where TFixture : AutoMockFixtureBase, new()
        where TType : class
    {
        // Arrange
        var fixture = new TFixture();

        // Act Assert

        new[]{
            fixture.Create<TType>(true),
            fixture.CreateWithAutoMockDependencies<TType>(true),
            fixture.CreateAutoMock<TType>(true),
            fixture.CreateNonAutoMock<TType>(true),
            fixture.Freeze<TType>(true),
        }.Select(x => AutoMock.Get(x)!.CallBase).Should().AllBeTrue();
    }

    [Test]
    [TestCase<UnitFixture, AutoMock<InternalSimpleTestClass>>]
    [TestCase<UnitFixture, AutoMock<InternalAbstractMethodTestClass>>]
    [TestCase<UnitFixture, AutoMock<ITestInterface>>]
    [TestCase<IntegrationFixture, AutoMock<InternalSimpleTestClass>>]
    [TestCase<IntegrationFixture, AutoMock<InternalAbstractMethodTestClass>>]
    [TestCase<IntegrationFixture, AutoMock<ITestInterface>>]
    public void Test_SUT_IsCallBase_WhenCallBase_WhenAutoMockSpecified<TFixture, TType>()
        where TFixture : AutoMockFixtureBase, new()
        where TType : class, IAutoMock
    {
        // Arrange
        var fixture = new TFixture();

        // Act Assert

        new[]{
            fixture.Create<TType>(true),
            fixture.CreateWithAutoMockDependencies<TType>(true),
            fixture.CreateAutoMock<TType>(true),
            fixture.CreateNonAutoMock<TType>(true),
            fixture.Freeze<TType>(true),
        }.Select(x => x!.CallBase).Should().AllBeTrue();
    }

    [Test]
    [TestCase<UnitFixture>]
    [TestCase<IntegrationFixture>]
    public void Test_NonSUT_NonAutoMock_IsCallBase_WhenCallBase<TFixture>() where TFixture : AutoMockFixtureBase, new()
    {
        // Arrange
        var fixture = new TFixture();

        // Act

        var abstractCreate = fixture.Create<SUT_WithAbtsractInner>(true);
        var abstractCreateWithSpecified = fixture.Create<AutoMock<SUT_WithAbtsractInner>>(true);

        var abstractInnerInnerCreate = fixture.Create<SUT_WithNonAbtsractInner_AbstractInnerInner>(true);
        var abstractInnerInnerCreateWithSpecified = fixture.Create<AutoMock<SUT_WithNonAbtsractInner_AbstractInnerInner>>(true);


        var abstractCreateMock = fixture.CreateAutoMock<SUT_WithAbtsractInner>(true);
        var abstractCreateMockWithSpecified = fixture.CreateAutoMock<AutoMock<SUT_WithAbtsractInner>>(true);

        var abstractInnerInnerCreateMock = fixture.CreateAutoMock<SUT_WithNonAbtsractInner_AbstractInnerInner>(true);
        var abstractInnerInnerCreateMockWithSpecified = fixture.CreateAutoMock<AutoMock<SUT_WithNonAbtsractInner_AbstractInnerInner>>(true);

        var byCreateDependsWithSpecified = fixture.CreateWithAutoMockDependencies<AutoMock<SUT_WithNonAbtsractInner>>(true);

        var abstractCreateDepends = fixture.CreateWithAutoMockDependencies<SUT_WithAbtsractInner>(true);
        var abstractCreateDependsWithSpecified = fixture.CreateWithAutoMockDependencies<AutoMock<SUT_WithAbtsractInner>>(true);

        var abstractInnerInnerCreateDepends = fixture.CreateWithAutoMockDependencies<SUT_WithNonAbtsractInner_AbstractInnerInner>(true);
        var abstractInnerInnerCreateDependsWithSpecified = fixture.CreateWithAutoMockDependencies<AutoMock<SUT_WithNonAbtsractInner_AbstractInnerInner>>(true);


        var abstractCreateNonMock = fixture.CreateNonAutoMock<SUT_WithAbtsractInner>(true);
        var abstractCreateNonMockWithSpecified = fixture.CreateNonAutoMock<AutoMock<SUT_WithAbtsractInner>>(true);

        var abstractInnerInnerCreateNonMock = fixture.CreateNonAutoMock<SUT_WithNonAbtsractInner_AbstractInnerInner>(true);
        var abstractInnerInnerCreateNonMockWithSpecified = fixture.CreateNonAutoMock<AutoMock<SUT_WithNonAbtsractInner_AbstractInnerInner>>(true);

        // Assert

        AutoMock.Get(abstractCreate!.TestClassProp)!.CallBase.Should().BeTrue();
        AutoMock.Get(abstractCreateWithSpecified!.Object.TestClassProp)!.CallBase.Should().BeTrue();

        AutoMock.Get(abstractInnerInnerCreate!.TestClassProp!.TestClassProp)!.CallBase.Should().BeTrue();
        AutoMock.Get(abstractInnerInnerCreateWithSpecified!.Object.TestClassProp!.TestClassProp)!.CallBase.Should().BeTrue();


        AutoMock.Get(abstractCreateMock!.TestClassProp)!.CallBase.Should().BeTrue();
        AutoMock.Get(abstractCreateMockWithSpecified!.Object.TestClassProp)!.CallBase.Should().BeTrue();

        AutoMock.Get(abstractInnerInnerCreateMock!.TestClassProp!.TestClassProp)!.CallBase.Should().BeTrue();
        AutoMock.Get(abstractInnerInnerCreateMockWithSpecified!.Object.TestClassProp!.TestClassProp)!.CallBase.Should().BeTrue();


        AutoMock.Get(abstractCreateNonMock!.TestClassProp)!.CallBase.Should().BeTrue();
        AutoMock.Get(abstractCreateNonMockWithSpecified!.Object.TestClassProp)!.CallBase.Should().BeTrue();

        AutoMock.Get(abstractInnerInnerCreateNonMock!.TestClassProp!.TestClassProp)!.CallBase.Should().BeTrue();
        AutoMock.Get(abstractInnerInnerCreateNonMockWithSpecified!.Object.TestClassProp!.TestClassProp)!.CallBase.Should().BeTrue();

        AutoMock.Get(byCreateDependsWithSpecified!.Object.TestClassProp)!.CallBase.Should().BeTrue();
        AutoMock.Get(abstractCreateDepends!.TestClassProp)!.CallBase.Should().BeTrue();
        AutoMock.Get(abstractCreateDependsWithSpecified!.Object.TestClassProp)!.CallBase.Should().BeTrue();

        AutoMock.Get(abstractInnerInnerCreateDepends!.TestClassProp!.TestClassProp)!.CallBase.Should().BeTrue();
        AutoMock.Get(abstractInnerInnerCreateDependsWithSpecified!.Object.TestClassProp!.TestClassProp)!.CallBase.Should().BeTrue();
    }
}
