using AutoMoqExtensions.AutoMockUtils;

namespace AutoMoqExtensions.Test.AutoMockFixture_Tests;

internal class AutoDataIntegrationAttribute_Tests
{
    [Test]
    [IntegrationAutoData]
    public void Test_IntegrationAutoDataAttribute_Works(InternalSimpleTestClass testClass)
    {
        testClass.Should().NotBeNull();
        testClass.InternalTest.Should().NotBeNullOrWhiteSpace();
    }

    [Test]
    [IntegrationAutoData]
    public void Test_IntegrationAutoDataAttribute_WorksWithAutoMock(AutoMock<InternalSimpleTestClass> testClass)
    {
        testClass.Should().NotBeNull();

        var inner = testClass.GetMocked();

        inner.Should().NotBeNull();
        inner.InternalTest.Should().NotBeNullOrWhiteSpace();
    }

    [Test]
    [IntegrationAutoData]
    public void Test_IntegrationAutoDataAttribute_DoesNotAutoMockDependencies(WithCtorArgsTestClass testClass)
    {
        testClass.Should().NotBeNull();

        testClass.TestCtorArg.Should().NotBeNull();
        AutoMockHelpers.GetAutoMock(testClass.TestCtorArg).Should().BeNull();
    }

    [Test]
    [IntegrationAutoData]
    public void Test_IntegrationAutoDataAttribute_DoesNotAutoMocksDependencies_WhenAutoMock(AutoMock<WithCtorArgsTestClass> testClass)
    {
        testClass.Should().NotBeNull();

        var inner = testClass.GetMocked();

        inner.Should().NotBeNull();

        inner.TestClassProp.Should().NotBeNull();
        AutoMockHelpers.GetAutoMock(inner.TestClassProp).Should().BeNull();
    }
}
