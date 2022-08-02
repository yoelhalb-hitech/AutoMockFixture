namespace AutoMoqExtensions.FixtureUtils.Requests.SpecialRequests;

internal class ListItemRequest : OneOfMultipleRequest
{
    public ListItemRequest(Type request, int index, bool? autoMock, ITracker? tracker) : base(request, index, autoMock, tracker)
    {
    }

    public override string InstancePath => $"[{Index}]";

    public override bool IsRequestEquals(ITracker other) => other is ListItemRequest && base.IsRequestEquals(other);
}
