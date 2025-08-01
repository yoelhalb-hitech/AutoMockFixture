﻿
namespace AutoMockFixture.FixtureUtils.Requests.SpecialRequests;

internal record InnerRequest : BaseTracker, IRequestWithType
{
    public InnerRequest(Type request, IRequestWithType outerRequest) : base(outerRequest)
    {
        Request = request;
        OuterRequest = outerRequest;
    }


    public Type Request { get; }
    public IRequestWithType OuterRequest { get; }

    public override string InstancePath => ""; // TODO... is this right for task and delegate?
}
