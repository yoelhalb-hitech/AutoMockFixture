
namespace AutoMoqExtensions.FixtureUtils.Trace;

public class TraceInfo
{
    public List<(ISpecimenBuilder builder, ISpecimenContext context,
        object request, object? response, Exception? exception, int depth)> TraceValues
    { get; } = new();

    public List<(ISpecimenBuilder builder, ISpecimenContext context,
        object request, object? response, Exception? exception, int depth)> GetWithValues() => TraceValues.Where(tv => tv.response is not NoSpecimen).ToList();
}
