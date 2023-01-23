
namespace AutoMoqExtensions.FixtureUtils.Requests;

internal interface IRequestWithType : ITracker
{
    public Type Request { get; }
}
