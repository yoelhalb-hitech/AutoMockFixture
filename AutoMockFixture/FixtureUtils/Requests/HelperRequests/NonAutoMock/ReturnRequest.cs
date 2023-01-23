using System.Reflection;

namespace AutoMockFixture.FixtureUtils.Requests.HelperRequests.NonAutoMock;

internal class ReturnRequest : BaseTracker
{
    public ReturnRequest(Type declaringType, MethodInfo methodInfo, Type returnType, ITracker? tracker)
        : base(tracker)
    {
        DeclaringType = declaringType;
        MethodInfo = methodInfo;
        ReturnType = returnType;
    }

    public virtual Type DeclaringType { get; }
    public virtual MethodInfo MethodInfo { get; }
    public Type ReturnType { get; }

    public override string InstancePath => "." + MethodInfo.GetTrackingPath();

    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), DeclaringType, MethodInfo, ReturnType);

    public override bool IsRequestEquals(ITracker other)
        => base.IsRequestEquals(other)
            && other is ReturnRequest request && DeclaringType == request.DeclaringType
            && MethodInfo == request.MethodInfo && ReturnType == request.ReturnType;
}

