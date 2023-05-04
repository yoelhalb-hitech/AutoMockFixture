
namespace AutoMockFixture.Moq4.FixtureUtils.Specifications;

internal class MockRequestSpecification : IRequestSpecification
{
    public bool IsSatisfiedBy(object request)
        => request is Type t && typeof(Mock).IsAssignableFrom(t) && t.IsGenericType;
}
