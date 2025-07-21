using System.ComponentModel;

namespace AutoMockFixture.FixtureUtils.Requests;

[EditorBrowsable(EditorBrowsableState.Never)]
public interface IFixtureTracker : ITracker
{
    public IAutoMockFixture Fixture { get; }

    /// <summary>
    /// This property dependes on the context of the request
    /// If it is being set to a non null value on a `AutoMockRequest` or `AutoMockDirectRequest` then it is
    /// </summary>
    public bool? MockShouldCallBase { get; }
    public bool MockDependencies { get; }

    public event EventHandler<UpdateData>? DataUpdated;

    public class UpdateData : EventArgs
    {
        public List<WeakReference<IAutoMock>> AutoMocks { get; set; } = new();
        public Dictionary<string, List<WeakReference?>> Paths { get; set; } = new();
    }
}
