namespace AutoMoqExtensions.FixtureUtils.Builders.SpecialBuilders;

internal class TaskBuilder : NonConformingBuilder
{
    public override Type[] SupportedTypes => new Type[] 
    {
        typeof(Task),
        typeof(ValueTask),
        typeof(Task<>),
        typeof(ValueTask<>)
    };
    public override int Repeat => 1;

    public override object CreateResult(Type requestType, object[][] innerResults)
    {
        var nonGenericType = requestType.IsGenericType ? requestType.BaseType : requestType;

        var args = innerResults.FirstOrDefault().FirstOrDefault() ?? new object(); // For the non generic we use object
        var specimen = nonGenericType.GetMethod(nameof(Task.FromResult))
                            .MakeGenericMethod(requestType.GetGenericArguments().First())
                            .Invoke(null, new[] { args });
        return specimen;
    }
}
