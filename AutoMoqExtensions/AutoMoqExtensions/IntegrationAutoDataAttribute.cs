using AutoFixture.NUnit3;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace AutoMoqExtensions;

// TODO... add injections
// Based on https://docs.educationsmediagroup.com/unit-testing-csharp/autofixture/combining-autofixture-with-nunit-and-moq
[AttributeUsage(AttributeTargets.Method)]
public class IntegrationAutoDataAttribute : Attribute, ITestBuilder
{
    private readonly bool configureMembers;
    private readonly bool generateDelegates;

    public IntegrationAutoDataAttribute(bool configureMembers = true, bool generateDelegates = true)
    {
        this.configureMembers = configureMembers;
        this.generateDelegates = generateDelegates;
    }

    internal class AutoMockIntegrationData : AutoDataAttribute
    {
        public AutoMockIntegrationData(bool configureMembers = true, bool generateDelegates = true)
            : base(() => new IntegrationFixture(configureMembers, generateDelegates))
        {
        }
    }

    public IEnumerable<TestMethod> BuildFrom(IMethodInfo method, Test? suite)
    {
        // We need a fixture per method and per exectution, otherwise we can run in problems...
        var builder = new AutoMockIntegrationData(configureMembers, generateDelegates);
        return builder.BuildFrom(method, suite);
    }
}
