﻿
namespace AutoMockFixture.FixtureUtils.Requests.HelperRequests.NonAutoMock;

internal record PropertyRequest : BaseTracker
{
    public PropertyRequest(Type declaringType, PropertyInfo propertyInfo, ITracker? tracker) : base(tracker)
    {
        DeclaringType = declaringType;
        PropertyInfo = propertyInfo;
    }

    public virtual Type DeclaringType { get; }
    public virtual PropertyInfo PropertyInfo { get; }

    public override string InstancePath => "." + PropertyInfo.Name;


    public override bool IsRequestEquals(ITracker other)
        => base.IsRequestEquals(other) && other is PropertyRequest request
                && DeclaringType == request.DeclaringType
                && PropertyInfo == request.PropertyInfo;
}
