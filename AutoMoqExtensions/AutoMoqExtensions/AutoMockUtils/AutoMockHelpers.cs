using AutoFixture;
using AutoMoqExtensions.FixtureUtils.Requests;
using Moq;
using System.Collections;
using System.Reflection;
using System.Threading;

namespace AutoMoqExtensions.AutoMockUtils;

public static class AutoMockHelpers
{
    public static AutoMock<T>? GetAutoMock<T>(T? obj) where T : class
    {
        if (obj is null) return null;

        try { return Mock.Get<T>(obj) as AutoMock<T>;}
        catch{ return null; }
    }

    public static bool IsAutoMock<T>() => IsAutoMock(typeof(T));
    public static bool IsAutoMock(Type? t) => t?.IsGenericType == true && t.GetGenericTypeDefinition() == typeof(AutoMock<>);
    public static Type? GetMockedType(Type? t) => t?.IsGenericType == true && t.GetGenericTypeDefinition() == typeof(AutoMock<>) ? t.GetTypeInfo().GetGenericArguments().Single() : null;
    /// <summary>
    /// Returns the non generic base interface, for use when it might be already an AutoMock but we don't know the generic type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static IAutoMock? GetFromObj(object obj) => obj is IAutoMock m ? m : (obj as IMocked ?? (obj as Delegate)?.Target as IMocked)?.Mock as IAutoMock;
    public static Type GetAutoMockType(Type inner) => typeof(AutoMock<>).MakeGenericType(inner);

    internal static bool IsAutoMockAllowed(Type t)
    {
        if (t is null || t.IsPrimitive || t == typeof(string) || t == typeof(object) || t.IsValueType 
                    || (t.IsSealed && !typeof(System.Delegate).IsAssignableFrom(t))
                    || t == typeof(Array) 
                    || typeof(IEnumerable).IsAssignableFrom(t)|| typeof(ICollection).IsAssignableFrom(t) || typeof(IList).IsAssignableFrom(t)

#if NET461_OR_GREATER || NETSTANDARD2_0_OR_GREATER             
                    || t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>))
#endif

                    // This way we cover all different Tuple types...
                    || (t.Assembly == typeof(Tuple).Assembly) && t.FullName.StartsWith(typeof(Tuple).FullName)
                    || (t.Assembly == typeof(ValueTuple).Assembly) && t.FullName.StartsWith(typeof(ValueTuple).FullName)
                    
                    || t == typeof(IntPtr) || t == typeof(UIntPtr)        
                    || typeof(Mock).IsAssignableFrom(t)
                    || typeof(Type).IsAssignableFrom(t)
                    || t.Assembly == typeof(Mock).Assembly
                    || typeof(IFixture).IsAssignableFrom(t) || typeof(IAutoMock).IsAssignableFrom(t) || typeof(ITracker).IsAssignableFrom(t)
                    // TODO...have to figure out why it has a problem to mock it and hwo we can expect it in general
                    //     but maybe with our CustomMockVirtualMethodsCommand it is already fixed
                    || new[] { typeof(Assembly).Namespace, typeof(Thread).Namespace, typeof(Task).Namespace }.Contains(t.Namespace))
        {
            return false;
        }

        if (t.IsGenericType && new[]
{
            typeof(KeyValuePair<,>),
#if NET461_OR_GREATER || NETSTANDARD2_0_OR_GREATER
            typeof(IAsyncEnumerable<>),
#endif
            typeof(Nullable<>),
            typeof(AutoMock<>),
        }.Contains(t.GetGenericTypeDefinition()))
            return false;

        return true;            
    }
}
