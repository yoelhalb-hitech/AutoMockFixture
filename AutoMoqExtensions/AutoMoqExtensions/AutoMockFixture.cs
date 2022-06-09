using AutoFixture;
using AutoFixture.Kernel;
using AutoMoqExtensions.AutoMockUtils;
using AutoMoqExtensions.FixtureUtils.Commands;
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

            Customizations.Add(new Postprocessor(
                                    new AutoMockMethodInvoker(
                                        new CustomConstructorQueryWrapper(
                                            new ModestConstructorQuery())),
                                    new CustomAutoPropertiesCommand(),
                                    new AnyTypeSpecification()));

            // Console.WriteLine("Moqfixture.ctor {0}", Environment.StackTrace);
            Customize(new AutoMockCustomization { ConfigureMembers = configureMembers, GenerateDelegates = generateDelegates });
        }

        public T Create<T>() => new SpecimenContext(this).Create<T>();
        public object CreateAutoMock(Type t)
        {
            if (t.IsValueType) throw new Exception("Type must be a reference type");

            var result = new SpecimenContext(this).Resolve(new AutoMockRequest(t));
            return AutoMockHelpers.GetFromObj(result)!.GetMocked(); // It appears that the cast operators only work when statically typed
        }
        public T CreateAutoMock<T>() where T : class => (T)CreateAutoMock(typeof(T));
    }
}
