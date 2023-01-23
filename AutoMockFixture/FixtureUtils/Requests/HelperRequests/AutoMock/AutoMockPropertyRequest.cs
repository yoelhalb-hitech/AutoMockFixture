using AutoMockFixture.FixtureUtils.Requests.HelperRequests.NonAutoMock;
using System.Reflection;

namespace AutoMockFixture.FixtureUtils.Requests.HelperRequests.AutoMock;

internal class AutoMockPropertyRequest : PropertyRequest, IAutoMockRequest
{
    public AutoMockPropertyRequest(Type declaringType, PropertyInfo propertyInfo, ITracker? tracker)
        : base(declaringType, propertyInfo, tracker)
    {
    }

    public override bool IsRequestEquals(ITracker other)
        => other is AutoMockPropertyRequest && base.IsRequestEquals(other);
}
