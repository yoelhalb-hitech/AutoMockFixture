using AutoMoqExtensions.FixtureUtils.Requests.HelperRequests.NonAutoMock;
using System.Reflection;

namespace AutoMoqExtensions.FixtureUtils.Requests.HelperRequests.AutoMock;

internal class AutoMockFieldRequest : FieldRequest, IAutoMockRequest
{
    public AutoMockFieldRequest(Type declaringType, FieldInfo fieldInfo, ITracker? tracker)
        : base(declaringType, fieldInfo, tracker)
    {
    }

    public override bool IsRequestEquals(ITracker other)
        => other is AutoMockFieldRequest && base.IsRequestEquals(other);
}
