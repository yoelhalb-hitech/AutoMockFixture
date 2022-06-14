using Moq;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace AutoMoqExtensions.AutoMockUtils
{
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
        public static IAutoMock? GetFromObj(object obj) => obj is IAutoMock m ? m : (obj as IMocked)?.Mock as IAutoMock;
        public static Type GetAutoMockType(Type inner) => typeof(AutoMock<>).MakeGenericType(inner);

        internal static bool IsAutoMockAllowed(Type t)
        {
           
            if (t is null || t.IsPrimitive || t == typeof(string) || t.IsValueType
                       // || t == typeof(Array) || typeof(IEnumerable).IsAssignableFrom(t)
                        || t == typeof(IntPtr) || t == typeof(UIntPtr)
                        //|| (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                        //|| t == typeof(Mock)
                        //|| (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(AutoMock<>))
                        //|| (t.IsSealed && !typeof(System.Delegate).IsAssignableFrom(t))
                        || t == typeof(Type)
                        || (t.Assembly == typeof(Mock).Assembly && !typeof(Mock).IsAssignableFrom(t))
                        // TODO...have to figure out why it has a problem to mock it and hwo we can expect it in general
                        //     but maybe with our CustomMockVirtualMethodsCommand it is already fixed
                        || new[] { typeof(Assembly).Namespace, typeof(Thread).Namespace, typeof(Task).Namespace }.Contains(t.Namespace))
            {
                return false;
            }

            return true;            
        }
    }
}
