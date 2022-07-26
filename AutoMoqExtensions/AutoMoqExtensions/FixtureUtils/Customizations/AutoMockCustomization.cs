﻿using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;
using AutoMoqExtensions.AutoMockUtils;
using AutoMoqExtensions.FixtureUtils.Builders.SpecialBuilders;
using AutoMoqExtensions.FixtureUtils.Commands;
using AutoMoqExtensions.FixtureUtils.MethodInvokers;
using AutoMoqExtensions.FixtureUtils.MethodQueries;
using AutoMoqExtensions.FixtureUtils.Postprocessors;
using AutoMoqExtensions.FixtureUtils.Requests;
using AutoMoqExtensions.FixtureUtils.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Customizations
{
    public class AutoMockCustomization : ICustomization
    {
        public bool ConfigureMembers { get; set; } = true;
        public bool GenerateDelegates { get; set; } = true;
        public void Customize(IFixture fixture)
        {
            if (fixture == null || fixture is not AutoMockFixture mockFixture) throw new ArgumentNullException(nameof(fixture));

            fixture.Customizations.Add(new FilteringSpecimenBuilder(
                                            new EnumerableBuilder(),
                                            new TypeMatchSpecification(typeof(IRequestWithType))));

            fixture.Customizations.Add(new FilteringSpecimenBuilder(                                
                                            new TaskBuilder(),                                    
                                            new TypeMatchSpecification(typeof(IRequestWithType))));

            fixture.Customizations.Add(new FilteringSpecimenBuilder(                                
                                            new TupleBuilder(),
                                            new TypeMatchSpecification(typeof(IRequestWithType))));

            fixture.Customizations.Add(new FilteringSpecimenBuilder(
                                            new PostprocessorWithRecursion(
                                                new AutoMockDependenciesPostprocessor(
                                                    new DependencyInjectionMethodinvoker(
                                                        new CustomModestConstructorQuery())),
                                                new CustomAutoPropertiesCommand(mockFixture)),
                                            new TypeMatchSpecification(typeof(AutoMockDependenciesRequest))));

            fixture.Customizations.Add(new FilteringSpecimenBuilder(
                                            new AutoMockConstructorArgumentPostprocessor(mockFixture.ConstructorArgumentValues),
                                            new TypeMatchSpecification(typeof(AutoMockConstructorArgumentRequest))));

            fixture.Customizations.Add(new FilteringSpecimenBuilder(
                                            new AutoMockPropertyPostprocessor(),
                                            new TypeMatchSpecification(typeof(PropertyRequest))));

            fixture.Customizations.Add(new FilteringSpecimenBuilder(
                                            new AutoMockFieldPostprocessor(),
                                            new TypeMatchSpecification(typeof(FieldRequest))));

            fixture.Customizations.Add(new FilteringSpecimenBuilder(
                                            new AutoMockReturnPostprocessor(),
                                            new TypeMatchSpecification(typeof(AutoMockReturnRequest))));

            fixture.Customizations.Add(new FilteringSpecimenBuilder(
                                            new AutoMockOutParameterPostprocessor(),
                                            new TypeMatchSpecification(typeof(AutoMockOutParameterRequest))));

            fixture.Customizations.Add(new FilteringSpecimenBuilder(
                                            new AutoMockRequestPostprocessor(),
                                            new AutoMockRequestSpecification()));

            ISpecimenBuilder mockBuilder = new AutoMockPostprocessor(
                                              new AutoMockMethodInvoker(
                                                new AutoMockConstructorQuery()));

            // If members should be automatically configured, wrap the builder with members setup postprocessor.
            if (ConfigureMembers)
            {
                mockBuilder = new FilteringSpecimenBuilder(
                                    new PostprocessorWithRecursion(
                                        builder: mockBuilder,
                                        command: new CompositeSpecimenCommand(                                                   
                                                    // First stub so we should be able to have ready for constructing the object
                                                    new StubPropertiesCommand(),
                                                    new AutoMockVirtualMethodsCommand(),
                                                    // TODO... as of now when constructing the actual object the auto properties won't have values, maybe we should change that
                                                    new EnsureObjectCommand(),
                                                    new SetCallBaseCommand(),
                                                    // Only after constructing the object we can set the other properties on the object directly
                                                    new AutoMockAutoPropertiesCommand())),
                                    new TypeMatchSpecification(typeof(AutoMockDirectRequest)));
            }

            fixture.Customizations.Add(mockBuilder);
            fixture.Customizations.Add(new FilteringSpecimenBuilder(
                                            new Postprocessor(
                                                new LastResortBuilder(),
                                                new CacheCommand(mockFixture.Cache)),
                                            new TypeMatchSpecification(typeof(IRequestWithType))));

            fixture.ResidueCollectors.Add(new AutoMockRelay(mockFixture));

            if (GenerateDelegates)
            {
                fixture.Customizations.Add(new AutoMockRelay(new DelegateSpecification(), mockFixture));
            }
        }
    }
}
