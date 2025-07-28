using AutoFixture;
using AutoMockFixture.FixtureUtils;
using AutoMockFixture.FixtureUtils.Customizations;
using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Trace;
using System.ComponentModel;

namespace AutoMockFixture;

public interface IAutoMockFixture
{
    IAutoMockFixture Customize(ICustomization customization);

    AutoMockTypeControl AutoMockTypeControl { get; set; }

    /// <summary>
    /// A list of <see cref="Type"/> for which we should setup the properties with the private getters (private setters will always be setup for non <see cref="IAutoMock.CallBase">)
    /// </summary>
    /// <remarks>Only applicable when the instance of the <see cref="Type"/> will be an <see cref="IAutoMock"/> and will not have set <see cref="IAutoMock.CallBase"></remarks>
    IList<Type> TypesToSetupPrivateGetters {  get; }

    bool? CallBase { get; set; }

    /// <summary>
    /// If the fixture should automatically register to use a sub type by considering the attributes <see cref="SequelPay.DotNetPowerExtensions.TransientAttribute" />/<see cref="SequelPay.DotNetPowerExtensions.ScopedAttribute" />/<see cref="SequelPay.DotNetPowerExtensions.SingletonAttribute" />
    /// </summary>
    bool AutoTransformBySericeAttributes { get; set; }

    TraceInfo Trace();

    #region Freeze

    /// <summary>
    /// Freezes the type <typeparamref name="T"/>, meaning anytime it encounters type <typeparamref name="T"/> it will return the same instance.
    /// </summary>
    /// <typeparam name="T">The type to freeze</typeparam>
    /// <remarks>
    /// Even when frozen it will still return different instances for differences in 1) CallBase, 2) Mocking, 3) Mocking Dependencies
    /// <br /><br />
    /// Also note that if the type <typeparamref name="T"/> is AutoMock it will only freeze mocked instances, but not non mocked instances.
    /// <br /><br />
    /// Also note that it will also freeze subtypes of the type <typeparamref name="T"/> as well as mocked versions of the type <typeparamref name="T"/> if it is a class.
    /// </remarks>
    void JustFreeze<T>();

    /// <summary>
    /// Freezes the <paramref name="type" />, meaning anytime it encounters the <paramref name="type" /> it will return the same instance.
    /// </summary>
    /// <param name="type">The type to freeze</param>
    /// <remarks>
    /// Even when frozen it will still return different instances for differences in 1) CallBase, 2) Mocking, 3) Mocking Dependencies
    /// <br /><br />
    /// Also note that if the <paramref name="type" /> parameter is AutoMock it will only freeze mocked instances, but not non mocked instances.
    /// <br /><br />
    /// Also note that it will also freeze subtypes of the <paramref name="type" /> as well as mocked versions of the type <paramref name="type" /> if it is a class.
    /// </remarks>
    void JustFreeze(Type type);

    /// <summary>
    /// Freezes the <typeparamref name="T"/>, meaning anytime it encounters type <typeparamref name="T"/> it will return the <paramref name="frozenObject" />.
    /// </summary>
    /// <typeparam name="T">The type to freeze</typeparam>
    /// <param name="frozenObject">The object to return on every encounter of type <typeparamref name="T" /></param>
    void JustFreeze<T>(T frozenObject);

    /// <summary>
    /// Freezes the <paramref name="type" />, meaning anytime it encounters the <paramref name="type" /> it will return the <paramref name="frozenObject" />.
    /// </summary>
    /// <param name="type">The type to freeze</param>
    /// <param name="frozenObject">The object to return on every encounter of type <paramref name="type" /></param>
    void JustFreeze(Type type, object? frozenObject);

    /// <inheritdoc cref="JustFreeze{T}()" />
    /// <returns>An instance of type <typeparamref name="T"/></returns>
    T? Freeze<T>();

    /// <inheritdoc cref="Freeze{T}()" />
    Task<T?> FreezeAsync<T>();

    /// <inheritdoc cref="JustFreeze(Type)" />
    /// <returns>An instance of <paramref name="type" /></returns>
    object? Freeze(Type type);

    /// <inheritdoc cref="Freeze(Type)" />
    Task<object?> FreezeAsync(Type type);

    /// <inheritdoc cref="Freeze{T}()" />
    /// <param name="callBase">If the returned object should be callBase</param>
    T? Freeze<T>(bool? callBase);

    /// <inheritdoc cref="Freeze{T}(bool?)" />/>
    Task<T?> FreezeAsync<T>(bool? callBase);

    /// <inheritdoc cref="Freeze(Type)" />
    /// <param name="callBase">If the returned object should be callBase</param>
    object? Freeze(Type type, bool? callBase);

    /// <inheritdoc cref="Freeze(Type, bool?)" />
    Task<object?> FreezeAsync(Type type, bool? callBase);

    #endregion

    #region Create

    T? Create<T>();
    Task<T?> CreateAsync<T>();

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    T? Create<T>(bool? callBase, AutoMockTypeControl? autoMockTypeControl = null);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    Task<T?> CreateAsync<T>(bool? callBase, AutoMockTypeControl? autoMockTypeControl = null);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    object? Create(Type type, bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    Task<object?> CreateAsync(Type type, bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null);

    #endregion

    #region AutoMockDependencies

    T? CreateWithAutoMockDependencies<T>(bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null);
    Task<T?> CreateWithAutoMockDependenciesAsync<T>(bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    object? CreateWithAutoMockDependencies(Type type, bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    Task<object?> CreateWithAutoMockDependenciesAsync(Type type, bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null);

    #endregion

    #region NonAutoMock

    T? CreateNonAutoMock<T>(bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null);
    Task<T?> CreateNonAutoMockAsync<T>(bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    object? CreateNonAutoMock(Type type, bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    Task<object?> CreateNonAutoMockAsync(Type type, bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null);

    #endregion

    #region AutoMock

    T? CreateAutoMock<T>(bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null) where T : class;

    Task<T?> CreateAutoMockAsync<T>(bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null) where T : class;

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    object? CreateAutoMock(Type type, bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    Task<object?> CreateAutoMockAsync(Type type, bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null);

    #endregion

    internal IAutoMockHelpers AutoMockHelpers { get; }

    // We can have multiple result at the same path such as in LazyDifferent mode or possibly generics
    internal Dictionary<WeakReference, Task<Dictionary<string, List<WeakReference?>>>> PathsDict { get; }
    internal Dictionary<WeakReference, Task<Dictionary<Type, List<WeakReference<IAutoMock>>>>> MocksByTypeDict { get; }
    internal Dictionary<WeakReference, ITracker> TrackerDict { get; }
    internal Dictionary<WeakReference, Task<List<WeakReference<IAutoMock>>>> MocksDict { get; }

    internal Dictionary<object, ITracker> ProcessingTrackerDict { get; }

    internal Cache Cache { get; }

    internal MethodSetupTypes MethodSetupType { get; }
    internal List<ConstructorArgumentValue> ConstructorArgumentValues { get; }
}
