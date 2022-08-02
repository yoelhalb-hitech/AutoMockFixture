using System.Reflection;

namespace AutoMoqExtensions.FixtureUtils.Requests.HelperRequests.NonAutoMock;

internal class FieldRequest : BaseTracker
{
    public FieldRequest(Type declaringType, FieldInfo fieldInfo, ITracker? tracker) : base(tracker)
    {
        DeclaringType = declaringType;
        FieldInfo = fieldInfo;
    }

    public virtual Type DeclaringType { get; }
    public virtual FieldInfo FieldInfo { get; }

    public override string InstancePath => "." + FieldInfo.Name;

    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), DeclaringType, FieldInfo);

    public override bool IsRequestEquals(ITracker other)
     => base.IsRequestEquals(other) && other is FieldRequest otherRequest
            && DeclaringType == otherRequest.DeclaringType && FieldInfo == otherRequest.FieldInfo;
}
