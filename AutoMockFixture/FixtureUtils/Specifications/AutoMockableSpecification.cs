
namespace AutoMockFixture.FixtureUtils.Specifications;

internal class AutoMockableSpecification : IRequestSpecification
{
    public AutoMockableSpecification(IAutoMockHelpers autoMockHelpers)
    {
        AutoMockHelpers = autoMockHelpers;
    }

    public IAutoMockHelpers AutoMockHelpers { get; }

    public bool IsSatisfiedBy(object request)
    {
        var t = request as Type;
        if (t is null) return false;

        return AutoMockHelpers.IsAutoMockAllowed(t);
    }
}
