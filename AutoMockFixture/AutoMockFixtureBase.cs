﻿using AutoFixture;
using AutoMockFixture.AutoMockUtils;
using AutoMockFixture.FixtureUtils;
using AutoMockFixture.FixtureUtils.Builders.HelperBuilders;
using AutoMockFixture.FixtureUtils.Commands;
using AutoMockFixture.FixtureUtils.Customizations;
using AutoMockFixture.FixtureUtils.Postprocessors;
using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;
using AutoMockFixture.FixtureUtils.Specifications;
using SequelPay.DotNetPowerExtensions;
using System.ComponentModel;
using System.Reflection;

namespace AutoMockFixture.FixtureUtils; // Use this namespace not to be in the main namespace (would have made it internal but then the subclasses would also have to be internal)

/// <summary>
/// CAUTION: the methods are not thread safe
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public abstract partial class AutoMockFixtureBase : Fixture, IAutoMockFixture
{
    MethodSetupTypes IAutoMockFixture.MethodSetupType => MethodSetupType;
    IAutoMockFixture IAutoMockFixture.Customize(AutoFixture.ICustomization customization) => (IAutoMockFixture)Customize(customization);

    internal virtual MethodSetupTypes MethodSetupType { get; set; } = MethodSetupTypes.LazySame;
    private readonly static MethodInfo replaceNodeMethod;
    private readonly static FieldInfo graphField;
    private readonly static MethodInfo updateGraphAndSetupAdapterMethod;
    
    static AutoMockFixtureBase()
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
    public AutoMockFixtureBase(bool noConfigureMembers = false, bool generateDelegates = false, MethodSetupTypes? methodSetupType = null)
    {
        var engine = new CompositeSpecimenBuilder(new CustomEngineParts(this.AutoMockHelpers));
        
        var newAutoProperties = new AutoPropertiesTarget(
                                    new PostprocessorWithRecursion(
                                        this,
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
                                    new TypeOrRequestSpecification(new ExactTypeSpecification(typeof(AutoMockFixtureBase)), AutoMockHelpers),
                                    new TypeOrRequestSpecification(new ExactTypeSpecification(typeof(Fixture)), AutoMockHelpers),
                                    new TypeOrRequestSpecification(new ExactTypeSpecification(typeof(IFixture)), AutoMockHelpers),
                                    new TypeOrRequestSpecification(new ExactTypeSpecification(typeof(ISpecimenBuilder)), AutoMockHelpers))));

        Customize(new AutoMockCustomization { ConfigureMembers = !noConfigureMembers, GenerateDelegates = generateDelegates });

        Customize(new FreezeCustomization(new TypeOrRequestSpecification(new AttributeMatchSpecification(typeof(SingletonAttribute)), AutoMockHelpers)));
        // Considering it scoped as it is per fixture which is normally scoped
        Customize(new FreezeCustomization(new TypeOrRequestSpecification(new AttributeMatchSpecification(typeof(ScopedAttribute)), AutoMockHelpers)));

        Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                        .ForEach(b => Behaviors.Remove(b));
        Behaviors.Add(new FreezeRecursionBehavior());

        if (methodSetupType is not null) MethodSetupType = methodSetupType.Value;
    }

    public AutoMockTypeControl AutoMockTypeControl { get; set; } = new AutoMockTypeControl();

    Cache IAutoMockFixture.Cache => Cache;
    internal Cache Cache { get; } = new Cache();

    #region Create
    // Override to use our own
    public virtual T? Freeze<T>()
    {
        Customize(new FreezeCustomization(new TypeOrRequestSpecification(new TypeSpecification(typeof(T)), AutoMockHelpers)));
        
        return Create<T>();
    }

    public T? Create<T>(AutoMockTypeControl? autoMockTypeControl = null) => (T?)Create(typeof(T), autoMockTypeControl);

    public abstract object? Create(Type t, AutoMockTypeControl? autoMockTypeControl = null);

    public object? CreateWithAutoMockDependencies(Type t, bool callBase = false, AutoMockTypeControl? autoMockTypeControl = null)
    {
        if (t.IsValueType) return new SpecimenContext(this).Resolve(new SeededRequest(t, t.GetDefault()));

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
                    : !AutoMockHelpers.MockRequestSpecification.IsSatisfiedBy(t)
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

    internal abstract IAutoMockHelpers AutoMockHelpers { get; }
    IAutoMockHelpers IAutoMockFixture.AutoMockHelpers => AutoMockHelpers;

    List<ConstructorArgumentValue> IAutoMockFixture.ConstructorArgumentValues => ConstructorArgumentValues;
    internal List<ConstructorArgumentValue> ConstructorArgumentValues = new();

    internal ITracker? GetTracker(object obj) => TrackerDict
                    .SingleOrDefault(t => t.Key.IsAlive && t.Key.Target == (AutoMockHelpers.GetFromObj(obj) ?? obj))
                    .Value;

    Dictionary<WeakReference, Task<Dictionary<string, List<WeakReference?>>>> IAutoMockFixture.PathsDict => PathsDict;

    internal Dictionary<WeakReference, Task<Dictionary<string, List<WeakReference?>>>> PathsDict = new();

    Dictionary<WeakReference, Task<List<WeakReference<IAutoMock>>>> IAutoMockFixture.MocksDict => MocksDict;
    internal Dictionary<WeakReference, Task<List<WeakReference<IAutoMock>>>> MocksDict = new();
    Dictionary<WeakReference, Task<Dictionary<Type, List<WeakReference<IAutoMock>>>>> IAutoMockFixture.MocksByTypeDict => MocksByTypeDict;
    internal Dictionary<WeakReference, Task<Dictionary<Type, List<WeakReference<IAutoMock>>>>> MocksByTypeDict = new();

    Dictionary<WeakReference, ITracker> IAutoMockFixture.TrackerDict => TrackerDict;
    internal Dictionary<WeakReference, ITracker> TrackerDict = new();

    Dictionary<object, ITracker> IAutoMockFixture.ProcessingTrackerDict => ProcessingTrackerDict;
    internal Dictionary<object, ITracker> ProcessingTrackerDict = new(); // To track while processing
    
    private object? Execute(ITracker request, AutoMockTypeControl? autoMockTypeControl = null)
    {
        try
        {
            var result = new RecursionContext(this, this) { AutoMockTypeControl = autoMockTypeControl }.Resolve(request);
            
            // TODO... we might have a problem if there is duplicates (for example for primitive typs or strings)

            // We will rather deal with the underlying mock for consistancy
            // but also to avoid having to call .Equals on the object which it can then later report as called in the verify process
            // WeakReference won't block Garbage Collection, and also avoids the issue of duplicates
            var key = (AutoMockHelpers.GetFromObj(result) ?? result).ToWeakReference();
            TrackerDict[key] = request;

            PathsDict[key] = Task.Run(() => request.GetChildrensPaths()
                        .ToDictionary(c => c.Key, c => c.Value)
                    ?? new Dictionary<string, List<WeakReference?>>());

            PathsDict[key].ContinueWith(_ => request.StartTracker.DataUpdated += (_, d) =>
            {
                d.Paths.ToList().ForEach(i =>
                {
                    if (PathsDict[key].Result?.ContainsKey(i.Key) == true) PathsDict[key]!.Result[i.Key].AddRange(i.Value);
                    else PathsDict[key]!.Result[i.Key] = i.Value;
                });
            });

            MocksDict[key] = Task.Run(() => request.GetAllMocks() ?? new List<WeakReference<IAutoMock>>());
            MocksDict[key].ContinueWith(_ => request.StartTracker.DataUpdated += (_, d) =>
            {
                MocksDict[key].Result.AddRange(d.AutoMocks);
            });

            MocksByTypeDict[key] = Task.Run(() => MocksDict[key].Result
                                            .GroupBy(m => m.GetTarget()?.GetInnerType())
                                            .Where(g => g.Key is not null)
                                            .ToDictionary(d => d.Key!, d => d.ToList()));

            MocksDict[key].ContinueWith(_ => request.StartTracker.DataUpdated += (_, d) =>
            {
                d.AutoMocks.ForEach(m =>
                {
                    var t = m.GetTarget()?.GetInnerType();
                    if (t is null) return;

                    if(!MocksByTypeDict[key].Result.ContainsKey(t))
                        MocksByTypeDict[key].Result[t] = new List<WeakReference<IAutoMock>>();

                    MocksByTypeDict[key].Result[t].Add(m);
                });
            });

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
}