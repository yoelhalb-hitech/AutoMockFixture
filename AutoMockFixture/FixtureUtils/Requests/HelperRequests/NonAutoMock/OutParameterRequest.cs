
namespace AutoMockFixture.FixtureUtils.Requests.HelperRequests.NonAutoMock;

internal record OutParameterRequest : BaseTracker
{
    public OutParameterRequest(Type declaringType, MethodInfo methodInfo,
        ParameterInfo parameterInfo, Type parameterType, ITracker? tracker) : base(tracker)
    {
        DeclaringType = declaringType;
        MethodInfo = methodInfo;
        ParameterInfo = parameterInfo;
        ParameterType = parameterType;
    }

    public virtual Type DeclaringType { get; }
    public virtual MethodInfo MethodInfo { get; }
    public virtual ParameterInfo ParameterInfo { get; }
    public virtual Type ParameterType { get; }

    public override string InstancePath => "." + MethodInfo.GetTrackingPath() + "->" + ParameterInfo;


    public override bool IsRequestEquals(ITracker other)
        => base.IsRequestEquals(other)
        && other is OutParameterRequest outRequest && DeclaringType == outRequest.DeclaringType
        && MethodInfo == outRequest.MethodInfo
        && ParameterInfo == outRequest.ParameterInfo && ParameterType == outRequest.ParameterType;
}
