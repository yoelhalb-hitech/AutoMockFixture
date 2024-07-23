using AutoFixture;
using AutoMockFixture.FixtureUtils;
using AutoMockFixture.Moq4;
using System;
using System.Collections.Generic;

namespace AutoMockFixture.NUnit3.Moq4;

// TODO... add injections
// Based on https://docs.educationsmediagroup.com/unit-testing-csharp/autofixture/combining-autofixture-with-nunit-and-moq
[AttributeUsage(AttributeTargets.Method)]
public class IntegrationAutoDataAttribute : AutoDataBaseAttribute
{
    public IntegrationAutoDataAttribute(bool noConfigureMembers = false, bool generateDelegates = false) : base(noConfigureMembers, generateDelegates)
    {

    }

    // Cannot have defualt value or the calls might be ambiguous
    public IntegrationAutoDataAttribute(bool noConfigureMembers, bool generateDelegates, MethodSetupTypes methodSetupType)
        : base(noConfigureMembers, generateDelegates, methodSetupType)
    {
    }

    public bool CallBase { get; set; }

    protected override AutoMockFixtureBase CreateFixture() => new IntegrationFixture(noConfigureMembers, generateDelegates, methodSetupType) { CallBase = CallBase };
}

[AttributeUsage(AttributeTargets.Method)]
public class IntegrationAutoDataAttribute<TCustomization> : IntegrationAutoDataAttribute where TCustomization : ICustomization, new()
{
    protected override List<ICustomization> Customizations => new List<ICustomization> { new TCustomization() };
}

[AttributeUsage(AttributeTargets.Method)]
public class IntegrationAutoDataAttribute<TCustomization1, TCustomization2> : IntegrationAutoDataAttribute
    where TCustomization1 : ICustomization, new()
    where TCustomization2 : ICustomization, new()
{
    protected override List<ICustomization> Customizations => new List<ICustomization> { new TCustomization1(), new TCustomization2() };
}

[AttributeUsage(AttributeTargets.Method)]
public class IntegrationAutoDataAttribute<TCustomization1, TCustomization2, TCustomization3> : IntegrationAutoDataAttribute
    where TCustomization1 : ICustomization, new()
    where TCustomization2 : ICustomization, new()
    where TCustomization3 : ICustomization, new()
{
    protected override List<ICustomization> Customizations => new List<ICustomization> { new TCustomization1(), new TCustomization2(), new TCustomization3() };
}

[AttributeUsage(AttributeTargets.Method)]
public class IntegrationAutoDataAttribute<TCustomization1, TCustomization2, TCustomization3, TCustomization4> : IntegrationAutoDataAttribute
    where TCustomization1 : ICustomization, new()
    where TCustomization2 : ICustomization, new()
    where TCustomization3 : ICustomization, new()
    where TCustomization4 : ICustomization, new()
{
    protected override List<ICustomization> Customizations => new List<ICustomization> { new TCustomization1(), new TCustomization2(), new TCustomization3(), new TCustomization4() };
}
