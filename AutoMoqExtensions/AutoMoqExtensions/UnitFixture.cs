using AutoMoqExtensions.FixtureUtils;
using AutoMoqExtensions.FixtureUtils.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions
{
    public class UnitFixture : AutoMockFixture
    {
        public UnitFixture(bool noConfigureMembers = false, bool generateDelegates = false, MethodSetupTypes? methodSetupType = null) 
                    : base(noConfigureMembers, generateDelegates, methodSetupType)
        {
            Customizations.Add(new FilteringSpecimenBuilder(
                                    new FixedBuilder(this),
                                    new TypeOrRequestSpecification(new ExactTypeSpecification(typeof(UnitFixture)))));
        }

        public override object? Create(Type t, AutoMockTypeControl? autoMockTypeControl = null)
            => CreateWithAutoMockDependencies(t, false, autoMockTypeControl);
    }
}
