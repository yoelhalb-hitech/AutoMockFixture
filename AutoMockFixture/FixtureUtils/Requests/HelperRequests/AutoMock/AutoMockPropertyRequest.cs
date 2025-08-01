﻿using AutoMockFixture.FixtureUtils.Requests.HelperRequests.NonAutoMock;

namespace AutoMockFixture.FixtureUtils.Requests.HelperRequests.AutoMock;

internal record AutoMockPropertyRequest : PropertyRequest, IAutoMockRequest
{
    public AutoMockPropertyRequest(Type declaringType, PropertyInfo propertyInfo, ITracker? tracker)
        : base(declaringType, propertyInfo, tracker)
    {
    }
}
