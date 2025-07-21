using AutoFixture;
using AutoMockFixture.FixtureUtils.Builders.HelperBuilders;
using AutoMockFixture.FixtureUtils.Builders.SpecialBuilders;
using AutoMockFixture.FixtureUtils.Commands;
using AutoMockFixture.FixtureUtils.Customizations;
using AutoMockFixture.FixtureUtils.Postprocessors;
using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Specifications;
using AutoMockFixture.FixtureUtils.Trace;
using SequelPay.DotNetPowerExtensions;
using SequelPay.DotNetPowerExtensions.Reflection;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
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
        Cache = new Cache(this);

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
                                    new TypeOrRequestSpecification(new ExactTypeSpecification(this.GetType()), AutoMockHelpers),
                                    new TypeOrRequestSpecification(new ExactTypeSpecification(typeof(AutoMockFixtureBase)), AutoMockHelpers),
                                    new TypeOrRequestSpecification(new ExactTypeSpecification(typeof(IAutoMockFixture)), AutoMockHelpers),
                                    new TypeOrRequestSpecification(new ExactTypeSpecification(typeof(Fixture)), AutoMockHelpers),
                                    new TypeOrRequestSpecification(new ExactTypeSpecification(typeof(IFixture)), AutoMockHelpers),
                                    new TypeOrRequestSpecification(new ExactTypeSpecification(typeof(ISpecimenBuilder)), AutoMockHelpers))));

        Customize(new SubclassOpenGenericCustomization<IList<object>, List<object>>());
        Customize(new SubclassOpenGenericCustomization<IDictionary<object, object>, Dictionary<object, object>>());
        Customize(new SubclassOpenGenericCustomization<ICollection<object>, Collection<object>>());
        Customize(new SubclassOpenGenericCustomization<ISet<object>, HashSet<object>>());

        if (Type.GetType("System.Data.Objects.IObjectSet`1") is var iobjectSet && iobjectSet is not null)
        {
            try
            {
                Customize(new SubclassTransformCustomization(iobjectSet!, Type.GetType("System.Data.Objects.ObjectSet`1")!));
            }
            catch { }
        }

        Customize(new SubclassOpenGenericCustomization<IProducerConsumerCollection<object>, ConcurrentBag<object>>());

        Customize(new SubclassOpenGenericCustomization<IReadOnlyCollection<object>, ReadOnlyCollection<object>>());
        Customize(new SubclassOpenGenericCustomization<IReadOnlyList<object>, ReadOnlyCollection<object>>());
        Customize(new SubclassOpenGenericCustomization<IReadOnlyDictionary<object, object>, ReadOnlyDictionary<object, object>>());

        if (Type.GetType("System.Collections.Immutable.IImmutableList`1") is not null)
        {
            try
            {
                var t = (string s) => Type.GetType("System.Collections.Immutable." + s);
                Customize(new SubclassTransformCustomization(t("IImmutableList`1"), t("ImmutableList`1")));
                Customize(new SubclassTransformCustomization(t("IImmutableSet`1"), t("ImmutableHashSet`1")));
                Customize(new SubclassTransformCustomization(t("IImmutableStack`1"), t("ImmutableStack`1")));
                Customize(new SubclassTransformCustomization(t("IImmutableQueue`1"), t("IImmutableQueue`1")));
                Customize(new SubclassTransformCustomization(t("IImmutableDictionary`2"), t("ImmutableDictionary`2")));
            }
            catch { }
        }

#if NET5_0_OR_GREATER
        try
        {
            Customize(new SubclassOpenGenericCustomization<IReadOnlySet<object>, HashSet<object>>());
        }
        catch { }
#endif

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
    /// Fixture wide default callBase setting, can be overriden on the individual request level
    /// </summary>
    public virtual bool? CallBase { get; set; }

    /// <summary>
    /// If the fixture should automatically register to use a sub type by considering the attributes <see cref="SequelPay.DotNetPowerExtensions.TransientAttribute" />/<see cref="SequelPay.DotNetPowerExtensions.ScopedAttribute" />/<see cref="SequelPay.DotNetPowerExtensions.SingletonAttribute" />
    /// </summary>
    public bool AutoTransformBySericeAttributes
    {
        get => autoTransformBySericeAttributes;
        set
        {
            autoTransformBySericeAttributes = value;

            if (value) servicesCustomization.Customize(this);
            else servicesCustomization.RemoveCustomization(this);
        }
    }
    private bool autoTransformBySericeAttributes { get; set; }
    private ServicesCustomization servicesCustomization = new ();

    /// <summary>
    /// A list of <see cref="Type"/> for which we should setup the properties with the private getters (private setters will always be setup for non <see cref="IAutoMock.CallBase">)
    /// </summary>
    /// <remarks>Only applicable when the instance of the <see cref="Type"/> will be an <see cref="IAutoMock"/> and will not have set <see cref="IAutoMock.CallBase"></remarks>    /// <remarks>Only applicable when the instance of the <see cref="Type"/> will be an <see cref="IAutoMock"/> and will not have set <see cref="IAutoMock.CallBase"></remarks>
    public IList<Type> TypesToSetupPrivateGetters { get; } = new List<Type>();

    #region Freeze

    public virtual void JustFreeze<T>() => JustFreeze(typeof(T));
    public virtual void JustFreeze(Type type)
            => Customize(new FreezeCustomization(new TypeOrRequestSpecification(new TypeSpecification(type, this.AutoMockHelpers), AutoMockHelpers)));

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

    public T? Create<T>() => Create<T>(null, null);
    public Task<T?> CreateAsync<T>() => CreateAsync<T>(null, null);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public abstract T? Create<T>(bool? callBase, AutoMockTypeControl? autoMockTypeControl = null);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public abstract Task<T?> CreateAsync<T>(bool? callBase, AutoMockTypeControl? autoMockTypeControl = null);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public abstract object? Create(Type type, bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public abstract Task<object?> CreateAsync(Type type, bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null);

    #endregion

    #region AutoMockDependencies

    /// <summary>
    /// Get an object of type <typeparamref name="T"/> where all the properties/fields/ctor args will be mocked automatically
    /// </summary>
    /// <typeparam name="T">The type of object to return</typeparam>
    /// <param name="callBase">Whether the mocks should callBase, will override the fixture wide <see cref="IAutoMockFixture.CallBase"/> if specified</param>
    /// <param name="autoMockTypeControl">A <see cref="FixtureUtils.AutoMockTypeControl"/> instance, this has a higher priority then the fixture wide <see cref="AutoMockTypeControl"/></param>
    /// <remarks>When <paramref name="callBase"/> is false any mock will not have any ctor args provided, instead a newely created default ctor will be called</remarks>
    /// <returns>A non mocked object of <typeparamref name="T"/> (unless <typeparamref name="T"/> is itself <see cref="IAutoMock"/>) with all properties/field/ctor args mocked</returns>
    public T? CreateWithAutoMockDependencies<T>(bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null)
            => AutoMockEngine.CreateWithAutoMockDependencies<T>(callBase, autoMockTypeControl);

    /// <inheritdoc cref="CreateWithAutoMockDependencies{T}(bool?, AutoMockTypeControl?)" />
    public Task<T?> CreateWithAutoMockDependenciesAsync<T>(bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null)
        => AutoMockEngine.CreateWithAutoMockDependenciesAsync<T>(callBase, autoMockTypeControl);

    /// <summary>
    /// Get an object of type <paramref name="type"/> where all the properties/field/ctor args will be mocked automatically
    /// </summary>
    /// <param name="type">The type of object to return</typeparam>
    /// <returns>A non mocked object of <paramref name="type"/> (unless <paramref name="type"/> is itself <see cref="IAutoMock"/>) with all properties/field/ctor args mocked</returns>
    /// <inheritdoc cref="CreateWithAutoMockDependencies{T}(bool?, AutoMockTypeControl?)" />
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public object? CreateWithAutoMockDependencies(Type type, bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null)
        => AutoMockEngine.CreateWithAutoMockDependencies(type, callBase, autoMockTypeControl);

    /// <inheritdoc cref="CreateWithAutoMockDependencies(Type, bool?, AutoMockTypeControl?)" />
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public Task<object?> CreateWithAutoMockDependenciesAsync(Type type, bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null)
        => AutoMockEngine.CreateWithAutoMockDependenciesAsync(type, callBase, autoMockTypeControl);

    #endregion

    #region NonAutoMock

    /// <summary>
    /// Get an object of type <typeparamref name="T"/> where all the properties/fields/ctor args are not mocks (unless the type is abstract/interface or has explictly been requested to be a mock via <paramref name="autoMockTypeControl"/>)
    /// </summary>
    /// <returns>A non mocked object of <typeparamref name="T"/> (unless <typeparamref name="T"/> is itself <see cref="IAutoMock"/>)</returns>
    /// <inheritdoc cref="CreateWithAutoMockDependencies{T}(bool?, AutoMockTypeControl?)" />
    public T? CreateNonAutoMock<T>(bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null)
        => AutoMockEngine.CreateNonAutoMock<T>(callBase, autoMockTypeControl);

    /// <inheritdoc cref="CreateNonAutoMock{T}(bool?, AutoMockTypeControl?)" />
    public Task<T?> CreateNonAutoMockAsync<T>(bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null)
        => AutoMockEngine.CreateNonAutoMockAsync<T>(callBase, autoMockTypeControl);

    /// <summary>
    /// Get an object of type <paramref name="type"/> where all the properties/fields/ctor args are not mocks (unless the type is abstract/interface or has explictly been requested to be a mock via <paramref name="autoMockTypeControl"/>)
    /// </summary>
    /// <param name="type">The type of object to return</typeparam>
    /// <returns>A non mocked object of <paramref name="type"/> (unless <typeparamref name="type"/> is itself <see cref="IAutoMock"/>)</returns>
    /// <inheritdoc cref="CreateNonAutoMock{T}(bool?, AutoMockTypeControl?)" />
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public object? CreateNonAutoMock(Type type, bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null)
                => AutoMockEngine.CreateNonAutoMock(type, callBase, autoMockTypeControl);

    /// <inheritdoc cref="CreateNonAutoMock(Type, bool?, AutoMockTypeControl?)" />
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public Task<object?> CreateNonAutoMockAsync(Type type, bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null)
            => AutoMockEngine.CreateNonAutoMockAsync(type, callBase, autoMockTypeControl);

    #endregion

    #region AutoMock

    public T? CreateAutoMock<T>(bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null) where T : class
            => AutoMockEngine.CreateAutoMock<T>(callBase, autoMockTypeControl);

    public Task<T?> CreateAutoMockAsync<T>(bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null) where T : class
            => AutoMockEngine.CreateAutoMockAsync<T>(callBase, autoMockTypeControl);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public object? CreateAutoMock(Type type, bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null)
        => AutoMockEngine.CreateAutoMock(type, callBase, autoMockTypeControl);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public Task<object?> CreateAutoMockAsync(Type type, bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null)
        => AutoMockEngine.CreateAutoMockAsync(type, callBase, autoMockTypeControl);

    #endregion

    #region Utils

    private readonly static MethodInfo? replaceNodeMethod;
    private readonly static FieldInfo? graphField;
    private readonly static MethodInfo? updateGraphAndSetupAdapterMethod;


    private protected AutoMockFixtureEngine AutoMockEngine;

    internal abstract TrackerWithFixture GetStartTrackerForAutoMock(Type type, bool? callBase);

    internal abstract IAutoMockHelpers AutoMockHelpers { get; }

    internal List<ConstructorArgumentValue> ConstructorArgumentValues = new();

    internal ITracker? GetTracker(object obj) => AutoMockEngine.TrackerDict
                    .SingleOrDefault(t => t.Key.IsAlive && t.Key.Target == (AutoMockHelpers.GetFromObj(obj) ?? obj))
                    .Value;

    internal virtual Cache Cache { get; }
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

    // We can have multiple result at the same path such as in LazyDifferent mode or possibly generics
    Dictionary<WeakReference, Task<Dictionary<string, List<WeakReference?>>>> IAutoMockFixture.PathsDict => AutoMockEngine.PathsDict;

    Dictionary<WeakReference, Task<List<WeakReference<IAutoMock>>>> IAutoMockFixture.MocksDict => AutoMockEngine.MocksDict;
    Dictionary<WeakReference, Task<Dictionary<Type, List<WeakReference<IAutoMock>>>>> IAutoMockFixture.MocksByTypeDict => AutoMockEngine.MocksByTypeDict;
    Dictionary<WeakReference, ITracker> IAutoMockFixture.TrackerDict => AutoMockEngine.TrackerDict;
    Dictionary<object, ITracker> IAutoMockFixture.ProcessingTrackerDict => AutoMockEngine.ProcessingTrackerDict;

    #endregion
}
