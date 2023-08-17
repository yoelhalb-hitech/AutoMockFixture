
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

    [Test]
    [IntegrationAutoData]
    public void Test_IntegrationAutoDataAttribute_CallBase_WhenCallBaseAttributeTrue([CallBase(true)] AutoMock<WithCtorArgsTestClass> autoMockTestClass,
                                                [CallBase(true)] WithCtorArgsTestClass dependencyTestClass,
                                                IntegrationFixture fixture)
    {
        autoMockTestClass.CallBase.Should().BeTrue();
        foreach (var path in fixture.GetPaths(dependencyTestClass))
        {
            if (fixture.TryGetAutoMock(dependencyTestClass, path, out var autoMock)) autoMock.CallBase.Should().BeTrue();
        }
    }

    [Test]
    [IntegrationAutoData]
    public void Test_IntegrationAutoDataAttribute_CallBase_WhenCallBaseAttributeEmpty([CallBase] AutoMock<WithCtorArgsTestClass> autoMockTestClass,
                                            [CallBase] WithCtorArgsTestClass dependencyTestClass,
                                            IntegrationFixture fixture)
    {
        autoMockTestClass.CallBase.Should().BeTrue();
        foreach (var path in fixture.GetPaths(dependencyTestClass))
        {
            if (fixture.TryGetAutoMock(dependencyTestClass, path, out var autoMock)) autoMock.CallBase.Should().BeTrue();
        }
    }

    [Test]
    [IntegrationAutoData]
    public void Test_IntegrationAutoDataAttribute_NotCallBaseForMain_ButCallsForChildren_WhenCallBaseAttributeFalse(
                                                    [CallBase(false)] AutoMock<WithCtorArgsTestClass> autoMockTestClass,
                                                    [CallBase(false)] WithCtorArgsTestClass dependencyTestClass,
                                                    IntegrationFixture fixture)
    {
        autoMockTestClass.CallBase.Should().BeFalse();

        foreach (var path in fixture.GetPaths(dependencyTestClass))
        {
            if (fixture.TryGetAutoMock(dependencyTestClass, path, out var autoMock)) autoMock.CallBase.Should().BeTrue();
        }
    }

    [Test]
    [IntegrationAutoData]
    public void Test_IntegrationAutoDataAttribute_NotCallBaseForMain_ButCallsForChildren_WhenNoCallBaseAttribute(AutoMock<WithCtorArgsTestClass> autoMockTestClass,
                                                                                    WithCtorArgsTestClass dependencyTestClass,
                                                                                    IntegrationFixture fixture)
    {
        autoMockTestClass.CallBase.Should().BeFalse();

        foreach (var path in fixture.GetPaths(dependencyTestClass))
        {
            if (fixture.TryGetAutoMock(dependencyTestClass, path, out var autoMock)) autoMock.CallBase.Should().BeTrue();
        }
    }
}
