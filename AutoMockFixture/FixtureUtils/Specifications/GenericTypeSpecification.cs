
namespace AutoMockFixture.FixtureUtils.Specifications;

internal class GenericTypeSpecification : IRequestSpecification
{
    public GenericTypeSpecification(Type openGenericType)
    {
        OpenGenericType = openGenericType;
    }

    public Type OpenGenericType { get; }

    public bool IsSatisfiedBy(object request) => request is Type type && type.IsGenericType && type.GetGenericTypeDefinition() == OpenGenericType;
}
