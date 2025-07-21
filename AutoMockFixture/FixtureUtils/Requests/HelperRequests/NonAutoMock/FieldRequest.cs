
namespace AutoMockFixture.FixtureUtils.Requests.HelperRequests.NonAutoMock;

internal record FieldRequest : BaseTracker
{
    public FieldRequest(Type declaringType, FieldInfo fieldInfo, ITracker? tracker) : base(tracker)
    {
        DeclaringType = declaringType;
        FieldInfo = fieldInfo;
    }

    public virtual Type DeclaringType { get; }
    public virtual FieldInfo FieldInfo { get; }

    public override string InstancePath => "." + FieldInfo.Name;
}
