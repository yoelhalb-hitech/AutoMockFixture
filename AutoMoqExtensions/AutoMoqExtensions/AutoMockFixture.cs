using AutoFixture;
using AutoFixture.Kernel;
using AutoMoqExtensions.AutoMockUtils;
using AutoMoqExtensions.Extensions;
using AutoMoqExtensions.FixtureUtils.Commands;
using AutoMoqExtensions.FixtureUtils.Postprocessors;
using AutoMoqExtensions.FixtureUtils.Requests;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions
{
    public class AutoMockFixture : Fixture
    {
        public AutoMockFixture(bool configureMembers = true, bool generateDelegates = true)
        {
            Customizations.Add(new FilteringSpecimenBuilder(
                                    new FixedBuilder(this),
                                    new OrRequestSpecification(
                                        new ExactTypeSpecification(typeof(AutoMockFixture)),
                                        new ExactTypeSpecification(typeof(IFixture)),
                                        new ExactTypeSpecification(
                                            typeof(ISpecimenBuilder)))));


            // Console.WriteLine("Moqfixture.ctor {0}", Environment.StackTrace);
            Customize(new AutoMockCustomization { ConfigureMembers = configureMembers, GenerateDelegates = generateDelegates });
        
            // Needs to be after the automock customization, otherwise it will first try this
            Customizations.Add(new CorrectPostprocessor(
                                    new AutoMockMethodInvoker(
                                        new CustomConstructorQueryWrapper(
                                            new ModestConstructorQuery())),
                                    new CustomAutoPropertiesCommand(),
                                    new AnyTypeSpecification()));
        }

        public T Create<T>() => (T)Create(typeof(T));
        public object Create(Type t)
        {
            var context = new SpecimenContext(this);
            if (t.IsValueType) return context.Resolve(new SeededRequest(t, t.GetDefault()));

            if(AutoMockHelpers.IsAutoMock(t)) return context.Resolve(new AutoMockDirectRequest(t));

            return context.Resolve(new AutoMockDependenciesRequest(t));
        }
        public object CreateAutoMock(Type t)
        {
            if (t.IsValueType) throw new Exception("Type must be a reference type");

            var result = new SpecimenContext(this).Resolve(new AutoMockRequest(t));
            return AutoMockHelpers.GetFromObj(result)!.GetMocked(); // It appears that the cast operators only work when statically typed
        }
        public T CreateAutoMock<T>() where T : class => (T)CreateAutoMock(typeof(T));
    }
}
