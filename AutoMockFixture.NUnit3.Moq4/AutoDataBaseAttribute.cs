using AutoFixture;
using AutoMockFixture.FixtureUtils;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace AutoMockFixture.NUnit3.Moq4;

// TODO... add injections
// Based on https://docs.educationsmediagroup.com/unit-testing-csharp/autofixture/combining-autofixture-with-nunit-and-moq
[AttributeUsage(AttributeTargets.Method)]
[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class AutoDataBaseAttribute : Attribute, ITestBuilder, IWrapSetUpTearDown
{
    protected readonly bool noConfigureMembers;
    protected readonly bool generateDelegates;
    protected readonly MethodSetupTypes? methodSetupType;

    // Attributes can only deal with non nullable enums
    public AutoDataBaseAttribute(bool noConfigureMembers = false, bool generateDelegates = false)
    {
        this.noConfigureMembers = noConfigureMembers;
        this.generateDelegates = generateDelegates;
    }

    // Cannot have defualt value or the calls might be ambiguous
    public AutoDataBaseAttribute(bool noConfigureMembers, bool generateDelegates, MethodSetupTypes methodSetupType)
        : this(noConfigureMembers, generateDelegates)
    {
        this.methodSetupType = methodSetupType;
    }

    protected virtual List<ICustomization> Customizations => new List<ICustomization>();

    public IEnumerable<TestMethod> BuildFrom(IMethodInfo method, Test? suite)
    {
        try
        {
            // We need a fixture per method and per exectution, otherwise we can run in problems...
            var builder = new AutoMockData(GetFixture);
            return builder.BuildFrom(method, suite).ToArray(); // Enumerate here to force throw the error if there is
        }
        catch
        {
            return new TestMethod[] { }; // This building part happens inside the test harness and an exception here will cause all tests in the entire assembly to be ignored
        }
    }

    protected virtual AutoMockFixtureBase GetFixture()
    {
        fixture = CreateFixture();
        Customizations.ForEach(c => fixture.Customize(c));

        return fixture;
    }

    protected abstract AutoMockFixtureBase CreateFixture();
    protected AutoMockFixtureBase? fixture;

    TestCommand? ICommandWrapper.Wrap(TestCommand command)
    {
        return fixture is not null ? new DisposeFixtureCommand(command, fixture) : command;
    }
}
