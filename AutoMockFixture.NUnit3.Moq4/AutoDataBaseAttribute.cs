using AutoFixture;
using AutoFixture.Kernel;
using AutoFixture.NUnit3;
using AutoMockFixture.FixtureUtils;
using AutoMockFixture.FixtureUtils.Customizations;
using AutoMockFixture.FixtureUtils.Specifications;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;
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

    // Attributes can only deal with non nullable value types
    public AutoDataBaseAttribute() { }

    public AutoDataBaseAttribute(bool callBase)
    {
        CallBase = callBase;
    }

    // Cannot have defualt value or the calls might be ambiguous
    public AutoDataBaseAttribute(bool callBase, MethodSetupTypes methodSetupType) : this(callBase)
    {
        this.methodSetupType = methodSetupType;
    }

    public AutoDataBaseAttribute(params Type[] typesToFreeze)
    {
        TypesToFreeze = typesToFreeze;
    }

    public AutoDataBaseAttribute(bool callBase, params Type[] typesToFreeze) : this(typesToFreeze)
    {
        CallBase = callBase;
    }


    public AutoDataBaseAttribute(bool callBase, MethodSetupTypes methodSetupType, params Type[] typesToFreeze)
        : this(callBase, typesToFreeze)
    {
        this.methodSetupType = methodSetupType;
    }

    protected virtual List<ICustomization> Customizations => new List<ICustomization>();

    public bool? CallBase { get; }
    public Type[]? TypesToFreeze { get; }

    public IEnumerable<TestMethod> BuildFrom(IMethodInfo method, Test? suite)
    {
        try
        {
            try
            {
                // We need a fixture per method and per exectution, otherwise we can run in problems...
                var builder = new AutoMockData(GetFixture);
                return builder.BuildFrom(method, suite).ToArray(); // Enumerate here to force throw the error if there is
            }
            catch (AggregateException ax)
            {
                // This building part happens inside the test harness and an exception here will cause all tests in the entire assembly to be ignored

                var parms = new TestCaseParameters { RunState = RunState.NotRunnable };
                parms.Properties.Set(PropertyNames.SkipReason, $"Exception of type '{ax.InnerExceptions.First().GetType().FullName}' was thrown, message is '{ax.InnerExceptions.First().Message}'");
                return new[] { new NUnitTestCaseBuilder().BuildTestMethod(method, suite, parms) };
            }
            catch (Exception ex)
            {
                // This building part happens inside the test harness and an exception here will cause all tests in the entire assembly to be ignored

                var parms = new TestCaseParameters { RunState = RunState.NotRunnable };
                parms.Properties.Set(PropertyNames.SkipReason, $"Exception of type '{ex.GetType().FullName}' was thrown, message is '{ex.Message}'");
                return new[] { new NUnitTestCaseBuilder().BuildTestMethod(method, suite, parms) };
            }
        }
        catch
        {
            return new TestMethod[] { }; // This building part happens inside the test harness and an exception here will cause all tests in the entire assembly to be ignored
        }
    }

    protected virtual AutoMockFixtureBase GetFixture()
    {
        fixture = CreateFixture();

        TypesToFreeze?.ToList().ForEach(t =>
            fixture.Customize(new FreezeCustomization(
                    new TypeOrRequestSpecification(new TypeSpecification(t, fixture.AutoMockHelpers), fixture.AutoMockHelpers))));

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
