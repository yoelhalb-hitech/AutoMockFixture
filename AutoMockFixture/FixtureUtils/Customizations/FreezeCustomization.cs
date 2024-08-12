using AutoFixture;

namespace AutoMockFixture.FixtureUtils.Customizations;

public class FreezeCustomization : IRemovableCustomization
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

        mockFixture.Cache.CacheSpecifications.Remove(Specification);
        mockFixture.Cache.CacheSpecifications.Insert(0, Specification);
    }

    public void RemoveCustomization(IFixture fixture)
    {
        var mockFixture = fixture as IAutoMockFixture;

        mockFixture?.Cache.CacheSpecifications.Remove(Specification);
    }
}
