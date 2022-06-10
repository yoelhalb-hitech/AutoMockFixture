using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;
using AutoMoqExtensions.FixtureUtils.Commands;
using AutoMoqExtensions.FixtureUtils.Postprocessors;
using AutoMoqExtensions.FixtureUtils.Requests;
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

            fixture.Customizations.Add(new CorrectPostprocessor(
                                            new AutoMockDependenciesPostprocessor(
                                                new AutoMockMethodInvoker(
                                                    new CustomConstructorQueryWrapper(
                                                        new ModestConstructorQuery()))),
                                            new CustomAutoPropertiesCommand(),
                                            new TypeMatchSpecification(typeof(AutoMockDependenciesRequest))));

            fixture.Customizations.Add(new FilteringSpecimenBuilder(
                                            new AutoMockConstructorArgumentPostprocessor(),
                                            new TypeMatchSpecification(typeof(AutoMockConstructorArgumentRequest))));

            fixture.Customizations.Add(new FilteringSpecimenBuilder(
                                            new AutoMockPropertyPostprocessor(),
                                            new TypeMatchSpecification(typeof(AutoMockPropertyRequest))));

            fixture.Customizations.Add(new FilteringSpecimenBuilder(
                                            new AutoMockFieldPostprocessor(),
                                            new TypeMatchSpecification(typeof(AutoMockFieldRequest))));

            fixture.Customizations.Add(new FilteringSpecimenBuilder(
                                            new AutoMockReturnPostprocessor(),
                                            new TypeMatchSpecification(typeof(AutoMockReturnRequest))));

            fixture.Customizations.Add(new FilteringSpecimenBuilder(
                                            new AutoMockRequestPostprocessor(),
                                            new AutoMockRequestSpecification()));

            ISpecimenBuilder mockBuilder = new AutoMockPostprocessor(
                                              new AutoMockMethodInvoker(
                                                 new CustomConstructorQueryWrapper(
                                                    new AutoMockConstructorQuery())));

            // If members should be automatically configured, wrap the builder with members setup postprocessor.
            if (this.ConfigureMembers)
            {
                mockBuilder = new CorrectPostprocessor(
                    builder: mockBuilder,
                    command: new CompositeSpecimenCommand(
                                new EnsureObjectCommand(),
                                new SetCallBaseCommand(),
                                new StubPropertiesCommand(),
                                new AutoMockVirtualMethodsCommand(),
                                new AutoMockAutoPropertiesCommand()),
                    specification: new TypeMatchSpecification(typeof(AutoMockDirectRequest)));
            }

            fixture.Customizations.Add(mockBuilder);

            fixture.ResidueCollectors.Add(new AutoMockRelay());

            if (this.GenerateDelegates)
            {
                fixture.Customizations.Add(new AutoMockRelay(new DelegateSpecification()));
            }
        }
    }
}
