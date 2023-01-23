using AutoFixture;

namespace AutoMoqExtensions.FixtureUtils.Customizations;

internal class FreezeCustomization : ICustomization
{
    public FreezeCustomization(IRequestSpecification specification)
    {
        Specification = specification;
    }

    public IRequestSpecification Specification { get; }

    public void Customize(IFixture fixture)
    {
        var mockFixture = fixture as AutoMockFixture;
        if (mockFixture is null) throw new Exception($"{nameof(FreezeCustomization)} can only work with {nameof(AutoMockFixture)}");

        if (!mockFixture.Cache.CacheSpecifications.Contains(Specification))
            mockFixture.Cache.CacheSpecifications.Add(Specification);
    }
}
