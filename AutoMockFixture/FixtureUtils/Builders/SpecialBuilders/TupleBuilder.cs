using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.SpecialRequests;

namespace AutoMockFixture.FixtureUtils.Builders.SpecialBuilders;

internal class TupleBuilder : NonConformingBuilder
{
    public override Type[] SupportedTypes => new Type[]
    {
        typeof(Tuple<>), typeof(ValueTuple<>),
        typeof(Tuple<,>), typeof(ValueTuple<,>),
        typeof(Tuple<,,>), typeof(ValueTuple<,,>),
        typeof(Tuple<,,,>), typeof(ValueTuple<,,,>),
        typeof(Tuple<,,,,>), typeof(ValueTuple<,,,,>),
        typeof(Tuple<,,,,,>), typeof(ValueTuple<,,,,,>),
        typeof(Tuple<,,,,,,>), typeof(ValueTuple<,,,,,,>),
        typeof(Tuple<,,,,,,,>), typeof(ValueTuple<,,,,,,,>),
        typeof(KeyValuePair<,>),
    };

    protected override InnerRequest GetInnerRequest(Type type, IRequestWithType originalRequest, int argIndex)
         => new TupleItemRequest(type, originalRequest, argIndex);

    public override object? CreateResult(Type requestType, object[] innerResults, IRequestWithType typeRequest, ISpecimenContext context)
    {
        return requestType.GetConstructor(requestType.GenericTypeArguments)? // Will also filter out static ctors as they don't have arguments
                .Invoke(innerResults);
    }
}
