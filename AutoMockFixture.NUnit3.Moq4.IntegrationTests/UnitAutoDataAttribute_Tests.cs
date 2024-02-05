using AutoMockFixture.AutoMockUtils;
using AutoMockFixture.Moq4.AutoMockUtils;
using NUnit.Framework.Internal;
using AutoMockFixture.NUnit3;

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

    [Test]
    [UnitAutoData]
    public void Test_UnitAutoDataAttribute_ReturnsSelf_WhenUnitFixture(UnitFixture fixture1, UnitFixture fixture2, AutoMock<WithCtorArgsTestClass> testClass)
    {
        fixture1.Should().NotBeNull();
        fixture2.Should().NotBeNull();
        testClass.Should().NotBeNull();

        fixture1.Should().BeSameAs(fixture2);
        Assert.DoesNotThrow(() => fixture1.GetPaths(testClass.Object).Should().NotBeNull());
    }

    [Test]
    [UnitAutoData]
    public void Test_UnitAutoDataAttribute_CallBase_WhenCallBaseAttributeTrue([CallBase(true)] AutoMock<WithCtorArgsTestClass> autoMockTestClass,
                                                    [CallBase(true)] WithCtorArgsTestClass dependencyTestClass,
                                                    UnitFixture fixture)
    {
        autoMockTestClass.CallBase.Should().BeTrue();
        foreach (var path in fixture.GetPaths(dependencyTestClass))
        {
            if(fixture.TryGetAutoMock(dependencyTestClass, path, out var autoMock)) autoMock.CallBase.Should().BeTrue();
        }
    }

    [Test]
    [UnitAutoData]
    public void Test_UnitAutoDataAttribute_CallBase_WhenCallBaseAttributeEmpty([CallBase] AutoMock<WithCtorArgsTestClass> autoMockTestClass,
                                                [CallBase] WithCtorArgsTestClass dependencyTestClass,
                                                UnitFixture fixture)
    {
        autoMockTestClass.CallBase.Should().BeTrue();
        foreach (var path in fixture.GetPaths(dependencyTestClass))
        {
            if (fixture.TryGetAutoMock(dependencyTestClass, path, out var autoMock)) autoMock.CallBase.Should().BeTrue();
        }
    }

    [Test]
    [UnitAutoData]
    public void Test_UnitAutoDataAttribute_NotCallBase_WhenCallBaseAttributeFalse([CallBase(false)] AutoMock<WithCtorArgsTestClass> autoMockTestClass,
                                                    [CallBase(false)] WithCtorArgsTestClass dependencyTestClass,
                                                    UnitFixture fixture)
    {
        autoMockTestClass.CallBase.Should().BeFalse();

        foreach (var path in fixture.GetPaths(dependencyTestClass))
        {
            if (fixture.TryGetAutoMock(dependencyTestClass, path, out var autoMock)) autoMock.CallBase.Should().BeFalse();
        }
    }

    [Test]
    [UnitAutoData]
    public void Test_UnitAutoDataAttribute_NotCallBase_WhenNoCallBaseAttribute(AutoMock<WithCtorArgsTestClass> autoMockTestClass,
                                                                                    WithCtorArgsTestClass dependencyTestClass,
                                                                                    UnitFixture fixture)
    {
        autoMockTestClass.CallBase.Should().BeFalse();

        foreach (var path in fixture.GetPaths(dependencyTestClass))
        {
            if (fixture.TryGetAutoMock(dependencyTestClass, path, out var autoMock)) autoMock.CallBase.Should().BeFalse();
        }
    }

    [Test]
    public void Test_UnitAutoDataAttribute_DoesNotThrow()
    {
        // If this doesn't throw then there is no point in the test...
        Assert.Catch(() => new AutoMockData(() => new UnitFixture()).BuildFrom(null!, null).ToArray()); // Will only throw on enumeration

        var attribute = new UnitAutoDataAttribute();

        IEnumerable<TestMethod>? result = null;
        Assert.DoesNotThrow(() => result = attribute.BuildFrom(null!, null));

        Assert.DoesNotThrow(() => result!.ToArray());

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}
