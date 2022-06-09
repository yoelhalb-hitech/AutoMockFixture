﻿using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;
using AutoMoqExtensions.FixtureUtils.Commands;
using AutoMoqExtensions.FixtureUtils.Postprocessors;
using AutoMoqExtensions.FixtureUtils.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.AutoMockUtils
{
    public class AutoMockCustomization : ICustomization
    {
        public bool ConfigureMembers { get; set; } = true;
        public bool GenerateDelegates { get; set; } = true;
        public void Customize(IFixture fixture)
        {
            if (fixture == null) throw new ArgumentNullException(nameof(fixture));

            ISpecimenBuilder mockBuilder = new AutoMockPostprocessor(
                                              new AutoMockMethodInvoker(
                                                 new CustomConstructorQueryWrapper(
                                                    new AutoMockConstructorQuery())));

            // If members should be automatically configured, wrap the builder with members setup postprocessor.
            if (this.ConfigureMembers)
            {
                mockBuilder = new Postprocessor(
                    builder: mockBuilder,
                    command: new CompositeSpecimenCommand(
                                new EnsureObjectCommand(),
                                new SetCallBaseCommand(),
                                new StubPropertiesCommand(),
                                new AutoMockVirtualMethodsCommand(),
                                new AutoMockAutoPropertiesCommand()));
            }

            fixture.Customizations.Add(mockBuilder);

            fixture.Customizations.Add(new FilteringSpecimenBuilder(
                                                new AutoMockConstructorArgumentPostprocessor(),
                                                new AutoMockConstructorArgumentSpecification()));
            fixture.Customizations.Add(new FilteringSpecimenBuilder(
                                    new AutoMockRequestPostprocessor(),
                                    new AutoMockRequestSpecification()));


            fixture.ResidueCollectors.Add(new AutoMockRelay());

            if (this.GenerateDelegates)
            {
                fixture.Customizations.Add(new AutoMockRelay(new DelegateSpecification()));
            }
        }
    }
}
