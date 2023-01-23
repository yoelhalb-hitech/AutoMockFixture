
namespace AutoMoqExtensions.FixtureUtils.Specifications;

internal class AttributeMatchSpecification : IRequestSpecification
{
    public AttributeMatchSpecification(Type type)
    {
        Type = type;
    }

    public Type Type { get; }

    public bool IsSatisfiedBy(object request)
    {
        if (request is not Type t) return false;

        return t.IsDefined(Type, false);
    }
}
