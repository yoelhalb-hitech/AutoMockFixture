using System.Collections;

namespace AutoMockFixture.FixtureUtils.Trace;

internal class TraceWrapper : ISpecimenBuilderNode
{
    public TraceWrapper(ISpecimenBuilder builder, TraceInfo traceInfo, int depth)
    {
        Builder = builder ?? throw new ArgumentNullException(nameof(builder));
        TraceInfo = traceInfo ?? throw new ArgumentNullException(nameof(traceInfo));
        Depth = depth;
        TraceGenerator = new TraceGenerator(builder, traceInfo, depth);
    }

    public ISpecimenBuilder Builder { get; }
    public TraceInfo TraceInfo { get; }
    public int Depth { get; }
    public TraceGenerator TraceGenerator { get; }

    private static MethodInfo? ComposeIfMultipleMethod = typeof(CompositeSpecimenBuilder).GetMethod("ComposeIfMultiple", BindingFlags.NonPublic | BindingFlags.Static);
    public ISpecimenBuilderNode? Compose(IEnumerable<ISpecimenBuilder> builders)
    {
        var wrapped = builders.Select(b => b is TraceWrapper w ? w : new TraceWrapper(b, TraceInfo, Depth + 1));

        var result = Builder is ISpecimenBuilderNode n ? n.Compose(wrapped) : ComposeIfMultipleMethod?.Invoke(null, new object[] { wrapped }) as ISpecimenBuilder;

        if (result is null) return null;
        return new TraceWrapper(result, TraceInfo, Depth + 1);
    }

    public object Create(object request, ISpecimenContext context)
    {
        // Do it here instead of the ctor, so we will only do it when used
        // We anyway have to do it here in case anything changed
        TraceGenerator.EnsureAllBuilders();

        TraceInfo.TraceValues.Add((Builder, context, request, null, null, Depth));// First add so it should be in order

        var index = TraceInfo.TraceValues.Count - 1;
        try
        {
            var result = Builder.Create(request, context);
            TraceInfo.TraceValues[index] = (Builder, context, request, result, null, Depth);
            return result;
        }
        catch (Exception ex)
        {
            TraceInfo.TraceValues[index] = (Builder, context, request, null, ex, Depth);
            throw;
        }
    }

    /// <inheritdoc />
    public virtual IEnumerator<ISpecimenBuilder> GetEnumerator()
    {
        yield return Builder;
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
