using System.Reflection;

namespace AutoMoqExtensions.FixtureUtils.Requests.HelperRequests.NonAutoMock;

internal class ConstructorArgumentRequest : BaseTracker
{
    public ConstructorArgumentRequest(Type declaringType, ParameterInfo parameterInfo, ITracker? tracker)
        : base(tracker)
    {
        DeclaringType = declaringType;
        ParameterInfo = parameterInfo;
    }

    public virtual Type DeclaringType { get; }
    public virtual ParameterInfo ParameterInfo { get; }

    public override string InstancePath => "->" + ParameterInfo.Name;

    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), DeclaringType, ParameterInfo);

    public override bool IsRequestEquals(ITracker other)
        => other is ConstructorArgumentRequest argumentRequest
            && DeclaringType == argumentRequest.DeclaringType
            && ParameterInfo == argumentRequest.ParameterInfo
            && base.IsRequestEquals(other);
}
