using AutoFixture;
using AutoFixture.Kernel;
using AutoMoqExtensions.AutoMockUtils;
using AutoMoqExtensions.Extensions;
using AutoMoqExtensions.FixtureUtils;
using AutoMoqExtensions.FixtureUtils.Commands;
using AutoMoqExtensions.FixtureUtils.Customizations;
using AutoMoqExtensions.FixtureUtils.Postprocessors;
using AutoMoqExtensions.FixtureUtils.Requests;
using Moq;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoMoqExtensions
{
    /// <summary>
    /// Caustion the methods are not thread safe
    /// </summary>
    public partial class AutoMockFixture : Fixture
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
                                    new CustomAutoPropertiesCommand(this),
                                    new AnyTypeSpecification()));
        }

        #region Create from Tracker

        // TODO... make freeze if it has singleton or scoped attributes
        // We might do it in the depdendecny and request handlers and just inject it directly after creating it
        // TODO... test to make sure it is safe to do it multiple times
        internal T Freeze<T>(ITracker tracker)
        {
            var value = this.Create<T>(tracker);
            this.Inject(value);
            return value;
        }

        internal T Create<T>(ITracker tracker) => (T)Create(typeof(T), tracker);
        internal object Create(Type t, ITracker tracker)
        {
            if (t.IsValueType) return new SpecimenContext(this).Resolve(new SeededRequest(t, t.GetDefault()));

            if (AutoMockHelpers.IsAutoMock(t)) return Execute(new AutoMockDirectRequest(t, tracker));

            return Execute(new AutoMockDependenciesRequest(t, tracker));
        }

        #endregion

        #region Create
        // Override to use our own
        public T Freeze<T>()
        {
            var value = this.Create<T>();
            this.Inject(value);
            return value;
        }

        public T Create<T>() => (T)Create(typeof(T));

        internal object Create(Type t)
        {
            if (t.IsValueType) return new SpecimenContext(this).Resolve(new SeededRequest(t, t.GetDefault()));

            if (AutoMockHelpers.IsAutoMock(t)) return Execute(new AutoMockDirectRequest(t, this));

            return Execute(new AutoMockDependenciesRequest(t, this));
        }
        public object CreateAutoMock(Type t)
        {
            if (t.IsValueType) throw new Exception("Type must be a reference type");

            var result = Execute(new AutoMockRequest(t, this));

            return AutoMockHelpers.GetFromObj(result)!.GetMocked(); // It appears that the cast operators only work when statically typed
        }
        public T CreateAutoMock<T>() where T : class => (T)CreateAutoMock(typeof(T));

        #endregion

        #region Utils

        internal List<ConstructorArgumentValue> ConstructorArgumentValues = new List<ConstructorArgumentValue>();

        internal ITracker? GetTracker(object obj) => TrackerDict[AutoMockHelpers.GetFromObj(obj) ?? obj];

        internal Dictionary<object, ITracker> TrackerDict = new Dictionary<object, ITracker>();
        internal Dictionary<object, ITracker> ProcessingTrackerDict = new Dictionary<object, ITracker>(); // To track while processing
        
        private object Execute(ITracker request)
        {
            try
            {
                var result = new SpecimenContext(this).Resolve(request);
                request.SetCompleted();

                // We will rather deal with the underlying mock for consistance
                var key = AutoMockHelpers.GetFromObj(result) ?? result;
                TrackerDict[key] = request;
                ProcessingTrackerDict.Clear(); // No need to keep it around, to make it thread safe we should keep it around till all requests are done
                return result;
            }
            catch (ObjectCreationException ex)
            {
                throw new Exception(@"Unable to create object, please check inner exception for details
This can happen if the object (or a dependendent object) constructor calls a method or property that has not been setup corretly.
You can troubleshoot why the method/property has not been setup, it might be private/protected or non virtual or generic with arguments or ref or out method.
You can also try to move out the call in a separate method and call it from your constuctor (will only work if CallBase is false)", ex);
            }
        }

        #endregion

        #region Getters

        public List<object?> GetAt(object obj, string path)
        {
            if (!TrackerDict.ContainsKey(obj)) throw new Exception("Object not found, ensure that it is a root object in the current fixture, and possibly verify that .Equals() works correctly on the object");
            if (string.IsNullOrWhiteSpace(path)) throw new Exception(nameof(path) + " doesn't have a value");

            path = path.Trim();

            var tracker = TrackerDict[obj];
            var paths = tracker.GetChildrensPaths();
            if (paths is null || !paths.ContainsKey(path)) throw new Exception($"`{path}` not found, please ensure that it is the correct path on the correct object");
            
            return paths[path];
        }
        public object? GetSingleAt(object obj, string path) => GetAt(obj, path).Single();
        public IAutoMock GetAutoMock(object obj, string path)
        {
            var result = GetSingleAt(obj, path);
            if (result is null) throw new Exception($"Result object is null and not an `{nameof(AutoMock<object>)}`");

            var mock = AutoMockHelpers.GetFromObj(result);

            if (mock is null) throw new Exception($"Result object is not an `{nameof(AutoMock<object>)}`");

            return mock;
        }

        public AutoMock<T> GetAutoMock<T>(object obj, string path) where T : class
        {
            var result = GetAutoMock(obj, path);
            if (result is not AutoMock<T> mock) 
                    throw new Exception($"Result object is `{nameof(AutoMock<object>)}` and not `{nameof(AutoMock<object>)}<{typeof(T).Name}>`");

            return mock;
        }

        #endregion

        #region Verify

        public void VerifyAll(object obj)
        {
            if (!TrackerDict.ContainsKey(obj)) throw new Exception("Object not found, ensure that it is a root object in the current fixture, and possibly verify that .Equals() works correctly on the object");

            var tracker = TrackerDict[obj];
            var mocks = tracker.GetAllMocks();
            mocks?.ForEach(m => m.VerifyAll());
        }

        public void VerifyAll()
        {
            TrackerDict.Keys.ToList().ForEach(k => VerifyAll(k));
        }

        #endregion
    }
}
