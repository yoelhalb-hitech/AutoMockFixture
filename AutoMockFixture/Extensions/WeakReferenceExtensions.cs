
namespace AutoMockFixture.Extensions;

internal static class WeakReferenceExtensions
{
    public static List<WeakReference> ToWeakReferences(this List<object?> list)
            => list.Select(l => new WeakReference(l, true)).ToList();
    public static List<WeakReference<T>> ToWeakReferences<T>(this List<T> list) where T : class
            => list.Select(l => new WeakReference<T>(l, true)).ToList();

    public static WeakReference ToWeakReference(this object? obj)
                            => new WeakReference(obj, true);

    public static WeakReference<T> ToWeakReference<T>(this T obj) where T : class
                                    => new WeakReference<T>(obj, true);

    public static T? GetTarget<T>(this WeakReference<T> weakReference) where T : class
    {
        weakReference.TryGetTarget(out var target);
        return target;
    }

    public static List<object?> GetValues(this List<WeakReference> list)
        // l.Target is actually of type `object?` not `object`
        => list.Select(l => (object?)l.Target).ToList();
    public static List<T?> GetValues<T>(this List<WeakReference<T>> list) where T : class
        => list.Select(l => l.GetTarget()).ToList();
    public static IEnumerable<T?> GetValues<T>(this IEnumerable<WeakReference<T>> list) where T : class
        => list.Select(l => l.GetTarget());
    public static List<object> GetValidValues(this List<WeakReference?> list)
        // l.Target is actually of type `object?` not `object`
        => list.Where(l => l is not null).Select(l => (object?)(l!.Target))
                .Where(l => l is not null).Select(l => l!)
                .ToList();
    public static List<T> GetValidValues<T>(this List<WeakReference<T>> list) where T : class
        => list.Select(l => l.GetTarget()).Where(l => l is not null).Select(l => l!).ToList();
    public static IEnumerable<T> GetValidValues<T>(this IEnumerable<WeakReference<T>> list) where T : class
        => list.Select(l => l.GetTarget()).Where(l => l is not null).Select(l => l!);
}
