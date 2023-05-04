using AutoFixture;

namespace AutoMockFixture.FixtureUtils.Customizations;

public interface IRemovableCustomization : ICustomization
{
    void RemoveCustomization(IFixture fixture);
}
