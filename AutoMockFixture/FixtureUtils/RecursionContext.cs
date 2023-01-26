
namespace AutoMockFixture.FixtureUtils;

internal class RecursionContext : SpecimenContext
{
    public RecursionContext(ISpecimenBuilder builder, IAutoMockFixture fixture) : base(builder)
    {
        Fixture = fixture;
    }

    internal Dictionary<Type, object> BuilderCache { get; } = new Dictionary<Type, object>();

    internal AutoMockTypeControl? AutoMockTypeControl { get; set; }

    internal IAutoMockFixture Fixture { get; set; }
}
