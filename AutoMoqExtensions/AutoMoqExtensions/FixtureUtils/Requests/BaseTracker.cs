using AutoMoqExtensions.AutoMockUtils;
using AutoMoqExtensions.FixtureUtils.Requests.MainRequests;

namespace AutoMoqExtensions.FixtureUtils.Requests;

internal abstract class BaseTracker : ITracker, IEquatable<BaseTracker>
{
    public BaseTracker(ITracker? tracker)
    {
        SetParent(tracker);
    }

    internal void SetParent(ITracker? tracker)
    {
        Parent = tracker;
        tracker?.AddChild(this);
    }

    protected object? result;
    protected List<ITracker> children = new List<ITracker>();
    protected List<IAutoMock>? allMocks;

    public virtual IFixtureTracker StartTracker => Parent?.StartTracker ?? this as IFixtureTracker ?? throw new Exception("No valid start tracker provided");
    public virtual object? StartObject => Parent?.StartObject ?? result;

    public virtual ITracker? Parent { get; private set; }

    public virtual List<ITracker> Children => children;
    public virtual void AddChild(ITracker tracker) => Children.Add(tracker);

    public string Path => BasePath + InstancePath;
    public abstract string InstancePath { get; }

    public string BasePath => Parent?.Path ?? "";

    public virtual List<IAutoMock>? GetAllMocks() => allMocks;
    public object? Result => result;
    protected Dictionary<string, List<object?>>? childrensPaths;
    public Dictionary<string, List<object?>>? GetChildrensPaths() => childrensPaths;
    protected bool completed;
    public bool IsCompleted => completed;

    public void SetCompleted()
    {
        if (completed) return;

        completed = true;
        UpdateResult();
    }
    public virtual void UpdateResult()
    {
        // TODO... maybe we should rather take it out from the ProcessingTrackerDict
        // TODO... what about setting up something that hasn't been created yet?
        // Note: It can happen by a generic method that hasn't been called yet and so the result is not yet set up
        var childrenWithResult = Children.Where(c => c.IsCompleted).ToList();

        allMocks = childrenWithResult.SelectMany(c => c.GetAllMocks()).ToList();
        if (result is not null && AutoMockHelpers.GetFromObj(result) is IAutoMock mock) allMocks.Add(mock);

        // Probably not worth to do Distinct here (as the caller will do it), unless it is the last one
        if (Parent is null) allMocks = allMocks.Distinct().ToList();

        if (result is null && Children.Count == 1 && Children[0].InstancePath == "") result = Children[0].Result;

        //if (result is null) throw new Exception("Expected result but there isn't"); can actually be null...
        Logger.LogInfo(ToString());
        Logger.LogInfo(Path);

        try
        {
            childrensPaths = childrenWithResult.SelectMany(c => c.GetChildrensPaths())
                        .GroupBy(c => c.Key) // We don't need null and it can cause duplicates (for example in factory method calling multiple times a constructor with different values)
                        .ToDictionary(c => c.Key, c => c.SelectMany(x => x.Value).Distinct().ToList());
            if (InstancePath != "" && !childrensPaths.ContainsKey(Path)) childrensPaths.Add(Path, new List<object?> { result });
            else if (InstancePath != "" && !childrensPaths[Path].Contains(result)) childrensPaths[Path].Add(result);
        }
        catch (Exception ex)
        {
            Logger.LogInfo("Error of type: " + ex.GetType().FullName + " - Has Inner: " + (ex.InnerException is not null).ToString());
            Logger.LogInfo(ex.Message);
            Logger.LogInfo(ToString());
            Logger.LogInfo(Path);
        }

        Parent?.UpdateResult();
    }
    public virtual void SetResult(object? result)
    {
        this.result = result;
        if (result is not null) StartTracker.Fixture.ProcessingTrackerDict[result] = this;
        SetCompleted();
    }

    public virtual bool IsRequestEquals(ITracker other)
        => StartTracker.IsStartTrackerEquals(other.StartTracker);

    public override bool Equals(object obj)
        => obj is BaseTracker other ? Equals(other) : base.Equals(obj);

    public override int GetHashCode() => HashCode.Combine(BasePath, StartTracker != this ? StartTracker : (ITracker?)null,
            StartTracker == this ? "StartTracker".GetHashCode() * 34526 : (int?)null, Parent, Children);

    // AutoFixture uses this to determine recursion
    public virtual bool Equals(BaseTracker other) => other is not null
            && other.StartTracker == StartTracker
            && IsRequestEquals(other);

}
