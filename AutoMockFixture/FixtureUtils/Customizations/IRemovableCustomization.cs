using AutoFixture;

namespace AutoMoqExtensions.FixtureUtils.Customizations
{
    public interface IRemovableCustomization : ICustomization
    {
        void RemoveCustomization(IFixture fixture);
    }
}
