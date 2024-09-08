using static AutoMockFixture.MethodSetupTypes;

namespace AutoMockFixture.Tests.AutoMockFixture_Tests;

internal class GetAutoMocks_Tests
{
    [Test]
    public void Test_Works_MainObject_CreatAutoMock_BugRepro(
        [Values(true, false)]bool callBase,
        [Values(Eager, LazySame, LazyDifferent)] MethodSetupTypes setupType)
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.MethodSetupType = setupType;

        var obj = fixture.CreateAutoMock<WithComplexTestClass>(callBase);
        obj.Should().BeAutoMock();

        var fromFixture = fixture.GetAutoMocks<WithComplexTestClass>(obj!);

        fromFixture.Should().NotBeNull();
        fromFixture.Should().HaveCount(1);
        fromFixture.First().Should().Be(AutoMock.Get(obj));

        var withPath = fixture.GetAutoMock<WithComplexTestClass>(obj!, "");

        withPath.Should().NotBeNull();
        withPath.Should().Be(AutoMock.Get(obj));
    }

    [Test]
    [TestCase(false, 8, 1, Eager)]
    [TestCase(false, 8, 1, LazySame)]
    [TestCase(false, 8, 1, LazyDifferent)]
    [TestCase(true, 6, 0, Eager)]
    [TestCase(true, 6, 0, LazySame)]
    [TestCase(true, 6, 0, LazyDifferent)]
    public void Test_Works_CreatAutoMock(bool callBase, int expectSimple, int expectedAbstractMethod, MethodSetupTypes setupType)
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.MethodSetupType = setupType;

        var obj = fixture.CreateAutoMock<WithComplexTestClass>(callBase);

        if (setupType != MethodSetupTypes.Eager)
        {
            _ = obj!.WithCtorArgs!.TestClassPropGet;
            _ = obj!.WithCtorArgs!.TestCtorArgVirtualProp;
            _ = obj!.WithCtorArgs!.TestCtorArgVirtualPrivateProp;
            _ = obj!.WithCtorArgs!.TestClassPropWithPrivateSet;
        }

        obj.Should().NotBeNull();
        var mocks = fixture.GetAutoMocks<WithCtorArgsTestClass>(obj!);

        mocks.Should().NotBeNull();
        mocks.Count().Should().Be(1);
        mocks.First().Should().NotBeNull();


        var mocks1 = fixture.GetAutoMocks<InternalSimpleTestClass>(obj!);

        mocks1.Should().NotBeNull();
        mocks1.Count().Should().Be(expectSimple);
        mocks1.Contains(null).Should().BeFalse();

        var mocks2 = fixture.GetAutoMocks<InternalAbstractMethodTestClass>(obj!);

        mocks2.Should().NotBeNull();
        mocks2.Count().Should().Be(expectedAbstractMethod);
        mocks2.Contains(null).Should().BeFalse();

        var mocks3 = fixture.GetAutoMocks<InternalAbstractSimpleTestClass>(obj!);

        mocks3.Should().NotBeNull();
        mocks3.Count().Should().Be(1);
        mocks3.Contains(null).Should().BeFalse();
    }

    [Test]
    [TestCase(false, 8, 1, Eager)]
    [TestCase(false, 8, 1, LazySame)]
    [TestCase(false, 8, 1, LazyDifferent)]
    [TestCase(true, 6, 0, Eager)]
    [TestCase(true, 6, 0, LazySame)]
    [TestCase(true, 6, 0, LazyDifferent)]
    public void Test_Works_CreatAutoMockDepndencies(bool callBase, int expectSimple, int expectedAbstractMethod, MethodSetupTypes setupType)
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.MethodSetupType = setupType;

        var obj = fixture.CreateWithAutoMockDependencies<WithComplexTestClass>(callBase);

        if (setupType != MethodSetupTypes.Eager)
        {
            _ = obj!.WithCtorArgs!.TestClassPropGet;
            _ = obj!.WithCtorArgs!.TestCtorArgVirtualProp;
            _ = obj!.WithCtorArgs!.TestCtorArgVirtualPrivateProp;
            _ = obj!.WithCtorArgs!.TestClassPropWithPrivateSet;
        }

        obj.Should().NotBeNull();
        var mocks = fixture.GetAutoMocks<WithCtorArgsTestClass>(obj!);

        mocks.Should().NotBeNull();
        mocks.Count().Should().Be(1);
        mocks.First().Should().NotBeNull();


        var mocks1 = fixture.GetAutoMocks<InternalSimpleTestClass>(obj!);

        mocks1.Should().NotBeNull();
        mocks1.Count().Should().Be(expectSimple);
        mocks1.Contains(null).Should().BeFalse();

        var mocks2 = fixture.GetAutoMocks<InternalAbstractMethodTestClass>(obj!);

        mocks2.Should().NotBeNull();
        mocks2.Count().Should().Be(expectedAbstractMethod);
        mocks2.Contains(null).Should().BeFalse();

        var mocks3 = fixture.GetAutoMocks<InternalAbstractSimpleTestClass>(obj!);

        mocks3.Should().NotBeNull();
        mocks3.Count().Should().Be(1);
        mocks3.Contains(null).Should().BeFalse();
    }
}
