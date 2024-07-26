using AutoFixture;
using AutoMockFixture.FixtureUtils;
using AutoMockFixture.Moq4;
using System;
using System.Collections.Generic;

namespace AutoMockFixture.NUnit3.Moq4;

[AttributeUsage(AttributeTargets.Method)]
public class UnitAutoDataAttribute : AutoDataBaseAttribute
{
    // Cannot have defualt value or the calls might be ambiguous

    public UnitAutoDataAttribute() { }

    public UnitAutoDataAttribute(bool callBase) : base(callBase) { }

    public UnitAutoDataAttribute(bool callBase, MethodSetupTypes methodSetupType) : base(callBase, methodSetupType) { }

    public UnitAutoDataAttribute(bool callBase, params Type[] typesToFreeze) : base(callBase, typesToFreeze) { }

    public UnitAutoDataAttribute(params Type[] typesToFreeze) : base(typesToFreeze) { }

    public UnitAutoDataAttribute(bool callBase, MethodSetupTypes methodSetupType, params Type[] typesToFreeze)
        : base(callBase, methodSetupType, typesToFreeze) { }


    protected override AutoMockFixtureBase CreateFixture() => new UnitFixture(noConfigureMembers, generateDelegates, methodSetupType) { CallBase = CallBase };
}

[AttributeUsage(AttributeTargets.Method)]
public class UnitAutoDataAttribute<TCustomization> : UnitAutoDataAttribute where TCustomization : ICustomization, new()
{

    public UnitAutoDataAttribute() { }

    public UnitAutoDataAttribute(bool callBase) : base(callBase) { }

    public UnitAutoDataAttribute(bool callBase, MethodSetupTypes methodSetupType) : base(callBase, methodSetupType) { }

    public UnitAutoDataAttribute(bool callBase, params Type[] typesToFreeze) : base(callBase, typesToFreeze) { }

    public UnitAutoDataAttribute(params Type[] typesToFreeze) : base(typesToFreeze) { }

    public UnitAutoDataAttribute(bool callBase, MethodSetupTypes methodSetupType, params Type[] typesToFreeze)
        : base(callBase, methodSetupType, typesToFreeze) { }


    protected override List<ICustomization> Customizations => new List<ICustomization> { new TCustomization() };
}

[AttributeUsage(AttributeTargets.Method)]
public class UnitAutoDataAttribute<TCustomization1, TCustomization2> : UnitAutoDataAttribute
    where TCustomization1 : ICustomization, new()
    where TCustomization2 : ICustomization, new()
{

    public UnitAutoDataAttribute() { }

    public UnitAutoDataAttribute(bool callBase) : base(callBase) { }

    public UnitAutoDataAttribute(bool callBase, MethodSetupTypes methodSetupType) : base(callBase, methodSetupType) { }

    public UnitAutoDataAttribute(bool callBase, params Type[] typesToFreeze) : base(callBase, typesToFreeze) { }

    public UnitAutoDataAttribute(params Type[] typesToFreeze) : base(typesToFreeze) { }

    public UnitAutoDataAttribute(bool callBase, MethodSetupTypes methodSetupType, params Type[] typesToFreeze)
        : base(callBase, methodSetupType, typesToFreeze) { }


    protected override List<ICustomization> Customizations => new List<ICustomization> { new TCustomization1(), new TCustomization2() };
}

[AttributeUsage(AttributeTargets.Method)]
public class UnitAutoDataAttribute<TCustomization1, TCustomization2, TCustomization3> : UnitAutoDataAttribute
    where TCustomization1 : ICustomization, new()
    where TCustomization2 : ICustomization, new()
    where TCustomization3 : ICustomization, new()
{

    public UnitAutoDataAttribute() { }

    public UnitAutoDataAttribute(bool callBase) : base(callBase) { }

    public UnitAutoDataAttribute(bool callBase, MethodSetupTypes methodSetupType) : base(callBase, methodSetupType) { }

    public UnitAutoDataAttribute(bool callBase, params Type[] typesToFreeze) : base(callBase, typesToFreeze) { }

    public UnitAutoDataAttribute(params Type[] typesToFreeze) : base(typesToFreeze) { }

    public UnitAutoDataAttribute(bool callBase, MethodSetupTypes methodSetupType, params Type[] typesToFreeze)
        : base(callBase, methodSetupType, typesToFreeze) { }


    protected override List<ICustomization> Customizations => new List<ICustomization> { new TCustomization1(), new TCustomization2(), new TCustomization3() };
}

[AttributeUsage(AttributeTargets.Method)]
public class UnitAutoDataAttribute<TCustomization1, TCustomization2, TCustomization3, TCustomization4> : UnitAutoDataAttribute
    where TCustomization1 : ICustomization, new()
    where TCustomization2 : ICustomization, new()
    where TCustomization3 : ICustomization, new()
    where TCustomization4 : ICustomization, new()
{
    public UnitAutoDataAttribute() { }

    public UnitAutoDataAttribute(bool callBase) : base(callBase) { }

    public UnitAutoDataAttribute(bool callBase, MethodSetupTypes methodSetupType) : base(callBase, methodSetupType) { }

    public UnitAutoDataAttribute(bool callBase, params Type[] typesToFreeze) : base(callBase, typesToFreeze) { }

    public UnitAutoDataAttribute(params Type[] typesToFreeze) : base(typesToFreeze) { }

    public UnitAutoDataAttribute(bool callBase, MethodSetupTypes methodSetupType, params Type[] typesToFreeze)
        : base(callBase, methodSetupType, typesToFreeze) { }


    protected override List<ICustomization> Customizations => new List<ICustomization> { new TCustomization1(), new TCustomization2(), new TCustomization3(), new TCustomization4() };
}