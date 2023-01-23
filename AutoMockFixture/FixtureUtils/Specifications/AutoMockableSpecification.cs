using AutoMockFixture.AutoMockUtils;

namespace AutoMockFixture.FixtureUtils.Specifications;

internal class AutoMockableSpecification : IRequestSpecification
{
    public bool IsSatisfiedBy(object request)
    {
        var t = request as Type;
        if (t is null) return false;

        return AutoMockHelpers.IsAutoMockAllowed(t);
    }
}
