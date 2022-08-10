using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMoqExtensions.FixtureUtils.Builders.HelperBuilders;
using AutoMoqExtensions.FixtureUtils.Builders.MainBuilders;
using AutoMoqExtensions.FixtureUtils.Builders.SpecialBuilders;
using AutoMoqExtensions.FixtureUtils.Commands;
using AutoMoqExtensions.FixtureUtils.FixtureUtils.Builders.MainBuilders;
using AutoMoqExtensions.FixtureUtils.MethodInvokers;
using AutoMoqExtensions.FixtureUtils.MethodQueries;
using AutoMoqExtensions.FixtureUtils.Postprocessors;
using AutoMoqExtensions.FixtureUtils.Requests;
using AutoMoqExtensions.FixtureUtils.Requests.HelperRequests.NonAutoMock;
using AutoMoqExtensions.FixtureUtils.Requests.MainRequests;
using AutoMoqExtensions.FixtureUtils.Specifications;

namespace AutoMoqExtensions.FixtureUtils.Customizations;

public class AutoMockCustomization : ICustomization
{
    public bool ConfigureMembers { get; set; } = true;
    public bool GenerateDelegates { get; set; } = true;
    public void Customize(IFixture fixture)
    {
        if (fixture == null || fixture is not AutoMockFixture mockFixture) throw new ArgumentNullException(nameof(fixture));

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
                                        new PostprocessorWithRecursion(
                                            new AutoMockDependenciesBuilder(
                                                new DependencyInjectionMethodInvoker(
                                                    new CustomModestConstructorQuery())),
                                            new AutoMockDependenciesAutoPropertiesHandlerCommand(mockFixture)),
                                        new TypeMatchSpecification(typeof(AutoMockDependenciesRequest))));

        fixture.Customizations.Add(new FilteringSpecimenBuilder(
                                        new PostprocessorWithRecursion(
                                            new NonAutoMockBuilder(
                                                new MethodInvokerWithRecursion(
                                                    new CustomModestConstructorQuery())),
                                            new CustomAutoPropertiesCommand(mockFixture)),
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
                                        new AutoMockRequestBuilder(),
                                        new AutoMockRequestSpecification()));

        ISpecimenBuilder mockBuilder = new AutoMockBuilder(
                                          new AutoMockMethodInvoker(
                                            new AutoMockConstructorQuery()));

        // If members should be automatically configured, wrap the builder with members setup postprocessor.
        if (ConfigureMembers)
        {
            mockBuilder = new FilteringSpecimenBuilder(
                                new PostprocessorWithRecursion(
                                    builder: mockBuilder,
                                    command: new CompositeSpecimenCommand(
                                                new AutoMockStubAllPropertiesCommand(),
                                                new AutoMockVirtualMethodsCommand(),
                                                new AutoMockAutoPropertiesHandlerCommand())),
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
