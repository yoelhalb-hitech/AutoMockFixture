﻿using AutoMockFixture.AutoMockUtils;
using AutoMockFixture.FixtureUtils;
using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;
using AutoMockFixture.Moq4.AutoMockUtils;

namespace AutoMockFixture.TestUtils;

public class AbstractAutoMockFixture : FixtureUtils.AutoMockFixtureBase
{
    public AbstractAutoMockFixture(bool noConfigureMembers = false) : base(noConfigureMembers) { }

    internal override IAutoMockHelpers AutoMockHelpers => new AutoMockHelpers();

    public override object Create(Type t, AutoMockTypeControl? autoMockTypeControl = null) => throw new NotSupportedException();
    public override object Create(Type t, bool callbase = false, AutoMockTypeControl? autoMockTypeControl = null) => throw new NotSupportedException();
    public override T Freeze<T>()
    {
        try
        {
            base.Freeze<T>();
        }
        catch { }
#pragma warning disable CS8603 // Possible null reference return.
        return default;
#pragma warning restore CS8603 // Possible null reference return.
    }

    // Essentially this will make it behave like the UnitFixture for the CreateAutoMock which is what the tests in general expect
    internal override TrackerWithFixture GetStartTrackerForAutoMock(Type type, bool callBase) => new AutoMockRequest(type, this) { MockShouldCallbase = callBase };
}
