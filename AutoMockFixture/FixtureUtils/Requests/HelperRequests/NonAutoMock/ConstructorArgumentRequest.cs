
namespace AutoMockFixture.FixtureUtils.Requests.HelperRequests.NonAutoMock;

internal record ConstructorArgumentRequest : BaseTracker
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


    public override bool IsRequestEquals(ITracker other)
        => other is ConstructorArgumentRequest argumentRequest
            && DeclaringType == argumentRequest.DeclaringType
            && ParameterInfo == argumentRequest.ParameterInfo
            && base.IsRequestEquals(other);
}
