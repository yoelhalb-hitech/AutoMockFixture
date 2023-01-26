using AutoFixture;
using AutoMockFixture.FixtureUtils.Builders.HelperBuilders;
using AutoMockFixture.FixtureUtils.Builders.MainBuilders;
using AutoMockFixture.FixtureUtils.Builders.SpecialBuilders;
using AutoMockFixture.FixtureUtils.Commands;
using AutoMockFixture.FixtureUtils.FixtureUtils.Builders.MainBuilders;
using AutoMockFixture.FixtureUtils.MethodInvokers;
using AutoMockFixture.FixtureUtils.MethodQueries;
using AutoMockFixture.FixtureUtils.Postprocessors;
using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.HelperRequests.NonAutoMock;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;
using AutoMockFixture.FixtureUtils.Requests.SpecialRequests;
using AutoMockFixture.FixtureUtils.Specifications;

namespace AutoMockFixture.FixtureUtils.Customizations;

public class AutoMockCustomization : ICustomization
{
    public bool ConfigureMembers { get; set; } = true;
    public bool GenerateDelegates { get; set; } = true;
    public void Customize(IFixture fixture)
    {
        if (fixture == null || fixture is not IAutoMockFixture mockFixture) throw new ArgumentNullException(nameof(fixture));

        fixture.Customizations.Add(new FilteringSpecimenBuilder(
                                        new AutoMockTypeControlBuilder(),
                                        new TypeMatchSpecification(typeof(IRequestWithType))));

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
                                        new DelegateBuilder(),
                                        new TypeMatchSpecification(typeof(IRequestWithType))));

        fixture.Customizations.Add(new FilteringSpecimenBuilder(
                                        new InnerBuilder(mockFixture.AutoMockHelpers),
                                        new TypeMatchSpecification(typeof(InnerRequest))));

        fixture.Customizations.Add(new FilteringSpecimenBuilder(
                                        new PostprocessorWithRecursion(
                                            mockFixture,
                                            new AutoMockDependenciesBuilder(
                                                new DependencyInjectionMethodInvoker(
                                                    new CustomModestConstructorQuery(mockFixture.AutoMockHelpers)),
                                                mockFixture.AutoMockHelpers),
                                            ConfigureMembers ? new AutoMockDependenciesAutoPropertiesHandlerCommand(mockFixture) : new EmptyCommand()),
                                        new TypeMatchSpecification(typeof(AutoMockDependenciesRequest))));

        fixture.Customizations.Add(new FilteringSpecimenBuilder(
                                        new PostprocessorWithRecursion(
                                            mockFixture,
                                            new NonAutoMockBuilder(
                                                new MethodInvokerWithRecursion(
                                                    new CustomModestConstructorQuery(mockFixture.AutoMockHelpers)),
                                                mockFixture.AutoMockHelpers),
                                            ConfigureMembers ? new CustomAutoPropertiesCommand(mockFixture) : new EmptyCommand()),
                                        new TypeMatchSpecification(typeof(NonAutoMockRequest))));

        fixture.Customizations.Add(new FilteringSpecimenBuilder(
                                        new ConstructorArgumentBuilder(mockFixture.ConstructorArgumentValues),
                                        new TypeMatchSpecification(typeof(ConstructorArgumentRequest))));

        fixture.Customizations.Add(new FilteringSpecimenBuilder(
                                        new PropertyBuilder(),
                                        new TypeMatchSpecification(typeof(PropertyRequest))));

        fixture.Customizations.Add(new FilteringSpecimenBuilder(
                                        new FieldBuilder(),
                                        new TypeMatchSpecification(typeof(FieldRequest))));

        fixture.Customizations.Add(new FilteringSpecimenBuilder(
                                        new ReturnBuilder(),
                                        new TypeMatchSpecification(typeof(ReturnRequest))));

        fixture.Customizations.Add(new FilteringSpecimenBuilder(
                                        new OutParameterBuilder(),
                                        new TypeMatchSpecification(typeof(OutParameterRequest))));

        fixture.Customizations.Add(new FilteringSpecimenBuilder(
                                        new AutoMockRequestBuilder(mockFixture.AutoMockHelpers),
                                        new AutoMockRequestSpecification(mockFixture.AutoMockHelpers)));

        ISpecimenBuilder mockBuilder = new AutoMockBuilder(
                                          new AutoMockMethodInvoker(
                                            new AutoMockConstructorQuery(mockFixture.AutoMockHelpers),
                                            mockFixture.AutoMockHelpers.GetAutoMockInitCommand()),
                                          mockFixture.AutoMockHelpers);

        // If members should be automatically configured, wrap the builder with members setup postprocessor.
        // This might be useful when wanting to have control over the setup        
        if (ConfigureMembers)
        {
            var setupFactory = mockFixture.AutoMockHelpers.GetSetupServiceFactory(() => mockFixture.MethodSetupType);

            mockBuilder = new PostprocessorWithRecursion(
                                    fixture: mockFixture,
                                    builder: mockBuilder,
                                    command: new CompositeSpecimenCommand(
                                                mockFixture.AutoMockHelpers.GetStubAllPropertiesCommand(),
                                                new AutoMockVirtualMethodsCommand(mockFixture.AutoMockHelpers, setupFactory),
                                                // This one has to be after `AutoMockStubAllPropertiesCommand`
                                                new AutoMockAutoPropertiesHandlerCommand(mockFixture.AutoMockHelpers),
                                                // This one has to be after `AutoMockAutoPropertiesHandlerCommand`
                                                mockFixture.AutoMockHelpers.GetClearInvocationsCommand()));
        }

        fixture.Customizations.Add(new FilteringSpecimenBuilder(mockBuilder,
                                        new TypeMatchSpecification(typeof(AutoMockDirectRequest))));

        fixture.Customizations.Add(new FilteringSpecimenBuilder(
                                        new Postprocessor(
                                            new LastResortBuilder(mockFixture.AutoMockHelpers),
                                            new CacheCommand(mockFixture.Cache)),
                                        new TypeMatchSpecification(typeof(IRequestWithType))));

        fixture.ResidueCollectors.Add(new AutoMockRelay(mockFixture));

        if (GenerateDelegates)
        {
            fixture.Customizations.Add(new AutoMockRelay(new DelegateSpecification(), mockFixture));
        }
    }
}
