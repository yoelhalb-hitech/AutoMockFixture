
namespace AutoMoqExtensions.FixtureUtils.Requests;

public interface ITracker
{
    public IFixtureTracker StartTracker { get; }
    public ITracker? Parent { get; }
    public List<ITracker>? Children { get; }
    public List<IAutoMock>? GetAllMocks();
    public Dictionary<string, List<object?>>? GetChildrensPaths();
    public string Path { get; }
    public string InstancePath { get; }
    public string BasePath { get; }
    public object? Result { get; }
    public ISpecimenBuilder? Builder { get; }
    public ISpecimenCommand? Command { get; }
    public void SetResult(object? result, ISpecimenBuilder? builder);
    public bool IsCompleted { get; }
    public void SetCompleted(ISpecimenBuilder? builder);
    public void SetCompleted(ISpecimenCommand command);
    public void UpdateResult();
    public void AddChild(ITracker tracker);    
    public bool IsRequestEquals(ITracker other);
}
