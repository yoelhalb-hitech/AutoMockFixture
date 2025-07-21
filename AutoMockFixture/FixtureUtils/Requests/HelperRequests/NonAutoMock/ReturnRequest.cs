
namespace AutoMockFixture.FixtureUtils.Requests.HelperRequests.NonAutoMock;

internal record ReturnRequest : BaseTracker
{
    public ReturnRequest(Type declaringType, MethodInfo methodInfo, Type returnType, ITracker? tracker, string trackingPath)
        : base(tracker)
    {
        DeclaringType = declaringType;
        MethodInfo = methodInfo;
        ReturnType = returnType;
        TrackingPath = trackingPath;
    }

    public virtual Type DeclaringType { get; }
    public virtual MethodInfo MethodInfo { get; }
    public Type ReturnType { get; }

    public string TrackingPath { get; } // For example when setting up a readonly property via setup method

    public override string InstancePath => "." + TrackingPath; // TODO... There might be a conflict if the out argument name is the same as the ctor arg name of the return type
}

