
namespace AutoMockFixture.FixtureUtils.Trace;

public class TraceBehavior : ISpecimenBuilderTransformation
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
