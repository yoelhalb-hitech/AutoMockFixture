
namespace AutoMockFixture.FixtureUtils.Trace;

internal class TraceGenerator
{
    public TraceGenerator(ISpecimenBuilder builder, TraceInfo traceInfo, int depth)
    {
        Builder = builder ?? throw new ArgumentNullException(nameof(builder));
        TraceInfo = traceInfo ?? throw new ArgumentNullException(nameof(traceInfo));
        Depth = depth;

        var properties = builder.GetType().GetAllProperties(true);
        SingleProps = properties.Where(p =>
            (typeof(ISpecimenBuilder) == p.PropertyType || typeof(ISpecimenBuilderNode) == p.PropertyType)
            && p.SetMethod is not null).ToArray();

        EnumerableProps = properties.Where(p => typeof(IList<ISpecimenBuilder>).IsAssignableFrom(p.PropertyType)
                                || typeof(IList<ISpecimenBuilderNode>).IsAssignableFrom(p.PropertyType)).ToArray();

        var fields = builder.GetType().GetAllFields(true);
        SingleFields = fields.Where(f => (typeof(ISpecimenBuilder) == f.FieldType
                                                || typeof(ISpecimenBuilderNode) == f.FieldType)).ToArray();

        EnumerableFields = fields.Where(f => typeof(IList<ISpecimenBuilder>).IsAssignableFrom(f.FieldType)
                                || typeof(IList<ISpecimenBuilderNode>).IsAssignableFrom(f.FieldType)).ToArray();
    }

    public ISpecimenBuilder Builder { get; }
    public TraceInfo TraceInfo { get; }
    public int Depth { get; }

    public IEnumerable<PropertyInfo> SingleProps { get; }
    public IEnumerable<PropertyInfo> EnumerableProps { get; }
    public IEnumerable<FieldInfo> SingleFields { get; }
    public IEnumerable<FieldInfo> EnumerableFields { get; }


    public void EnsureAllBuilders()
    {
        foreach (var field in SingleFields)
        {
            var val = field.GetValue(Builder);
            if (val is null || val is TraceWrapper || val is not ISpecimenBuilder sb) continue;

            var wrapper = new TraceWrapper(sb, TraceInfo, Depth + 1);
            field.SetValue(Builder, wrapper);
        }
        foreach (var fieldList in EnumerableFields)
        {
            var val = fieldList.GetValue(Builder);
            if (val is null || val is TraceWrapper || val is not IList<ISpecimenBuilder> sbList) continue;

            for (int i = 0; i < sbList.Count; i++)
            {
                var sb = sbList[i];
                if (sb is null || sb is TraceWrapper) continue;

                sbList[i] = new TraceWrapper(sb, TraceInfo, Depth + 1);
            }
        }
        foreach (var prop in SingleProps)
        {
            var val = prop.GetValue(Builder);
            if (val is null || val is TraceWrapper || val is not ISpecimenBuilder sb) continue;

            var wrapper = new TraceWrapper(sb, TraceInfo, Depth + 1);
            prop.SetValue(Builder, wrapper);
        }
        foreach (var propList in EnumerableProps)
        {
            var val = propList.GetValue(Builder);
            if (val is null || val is TraceWrapper || val is not IList<ISpecimenBuilder> sbList) continue;

            for (int i = 0; i < sbList.Count; i++)
            {
                var sb = sbList[i];
                if (sb is null || sb is TraceWrapper) continue;

                sbList[i] = new TraceWrapper(sb, TraceInfo, Depth + 1); ;
            }
        }
    }
}