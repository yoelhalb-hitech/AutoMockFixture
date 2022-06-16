using AutoMoqExtensions.AutoMockUtils;
using AutoMoqExtensions.FixtureUtils.Requests;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoMoqExtensions.FixtureUtils
{
    internal abstract class BaseTracker: ITracker, IEquatable<BaseTracker>
    {
        public BaseTracker(ITracker? tracker)
        {
            Parent = tracker;
            tracker?.AddChild(this);
            // Note: Even if StartTracker.IsInAutoMockChain the parent might not necessarily be as there might an object that couldn't be an automocked
            // Note: At this point our `StartTracker` might still be null so we use the parents
            IsInAutoMockChain = Parent?.StartTracker.IsInAutoMockChain == true 
                                    || Parent?.IsInAutoMockChain == true || this is IAutoMockRequest;
        }

        protected object? result;
        protected List<ITracker> children = new List<ITracker>();
        protected List<IAutoMock>? allMocks;

        public virtual IFixtureTracker StartTracker => Parent?.StartTracker ?? this as IFixtureTracker ?? throw new Exception("No valid start tracker provided");
        public virtual object? StartObject => Parent?.StartObject ?? result;

        public virtual ITracker? Parent { get; }
        public virtual bool IsInAutoMockChain { get; }
        
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
            this.UpdateResult();
        }
        public void UpdateResult()
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
            try
            {
                childrensPaths = childrenWithResult.SelectMany(c => c.GetChildrensPaths())
                            .GroupBy(c => c.Key) // We don't need null and it can cause duplicates (for example in factory method calling multiple times a constructor with different values)
                            .ToDictionary(c => c.Key, c => c.SelectMany(x => x.Value).Distinct().ToList());
                if (this.InstancePath != "" && !childrensPaths.ContainsKey(Path)) childrensPaths.Add(Path, new List<object?> { result });
                else if (this.InstancePath != "" && !childrensPaths[Path].Contains(result)) childrensPaths[Path].Add(result);
            }
            catch (Exception ex) 
            {
            }

            Parent?.UpdateResult();
        }
        public virtual void SetResult(object? result)
        {
            this.result = result;
            if(result is not null) StartTracker.Fixture.ProcessingTrackerDict[result] = this;
            SetCompleted();
        }

        public override bool Equals(object obj)
            => obj is BaseTracker other ? this.Equals(other) : base.Equals(obj);

        public override int GetHashCode() => HashCode.Combine(BasePath, StartTracker != this ? StartTracker : (ITracker?)null, 
                StartTracker == this ? "StartTracker".GetHashCode() * 34526 : (int?)null, Parent, Children);
        public virtual bool Equals(BaseTracker other) => other is not null
                && other.BasePath == BasePath && other.StartTracker == this.StartTracker
                && other.Parent == Parent && other.Children.SequenceEqual(Children);

    }
}
