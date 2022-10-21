using AutoFixture;
using AutoFixture.NUnit3;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using System.Threading;

namespace AutoMoqExtensions.Attributes;

// TODO... add injections
// Based on https://docs.educationsmediagroup.com/unit-testing-csharp/autofixture/combining-autofixture-with-nunit-and-moq
[AttributeUsage(AttributeTargets.Method)]
public class UnitAutoDataAttribute : Attribute, ITestBuilder
{
    private readonly bool noConfigureMembers;
    private readonly bool generateDelegates;
    private readonly MethodSetupTypes? methodSetupType;

    // Attributes can only deal with non nullable enums
    public UnitAutoDataAttribute(bool noConfigureMembers = false, bool generateDelegates = false)
    {
        this.noConfigureMembers = noConfigureMembers;
        this.generateDelegates = generateDelegates;
    }

    // Cannot have defualt value or the calls might be ambiguous
    public UnitAutoDataAttribute(bool noConfigureMembers, bool generateDelegates, MethodSetupTypes methodSetupType)
        : this(noConfigureMembers, generateDelegates)
    {
        
        this.methodSetupType = methodSetupType;
    }

    public IEnumerable<TestMethod> BuildFrom(IMethodInfo method, Test? suite)
    {
        // We need a fixture per method and per exectution, otherwise we can run in problems...
        var builder = new AutoMockData(() => new UnitFixture(noConfigureMembers, generateDelegates, methodSetupType));
        return builder.BuildFrom(method, suite);
    }
}
