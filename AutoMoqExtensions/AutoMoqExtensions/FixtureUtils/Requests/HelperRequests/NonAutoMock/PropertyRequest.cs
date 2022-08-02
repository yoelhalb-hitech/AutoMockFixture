using System.Reflection;

namespace AutoMoqExtensions.FixtureUtils.Requests.HelperRequests.NonAutoMock;

internal class PropertyRequest : BaseTracker
{
    public PropertyRequest(Type declaringType, PropertyInfo propertyInfo, ITracker? tracker) : base(tracker)
    {
        DeclaringType = declaringType;
        PropertyInfo = propertyInfo;
    }

    public virtual Type DeclaringType { get; }
    public virtual PropertyInfo PropertyInfo { get; }

    public override string InstancePath => "." + PropertyInfo.Name;

    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), DeclaringType, PropertyInfo);

    public override bool IsRequestEquals(ITracker other)
        => base.IsRequestEquals(other) && other is PropertyRequest request
                && DeclaringType == request.DeclaringType
                && PropertyInfo == request.PropertyInfo;
}
