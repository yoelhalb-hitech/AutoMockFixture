using AutoFixture;
using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;
using AutoMockFixture.FixtureUtils.Specifications;
using SequelPay.DotNetPowerExtensions.Reflection;

namespace AutoMockFixture.FixtureUtils.Customizations;

/// <summary>
/// Customization to use a given subclass instead of the original baseclass, using generic parameters
/// </summary>
/// <typeparam name="TOriginal">The original base class</typeparam>
/// <typeparam name="TSubclass">The sub class to user</typeparam>
public class SubclassCustomization<TOriginal, TSubclass> : SubclassTransformCustomization where TSubclass : TOriginal
{
    public SubclassCustomization() : base(typeof(TOriginal), typeof(TSubclass))
    {
    }
}
