
namespace AutoMockFixture.FixtureUtils.Requests;

internal abstract record BaseTracker : ITracker, IEquatable<BaseTracker>
{
    public BaseTracker(BaseTracker tracker)
    {
        SetParent(tracker);
        children = new List<ITracker>(); // Looks like a copy ctor doesn't intialize the fields by default
    }

    public BaseTracker(ITracker? tracker)
    {
        SetParent(tracker);
    }

    internal void SetParent(ITracker? tracker)
    {
        Parent = tracker;
        tracker?.AddChild(this);
    }

    protected WeakReference? result;
    protected List<ITracker> children = new List<ITracker>();
    protected List<WeakReference<IAutoMock>>? allMocks;

    public virtual IFixtureTracker StartTracker => Parent?.StartTracker ?? this as IFixtureTracker ?? throw new Exception("No valid start tracker provided");

    public virtual ITracker? Parent { get; private set; }

    public virtual List<ITracker> Children => children;
    public virtual void AddChild(ITracker tracker) => Children.Add(tracker);

    public string Path => BasePath + InstancePath;
    public abstract string InstancePath { get; }

    public string BasePath => Parent?.Path ?? "";

    public virtual List<WeakReference<IAutoMock>>? GetAllMocks() => allMocks;
    public WeakReference? Result => result;
    protected Dictionary<string, List<WeakReference?>>? childrensPaths;
    public Dictionary<string, List<WeakReference?>>? GetChildrensPaths() => childrensPaths;
    protected bool completed;
    public bool IsCompleted => completed;

    public ISpecimenBuilder? Builder { get; protected set; }
    public ISpecimenCommand? Command { get; protected set; }

    public void SetCompleted(ISpecimenBuilder? builder)
    {
        if (completed) return;

        this.Builder = builder;

        completed = true;
        UpdateResult();
    }

    public void SetCompleted(ISpecimenCommand command)
    {
        if (completed) return;

        this.Command = command;

        completed = true;
        UpdateResult();
    }

    public virtual void UpdateResult()
    {
        // TODO... maybe we should rather take it out from the ProcessingTrackerDict
        // TODO... what about setting up something that hasn't been created yet?
        // Note: It can happen by a generic method that hasn't been called yet and so the result is not yet set up
        var childrenWithResult = Children.Where(c => c.IsCompleted).ToList();

        allMocks = childrenWithResult.SelectMany(c => c.GetAllMocks() ?? new List<WeakReference<IAutoMock>>()).OfType<WeakReference<IAutoMock>>().ToList();
        if (result is not null && StartTracker.Fixture.AutoMockHelpers.GetFromObj(result) is IAutoMock mock) allMocks.Add(mock.ToWeakReference());

        // Probably not worth to do Distinct here (as the caller will do it), unless it is the last one
        if (Parent is null) allMocks = allMocks.Distinct().ToList();

        // Many trackers are just wrappers
        if (result is null && InstancePath != "" // Remember that we don't keep a reference to the main object for GC purposes
                && Children.Count == 1 && Children[0].InstancePath == "") result = Children[0].Result;

        //if (result is null) throw new Exception("Expected result but there isn't"); can actually be null...
        Logger.LogInfo(ToString()!);
        Logger.LogInfo(Path);

        try
        {
            childrensPaths = childrenWithResult!.SelectMany(c => c.GetChildrensPaths() ?? new Dictionary<string, List<WeakReference?>>())
                        .GroupBy(c => c.Key) // We don't need null and it can cause duplicates (for example in factory method calling multiple times a constructor with different values)
                        .ToDictionary(c => c.Key, c => c.SelectMany(x => x.Value).Distinct().ToList());
            if (InstancePath != "" && !childrensPaths.ContainsKey(Path)) childrensPaths.Add(Path, new List<WeakReference?> { result });
            else if (InstancePath != "" && !childrensPaths[Path].Contains(result)) childrensPaths[Path].Add(result);
        }
        catch (Exception ex)
        {
            Logger.LogInfo("Error of type: " + ex.GetType().FullName + " - Has Inner: " + (ex.InnerException is not null).ToString());
            Logger.LogInfo(ex.Message);
            Logger.LogInfo(ToString()!);
            Logger.LogInfo(Path);
        }

        Parent?.UpdateResult();
    }

    public virtual void SetResult(object? result, ISpecimenBuilder? builder)
    {
        if (completed) return;

        // We don't want a reference to the main result to avoid memory leaks
        if (Path != String.Empty) this.result = result.ToWeakReference();

        StartTracker.Fixture.Cache.AddIfNeeded(this, result);

        SetCompleted(builder);
    }

    public virtual bool IsRequestEquals(ITracker other)
        => StartTracker.IsStartTrackerEquals(other.StartTracker);

    //AutoFixture uses this to determine recursion
#pragma warning disable CS8851 // Record defines 'Equals' but not 'GetHashCode'.
    public virtual bool Equals(BaseTracker? other) => other is not null
            && other.StartTracker == StartTracker
            && IsRequestEquals(other);
#pragma warning restore CS8851 // Record defines 'Equals' but not 'GetHashCode'.

    public sealed override string? ToString() // Otherwise we can run in a stack overflow see an exmaple in https://stackoverflow.com/questions/67285095/c-sharp-record-tostring-causes-stack-overflow-and-stops-debugging-session-with
    {
        return base.ToString();
    }
}
