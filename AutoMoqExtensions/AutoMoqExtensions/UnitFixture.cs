using AutoMoqExtensions.FixtureUtils;
using AutoMoqExtensions.FixtureUtils.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions
{
    public class UnitFixture : AutoMockFixture
    {
        public UnitFixture(bool configureMembers = true, bool generateDelegates = true) 
                    : base(configureMembers, generateDelegates)
        {
            Customizations.Add(new FilteringSpecimenBuilder(
                                    new FixedBuilder(this),
                                    new TypeOrRequestSpecification(new ExactTypeSpecification(typeof(UnitFixture)))));
        }

        public override object Create(Type t, AutoMockTypeControl? autoMockTypeControl = null)
            => CreateWithAutoMockDependencies(t, false, autoMockTypeControl);
    }
}
