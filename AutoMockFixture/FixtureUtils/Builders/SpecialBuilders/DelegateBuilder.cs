using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.SpecialRequests;

namespace AutoMockFixture.FixtureUtils.Builders.SpecialBuilders;

internal class DelegateBuilder : NonConformingBuilder
{
    public override Type[] SupportedTypes => new[]
    {
        typeof(System.Delegate),
        typeof(System.MulticastDelegate),
    };

    protected override object[] GetInnerSpecimens(IRequestWithType originalRequest, ISpecimenContext context)
    {
        if (originalRequest.Request.IsGenericType) return new[] { new NoSpecimen() };

        return BuildInnerSpecimens(originalRequest, new[] { typeof(Action) }, context);
    }

    protected override InnerRequest GetInnerRequest(Type type, IRequestWithType originalRequest, int argIndex)
        => new InnerRequest(type, originalRequest);

    public override object CreateResult(Type requestType, object[] innerResults, IRequestWithType typeRequest, ISpecimenContext context)
    {
        return innerResults.First();
    }
}
