using SequelPay.DotNetPowerExtensions.Reflection;
using System.Diagnostics.CodeAnalysis;

namespace AutoMockFixture.FixtureUtils.Requests.MainRequests;

internal record AutoMockDirectRequest : TrackerWithFixture, IRequestWithType, IFixtureTracker, IDisposable
{
    [SetsRequiredMembers]
    public AutoMockDirectRequest(Type request, ITracker tracker) : base(request, tracker) { }

    [SetsRequiredMembers]
    public AutoMockDirectRequest(Type request, IAutoMockFixture fixture) : base(request, fixture) { }


    public override bool MockDependencies => StartTracker is null || StartTracker is AutoMockDirectRequest ? true : StartTracker.MockDependencies; // Avoid stack overflow

    public virtual bool IsRequestEquals(ITracker other)
    {
        if(other is not AutoMockDirectRequest request
            || request.Request != Request
            || !Object.ReferenceEquals(request.Fixture, Fixture)
            || request.MockDependencies != MockDependencies)
                return false;

        // The special logic for `AutoMockDependenciesRequest` is only for the SUT, but we're here dealing with dependencies, so we should be fine
        var dependeciesCallBaseEqual = (StartTracker.MockShouldCallBase ?? false) == (request.StartTracker.MockShouldCallBase ?? false);
        if (!dependeciesCallBaseEqual) return false;

        if(ShouldCallBase() == request.ShouldCallBase()) return true;

        // CAUTION: Can't rely on `IsInterface` as default implemented interfaces are also
        var inner = GetInner();
        if (!inner.IsInterface) return false; // They might have a different ctor minimum

        return
            new[] { inner }.Union(inner.GetInterfaces())
                        .All(i => i.GetMethods(BindingFlagsExtensions.AllBindings).All(m => m.IsAbstract));
    }

    private Type GetInner() => Fixture.AutoMockHelpers.GetMockedType(Request)!;

    public bool ShouldCallBase() => !GetInner().IsDelegate() // Moq does not allow to callbase for delegates
            && (MockShouldCallBase // This is the only place where we concern ourselves with the request explicit `MockShouldCallBase` as in general we should follow what the user requested
                    ?? StartTracker.MockShouldCallBase
                    ?? StartTracker.Fixture.CallBase // TBH: This should be the same as the `StartTracker`
                    ?? (!MockDependencies ? true :
                            Path == ""
                            && StartTracker.Children?.FirstOrDefault() is not AutoMockRequest { IsStartRequest: true}));


    public void Dispose() => SetCompleted((ISpecimenBuilder?)null);
}
