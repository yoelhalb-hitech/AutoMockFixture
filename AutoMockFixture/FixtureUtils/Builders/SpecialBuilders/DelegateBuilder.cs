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

    public override int Repeat => 1;

    protected override object GetInnerSpecimens(IRequestWithType originalRequest, int index, ISpecimenContext context)
    {
        if (originalRequest.Request.IsGenericType) return new NoSpecimen();

        return BuildInnerSpecimens(originalRequest, new[] { typeof(Action) }, index, context);
    }

    protected override InnerRequest GetInnerRequest(Type type, IRequestWithType originalRequest, int index, int argIndex)
        => new InnerRequest(type, originalRequest);

    public override object CreateResult(Type requestType, object[][] innerResults)
    {
        return innerResults.First().First();
    }
}
