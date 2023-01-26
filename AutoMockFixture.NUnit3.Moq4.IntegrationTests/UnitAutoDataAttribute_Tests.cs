using AutoMockFixture.AutoMockUtils;
using AutoMockFixture.Moq4.AutoMockUtils;

namespace AutoMockFixture.Tests.AutoMockFixture_Tests;

internal class AutoDataUnitAttribute_Tests
{
    [Test]
    [UnitAutoData]
    public void Test_UnitAutoDataAttribute_Works(InternalSimpleTestClass testClass)
    {
        testClass.Should().NotBeNull();
        testClass.InternalTest.Should().NotBeNullOrWhiteSpace();
    }

    [Test]
    [UnitAutoData]
    public void Test_UnitAutoDataAttribute_WorksWithAutoMock(AutoMock<InternalSimpleTestClass> testClass)
    {
        testClass.Should().NotBeNull();

        var inner = testClass.GetMocked();

        inner.Should().NotBeNull();
        inner.InternalTest.Should().NotBeNullOrWhiteSpace();
    }

    [Test]
    [UnitAutoData]
    public void Test_UnitAutoDataAttribute_AutoMocksDependencies(WithCtorArgsTestClass testClass)
    {
        testClass.Should().NotBeNull();

        testClass.TestCtorArg.Should().NotBeNull();
        AutoMock.IsAutoMock(testClass.TestCtorArg).Should().BeTrue();
    }

    [Test]
    [UnitAutoData]
    public void Test_UnitAutoDataAttribute_AutoMocksDependencies_WhenAutoMock(AutoMock<WithCtorArgsTestClass> testClass)
    {
        testClass.Should().NotBeNull();

        var inner = testClass.GetMocked();

        inner.Should().NotBeNull();

        inner.TestCtorArgProp.Should().NotBeNull();
        AutoMock.IsAutoMock(inner.TestCtorArgProp).Should().BeTrue();
    }
}
