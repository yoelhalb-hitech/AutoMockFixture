using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace AutoMockFixture;

public static class AutoMockFixtureExtensions
{
    /// <summary>
    /// Get the value at the specified path (property/field/ctor argument/out parameter/method result etc.)
    /// </summary>
    /// <param name="obj">An object created witht he current <see cref="AutoMockFixture"/></param>
    /// <param name="path">The path to get the value at</param>
    /// <returns></returns>
    /// <exception cref="Exception">Path not provided</exception>
    /// <exception cref="Exception">Object not found</exception>
    public static List<object> GetAt(this IAutoMockFixture fixture, object obj, string path)
    {
        obj = fixture.AutoMockHelpers.GetFromObj(obj) ?? obj;

        if (!fixture.PathsDict.Any(d => object.Equals(d.Key.Target, obj))) throw new Exception("Object not found, ensure that it is a root object in the current fixture, and that it is not garbage collected, and possibly verify that .Equals() works correctly on the object");
        if (string.IsNullOrWhiteSpace(path)) throw new Exception(nameof(path) + " doesn't have a value");

        path = path.Trim();

        var paths = fixture.PathsDict.First(d => object.Equals(d.Key.Target, obj)).Value.Result;
        if (paths is null || !paths.ContainsKey(path)) throw new Exception($"`{path}` not found, please ensure that it is the correct path on the correct object");

        return paths[path].GetValidValues();
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static string GetPathForObject(this IAutoMockFixture fixture, object mainObj, object currentObject)
    {
        currentObject = fixture.AutoMockHelpers.GetFromObj(currentObject) ?? currentObject;

        if (!fixture.PathsDict.Any(d => object.Equals(d.Key.Target, mainObj))) throw new Exception("Main object not found, ensure that it is a root object in the current fixture, and that it is not garbage collected, and possibly verify that .Equals() works correctly on the object");

        var path = fixture.PathsDict.First(d => object.Equals(d.Key.Target, mainObj)).Value.Result
                            .FirstOrDefault(kvp => kvp.Value.Contains(currentObject)).Key;

        if (path is null) throw new Exception("Object not found or is Garbage Collected, or .Equals() has been overriden incorrectly");

        return path;
    }
    public static object? GetSingleAt(this IAutoMockFixture fixture, object obj, string path) => fixture.GetAt(obj, path).SingleOrDefault();

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static IAutoMock GetAutoMock(this IAutoMockFixture fixture, object obj, string path)
    {
        var result = fixture.GetAt(obj, path).FirstOrDefault();
        if (result is null) throw new Exception($"Result object is null (or possibly garbage collected) and not an `{nameof(IAutoMock)}`");

        var mock = fixture.AutoMockHelpers.GetFromObj(result);

        if (mock is null) throw new Exception($"Result object is not an `{nameof(IAutoMock)}`");

        return mock;
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static bool TryGetAutoMock(this IAutoMockFixture fixture, object obj, string path, [NotNullWhen(true)] out IAutoMock? autoMock)
    {
        autoMock = null;

       // try
        {
            var result = fixture.GetAt(obj, path).FirstOrDefault();
            if (result is null) return false;

            var mock = fixture.AutoMockHelpers.GetFromObj(result);
            if (mock is null) return false;

            autoMock = mock;
        }
       // catch { return false; }
        return true;
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static IEnumerable<IAutoMock> GetAutoMocks(this IAutoMockFixture fixture, object obj, Type type)
    {
        obj = fixture.AutoMockHelpers.GetFromObj(obj) ?? obj;

        if (!fixture.MocksByTypeDict.Any(d => object.Equals(d.Key.Target, obj))) throw new Exception("Object not found, ensure that it is a root object in the current fixture, and that it is not garbage collected, and possibly verify that .Equals() works correctly on the object");

        var typeDict = fixture.MocksByTypeDict.First(d => object.Equals(d.Key.Target, obj)).Value.Result;

        return typeDict.Where(td => td.Key.IsAssignableFrom(type))
                            .SelectMany(td => td.Value.GetValidValues());
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static IAutoMock GetAutoMock(this IAutoMockFixture fixture, object obj, Type type)
            => fixture.GetAutoMocks(obj, type).SingleOrDefault() ?? throw new Exception("AutoMock not found or is garbage collected");



    /// <summary>
    /// Gets all paths already materialized for an object.
    /// </summary>
    /// <remarks>
    /// Does not include results of some method calls or some properties that were not yet called.
    /// </remarks>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static List<string> GetPaths(this IAutoMockFixture fixture, object obj)
    {
        obj = fixture.AutoMockHelpers.GetFromObj(obj) ?? obj;

        if (!fixture.PathsDict.Any(d => object.Equals(d.Key.Target, obj))) throw new Exception("Object not found, ensure that it is a root object in the current fixture that is not yet garbage collected, and possibly verify that .Equals() works correctly on the object");

        return fixture.PathsDict.First(d => object.Equals(d.Key.Target, obj)).Value.Result.Keys.ToList();
    }
}
