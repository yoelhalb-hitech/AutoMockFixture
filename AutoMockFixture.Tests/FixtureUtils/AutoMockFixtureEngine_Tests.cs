using AutoMockFixture.AutoMockUtils;
using AutoMockFixture.FixtureUtils;
using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;
using Moq;

namespace AutoMockFixture.Tests.FixtureUtils;

internal class AutoMockFixtureEngine_Tests
{
    private Mock<AutoMockFixtureBase> GetFixtureMock()
    {
        var helpers = Mock.Of<IAutoMockHelpers>(h =>
                                    h.IsAutoMockAllowed(It.IsAny<Type>(), It.IsAny<bool>()) == true
                                    && h.GetAutoMockInitCommand() == Mock.Of<ISpecimenCommand>()
                                    && h.MockRequestSpecification == Mock.Of<IRequestSpecification>());

        var fixtureMock = new AutoMock<AutoMockFixtureBase>();
        fixtureMock.SetupGet(m => m.AutoMockHelpers).Returns(helpers);
        fixtureMock.As<IAutoMockFixture>().SetupGet(m => m.AutoMockHelpers).Returns(helpers);

        var cache = new Cache(fixtureMock.Object);
        fixtureMock.SetupGet(m => m.Cache).Returns(cache);
        fixtureMock.As<IAutoMockFixture>().SetupGet(m => m.Cache).Returns(cache);

        var startTrackerMock = new Mock<TrackerWithFixture>(typeof(string), fixtureMock.Object) { CallBase = true };
        fixtureMock.Setup(m => m.GetStartTrackerForAutoMock(It.IsAny<Type>(), It.IsAny<bool>())).Returns(startTrackerMock.Object);

        return fixtureMock;
    }

    private void TestNonGeneric<T>(bool callBase, Func<AutoMockFixtureEngine, Type, AutoMockTypeControl, object?> func) where T : TrackerWithFixture, IRequestWithType
    {
        var fixtureMock = GetFixtureMock();

        var result = new object();
        var autoMockTypeControl = new AutoMockTypeControl();

        var t = typeof(object);

        fixtureMock.As<ISpecimenBuilder>().Setup(b => b.Create(It.Is<object>(r => r is T && (r as T)!.Request == t && (r as T)!.MockShouldCallBase == callBase),
                                                               It.Is<ISpecimenContext>(c => c is RecursionContext
                                                                    && (c as RecursionContext)!.AutoMockTypeControl == autoMockTypeControl)))
                                           .Returns(result)
                                           .Verifiable();

        var engine = new AutoMockFixtureEngine(fixtureMock.Object);
        var retValue = func(engine, t, autoMockTypeControl);

        retValue.Should().Be(result);
        fixtureMock.Verify();
    }

    private void TestGeneric<TTracker, TType>(bool callBase, Func<AutoMockFixtureEngine, AutoMockTypeControl, object?> func)
            where TTracker : TrackerWithFixture, IRequestWithType
    {
        var fixtureMock = GetFixtureMock();

        var result = new object();
        var autoMockTypeControl = new AutoMockTypeControl();

        fixtureMock.As<ISpecimenBuilder>().Setup(b => b.Create(It.Is<object>(r => r is TTracker && (r as TTracker)!.Request == typeof(TType)
                                                                                                && (r as TTracker)!.MockShouldCallBase == callBase),
                                                               It.Is<ISpecimenContext>(c => c is RecursionContext
                                                                    && (c as RecursionContext)!.AutoMockTypeControl == autoMockTypeControl)))
                                           .Returns(result)
                                           .Verifiable();

        var engine = new AutoMockFixtureEngine(fixtureMock.Object);
        var retValue = func(engine, autoMockTypeControl);

        retValue.Should().Be(result);
        fixtureMock.Verify();
    }

    #region AutoMockDependencies

    [Test]
    public void Test_CreateWithAutoMockDependencies_ForwardsCorretly([Values(true, false)] bool callBase)
    {
        TestNonGeneric<AutoMockDependenciesRequest>(callBase, (e, t, tc) => e.CreateWithAutoMockDependencies(t, callBase, tc));
    }

    [Test]
    public void Test_CreateWithAutoMockDependencies_Generic_ForwardsCorretly([Values(true, false)] bool callBase)
    {
        TestGeneric<AutoMockDependenciesRequest, object>(callBase, (e, tc) => e.CreateWithAutoMockDependencies<object>(callBase, tc));
    }

    [Test]
    public void Test_CreateWithAutoMockDependenciesAsync_ForwardsCorretly([Values(true, false)] bool callBase)
    {
        TestNonGeneric<AutoMockDependenciesRequest>(callBase, (e, t, tc) => e.CreateWithAutoMockDependenciesAsync(t, callBase, tc).Result);
    }

    [Test]
    public void Test_CreateWithAutoMockDependenciesAsync_Generic_ForwardsCorretly([Values(true, false)] bool callBase)
    {
        TestGeneric<AutoMockDependenciesRequest, object>(callBase, (e, tc) => e.CreateWithAutoMockDependenciesAsync<object>(callBase, tc).Result);
    }

    #endregion

    #region NonAutoMock

    [Test]
    public void Test_CreateNonAutoMock_ForwardsCorretly([Values(true, false)] bool callBase)
    {
        TestNonGeneric<NonAutoMockRequest>(callBase, (e, t, tc) => e.CreateNonAutoMock(t, callBase, tc));
    }

    [Test]
    public void Test_CreateNonAutoMock_Generic_ForwardsCorretly([Values(true, false)] bool callBase)
    {
        TestGeneric<NonAutoMockRequest, object>(callBase, (e, tc) => e.CreateNonAutoMock<object>(callBase, tc));
    }

    [Test]
    public void Test_CreateNonAutoMockAsync_ForwardsCorretly([Values(true, false)] bool callBase)
    {
        TestNonGeneric<NonAutoMockRequest>(callBase, (e, t, tc) => e.CreateNonAutoMockAsync(t, callBase, tc).Result);
    }

    [Test]
    public void Test_CreateNonAutoMockAsync_Generic_ForwardsCorretly([Values(true, false)] bool callBase)
    {
        TestGeneric<NonAutoMockRequest, object>(callBase, (e, tc) => e.CreateNonAutoMockAsync<object>(callBase, tc).Result);
    }

    #endregion

    #region AutoMock

    [Test]
    public void Test_CreateAutoMock_ForwardsCorretly([Values(true, false)] bool callBase)
    {
        TestNonGeneric<AutoMockRequest>(callBase, (e, t, tc) => e.CreateAutoMock(t, callBase, tc));
    }

    [Test]
    public void Test_CreateAutoMock_Generic_ForwardsCorretly([Values(true, false)] bool callBase)
    {
        TestGeneric<AutoMockRequest, object>(callBase, (e, tc) => e.CreateAutoMock<object>(callBase, tc));
    }

    [Test]
    public void Test_CreateAutoMockAsync_ForwardsCorretly([Values(true, false)] bool callBase)
    {
        TestNonGeneric<AutoMockRequest>(callBase, (e, t, tc) => e.CreateAutoMockAsync(t, callBase, tc).Result);
    }

    [Test]
    public void Test_CreateAutoMockAsync_Generic_ForwardsCorretly([Values(true, false)] bool callBase)
    {
        TestGeneric<AutoMockRequest, object>(callBase, (e, tc) => e.CreateAutoMockAsync<object>(callBase, tc).Result);
    }

    #endregion
}
