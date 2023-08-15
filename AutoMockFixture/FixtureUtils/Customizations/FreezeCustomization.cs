using AutoFixture;

namespace AutoMockFixture.FixtureUtils.Customizations;

public class FreezeCustomization : ICustomization
{
    public FreezeCustomization(IRequestSpecification specification)
    {
        Specification = specification;
    }

    public IRequestSpecification Specification { get; }

    public void Customize(IFixture fixture)
    {
        var mockFixture = fixture as IAutoMockFixture;
        if (mockFixture is null) throw new Exception($"{nameof(FreezeCustomization)} can only work with an {nameof(IAutoMockFixture)}");

        if (!mockFixture.Cache.CacheSpecifications.Contains(Specification))
            mockFixture.Cache.CacheSpecifications.Add(Specification);
    }
}
