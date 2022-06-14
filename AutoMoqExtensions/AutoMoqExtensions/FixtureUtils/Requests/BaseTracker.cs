using AutoMoqExtensions.AutoMockUtils;
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
        }

        protected object? result;
        protected List<ITracker> children = new List<ITracker>();
        protected List<IAutoMock>? allMocks;

        public virtual ITracker StartTracker => Parent?.StartTracker ?? this;
        public virtual object? StartObject => Parent?.StartObject ?? result;

        public virtual ITracker? Parent { get; }
        
        public virtual List<ITracker> Children => children;
        public virtual void AddChild(ITracker tracker) => Children.Add(tracker);

        public string Path => BasePath + InstancePath;
        public abstract string InstancePath { get; }

        public string BasePath => Parent?.Path ?? "";

        public virtual List<IAutoMock>? GetAllMocks() => allMocks;
        public object? Result => result;
        protected Dictionary<string, object?>? childrensPaths;
        public Dictionary<string, object?>? GetChildrensPaths() => childrensPaths;
        public void Completed()
        {
            allMocks = Children.SelectMany(c => c.GetAllMocks()).ToList();
            if (result is not null && AutoMockHelpers.GetFromObj(result) is IAutoMock mock) allMocks.Add(mock);

            // Probably not worth to do Distinct here (as the caller will do it), unless it is the last one
            if (Parent is null) allMocks = allMocks.Distinct().ToList();

            if (result is null && Children.Count == 1 && Children[0].InstancePath == "") result = Children[0].Result;

            //if (result is null) throw new Exception("Expected result but there isn't"); can actually be null...

            childrensPaths = Children.SelectMany(c => c.GetChildrensPaths()).ToDictionary(c => c.Key, c => c.Value);
            if (this.InstancePath != "") childrensPaths.Add(Path, result);
        }
        public virtual void SetResult(object? result)
        {
            this.result = result;
            Completed();
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
