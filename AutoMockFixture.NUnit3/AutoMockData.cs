using AutoFixture;
using AutoFixture.NUnit3;
using AutoMockFixture.FixtureUtils;
using AutoMockFixture.FixtureUtils.Customizations;
using AutoMockFixture.FixtureUtils.Specifications;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace AutoMockFixture.NUnit3;

/// <summary>
/// This ineherits the <see cref="AutoDataAttribute"/> class from <see cref="AutoFixture"/>
/// We don't expose it directly as the base fixture doesn't appear to be created per method so we have to do that manually
/// </summary>
internal class AutoMockData : AutoDataAttribute
{
    private readonly Lazy<FixtureUtils.AutoMockFixtureBase> fixtureLazy;

    private FixtureUtils.AutoMockFixtureBase Fixture => this.fixtureLazy.Value;

    public AutoMockData(Func<FixtureUtils.AutoMockFixtureBase> fixtureFactory) : base(fixtureFactory)
    {
        if (fixtureFactory is null) throw new ArgumentNullException(nameof(fixtureFactory));

        this.fixtureLazy = new Lazy<FixtureUtils.AutoMockFixtureBase>(fixtureFactory, LazyThreadSafetyMode.PublicationOnly);
    }

    public new IEnumerable<TestMethod> BuildFrom(IMethodInfo method, Test? suite)
    {
        if (method is null) throw new ArgumentNullException(nameof(method));

        var parameters = method.GetParameters()
            .Select(parameter =>
            {
                var customizeAttributes = parameter.GetCustomAttributes<IParameterCustomizationSource>(false)
                                                .OrderBy(x => x, new CustomizeAttributeComparer());

                foreach (var ca in customizeAttributes)
                {
                    var customization = ca.GetCustomization(parameter.ParameterInfo);
                    if(customization is FreezeOnMatchCustomization freezeCustomization)
                        this.Fixture
                            .Customize(new FreezeCustomization(new TypeOrRequestSpecification(freezeCustomization.Matcher, Fixture.AutoMockHelpers)));
                    else
                        this.Fixture.Customize(customization);
                }

                var autoMockType = parameter.GetCustomAttributes<AutoMockTypeAttribute>(false).FirstOrDefault();
                var callBase = parameter.GetCustomAttributes<CallBaseAttribute>(false).FirstOrDefault();
                var autoMockTypeControl = parameter.GetCustomAttributes<AutoMockTypeControlAttribute>(false).FirstOrDefault();

                var func = autoMockType?.AutoMockType switch
                {
                    AutoMockTypes.AutoMock => (Func<Type, bool?, AutoMockTypeControl?, Task<object?>>) Fixture.CreateAutoMockAsync,
                    AutoMockTypes.NonAutoMock => Fixture.CreateNonAutoMockAsync,
                    AutoMockTypes.AutoMockDependencies => Fixture.CreateWithAutoMockDependenciesAsync,
                    _ => Fixture.CreateAsync,
                };

                return func(parameter.ParameterType, callBase?.CallBase, autoMockTypeControl?.AutoMockTypeControl);
            });
        Task.WaitAll(parameters.ToArray());

        var test = this.TestMethodBuilder.Build(method, suite, parameters.Select(p => p.Result), 0);

        yield return test;
    }

    internal class CustomizeAttributeComparer : Comparer<IParameterCustomizationSource>
    {
        public override int Compare(IParameterCustomizationSource? x, IParameterCustomizationSource? y)
        {
            var xfrozen = x is FrozenAttribute;
            var yfrozen = y is FrozenAttribute;

            if (xfrozen && !yfrozen)
            {
                return 1;
            }

            if (yfrozen && !xfrozen)
            {
                return -1;
            }

            return 0;
        }
    }
}

