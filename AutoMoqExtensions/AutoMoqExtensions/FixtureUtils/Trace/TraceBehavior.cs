using AutoFixture.Kernel;
using AutoMoqExtensions.Extensions;
using System.Collections;

namespace AutoMoqExtensions.FixtureUtils.Trace;

internal class TraceBehavior : ISpecimenBuilderTransformation
{
    public TraceInfo TraceInfo { get; }

    public TraceBehavior(TraceInfo traceInfo)
    {
        TraceInfo = traceInfo;
    }

    public ISpecimenBuilderNode Transform(ISpecimenBuilder builder)
    {
        return new TraceWrapper(builder, TraceInfo, 1);
    }
}
