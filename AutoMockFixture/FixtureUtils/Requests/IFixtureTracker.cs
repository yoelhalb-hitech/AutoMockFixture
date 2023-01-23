
namespace AutoMoqExtensions.FixtureUtils.Requests;

public interface IFixtureTracker : ITracker
{
    public AutoMockFixture Fixture { get; }
    public bool? MockShouldCallbase { get; }
    public bool MockDependencies { get; }
    /// <summary>
    /// Compare two <see cref="IFixtureTracker"/> objects to see if an object in the chain (but not necessarily they themselves) are to be considered equal requests
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool IsStartTrackerEquals(IFixtureTracker other);
    public event EventHandler<UpdateData>? DataUpdated;

    public class UpdateData : EventArgs
    {
        public List<WeakReference<IAutoMock>> AutoMocks { get; set; } = new();
        public Dictionary<string, List<WeakReference?>> Paths { get; set; } = new();
    }
}
