using AutoFixture;
using DotNetPowerExtensions.Reflection;
using SequelPay.DotNetPowerExtensions;
using System.Diagnostics.CodeAnalysis;

namespace AutoMockFixture.FixtureUtils.Customizations;

internal class ServicesCustomization : IRemovableCustomization
{
    private static readonly List<SubclassTransformCustomization> customizations = [];
    public void Customize(IFixture fixture)
    {
        if (fixture is not IAutoMockFixture mockFixture) throw new ArgumentException("fixture is not an AutoMockFixture", nameof(fixture));

        if (customizations.Count == 0) Populate(fixture);

        customizations.ForEach(c => fixture.Customize(c));
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types")]
    private static void Populate(IFixture fixture)
    {
        var types = AppDomain.CurrentDomain.GetAssemblies()
                 .SelectMany(a =>
                 {
                     try { return a.GetTypes(); } catch { return []; } // Sometimes it throws
                 })
                 .Where(t =>
                 {
                     try { return Attribute.IsDefined(t, typeof(DependencyAttribute), false); } catch { return false; }
                 });

        foreach (var type in types.Where(t => !t.IsInterface && !t.IsAbstract))
        {
            foreach (var attribute in Attribute.GetCustomAttributes(type, typeof(DependencyAttribute), false).OfType<DependencyAttribute>())
            {
                if (attribute.DependencyType == DependencyType.None) continue;

                var implementingType = type.IsGenericTypeDefinition ? attribute.Use ?? type : type;

                var forTypes = (Type?[])attribute.GetType().InvokeMethod("get_For", attribute)!;
                if (forTypes.Length == 0) forTypes = new[] { type };

                foreach (var forType in forTypes.OfType<Type>())
                {
                    if (forType == implementingType) continue;

                    customizations.Add(new SubclassTransformCustomization(forType, implementingType));
                }
            }
        }
    }

    public void RemoveCustomization(IFixture fixture)
    {
        customizations.ForEach(c => c.RemoveCustomization(fixture));
    }
}