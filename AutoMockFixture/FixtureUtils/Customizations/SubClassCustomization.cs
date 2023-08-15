using AutoFixture;
using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;
using AutoMockFixture.FixtureUtils.Specifications;
using DotNetPowerExtensions.Reflection;

namespace AutoMockFixture.FixtureUtils.Customizations;

public class SubClassCustomization<TOriginal, TSubClass> : SubClassTransformCustomization
{
    public SubClassCustomization() : base(typeof(TOriginal), typeof(TSubClass))
    {
    }
}
