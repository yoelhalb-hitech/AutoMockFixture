using AutoFixture;
using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;
using AutoMockFixture.FixtureUtils.Specifications;
using DotNetPowerExtensions.Reflection;

namespace AutoMockFixture.FixtureUtils.Customizations;

public class SubclassCustomization<TOriginal, TSubclass> : SubclassTransformCustomization
{
    public SubclassCustomization() : base(typeof(TOriginal), typeof(TSubclass))
    {
    }
}
