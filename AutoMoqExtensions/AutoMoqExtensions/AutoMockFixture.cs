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
            Customizations.Add(new Postprocessor(
                                    new AutoMockMethodInvoker(
                                        new CustomConstructorQueryWrapper(
                                            new ModestConstructorQuery())),
                                    new CustomAutoPropertiesCommand(),
                                    new AnyTypeSpecification()));
        }

        internal ITracker? GetTracker(object obj) => TrackerDict[AutoMockHelpers.GetFromObj(obj) ?? obj];
        internal Dictionary<object, ITracker> TrackerDict = new Dictionary<object, ITracker>();
        private object Execute(ITracker request)
        {
            var result = new SpecimenContext(this).Resolve(request);
            request.Completed();

            // We will rather deal with the underlying mock for consistance
            var key = AutoMockHelpers.GetFromObj(result) ?? result;
            TrackerDict[key] = request;
            return result;
        }
        public T Create<T>() => (T)Create(typeof(T));
        public object Create(Type t)
        {
            if (t.IsValueType) return new SpecimenContext(this).Resolve(new SeededRequest(t, t.GetDefault()));

            if (AutoMockHelpers.IsAutoMock(t)) return Execute(new AutoMockDirectRequest(t, null));

            return Execute(new AutoMockDependenciesRequest(t, null));
        }
        public object CreateAutoMock(Type t)
        {
            if (t.IsValueType) throw new Exception("Type must be a reference type");

            var result = Execute(new AutoMockRequest(t, null));
           
            return AutoMockHelpers.GetFromObj(result)!.GetMocked(); // It appears that the cast operators only work when statically typed
        }
        public T CreateAutoMock<T>() where T : class => (T)CreateAutoMock(typeof(T));
    }
}
