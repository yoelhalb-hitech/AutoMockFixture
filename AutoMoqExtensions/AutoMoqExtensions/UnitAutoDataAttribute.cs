using AutoFixture.NUnit3;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace AutoMoqExtensions;

// TODO... add injections
// Based on https://docs.educationsmediagroup.com/unit-testing-csharp/autofixture/combining-autofixture-with-nunit-and-moq
[AttributeUsage(AttributeTargets.Method)]
public class UnitAutoDataAttribute : Attribute, ITestBuilder
{
    private readonly bool noConfigureMembers;
    private readonly bool generateDelegates;

    public UnitAutoDataAttribute(bool noConfigureMembers = false, bool generateDelegates = false)
    {
        this.noConfigureMembers = noConfigureMembers;
        this.generateDelegates = generateDelegates;
    }

    internal class AutoMockUnitData : AutoDataAttribute
    {
        public AutoMockUnitData(bool noConfigureMembers = false, bool generateDelegates = false)
            : this(() => new UnitFixture(noConfigureMembers, generateDelegates))
        {
        }
    }

    public IEnumerable<TestMethod> BuildFrom(IMethodInfo method, Test? suite)
    {
        // We need a fixture per method and per exectution, otherwise we can run in problems...
        var builder = new AutoMockUnitData(noConfigureMembers, generateDelegates);
        return builder.BuildFrom(method, suite);
    }
}
