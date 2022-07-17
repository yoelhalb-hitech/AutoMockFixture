namespace AutoMoqExtensions.FixtureUtils.Builders.SpecialBuilders;

internal class TaskBuilder : NonConformingBuilder
{
    public override Type[] SupportedTypes => new Type[] { typeof(Task<>), typeof(ValueTask<>) };
    public override int Repeat => 1;

    public override object CreateResult(Type requestType, object[][] innerResults)
    {
        var result = requestType.BaseType.GetMethod(nameof(Task.FromResult))
                            .MakeGenericMethod(requestType.GetGenericArguments().First())
                            .Invoke(null, new[] { innerResults.First().First() });
        return result;
    }
}
