using AutoFixture;
using AutoMockFixture.FixtureUtils;
using AutoMockFixture.FixtureUtils.Customizations;
using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Trace;

namespace AutoMockFixture;

public interface IAutoMockFixture
{
    T? Freeze<T>();
    IAutoMockFixture Customize(ICustomization customization);

    AutoMockTypeControl AutoMockTypeControl { get; set; }

    TraceInfo Trace();

    T? Create<T>(AutoMockTypeControl? autoMockTypeControl = null);
    object? CreateAutoMock(Type t, bool callBase = false, AutoMockTypeControl? autoMockTypeControl = null);
    T? CreateAutoMock<T>(bool callBase = false, AutoMockTypeControl? autoMockTypeControl = null) where T : class;

    object? CreateWithAutoMockDependencies(Type t, bool callBase = false, AutoMockTypeControl? autoMockTypeControl = null);
    T? CreateWithAutoMockDependencies<T>(bool callBase = false, AutoMockTypeControl? autoMockTypeControl = null) where T : class;

    object? CreateNonAutoMock(Type t, AutoMockTypeControl? autoMockTypeControl = null);
    T? CreateNonAutoMock<T>(AutoMockTypeControl? autoMockTypeControl = null);

    internal abstract IAutoMockHelpers AutoMockHelpers { get; }
    internal Dictionary<WeakReference, Task<Dictionary<string, List<WeakReference?>>>> PathsDict { get; }
    internal Dictionary<WeakReference, Task<Dictionary<Type, List<WeakReference<IAutoMock>>>>> MocksByTypeDict { get; }
    internal Dictionary<WeakReference, ITracker> TrackerDict { get; }
    internal Dictionary<WeakReference, Task<List<WeakReference<IAutoMock>>>> MocksDict { get; }

    internal Dictionary<object, ITracker> ProcessingTrackerDict { get; }

    internal Cache Cache { get; }

    internal MethodSetupTypes MethodSetupType { get; }
    internal List<ConstructorArgumentValue> ConstructorArgumentValues { get; }
}
