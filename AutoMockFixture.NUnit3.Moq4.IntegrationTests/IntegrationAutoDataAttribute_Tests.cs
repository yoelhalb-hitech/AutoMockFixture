

namespace AutoMockFixture.NUnit3.Moq4.IntegrationTests;

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
        AutoMock.IsAutoMock(testClass.TestCtorArg).Should().BeFalse();
    }

    [Test]
    [IntegrationAutoData]
    public void Test_IntegrationAutoDataAttribute_DoesNotAutoMocksDependencies_WhenAutoMock(AutoMock<WithCtorArgsTestClass> testClass)
    {
        testClass.Should().NotBeNull();

        var inner = testClass.GetMocked();

        inner.Should().NotBeNull();

        inner.TestClassProp.Should().NotBeNull();
        AutoMock.IsAutoMock(inner.TestClassProp).Should().BeFalse();
    }
}
