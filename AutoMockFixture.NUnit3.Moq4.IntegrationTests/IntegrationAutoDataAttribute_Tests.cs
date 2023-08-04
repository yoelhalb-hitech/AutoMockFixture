
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

    [Test]
    [IntegrationAutoData]
    public void Test_IntegrationAutoDataAttribute_ReturnsSelf_WhenIntegrationFixture(IntegrationFixture fixture1, IntegrationFixture fixture2, AutoMock<WithCtorArgsTestClass> testClass)
    {
        fixture1.Should().NotBeNull();
        fixture2.Should().NotBeNull();
        testClass.Should().NotBeNull();

        fixture1.Should().BeSameAs(fixture2);
        Assert.DoesNotThrow(() => fixture1.GetPaths(testClass.Object).Should().NotBeNull());
    }
}
