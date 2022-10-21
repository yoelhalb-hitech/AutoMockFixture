using AutoFixture;
using AutoFixture.Kernel;
using AutoMoqExtensions.AutoMockUtils;
using AutoMoqExtensions.Extensions;
using AutoMoqExtensions.FixtureUtils;
using AutoMoqExtensions.FixtureUtils.Builders.HelperBuilders;
using AutoMoqExtensions.FixtureUtils.Commands;
using AutoMoqExtensions.FixtureUtils.Customizations;
using AutoMoqExtensions.FixtureUtils.Postprocessors;
using AutoMoqExtensions.FixtureUtils.Requests;
using AutoMoqExtensions.FixtureUtils.Requests.MainRequests;
using AutoMoqExtensions.FixtureUtils.Specifications;
using DotNetPowerExtensions.DependencyManagement;
using Moq;
using System.Reflection;

namespace AutoMoqExtensions;

/// <summary>
/// For Test project purposes
/// </summary>
internal class AbstractAutoMockFixture : AutoMockFixture 
{
    public AbstractAutoMockFixture(bool noConfigureMembers = false) : base(noConfigureMembers) { }

    public override object Create(Type t, AutoMockTypeControl? autoMockTypeControl = null) => throw new NotSupportedException();
    public override T Freeze<T>()
    {
        try
        {
            base.Freeze<T>();
        }
        catch { }
#pragma warning disable CS8603 // Possible null reference return.
        return default;
#pragma warning restore CS8603 // Possible null reference return.
    }
}
/// <summary>
/// Caution the methods are not thread safe
/// </summary>
public abstract partial class AutoMockFixture : Fixture
{
    internal virtual MethodSetupTypes MethodSetupType { get; set; } = MethodSetupTypes.LazySame;
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
    public AutoMockFixture(bool noConfigureMembers = false, bool generateDelegates = false, MethodSetupTypes? methodSetupType = null)
    {
        var engine = new CompositeSpecimenBuilder(new CustomEngineParts(this));
        
        var newAutoProperties = new AutoPropertiesTarget(
                                    new PostprocessorWithRecursion(
                                        new CompositeSpecimenBuilder(
                                            engine,
                                            new MultipleRelay { Count = this.RepeatCount }),
                                        // AutoFixture expects `AutoPropertiesCommand` to be a single command, so we have to stuff anoything extra in an extra
                                        new CustomAutoPropertiesCommand(this),
                                        new AnyTypeSpecification(),
                                        new CacheCommand(this.Cache)));
        
        var currentGraph = graphField.GetValue(this);
        Func<ISpecimenBuilderNode, bool> matcher = node => node is AutoPropertiesTarget;
        var newGraph = replaceNodeMethod.Invoke(null, new[] { currentGraph, newAutoProperties, matcher }) as ISpecimenBuilderNode;
        updateGraphAndSetupAdapterMethod.Invoke(this, new[] { newGraph });
        
        Customizations.Add(new CacheBuilder(Cache));

        Customizations.Add(new FilteringSpecimenBuilder(
                                new FixedBuilder(this),
                                new OrRequestSpecification(
                                    new TypeOrRequestSpecification(new ExactTypeSpecification(typeof(AutoMockFixture))),
                                    new TypeOrRequestSpecification(new ExactTypeSpecification(typeof(Fixture))),
                                    new TypeOrRequestSpecification(new ExactTypeSpecification(typeof(IFixture))),
                                    new TypeOrRequestSpecification(new ExactTypeSpecification(typeof(ISpecimenBuilder))))));

        Customize(new AutoMockCustomization { ConfigureMembers = !noConfigureMembers, GenerateDelegates = generateDelegates });

        Customize(new FreezeCustomization(new TypeOrRequestSpecification(new AttributeMatchSpecification(typeof(SingletonAttribute)))));
        Customize(new FreezeCustomization(new TypeOrRequestSpecification(new AttributeMatchSpecification(typeof(ScopedAttribute))))); // Considering it scoped as it is per fixture whcih is normally scoped

        Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                        .ForEach(b => Behaviors.Remove(b));
        Behaviors.Add(new FreezeRecursionBehavior());

        if (methodSetupType is not null) MethodSetupType = methodSetupType.Value;
    }

    public AutoMockTypeControl AutoMockTypeControl { get; set; } = new AutoMockTypeControl();

    internal Cache Cache { get; } = new Cache();

    #region Create
    // Override to use our own
    public virtual T? Freeze<T>()
    {
        Customize(new FreezeCustomization(new TypeOrRequestSpecification(new TypeSpecification(typeof(T)))));
        
        return Create<T>();
    }

    public T? Create<T>(AutoMockTypeControl? autoMockTypeControl = null) => (T?)Create(typeof(T), autoMockTypeControl);

    public abstract object? Create(Type t, AutoMockTypeControl? autoMockTypeControl = null);

    public object? CreateWithAutoMockDependencies(Type t, bool callBase = false, AutoMockTypeControl? autoMockTypeControl = null)
    {
        if (t.IsValueType) return new SpecimenContext(this).Resolve(new SeededRequest(t, t.GetDefault()));

        if (AutoMockHelpers.IsAutoMock(t) || typeof(Mock).IsAssignableFrom(t)) 
            return CreateAutoMock(t, callBase, autoMockTypeControl);

        var result = Execute(new AutoMockDependenciesRequest(t, this) { MockShouldCallbase = callBase }, autoMockTypeControl);

        return result;
    }
    public T? CreateWithAutoMockDependencies<T>(bool callBase = false, AutoMockTypeControl? autoMockTypeControl = null) where T : class 
                => (T?)CreateWithAutoMockDependencies(typeof(T), callBase, autoMockTypeControl);

    public object? CreateNonAutoMock(Type t, AutoMockTypeControl? autoMockTypeControl = null)
    {
        var result = Execute(new NonAutoMockRequest(t, this), autoMockTypeControl);

        return result;
    }
    public T? CreateNonAutoMock<T>(AutoMockTypeControl? autoMockTypeControl = null) 
                => (T?)CreateNonAutoMock(typeof(T), autoMockTypeControl);

    public object? CreateAutoMock(Type t, bool callBase = false, AutoMockTypeControl? autoMockTypeControl = null)
    {
        if (t.IsValueType) throw new InvalidOperationException("Type must be a reference type");

        var type = AutoMockHelpers.IsAutoMock(t) 
                    ? AutoMockHelpers.GetMockedType(t)!
                    : !typeof(Mock).IsAssignableFrom(t) 
                        ? t 
                        : t.IsGenericType ? t.GenericTypeArguments.First(): typeof(object);
        if(!AutoMockHelpers.IsAutoMockAllowed(type))
            throw new InvalidOperationException($"{type.FullName} cannot be AutoMock");

        var result = Execute(new AutoMockRequest(type, this) { MockShouldCallbase = callBase }, autoMockTypeControl);

        return type != t ? AutoMockHelpers.GetFromObj(result)! : result; // It appears that the cast operators only work when statically typed
    }
    public T? CreateAutoMock<T>(bool callBase = false, AutoMockTypeControl? autoMockTypeControl = null) where T : class
                => (T?)CreateAutoMock(typeof(T), callBase, autoMockTypeControl);

    #endregion

    #region Utils

    internal List<ConstructorArgumentValue> ConstructorArgumentValues = new();

    internal ITracker? GetTracker(object obj) => TrackerDict[AutoMockHelpers.GetFromObj(obj) ?? obj];

    internal Dictionary<object, ITracker> TrackerDict = new();
    internal Dictionary<object, ITracker> ProcessingTrackerDict = new(); // To track while processing
    
    private object? Execute(ITracker request, AutoMockTypeControl? autoMockTypeControl = null)
    {
        try
        {
            var result = new RecursionContext(this, this) { AutoMockTypeControl = autoMockTypeControl }.Resolve(request);
            request.SetCompleted();

            // We will rather deal with the underlying mock for consistancy
            // but also to avoid having to call .Equals on the object which it can then later report as called in the verify process
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

    /// <summary>
    /// Get the value at the specified path (property/field/ctor argument/out parameter/method result etc.)
    /// </summary>
    /// <param name="obj">An object created witht he current <see cref="AutoMockFixture"/></param>
    /// <param name="path">The path to get the value at</param>
    /// <returns></returns>
    /// <exception cref="Exception">Path not provided</exception>
    /// <exception cref="Exception">Object not found</exception>
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

    // NOTE: We don't do VerifyAll() as it will try to verify all setups that the AutoMockFixture has done
    public void Verify(object obj)
    {
        if (!TrackerDict.ContainsKey(obj)) throw new Exception("Object not found, ensure that it is a root object in the current fixture, and possibly verify that .Equals() works correctly on the object");

        var tracker = TrackerDict[obj];
        var mocks = tracker.GetAllMocks();
        mocks?.ForEach(m => m.Verify());
    }

    public void Verify()
    {
        TrackerDict.Keys.ToList().ForEach(k => Verify(k));
    }
    
    public void VerifyNoOtherCalls(object obj)
    {
        if (!TrackerDict.ContainsKey(obj)) throw new Exception("Object not found, ensure that it is a root object in the current fixture, and possibly verify that .Equals() works correctly on the object");

        var tracker = TrackerDict[obj];
        var mocks = tracker.GetAllMocks();
        mocks?.ForEach(m => m.VerifyNoOtherCalls());
    }

    public void VerifyNoOtherCalls()
    {
        TrackerDict.Keys.ToList().ForEach(k => VerifyNoOtherCalls(k));
    }

    #endregion
}
