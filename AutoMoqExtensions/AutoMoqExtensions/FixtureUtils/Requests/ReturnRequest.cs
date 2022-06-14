using AutoMoqExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Requests;

internal class ReturnRequest : BaseTracker, IEquatable<ReturnRequest>
{
    public ReturnRequest(Type declaringType, MethodInfo methodInfo, ITracker? tracker)
        : base(tracker)
    {
        DeclaringType = declaringType;
        MethodInfo = methodInfo;
    }

    public virtual Type DeclaringType { get; }
    public virtual MethodInfo MethodInfo { get; }

    public override string InstancePath => "." + MethodInfo.GetTrackingPath() + ".";

    public override bool Equals(object obj) 
        => obj is ReturnRequest other ? this.Equals(other) : base.Equals(obj);

    public override int GetHashCode() => HashCode.Combine(DeclaringType, MethodInfo);

    public virtual bool Equals(ReturnRequest other)
        => other is not null && this.DeclaringType == other.DeclaringType && this.MethodInfo == other.MethodInfo;       
}

