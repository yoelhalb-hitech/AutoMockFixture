using AutoMoqExtensions.AutoMockUtils;

namespace AutoMoqExtensions.FixtureUtils.Specifications;

internal class AutoMockableSpecification : IRequestSpecification
{
    public bool IsSatisfiedBy(object request)
    {
        var t = request as Type;
        if (t is null) return false;

        return AutoMockHelpers.IsAutoMockAllowed(t);
    }
}
