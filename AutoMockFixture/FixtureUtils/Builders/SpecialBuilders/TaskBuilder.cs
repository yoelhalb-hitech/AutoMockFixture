using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.SpecialRequests;

namespace AutoMockFixture.FixtureUtils.Builders.SpecialBuilders;

internal class TaskBuilder : NonConformingBuilder
{
    public override Type[] SupportedTypes => new Type[]
    {
        typeof(Task),
        typeof(Task<>),
        typeof(ValueTask),
        typeof(ValueTask<>),
    };
    public override int Repeat => 1;

    protected override InnerRequest GetInnerRequest(Type type, IRequestWithType originalRequest, int index, int argIndex)
        => new InnerRequest(type, originalRequest);

    public override object? CreateResult(Type requestType, object[][] innerResults, IRequestWithType typeRequest, ISpecimenContext context)
    {
        var nonGenericType = requestType.IsGenericType ? requestType.BaseType : requestType;

        var args = innerResults.FirstOrDefault()?.FirstOrDefault() ?? new object(); // For the non generic we use object
        var specimen = nonGenericType?.GetMethod(nameof(Task.FromResult))?
                            .MakeGenericMethod(requestType.GetGenericArguments().FirstOrDefault() ?? typeof(object))?
                            .Invoke(null, new[] { args });
        return specimen;
    }
}
