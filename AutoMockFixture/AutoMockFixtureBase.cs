using AutoFixture;
using AutoMockFixture.FixtureUtils.Builders.HelperBuilders;
using AutoMockFixture.FixtureUtils.Builders.SpecialBuilders;
using AutoMockFixture.FixtureUtils.Commands;
using AutoMockFixture.FixtureUtils.Customizations;
using AutoMockFixture.FixtureUtils.Postprocessors;
using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Specifications;
using AutoMockFixture.FixtureUtils.Trace;
using DotNetPowerExtensions.Reflection;
using SequelPay.DotNetPowerExtensions;
using System.ComponentModel;

namespace AutoMockFixture.FixtureUtils; // Use this namespace not to be in the main namespace (would have made it internal but then the subclasses would also have to be internal)

/// <summary>
/// CAUTION: the methods are not thread safe
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public abstract partial class AutoMockFixtureBase : Fixture, ISpecimenBuilder, IAutoMockFixture, IDisposable
{
    static AutoMockFixtureBase()
    {
        replaceNodeMethod = typeof(SpecimenBuilderNode)
            .GetMethod("ReplaceNodes", BindingFlagsExtensions.AllBindings, null, new Type[]
            {
                typeof(ISpecimenBuilderNode),
                typeof(ISpecimenBuilderNode),
                typeof(Func<ISpecimenBuilderNode, bool>),
            }, null);

        graphField = typeof(Fixture).GetField("graph", BindingFlagsExtensions.AllBindings);

        updateGraphAndSetupAdapterMethod = typeof(Fixture).GetMethod("UpdateGraphAndSetupAdapters", BindingFlagsExtensions.AllBindings, null, new Type[]
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
                                        new CompositeSpecimenCommand(
                                            new CustomAutoPropertiesCommand(this),
                                            new PopulateEnumerableCommand(this.AutoMockHelpers, this, this.RepeatCount)),
                                        new AnyTypeSpecification(),
                                        new CacheCommand(this.Cache)));

        var currentGraph = graphField?.GetValue(this);
        Func<ISpecimenBuilderNode, bool> matcher = node => node is AutoPropertiesTarget;
        var newGraph = replaceNodeMethod?.Invoke(null, new[] { currentGraph, newAutoProperties, matcher }) as ISpecimenBuilderNode;
        updateGraphAndSetupAdapterMethod?.Invoke(this, new[] { newGraph });

        Customizations.Add(new FilteringSpecimenBuilder(
                                new FixedBuilder(this),
                                new OrRequestSpecification(
                                    new TypeOrRequestSpecification(new ExactTypeSpecification(typeof(AutoMockFixtureBase)), AutoMockHelpers),
                                    new TypeOrRequestSpecification(new ExactTypeSpecification(typeof(IAutoMockFixture)), AutoMockHelpers),
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

        AutoMockEngine = new AutoMockFixtureEngine(this);
    }

    public AutoMockTypeControl AutoMockTypeControl { get; set; } = new AutoMockTypeControl();
    internal virtual MethodSetupTypes MethodSetupType { get; set; } = MethodSetupTypes.LazySame;

    /// <summary>
    /// A list of <see cref="Type"/> for which we should setup the properties with the private getters (private setters will always be setup for non <see cref="IAutoMock.CallBase">)
    /// </summary>
    /// <remarks>Only applicable when the instance of the <see cref="Type"/> will be an <see cref="IAutoMock"/> and will not have set <see cref="IAutoMock.CallBase"></remarks>    /// <remarks>Only applicable when the instance of the <see cref="Type"/> will be an <see cref="IAutoMock"/> and will not have set <see cref="IAutoMock.CallBase"></remarks>
    public IList<Type> TypesToSetupPrivateGetters { get; } = new List<Type>();

    #region Freeze

    public virtual void JustFreeze<T>() => JustFreeze(typeof(T));
    public virtual void JustFreeze(Type type)
            => Customize(new FreezeCustomization(new TypeOrRequestSpecification(new ExactTypeSpecification(type), AutoMockHelpers)));

    public virtual T? Freeze<T>()
    {
        JustFreeze<T>();

        return Create<T>();
    }

    public virtual object? Freeze(Type type)
    {
        JustFreeze(type);

        return Create(type);
    }

    public virtual async Task<T?> FreezeAsync<T>()
    {
        JustFreeze<T>();

        return await CreateAsync<T>().ConfigureAwait(false);
    }

    public virtual async Task<object?> FreezeAsync(Type type)
    {
        JustFreeze(type);

        return await CreateAsync(type).ConfigureAwait(false);
    }

    #endregion

    #region Create

    public T? Create<T>() => Create<T>(false, null);
    public Task<T?> CreateAsync<T>() => CreateAsync<T>(false, null);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public abstract T? Create<T>(bool callBase, AutoMockTypeControl? autoMockTypeControl = null);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public abstract Task<T?> CreateAsync<T>(bool callBase, AutoMockTypeControl? autoMockTypeControl = null);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public abstract object? Create(Type t, bool callBase = false, AutoMockTypeControl? autoMockTypeControl = null);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public abstract Task<object?> CreateAsync(Type t, bool callBase = false, AutoMockTypeControl? autoMockTypeControl = null);

    #endregion

    #region AutoMockDependencies

    public T? CreateWithAutoMockDependencies<T>(bool callBase = false, AutoMockTypeControl? autoMockTypeControl = null)
            => AutoMockEngine.CreateWithAutoMockDependencies<T>(callBase, autoMockTypeControl);

    public Task<T?> CreateWithAutoMockDependenciesAsync<T>(bool callBase = false, AutoMockTypeControl? autoMockTypeControl = null)
        => AutoMockEngine.CreateWithAutoMockDependenciesAsync<T>(callBase, autoMockTypeControl);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public object? CreateWithAutoMockDependencies(Type t, bool callBase = false, AutoMockTypeControl? autoMockTypeControl = null)
        => AutoMockEngine.CreateWithAutoMockDependencies(t, callBase, autoMockTypeControl);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public Task<object?> CreateWithAutoMockDependenciesAsync(Type t, bool callBase = false, AutoMockTypeControl? autoMockTypeControl = null)
        => AutoMockEngine.CreateWithAutoMockDependenciesAsync(t, callBase, autoMockTypeControl);

    #endregion

    #region NonAutoMock

    public T? CreateNonAutoMock<T>(bool callBase = false, AutoMockTypeControl? autoMockTypeControl = null)
        => AutoMockEngine.CreateNonAutoMock<T>(callBase, autoMockTypeControl);

    public Task<T?> CreateNonAutoMockAsync<T>(bool callBase = false, AutoMockTypeControl? autoMockTypeControl = null)
        => AutoMockEngine.CreateNonAutoMockAsync<T>(callBase, autoMockTypeControl);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public object? CreateNonAutoMock(Type t, bool callBase = false, AutoMockTypeControl? autoMockTypeControl = null)
                => AutoMockEngine.CreateNonAutoMock(t, callBase, autoMockTypeControl);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public Task<object?> CreateNonAutoMockAsync(Type t, bool callBase = false, AutoMockTypeControl? autoMockTypeControl = null)
            => AutoMockEngine.CreateNonAutoMockAsync(t, callBase, autoMockTypeControl);

    #endregion

    #region AutoMock

    public T? CreateAutoMock<T>(bool callBase = false, AutoMockTypeControl? autoMockTypeControl = null) where T : class
            => AutoMockEngine.CreateAutoMock<T>(callBase, autoMockTypeControl);

    public Task<T?> CreateAutoMockAsync<T>(bool callBase = false, AutoMockTypeControl? autoMockTypeControl = null) where T : class
            => AutoMockEngine.CreateAutoMockAsync<T>(callBase, autoMockTypeControl);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public object? CreateAutoMock(Type t, bool callBase = false, AutoMockTypeControl? autoMockTypeControl = null)
        => AutoMockEngine.CreateAutoMock(t, callBase, autoMockTypeControl);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public Task<object?> CreateAutoMockAsync(Type t, bool callBase = false, AutoMockTypeControl? autoMockTypeControl = null)
        => AutoMockEngine.CreateAutoMockAsync(t, callBase, autoMockTypeControl);

    #endregion

    #region Utils

    private readonly static MethodInfo? replaceNodeMethod;
    private readonly static FieldInfo? graphField;
    private readonly static MethodInfo? updateGraphAndSetupAdapterMethod;


    private protected AutoMockFixtureEngine AutoMockEngine;

    internal abstract TrackerWithFixture GetStartTrackerForAutoMock(Type type, bool callBase);

    internal abstract IAutoMockHelpers AutoMockHelpers { get; }

    internal List<ConstructorArgumentValue> ConstructorArgumentValues = new();

    internal ITracker? GetTracker(object obj) => AutoMockEngine.TrackerDict
                    .SingleOrDefault(t => t.Key.IsAlive && t.Key.Target == (AutoMockHelpers.GetFromObj(obj) ?? obj))
                    .Value;

    internal Cache Cache { get; } = new Cache();
    CacheBuilder? cacheBuilder;
    AutoMockTypeControlBuilder? autoMockTypeControlBuilder;

    private List<WeakReference> disposables = new List<WeakReference>();

    #endregion

    #region InterfaceImplementations

    MethodSetupTypes IAutoMockFixture.MethodSetupType => MethodSetupType;
    Cache IAutoMockFixture.Cache => Cache;
    IAutoMockFixture IAutoMockFixture.Customize(AutoFixture.ICustomization customization) => (IAutoMockFixture)Customize(customization);

    object? ISpecimenBuilder.Create(object request, AutoFixture.Kernel.ISpecimenContext context)
    {
        if (autoMockTypeControlBuilder is null) autoMockTypeControlBuilder = new AutoMockTypeControlBuilder();
        if (cacheBuilder is null) cacheBuilder = new CacheBuilder(Cache);

        var result = autoMockTypeControlBuilder.Create(request, context); // We do it here so to ensure that it will always run the first thing

        if (result is NoSpecimen) result = cacheBuilder.Create(request, context); // Same

        if (result is NoSpecimen) result = base.Create(request, context);

        if (result?.GetType().GetInterfaces().Contains(typeof(IDisposable)) == true) disposables.Add(new WeakReference(result));

        return result;
    }

    public void Dispose()
    {
        foreach (var disposableRef in disposables.Where(d => d.IsAlive))
        {
            try { (disposableRef?.Target as IDisposable)?.Dispose(); } catch { }
        }

        foreach (var customization in Customizations.Where(c => c?.GetType().GetInterfaces().Contains(typeof(IDisposable)) == true))
        {
            try { (customization as IDisposable)?.Dispose(); } catch { }
        }
    }

    public virtual TraceInfo Trace()
    {
        var info = new TraceInfo();
        Behaviors.Add(new TraceBehavior(info));

        return info;
    }

    IAutoMockHelpers IAutoMockFixture.AutoMockHelpers => AutoMockHelpers;

    List<ConstructorArgumentValue> IAutoMockFixture.ConstructorArgumentValues => ConstructorArgumentValues;

    Dictionary<WeakReference, Task<Dictionary<string, List<WeakReference?>>>> IAutoMockFixture.PathsDict => AutoMockEngine.PathsDict;

    Dictionary<WeakReference, Task<List<WeakReference<IAutoMock>>>> IAutoMockFixture.MocksDict => AutoMockEngine.MocksDict;
    Dictionary<WeakReference, Task<Dictionary<Type, List<WeakReference<IAutoMock>>>>> IAutoMockFixture.MocksByTypeDict => AutoMockEngine.MocksByTypeDict;
    Dictionary<WeakReference, ITracker> IAutoMockFixture.TrackerDict => AutoMockEngine.TrackerDict;
    Dictionary<object, ITracker> IAutoMockFixture.ProcessingTrackerDict => AutoMockEngine.ProcessingTrackerDict;

    #endregion
}
