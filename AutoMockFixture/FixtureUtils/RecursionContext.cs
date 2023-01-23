
namespace AutoMoqExtensions.FixtureUtils;

internal class RecursionContext : SpecimenContext
{
    public RecursionContext(ISpecimenBuilder builder, AutoMockFixture fixture) : base(builder)
    {
        Fixture = fixture;
    }

    internal Dictionary<Type, object> BuilderCache { get; } = new Dictionary<Type, object>();

    internal AutoMockTypeControl? AutoMockTypeControl { get; set; }

    internal AutoMockFixture Fixture { get; set; }
}
