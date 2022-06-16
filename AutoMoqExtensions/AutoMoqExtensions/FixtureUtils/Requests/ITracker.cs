using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils
{
    public interface ITracker
    {
        public ITracker StartTracker { get; }
        public object? StartObject { get; }
        public ITracker? Parent { get; }
        public List<ITracker>? Children { get; }
        public List<IAutoMock>? GetAllMocks();
        public Dictionary<string, List<object?>>? GetChildrensPaths();
        public string Path { get; }
        public string InstancePath { get; }
        public string BasePath { get; }
        public object? Result { get; }
        public void SetResult(object result);
        public bool IsCompleted { get; }
        public void SetCompleted();
        public void UpdateResult();
        public void AddChild(ITracker tracker);
    }
}
