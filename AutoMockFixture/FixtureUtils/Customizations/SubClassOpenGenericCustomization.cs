using AutoFixture;
using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;
using AutoMockFixture.FixtureUtils.Specifications;
using DotNetPowerExtensions.Reflection;

namespace AutoMockFixture.FixtureUtils.Customizations;
/// <summary>
/// Customization for open generic types to replace with a derived type
/// </summary>
/// <remarks>Since the compiler won't allow an open generic type you should pass a closed type but the generic argumetns will be ignored</remarks>
/// <typeparam name="TOriginal">Stub generic type for the original, the generic parameters will be ignored</typeparam>
/// <typeparam name="TSubClass">Stub generic type for the subclass, the generic parameters will be ignored</typeparam>
public class SubClassOpenGenericCustomization<TOriginal, TSubClass> : SubClassTransformCustomization
{
    public SubClassOpenGenericCustomization() : base(typeof(TOriginal).GetGenericTypeDefinition(), typeof(TSubClass).GetGenericTypeDefinition())
    {
    }
}
