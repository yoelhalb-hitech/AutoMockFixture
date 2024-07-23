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

    bool CallBase { get; set; }

    TraceInfo Trace();

    #region Freeze

    void JustFreeze<T>();
    void JustFreeze(Type type);

    T? Freeze<T>();
    Task<T?> FreezeAsync<T>();
    object? Freeze(Type type);
    Task<object?> FreezeAsync(Type type);

    #endregion

    #region Create

    T? Create<T>();
    Task<T?> CreateAsync<T>();

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    T? Create<T>(bool? callBase, AutoMockTypeControl? autoMockTypeControl = null);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    Task<T?> CreateAsync<T>(bool? callBase, AutoMockTypeControl? autoMockTypeControl = null);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    object? Create(Type t, bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    Task<object?> CreateAsync(Type t, bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null);

    #endregion

    #region AutoMockDependencies

    T? CreateWithAutoMockDependencies<T>(bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null);
    Task<T?> CreateWithAutoMockDependenciesAsync<T>(bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    object? CreateWithAutoMockDependencies(Type t, bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    Task<object?> CreateWithAutoMockDependenciesAsync(Type t, bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null);

    #endregion

    #region NonAutoMock

    T? CreateNonAutoMock<T>(bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null);
    Task<T?> CreateNonAutoMockAsync<T>(bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    object? CreateNonAutoMock(Type t, bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    Task<object?> CreateNonAutoMockAsync(Type t, bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null);

    #endregion

    #region AutoMock

    T? CreateAutoMock<T>(bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null) where T : class;

    Task<T?> CreateAutoMockAsync<T>(bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null) where T : class;

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    object? CreateAutoMock(Type t, bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    Task<object?> CreateAutoMockAsync(Type t, bool? callBase = null, AutoMockTypeControl? autoMockTypeControl = null);

    #endregion

    internal IAutoMockHelpers AutoMockHelpers { get; }
    internal Dictionary<WeakReference, Task<Dictionary<string, List<WeakReference?>>>> PathsDict { get; }
    internal Dictionary<WeakReference, Task<Dictionary<Type, List<WeakReference<IAutoMock>>>>> MocksByTypeDict { get; }
    internal Dictionary<WeakReference, ITracker> TrackerDict { get; }
    internal Dictionary<WeakReference, Task<List<WeakReference<IAutoMock>>>> MocksDict { get; }

    internal Dictionary<object, ITracker> ProcessingTrackerDict { get; }

    internal Cache Cache { get; }

    internal MethodSetupTypes MethodSetupType { get; }
    internal List<ConstructorArgumentValue> ConstructorArgumentValues { get; }
}
