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
    // Cannot have defualt value or the calls might be ambiguous

    public IntegrationAutoDataAttribute(){}

    public IntegrationAutoDataAttribute(bool callBase) : base(callBase){}

    public IntegrationAutoDataAttribute(bool callBase, MethodSetupTypes methodSetupType) : base(callBase, methodSetupType) { }

    public IntegrationAutoDataAttribute(bool callBase, params Type[] typesToFreeze) : base(callBase, typesToFreeze) { }

    public IntegrationAutoDataAttribute(params Type[] typesToFreeze) : base(typesToFreeze) { }

    public IntegrationAutoDataAttribute(bool callBase, MethodSetupTypes methodSetupType, params Type[] typesToFreeze)
        : base(callBase, methodSetupType, typesToFreeze) { }

    protected override AutoMockFixtureBase CreateFixture() => new IntegrationFixture(noConfigureMembers, generateDelegates, methodSetupType) { CallBase = CallBase };
}

[AttributeUsage(AttributeTargets.Method)]
public class IntegrationAutoDataAttribute<TCustomization> : IntegrationAutoDataAttribute where TCustomization : ICustomization, new()
{
    public IntegrationAutoDataAttribute() { }

    public IntegrationAutoDataAttribute(bool callBase) : base(callBase) { }

    public IntegrationAutoDataAttribute(bool callBase, MethodSetupTypes methodSetupType) : base(callBase, methodSetupType) { }

    public IntegrationAutoDataAttribute(bool callBase, params Type[] typesToFreeze) : base(callBase, typesToFreeze) { }

    public IntegrationAutoDataAttribute(params Type[] typesToFreeze) : base(typesToFreeze) { }

    public IntegrationAutoDataAttribute(bool callBase, MethodSetupTypes methodSetupType, params Type[] typesToFreeze)
        : base(callBase, methodSetupType, typesToFreeze) { }

    protected override List<ICustomization> Customizations => new List<ICustomization> { new TCustomization() };
}

[AttributeUsage(AttributeTargets.Method)]
public class IntegrationAutoDataAttribute<TCustomization1, TCustomization2> : IntegrationAutoDataAttribute
    where TCustomization1 : ICustomization, new()
    where TCustomization2 : ICustomization, new()
{
    public IntegrationAutoDataAttribute() { }

    public IntegrationAutoDataAttribute(bool callBase) : base(callBase) { }

    public IntegrationAutoDataAttribute(bool callBase, MethodSetupTypes methodSetupType) : base(callBase, methodSetupType) { }

    public IntegrationAutoDataAttribute(bool callBase, params Type[] typesToFreeze) : base(callBase, typesToFreeze) { }

    public IntegrationAutoDataAttribute(params Type[] typesToFreeze) : base(typesToFreeze) { }

    public IntegrationAutoDataAttribute(bool callBase, MethodSetupTypes methodSetupType, params Type[] typesToFreeze)
        : base(callBase, methodSetupType, typesToFreeze) { }

    protected override List<ICustomization> Customizations => new List<ICustomization> { new TCustomization1(), new TCustomization2() };
}

[AttributeUsage(AttributeTargets.Method)]
public class IntegrationAutoDataAttribute<TCustomization1, TCustomization2, TCustomization3> : IntegrationAutoDataAttribute
    where TCustomization1 : ICustomization, new()
    where TCustomization2 : ICustomization, new()
    where TCustomization3 : ICustomization, new()
{
    public IntegrationAutoDataAttribute() { }

    public IntegrationAutoDataAttribute(bool callBase) : base(callBase) { }

    public IntegrationAutoDataAttribute(bool callBase, MethodSetupTypes methodSetupType) : base(callBase, methodSetupType) { }

    public IntegrationAutoDataAttribute(bool callBase, params Type[] typesToFreeze) : base(callBase, typesToFreeze) { }

    public IntegrationAutoDataAttribute(params Type[] typesToFreeze) : base(typesToFreeze) { }

    public IntegrationAutoDataAttribute(bool callBase, MethodSetupTypes methodSetupType, params Type[] typesToFreeze)
        : base(callBase, methodSetupType, typesToFreeze) { }

    protected override List<ICustomization> Customizations => new List<ICustomization> { new TCustomization1(), new TCustomization2(), new TCustomization3() };
}

[AttributeUsage(AttributeTargets.Method)]
public class IntegrationAutoDataAttribute<TCustomization1, TCustomization2, TCustomization3, TCustomization4> : IntegrationAutoDataAttribute
    where TCustomization1 : ICustomization, new()
    where TCustomization2 : ICustomization, new()
    where TCustomization3 : ICustomization, new()
    where TCustomization4 : ICustomization, new()
{
    public IntegrationAutoDataAttribute() { }

    public IntegrationAutoDataAttribute(bool callBase) : base(callBase) { }

    public IntegrationAutoDataAttribute(bool callBase, MethodSetupTypes methodSetupType) : base(callBase, methodSetupType) { }

    public IntegrationAutoDataAttribute(bool callBase, params Type[] typesToFreeze) : base(callBase, typesToFreeze) { }

    public IntegrationAutoDataAttribute(params Type[] typesToFreeze) : base(typesToFreeze) { }

    public IntegrationAutoDataAttribute(bool callBase, MethodSetupTypes methodSetupType, params Type[] typesToFreeze)
        : base(callBase, methodSetupType, typesToFreeze) { }

    protected override List<ICustomization> Customizations => new List<ICustomization> { new TCustomization1(), new TCustomization2(), new TCustomization3(), new TCustomization4() };
}
