using AutoFixture;
using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;
using AutoMockFixture.FixtureUtils.Specifications;
using DotNetPowerExtensions.Reflection;
using static AutoMockFixture.FixtureUtils.Customizations.SubclassTransformCustomization;

namespace AutoMockFixture.FixtureUtils; // Use this namespace not to be in the main namespace (would have made it internal but then the subclasses would also have to be internal)

/// <summary>
/// CAUTION: the methods are not thread safe
/// </summary>
internal class AutoMockFixtureEngine
{
    public AutoMockFixtureEngine(AutoMockFixtureBase fixture)
    {
        this.fixture = fixture;
    }

    private AutoMockFixtureBase fixture;

    #region AutoMockDependencies

    public object? CreateWithAutoMockDependencies(Type t, bool callBase = false, AutoMockTypeControl? autoMockTypeControl = null)
    {
        if (t.IsValueType) return new SpecimenContext(fixture).Resolve(new SeededRequest(t, t.GetDefault()));

        var result = Execute(new AutoMockDependenciesRequest(t, fixture) { MockShouldCallbase = callBase }, autoMockTypeControl);

        return result;
    }

    public async Task<object?> CreateWithAutoMockDependenciesAsync(Type t, bool callBase = false, AutoMockTypeControl? autoMockTypeControl = null)
    {
        if (t.IsValueType) return new SpecimenContext(fixture).Resolve(new SeededRequest(t, t.GetDefault()));

        var result = await ExecuteAsync(new AutoMockDependenciesRequest(t, fixture) { MockShouldCallbase = callBase }, autoMockTypeControl).ConfigureAwait(false);

        return result;
    }

    public T? CreateWithAutoMockDependencies<T>(bool callBase = false, AutoMockTypeControl? autoMockTypeControl = null)
    {
        EnsureValid(typeof(T), autoMockTypeControl);
        return Cast<T>(CreateWithAutoMockDependencies(typeof(T), callBase, autoMockTypeControl));
    }

    public async Task<T?> CreateWithAutoMockDependenciesAsync<T>(bool callBase = false, AutoMockTypeControl? autoMockTypeControl = null)
    {
        EnsureValid(typeof(T), autoMockTypeControl);
        return await CastAsync<T>(CreateWithAutoMockDependenciesAsync(typeof(T), callBase, autoMockTypeControl)).ConfigureAwait(false);
    }

    #endregion

    #region NonAutoMock

    public object? CreateNonAutoMock(Type t, bool callbase = false, AutoMockTypeControl? autoMockTypeControl = null)
                => Execute(new NonAutoMockRequest(t, fixture) { MockShouldCallbase = callbase }, autoMockTypeControl);

    public async Task<object?> CreateNonAutoMockAsync(Type t, bool callbase = false, AutoMockTypeControl? autoMockTypeControl = null)
            => await ExecuteAsync(new NonAutoMockRequest(t, fixture) { MockShouldCallbase = callbase }, autoMockTypeControl).ConfigureAwait(false);

    public T? CreateNonAutoMock<T>(bool callbase = false, AutoMockTypeControl? autoMockTypeControl = null)
    {
        EnsureValid(typeof(T), autoMockTypeControl);
        return Cast<T>(CreateNonAutoMock(typeof(T), callbase, autoMockTypeControl));
    }

    public async Task<T?> CreateNonAutoMockAsync<T>(bool callbase = false, AutoMockTypeControl? autoMockTypeControl = null)
    {
        EnsureValid(typeof(T), autoMockTypeControl);
        return await CastAsync<T>(CreateNonAutoMockAsync(typeof(T), callbase, autoMockTypeControl)).ConfigureAwait(false);
    }

    #endregion

    #region AutoMock

    public object? CreateAutoMock(Type t, bool callBase = false, AutoMockTypeControl? autoMockTypeControl = null)
    {
        if (t.IsValueType) throw new InvalidOperationException("Type must be a reference type");

        var type = fixture.AutoMockHelpers.IsAutoMock(t)
                    ? fixture.AutoMockHelpers.GetMockedType(t)!
                    : !fixture.AutoMockHelpers.MockRequestSpecification.IsSatisfiedBy(t)
                        ? t
                        : t.IsGenericType ? t.GenericTypeArguments.First(): typeof(object);
        if(!fixture.AutoMockHelpers.IsAutoMockAllowed(type))
            throw new InvalidOperationException($"{type.FullName} cannot be AutoMock");

        var result = Execute(new AutoMockRequest(type, fixture.GetStartTrackerForAutoMock(type, callBase)) { MockShouldCallbase = callBase }, autoMockTypeControl);

        return type != t ? fixture.AutoMockHelpers.GetFromObj(result)! : result; // It appears that the cast operators only work when statically typed
    }

    public async Task<object?> CreateAutoMockAsync(Type t, bool callBase = false, AutoMockTypeControl? autoMockTypeControl = null)
    {
        if (t.IsValueType) throw new InvalidOperationException("Type must be a reference type");

        var type = fixture.AutoMockHelpers.IsAutoMock(t)
                    ? fixture.AutoMockHelpers.GetMockedType(t)!
                    : !fixture.AutoMockHelpers.MockRequestSpecification.IsSatisfiedBy(t)
                        ? t
                        : t.IsGenericType ? t.GenericTypeArguments.First() : typeof(object);
        if (!fixture.AutoMockHelpers.IsAutoMockAllowed(type))
            throw new InvalidOperationException($"{type.FullName} cannot be AutoMock");

        var result = await ExecuteAsync(new AutoMockRequest(type, fixture.GetStartTrackerForAutoMock(type, callBase)) { MockShouldCallbase = callBase }, autoMockTypeControl)
                                                    .ConfigureAwait(false);

        return type != t ? fixture.AutoMockHelpers.GetFromObj(result)! : result; // It appears that the cast operators only work when statically typed
    }

    public T? CreateAutoMock<T>(bool callBase = false, AutoMockTypeControl? autoMockTypeControl = null) where T : class
    {
        EnsureValid(typeof(T), autoMockTypeControl);
        return Cast<T>(CreateAutoMock(typeof(T), callBase, autoMockTypeControl));
    }

    public async Task<T?> CreateAutoMockAsync<T>(bool callBase = false, AutoMockTypeControl? autoMockTypeControl = null) where T : class
    {
        EnsureValid(typeof(T), autoMockTypeControl);
        return await CastAsync<T>(CreateAutoMockAsync(typeof(T), callBase, autoMockTypeControl)).ConfigureAwait(false);
    }

    #endregion

    #region Utils

    private void EnsureValid(Type type, AutoMockTypeControl? autoMockTypeControl = null)
    {
        if (!fixture.AutoMockHelpers.IsAutoMock(type)) return; // TODO... do we need to handle Mock?

        // If he asks for an AutoMock but we won't be able to give it then we won't be able to cast even if we create something so just stop it here

        var innerType = fixture.AutoMockHelpers.GetMockedType(type)!;

        var specification = new AutoMockableSpecification(fixture.AutoMockHelpers);

        if (!specification.IsSatisfiedBy(innerType))
            throw new InvalidOperationException(
                $"The request for AutoMock on type `{innerType.Name}` cannot be fulfiled as it is not Mockable by the rules of AutoMock");

        if (autoMockTypeControl?.NeverAutoMockTypes.Contains(innerType) == true || fixture.AutoMockTypeControl.NeverAutoMockTypes.Contains(innerType) == true)
            throw new InvalidOperationException(
                $"The request for AutoMock on type `{innerType.Name}` cannot be fulfiled as it is not Mockable by the rules of the provided `{nameof(AutoMockTypeControl)}`");

    }

    private T? Cast<T>(object? result)
    {
        try
        {
            return (T?)result;
        }
        catch (InvalidCastException ex) when (fixture.AutoMockHelpers.IsAutoMock(typeof(T)))
        {
            var innerType = fixture.AutoMockHelpers.GetMockedType(typeof(T))!;
            var genericDefinition = innerType.IsGenericType ? innerType.GetGenericTypeDefinition() : null;

            var cust = fixture.Customizations
                    .OfType<SubClassTransformBuilder>()
                    .FirstOrDefault(c => c.OriginalType.IsGenericType && c.OriginalType.IsGenericTypeDefinition ? c.OriginalType == genericDefinition : c.OriginalType == innerType);
            if (cust is null) throw;

            throw new InvalidCastException($"Requested type {typeof(T).ToGenericTypeString()} has been modified to {(result?.GetType() ?? fixture.AutoMockHelpers.GetMockedType(cust.SubclassType))!.ToGenericTypeString()}\nYou can use instead the non generic overload or you can try the `CreateAutoMock()` method instead", ex);
        }
    }

    private async Task<T?> CastAsync<T>(Task<object?> result)
    {
        try
        {
            return (T?)await result.ConfigureAwait(false);
        }
        catch (InvalidCastException ex) when (fixture.AutoMockHelpers.IsAutoMock(typeof(T)))
        {
            var innerType = fixture.AutoMockHelpers.GetMockedType(typeof(T))!;
            var genericDefinition = innerType.IsGenericType ? innerType.GetGenericTypeDefinition() : null;

            var cust = fixture.Customizations
                    .OfType<SubClassTransformBuilder>()
                    .FirstOrDefault(c => c.OriginalType.IsGenericType && c.OriginalType.IsGenericTypeDefinition ? c.OriginalType == genericDefinition : c.OriginalType == innerType);
            if (cust is null) throw;

            throw new InvalidCastException($"Requested type {typeof(T).ToGenericTypeString()} has been modified to {(result?.GetType() ?? fixture.AutoMockHelpers.GetMockedType(cust.SubclassType))!.ToGenericTypeString()}\nYou can use instead the non generic overload or you can try the `CreateAutoMock()` method instead", ex);
        }

    }

    #endregion

    #region Executer

    internal ITracker? GetTracker(object obj) => TrackerDict
                    .SingleOrDefault(t => t.Key.IsAlive && object.Equals(t.Key.Target, (fixture.AutoMockHelpers.GetFromObj(obj) ?? obj)))
                    .Value;

    internal Dictionary<WeakReference, Task<Dictionary<string, List<WeakReference?>>>> PathsDict = new();

    internal Dictionary<WeakReference, Task<List<WeakReference<IAutoMock>>>> MocksDict = new();
    internal Dictionary<WeakReference, Task<Dictionary<Type, List<WeakReference<IAutoMock>>>>> MocksByTypeDict = new();

    internal Dictionary<WeakReference, ITracker> TrackerDict = new();

    internal Dictionary<object, ITracker> ProcessingTrackerDict = new(); // To track while processing

    private object? Execute(ITracker request, AutoMockTypeControl? autoMockTypeControl = null)
    {
        var (result, task) = ExecuteInternal(request, autoMockTypeControl);

        task?.Wait();

        return result;
    }

    private async Task<object?> ExecuteAsync(ITracker request, AutoMockTypeControl? autoMockTypeControl = null)
    {
        var (result, task) = ExecuteInternal(request, autoMockTypeControl);

        if(task is not null) await task.ConfigureAwait(false);

        return result;
    }

    private (object?, Task?) ExecuteInternal(ITracker request, AutoMockTypeControl? autoMockTypeControl = null)
    {
        try
        {
            var result = new RecursionContext(fixture, fixture) { AutoMockTypeControl = autoMockTypeControl }.Resolve(request);
            if (result == this) return (result, null);

            // TODO... we might have a problem if there is duplicates (for example for primitive typs or strings)

            // We will rather deal with the underlying mock for consistancy
            // but also to avoid having to call .Equals on the object which it can then later report as called in the verify process
            // WeakReference won't block Garbage Collection, and also avoids the issue of duplicates
            var key = (fixture.AutoMockHelpers.GetFromObj(result) ?? result).ToWeakReference();
            TrackerDict[key] = request;

            if (PathsDict.Any(m => object.Equals(m.Key.Target, key.Target))) return (result, null); // Probably from cache, CAUTION: referencing directly the object in the expression prevents if from GC, only when referencing via the wekreference target

            PathsDict[key] = Task.Run(() => request.GetChildrensPaths()?
                        .ToDictionary(c => c.Key, c => c.Value)
                    ?? new Dictionary<string, List<WeakReference?>>());

            PathsDict[key].ContinueWith(_ => request.StartTracker.DataUpdated += (_, d) => // For any lazy children
            {
                d.Paths.ToList().ForEach(i =>
                {
                    if (PathsDict[key].Result?.ContainsKey(i.Key) == true) PathsDict[key]!.Result[i.Key].AddRange(i.Value);
                    else PathsDict[key]!.Result[i.Key] = i.Value;
                });
            });


            MocksDict[key] = Task.Run(() => request.GetAllMocks()?
                                                    .Select(w => w.GetTarget()).OfType<IAutoMock>()
                                                    .Distinct() // Remember that for wrappers we resuse the child result
                                                    .Select(m => m.ToWeakReference()).ToList() ?? new List<WeakReference<IAutoMock>>());
            MocksDict[key].ContinueWith(_ =>  // For any lazy children
            {
                var obj = new object();
                request.StartTracker.DataUpdated += (_, d) =>
                {
                    var mocks = d.AutoMocks.Select(w => w.GetTarget()).OfType<IAutoMock>().Distinct();  // Remember that for wrappers we resuse the child result
                    lock (obj)
                    {
                        var existing = MocksDict[key].Result.Select(r => r.GetTarget()).OfType<IAutoMock>().ToList();
                        MocksDict[key].Result.AddRange(mocks.Where(m => !existing.Contains(m)).Select(m => m.ToWeakReference()));
                    }
                };
            });

            MocksByTypeDict[key] = Task.Run(() => MocksDict[key].Result
                                            .GroupBy(m => m.GetTarget()?.GetInnerType())
                                            .Where(g => g.Key is not null)
                                            .ToDictionary(d => d.Key!, d => d.ToList()));
            MocksByTypeDict[key].ContinueWith(_ => // For any lazy children
            {
                var obj = new object();
                request.StartTracker.DataUpdated += (_, d) =>
                {
                    lock(obj)
                    {
                        var dict = new Dictionary<Type, List<IAutoMock>>();
                        d.AutoMocks.ForEach(m =>
                        {
                            var target = m.GetTarget(); // This way it will at least live till after the block so it won't be GC'ed
                            var t = target?.GetInnerType();
                            if (target is null || t is null) return;

                            if (!MocksByTypeDict[key].Result.ContainsKey(t))
                                MocksByTypeDict[key].Result[t] = new List<WeakReference<IAutoMock>>();

                            if (!dict.ContainsKey(t)) dict[t] = MocksByTypeDict[key].Result[t].Select(r => r.GetTarget()).OfType<IAutoMock>().ToList();

                            if (dict[t].Contains(target)) return;

                            dict[t].Add(target);
                            MocksByTypeDict[key].Result[t].Add(m);
                        });
                    }
                };
            });

            return (result, MocksByTypeDict[key]);
        }
        catch (ObjectCreationException)
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
