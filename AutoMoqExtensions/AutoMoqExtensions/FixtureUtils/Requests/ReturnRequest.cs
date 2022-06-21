using AutoMoqExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Requests;

internal class ReturnRequest : BaseTracker
{
    public ReturnRequest(Type declaringType, MethodInfo methodInfo, Type returnType, ITracker? tracker)
        : base(tracker)
    {
        DeclaringType = declaringType;
        MethodInfo = methodInfo;
        ReturnType = returnType;
    }

    public virtual Type DeclaringType { get; }
    public virtual MethodInfo MethodInfo { get; }
    public Type ReturnType { get; }

    public override string InstancePath => "." + MethodInfo.GetTrackingPath() + ".";

    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), DeclaringType, MethodInfo, ReturnType);

    public override bool IsRequestEquals(ITracker other)
        => other is ReturnRequest request && this.DeclaringType == request.DeclaringType
            && this.MethodInfo == request.MethodInfo && this.ReturnType == request.ReturnType;
}

