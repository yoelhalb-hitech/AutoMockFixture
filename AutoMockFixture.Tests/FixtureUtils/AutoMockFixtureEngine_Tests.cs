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

        var startTrackerMock = new Mock<TrackerWithFixture>(fixtureMock.Object, null) { CallBase = true };
        fixtureMock.Setup(m => m.GetStartTrackerForAutoMock(It.IsAny<Type>(), It.IsAny<bool>())).Returns(startTrackerMock.Object);

        return fixtureMock;
    }

    private void TestNonGeneric<T>(bool callbase, Func<AutoMockFixtureEngine, Type, AutoMockTypeControl, object?> func) where T : TrackerWithFixture, IRequestWithType
    {
        var fixtureMock = GetFixtureMock();

        var result = new object();
        var autoMockTypeControl = new AutoMockTypeControl();

        var t = typeof(object);

        fixtureMock.As<ISpecimenBuilder>().Setup(b => b.Create(It.Is<object>(r => r is T && (r as T)!.Request == t && (r as T)!.MockShouldCallbase == callbase),
                                                               It.Is<ISpecimenContext>(c => c is RecursionContext
                                                                    && (c as RecursionContext)!.AutoMockTypeControl == autoMockTypeControl)))
                                           .Returns(result)
                                           .Verifiable();

        var engine = new AutoMockFixtureEngine(fixtureMock.Object);
        var retValue = func(engine, t, autoMockTypeControl);

        retValue.Should().Be(result);
        fixtureMock.Verify();
    }

    private void TestGeneric<TTracker, TType>(bool callbase, Func<AutoMockFixtureEngine, AutoMockTypeControl, object?> func)
            where TTracker : TrackerWithFixture, IRequestWithType
    {
        var fixtureMock = GetFixtureMock();

        var result = new object();
        var autoMockTypeControl = new AutoMockTypeControl();

        fixtureMock.As<ISpecimenBuilder>().Setup(b => b.Create(It.Is<object>(r => r is TTracker && (r as TTracker)!.Request == typeof(TType)
                                                                                                && (r as TTracker)!.MockShouldCallbase == callbase),
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
    public void Test_CreateWithAutoMockDependencies_ForwardsCorretly([Values(true, false)] bool callbase)
    {
        TestNonGeneric<AutoMockDependenciesRequest>(callbase, (e, t, tc) => e.CreateWithAutoMockDependencies(t, callbase, tc));
    }

    [Test]
    public void Test_CreateWithAutoMockDependencies_Generic_ForwardsCorretly([Values(true, false)] bool callbase)
    {
        TestGeneric<AutoMockDependenciesRequest, object>(callbase, (e, tc) => e.CreateWithAutoMockDependencies<object>(callbase, tc));
    }

    [Test]
    public void Test_CreateWithAutoMockDependenciesAsync_ForwardsCorretly([Values(true, false)] bool callbase)
    {
        TestNonGeneric<AutoMockDependenciesRequest>(callbase, (e, t, tc) => e.CreateWithAutoMockDependenciesAsync(t, callbase, tc).Result);
    }

    [Test]
    public void Test_CreateWithAutoMockDependenciesAsync_Generic_ForwardsCorretly([Values(true, false)] bool callbase)
    {
        TestGeneric<AutoMockDependenciesRequest, object>(callbase, (e, tc) => e.CreateWithAutoMockDependenciesAsync<object>(callbase, tc).Result);
    }

    #endregion

    #region NonAutoMock

    [Test]
    public void Test_CreateNonAutoMock_ForwardsCorretly([Values(true, false)] bool callbase)
    {
        TestNonGeneric<NonAutoMockRequest>(callbase, (e, t, tc) => e.CreateNonAutoMock(t, callbase, tc));
    }

    [Test]
    public void Test_CreateNonAutoMock_Generic_ForwardsCorretly([Values(true, false)] bool callbase)
    {
        TestGeneric<NonAutoMockRequest, object>(callbase, (e, tc) => e.CreateNonAutoMock<object>(callbase, tc));
    }

    [Test]
    public void Test_CreateNonAutoMockAsync_ForwardsCorretly([Values(true, false)] bool callbase)
    {
        TestNonGeneric<NonAutoMockRequest>(callbase, (e, t, tc) => e.CreateNonAutoMockAsync(t, callbase, tc).Result);
    }

    [Test]
    public void Test_CreateNonAutoMockAsync_Generic_ForwardsCorretly([Values(true, false)] bool callbase)
    {
        TestGeneric<NonAutoMockRequest, object>(callbase, (e, tc) => e.CreateNonAutoMockAsync<object>(callbase, tc).Result);
    }

    #endregion

    #region AutoMock

    [Test]
    public void Test_CreateAutoMock_ForwardsCorretly([Values(true, false)] bool callbase)
    {
        TestNonGeneric<AutoMockRequest>(callbase, (e, t, tc) => e.CreateAutoMock(t, callbase, tc));
    }

    [Test]
    public void Test_CreateAutoMock_Generic_ForwardsCorretly([Values(true, false)] bool callbase)
    {
        TestGeneric<AutoMockRequest, object>(callbase, (e, tc) => e.CreateAutoMock<object>(callbase, tc));
    }

    [Test]
    public void Test_CreateAutoMockAsync_ForwardsCorretly([Values(true, false)] bool callbase)
    {
        TestNonGeneric<AutoMockRequest>(callbase, (e, t, tc) => e.CreateAutoMockAsync(t, callbase, tc).Result);
    }

    [Test]
    public void Test_CreateAutoMockAsync_Generic_ForwardsCorretly([Values(true, false)] bool callbase)
    {
        TestGeneric<AutoMockRequest, object>(callbase, (e, tc) => e.CreateAutoMockAsync<object>(callbase, tc).Result);
    }

    #endregion
}
