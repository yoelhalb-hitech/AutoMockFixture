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

    TraceInfo Trace();

    #region Freeze

    T? Freeze<T>();
    Task<T?> FreezeAsync<T>();
    object? Freeze(Type type);
    Task<object?> FreezeAsync(Type type);

    #endregion

    #region Create

    T? Create<T>();
    Task<T?> CreateAsync<T>();

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    T? Create<T>(bool callbase, AutoMockTypeControl? autoMockTypeControl = null);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    Task<T?> CreateAsync<T>(bool callbase, AutoMockTypeControl? autoMockTypeControl = null);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    object? Create(Type t, bool callbase = false, AutoMockTypeControl? autoMockTypeControl = null);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    Task<object?> CreateAsync(Type t, bool callbase = false, AutoMockTypeControl? autoMockTypeControl = null);

    #endregion

    #region AutoMockDependencies

    T? CreateWithAutoMockDependencies<T>(bool callBase = false, AutoMockTypeControl? autoMockTypeControl = null);
    Task<T?> CreateWithAutoMockDependenciesAsync<T>(bool callbase = false, AutoMockTypeControl? autoMockTypeControl = null);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    object? CreateWithAutoMockDependencies(Type t, bool callbase = false, AutoMockTypeControl? autoMockTypeControl = null);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    Task<object?> CreateWithAutoMockDependenciesAsync(Type t, bool callbase = false, AutoMockTypeControl? autoMockTypeControl = null);

    #endregion

    #region NonAutoMock

    T? CreateNonAutoMock<T>(bool callbase = false, AutoMockTypeControl? autoMockTypeControl = null);
    Task<T?> CreateNonAutoMockAsync<T>(bool callbase = false, AutoMockTypeControl? autoMockTypeControl = null);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    object? CreateNonAutoMock(Type t, bool callbase = false, AutoMockTypeControl? autoMockTypeControl = null);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    Task<object?> CreateNonAutoMockAsync(Type t, bool callbase = false, AutoMockTypeControl? autoMockTypeControl = null);

    #endregion

    #region AutoMock

    T? CreateAutoMock<T>(bool callbase = false, AutoMockTypeControl? autoMockTypeControl = null) where T : class;

    Task<T?> CreateAutoMockAsync<T>(bool callbase = false, AutoMockTypeControl? autoMockTypeControl = null) where T : class;

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    object? CreateAutoMock(Type t, bool callbase = false, AutoMockTypeControl? autoMockTypeControl = null);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    Task<object?> CreateAutoMockAsync(Type t, bool callbase = false, AutoMockTypeControl? autoMockTypeControl = null);

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
