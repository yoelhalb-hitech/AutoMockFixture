﻿using AutoMockFixture.FixtureUtils;
using AutoMockFixture.FixtureUtils.Specifications;

namespace AutoMockFixture;

public class IntegrationFixture : AutoMockFixture
{
    public IntegrationFixture(bool noConfigureMembers = false, bool generateDelegates = false, MethodSetupTypes? methodSetupType = null) 
                : base(noConfigureMembers, generateDelegates, methodSetupType)
    {
        Customizations.Add(new FilteringSpecimenBuilder(
                            new FixedBuilder(this),
                            new TypeOrRequestSpecification(new ExactTypeSpecification(typeof(IntegrationFixture)))));
    }

    public override object? Create(Type t, AutoMockTypeControl? autoMockTypeControl = null)
        => CreateNonAutoMock(t, autoMockTypeControl);
}