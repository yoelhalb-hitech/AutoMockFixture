using AutoMoqExtensions.AutoMockUtils;
using AutoMoqExtensions.MockUtils;
using Moq;

namespace AutoMoqExtensions.Test.AutoMockFixture_Tests;

internal class Tracking_Tests
{
    [Test]
    public void Test_Create_AddsToTrackerDict_NonAutoMock()
    {
        var fixture = new AbstractAutoMockFixture();
        var result = fixture.CreateNonAutoMock<InternalSimpleTestClass>();

        fixture.TrackerDict.Should().HaveCount(1);
        fixture.TrackerDict.First().Key.Should().Be(result);
    }

    [Test]
    public void Test_Create_AddsToTrackerDict_NonAutoMock_WithDependencies()
    {
        var fixture = new AbstractAutoMockFixture();
        var result = fixture.CreateWithAutoMockDependencies<InternalSimpleTestClass>();

        fixture.TrackerDict.Should().HaveCount(1);
        fixture.TrackerDict.First().Key.Should().Be(result);
    }

    [Test]
    public void Test_Create_AddsUnderlyingMockToTrackerDict()
    {
        var fixture = new AbstractAutoMockFixture();
        var result = fixture.CreateAutoMock<InternalSimpleTestClass>();

        fixture.TrackerDict.Should().HaveCount(1);
        fixture.TrackerDict.First().Key.Should().Be(Mock.Get(result));
    }

    [Test]
    public void Test_ListsSetupMethods()
    {
        var fixture = new AbstractAutoMockFixture();
        var result = fixture.CreateAutoMock<WithCtorArgsTestClass>();
        var mock = AutoMockHelpers.GetAutoMock(result);

        mock!.MethodsSetup.Should().ContainKey("TestClassPropGet");
        mock!.MethodsNotSetup.Should().ContainKey("TestClassPrivateNonVirtualProp");
        mock!.MethodsNotSetup["TestClassPrivateNonVirtualProp"].Reason.Should().Be(CannotSetupMethodException.CannotSetupReason.NonVirtual);
    }

}
