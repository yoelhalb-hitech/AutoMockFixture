using AutoFixture;
using AutoFixture.NUnit3;
using AutoMockFixture.Attributes;
using AutoMockFixture.FixtureUtils.Customizations;
using AutoMockFixture.FixtureUtils.Specifications;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using System.Threading;

namespace AutoMockFixture;

/// <summary>
/// This ineherits the <see cref="AutoDataAttribute"/> class from <see cref="AutoFixture"/>
/// We don't expose it directly as the base fixture doesn't appear to be created per method so we have to do that manually
/// </summary>
internal class AutoMockData : AutoDataAttribute
{
    private readonly Lazy<AutoMockFixture> fixtureLazy;

    private AutoMockFixture Fixture => this.fixtureLazy.Value;

    public AutoMockData(Func<AutoMockFixture> fixtureFactory) : base(fixtureFactory)
    {
        if (fixtureFactory == null) throw new ArgumentNullException(nameof(fixtureFactory));

        this.fixtureLazy = new Lazy<AutoMockFixture>(fixtureFactory, LazyThreadSafetyMode.PublicationOnly);
    }

    public new IEnumerable<TestMethod> BuildFrom(IMethodInfo method, Test? suite)
    {
        if (method is null) throw new ArgumentNullException(nameof(method));

        var parameters = method.GetParameters()
            .Select(parameter =>
            {
                var customizeAttributes = parameter.GetCustomAttributes<Attribute>(false)
            .OfType<IParameterCustomizationSource>()
            .OrderBy(x => x, new CustomizeAttributeComparer());

                foreach (var ca in customizeAttributes)
                {
                    var customization = ca.GetCustomization(parameter.ParameterInfo);
                    if(customization is FreezeOnMatchCustomization freezeCustomization)                    
                        this.Fixture
                            .Customize(new FreezeCustomization(new TypeOrRequestSpecification(freezeCustomization.Matcher)));                    
                    else 
                        this.Fixture.Customize(customization);
                }

                var autoMockType = parameter.GetCustomAttributes<Attribute>(false)
                                    .OfType<AutoMockTypeAttribute>()
                                    .FirstOrDefault();

                return autoMockType?.AutoMockType switch
                {
                    AutoMockTypes.AutoMock => Fixture.CreateAutoMock(parameter.ParameterType),
                    AutoMockTypes.NonAutoMock => Fixture.CreateNonAutoMock(parameter.ParameterType),
                    AutoMockTypes.AutoMockDependencies => Fixture.CreateWithAutoMockDependencies(parameter.ParameterType),
                    _ => Fixture.Create(parameter.ParameterType),
                };
            });
        
        var test = this.TestMethodBuilder.Build(method, suite, parameters, 0);

        yield return test;
    }

    internal class CustomizeAttributeComparer : Comparer<IParameterCustomizationSource>
    {
        public override int Compare(IParameterCustomizationSource x, IParameterCustomizationSource y)
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

