using AutoFixture;
using AutoFixture.Kernel;
using AutoMoqExtensions.AutoMockUtils;
using AutoMoqExtensions.Extensions;
using AutoMoqExtensions.FixtureUtils;
using AutoMoqExtensions.FixtureUtils.Commands;
using AutoMoqExtensions.FixtureUtils.Customizations;
using AutoMoqExtensions.FixtureUtils.Postprocessors;
using AutoMoqExtensions.FixtureUtils.Requests;
using AutoMoqExtensions.FixtureUtils.Specifications;
using Moq;
using DotNetPowerExtensions.DependencyManagement;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMoqExtensions.FixtureUtils.MethodInvokers;
using System.Collections;
using System.Reflection;

namespace AutoMoqExtensions
{
    /// <summary>
    /// Caustion the methods are not thread safe
    /// </summary>
    public partial class AutoMockFixture : Fixture
    {
        private readonly static MethodInfo replaceNodeMethod;
        private readonly static FieldInfo graphField;
        private readonly static MethodInfo updateGraphAndSetupAdapterMethod;
        
        static AutoMockFixture()
        {
            replaceNodeMethod = typeof(SpecimenBuilderNode)
                .GetMethod("ReplaceNodes", Extensions.TypeExtensions.AllBindings, null, new Type[]
                {
                    typeof(ISpecimenBuilderNode),
                    typeof(ISpecimenBuilderNode),
                    typeof(Func<ISpecimenBuilderNode, bool>),
                }, null);
            
            graphField = typeof(Fixture).GetField("graph", Extensions.TypeExtensions.AllBindings);
            
            updateGraphAndSetupAdapterMethod = typeof(Fixture).GetMethod("UpdateGraphAndSetupAdapters", Extensions.TypeExtensions.AllBindings, null, new Type[]
            {
                typeof(ISpecimenBuilderNode),
            }, null);
        }
        public AutoMockFixture(bool configureMembers = true, bool generateDelegates = true)
        {
            var engine = new CompositeSpecimenBuilder(new CustomEngineParts(this));
            
            var newAutoProperties = new AutoPropertiesTarget(
                                        new PostprocessorWithRecursion(
                                            new CompositeSpecimenBuilder(
                                                engine,
                                                new MultipleRelay { Count = this.RepeatCount }),
                                            new CompositeSpecimenCommand(
                                                new CacheCommand(this.Cache),
                                                new CustomAutoPropertiesCommand(this)),
                                            new AnyTypeSpecification()));
            
            var currentGraph = graphField.GetValue(this);
            Func<ISpecimenBuilderNode, bool> matcher = node => node is AutoPropertiesTarget;
            var newGraph = replaceNodeMethod.Invoke(null, new[] { currentGraph, newAutoProperties, matcher }) as ISpecimenBuilderNode;
            updateGraphAndSetupAdapterMethod.Invoke(this, new[] { newGraph });
            
            Customizations.Add(new CachePostprocessor(Cache));

            Customizations.Add(new FilteringSpecimenBuilder(
                                    new FixedBuilder(this),
                                    new OrRequestSpecification(
                                        new TypeOrRequestSpecification(new ExactTypeSpecification(typeof(AutoMockFixture))),
                                        new TypeOrRequestSpecification(new ExactTypeSpecification(typeof(IFixture))),
                                        new TypeOrRequestSpecification(new ExactTypeSpecification(typeof(ISpecimenBuilder))))));

            Customize(new AutoMockCustomization { ConfigureMembers = configureMembers, GenerateDelegates = generateDelegates });
        
            Customize(new FreezeCustomization(new TypeOrRequestSpecification(new AttributeMatchSpecification(typeof(SingletonAttribute)))));
            Customize(new FreezeCustomization(new TypeOrRequestSpecification(new AttributeMatchSpecification(typeof(ScopedAttribute))))); // Considering it scoped as it is per fixture whcih is normally scoped

            Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                            .ForEach(b => Behaviors.Remove(b));
            Behaviors.Add(new FreezeRecursionBehavior());
        }

        public AutoMockTypeControl AutoMockTypeControl { get; set; } = new AutoMockTypeControl();

        internal Cache Cache { get; } = new Cache();

        #region Create
        // Override to use our own
        public T Freeze<T>()
        {
            Customize(new FreezeCustomization(new TypeOrRequestSpecification(new TypeMatchSpecification(typeof(T)))));
            
            return Create<T>();
        }

        public T Create<T>() => (T)Create(typeof(T));

        internal object Create(Type t)
        {
            if (t.IsValueType) return new SpecimenContext(this).Resolve(new SeededRequest(t, t.GetDefault()));
            
            if (AutoMockHelpers.IsAutoMock(t))
            {
                var inner = AutoMockHelpers.GetMockedType(t)!;
                
                if(AutoMockHelpers.IsAutoMockAllowed(inner)) 
                    return Execute(new AutoMockDirectRequest(t, this));

                throw new InvalidOperationException($"{AutoMockHelpers.GetMockedType(t)!.FullName} cannot be AutoMock");
            }
            
            return Execute(new AutoMockDependenciesRequest(t, this));
        }
        public object CreateWithAutoMockDependencies(Type t, bool callBase = false, AutoMockTypeControl? autoMockTypeControl = null)
        {
            if (t.IsValueType) throw new Exception("Type must be a reference type");

            var result = Execute(new AutoMockDependenciesRequest(t, this) { MockShouldCallbase = callBase }, autoMockTypeControl);

            return result;
        }
        public T CreateWithAutoMockDependencies<T>(bool callBase = false, AutoMockTypeControl? autoMockTypeControl = null) where T : class 
                    => (T)CreateWithAutoMockDependencies(typeof(T), callBase, autoMockTypeControl);

        public object CreateNonAutoMock(Type t, AutoMockTypeControl? autoMockTypeControl = null)
        {
            var result = Execute(new NonAutoMockRequest(t, this), autoMockTypeControl);

            return result;
        }
        public T CreateNonAutoMock<T>(AutoMockTypeControl? autoMockTypeControl = null) 
                    => (T)CreateNonAutoMock(typeof(T), autoMockTypeControl);

        public object CreateAutoMock(Type t, bool callBase = false, AutoMockTypeControl? autoMockTypeControl = null)
        {
            if (t.IsValueType) throw new Exception("Type must be a reference type");

            var type = AutoMockHelpers.IsAutoMock(t) ? AutoMockHelpers.GetMockedType(t)! : t;
            if(!AutoMockHelpers.IsAutoMockAllowed(type))
                throw new InvalidOperationException($"{type.FullName} cannot be AutoMock");

            var result = Execute(new AutoMockRequest(type, this) { MockShouldCallbase = callBase }, autoMockTypeControl);

            return type != t ? AutoMockHelpers.GetFromObj(result)! : result; // It appears that the cast operators only work when statically typed
        }
        public T CreateAutoMock<T>(bool callBase = false, AutoMockTypeControl? autoMockTypeControl = null) where T : class
                    => (T)CreateAutoMock(typeof(T), callBase, autoMockTypeControl);

        #endregion

        #region Utils

        internal List<ConstructorArgumentValue> ConstructorArgumentValues = new();

        internal ITracker? GetTracker(object obj) => TrackerDict[AutoMockHelpers.GetFromObj(obj) ?? obj];

        internal Dictionary<object, ITracker> TrackerDict = new();
        internal Dictionary<object, ITracker> ProcessingTrackerDict = new(); // To track while processing
        
        private object Execute(ITracker request, AutoMockTypeControl? autoMockTypeControl = null)
        {
            try
            {
                var result = new RecursionContext(this, this) { AutoMockTypeControl = autoMockTypeControl }.Resolve(request);
                request.SetCompleted();

                // We will rather deal with the underlying mock for consistance
                var key = AutoMockHelpers.GetFromObj(result) ?? result;
                TrackerDict[key] = request;
                ProcessingTrackerDict.Clear(); // No need to keep it around, to make it thread safe we should keep it around till all requests are done
                return result;
            }
            catch (ObjectCreationException ex)
            {
                throw;
                // Only use the following if callbase is false, but specified to call the base constructors, so far we don't support that
//                throw new Exception(@"Unable to create object, please check inner exception for details
//This can happen if the object (or a dependendent object) constructor calls a method or property that has not been setup corretly.
//You can troubleshoot why the method/property has not been setup, it might be private/protected or non virtual or generic with arguments or ref or out method.
//You can also try to move out the call in a separate method and call it from your constuctor (will only work if CallBase is false)", ex);
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
