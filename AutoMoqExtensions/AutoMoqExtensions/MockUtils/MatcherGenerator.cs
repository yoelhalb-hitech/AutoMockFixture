using AutoMoqExtensions.Extensions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace AutoMoqExtensions.MockUtils
{
    public class MatcherGenerator
    {
        private static Dictionary<string, Type> matcherDict = new Dictionary<string, Type>();
        private static object lockObject = new object(); // TODO... Maybe we can improve that by locking on unique things such as interned strings

        /// <summary>
        /// Gets a matcher for <see cref="ParameterInfo"/> that is potentially a generic type
        /// </summary>
        /// <param name="parameterInfo">The potentialy generic parameter</param>
        /// <returns>A <see cref="Type"/> instance of the correct matcher</returns>
        public static Type GetGenericMatcher(ParameterInfo parameterInfo)
        {
            return GetMatcherForParameterInternal(parameterInfo.ParameterType).Item1;
        }

        /// <summary>
        /// Gets a matcher for a <see cref="Type"/> object that is potentially generic
        /// </summary>
        /// <param name="parameterInfo">The potentialy generic parameter</param>
        /// <returns>A <see cref="Type"/> instance of the correct matcher</returns>
        public static Type GetGenericMatcher(Type genericType)
        {
            var isValueType = (genericType.GenericParameterAttributes &
            GenericParameterAttributes.SpecialConstraintMask & GenericParameterAttributes.NotNullableValueTypeConstraint) != 0;
            var constraints = genericType.GetGenericParameterConstraints();

            if (!constraints.Any()) return isValueType ? typeof(It.IsValueType) : typeof(It.IsAnyType);

            else
            {
                // https://stackoverflow.com/a/59144369/640195
                if (isValueType) constraints = constraints.Union(new[] { typeof(System.ValueType) }).ToArray();
                return GetOrAdd(constraints);
            }
        }

        private static Tuple<Type, bool> GetMatcherForParameterInternal(Type parameterType)
        {
            if (parameterType.IsArray)
            {
                var recurse = GetMatcherForParameterInternal(parameterType.GetElementType());
                if (recurse.Item2) return Tuple.Create(recurse.Item1.MakeArrayType(), true);
                return Tuple.Create(parameterType, false);
            }

            if (parameterType.IsGenericType)
            {
                var parameters = parameterType.GenericTypeArguments;
                var recurses = parameters.Select(p => GetMatcherForParameterInternal(p)).ToList();
                if (recurses.All(r => !r.Item2)) return Tuple.Create(parameterType, false);

                return Tuple.Create(parameterType.MakeGenericType(recurses.Select(r => r.Item1).ToArray()), true);
            }

            if (parameterType.IsGenericParameter)
            {
                return Tuple.Create(GetGenericMatcher(parameterType), true);
            }

            return Tuple.Create(parameterType, false);
        }

        private static Type GetOrAdd(IEnumerable<Type> types)
        {
            var tag = types.GetTagForTypes();
            if (matcherDict.ContainsKey(tag)) return matcherDict[tag];
            lock (lockObject)
            {
                if (matcherDict.ContainsKey(tag)) return matcherDict[tag];

                // Making sure that the name is unique but staring with a alpha letter
                var newType = CompileTypeMatcher("D" + Guid.NewGuid().ToString("N"), types.FirstOrDefault(c => !c.IsInterface), types.Where(c => c.IsInterface).ToArray());
                matcherDict[tag] = newType;
                return newType;
            }
        }

        // TODO... Maybe we can improve perfomance by batching them together in 1 assembly,
        //        (my benchmarks showed that until 10k there is a performance benefit, but afterwards it degardes)
        // We can do it even intially to round up all generics of the object and have it created in a background thread while executing the other methods
        // However in this case I would recommend doing only small batches at once, so we should be able to start setting up as soon as it compiles some
        private static Type CompileTypeMatcher(string name, Type? parent, Type[] interfaces)
        {
            var methods = interfaces.SelectMany(i => i.GetAllMethods()).Union(parent?.GetAllMethods().Where(m => m.IsAbstract) ?? new MethodInfo[] { }).ToArray();

            interfaces = interfaces.Union(new[] { typeof(ITypeMatcher) }).ToArray();
            var an = new AssemblyName(name);
            //AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(an, AssemblyBuilderAccess.RunAndCollect);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            var tb = moduleBuilder.DefineType(name,
                    TypeAttributes.Public |
                    TypeAttributes.Class |
                    TypeAttributes.AutoClass |
                    TypeAttributes.AnsiClass |
                    TypeAttributes.BeforeFieldInit |
                    TypeAttributes.AutoLayout,
                    parent, interfaces);
            tb.SetCustomAttribute(new CustomAttributeBuilder(typeof(TypeMatcherAttribute).GetConstructor(new Type[] { }), new object[] { }));

            ConstructorBuilder constructor = tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
            // TODO... we might need to handle the case when there is a base and the base doesn't have a default constructor


            var matchesMthdBldr = tb.DefineMethod(nameof(ITypeMatcher.Matches), MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, typeof(bool), new[] { typeof(Type) });
            var matchesIl = matchesMthdBldr.GetILGenerator();
            matchesIl.Emit(OpCodes.Ldc_I4_1);
            matchesIl.Emit(OpCodes.Ret);
            tb.DefineMethodOverride(matchesMthdBldr, typeof(ITypeMatcher).GetMethod(nameof(ITypeMatcher.Matches)));
            foreach (var method in methods)
            {
                var accessMethod = method switch
                {
                    { IsPublic: true } => MethodAttributes.Public,
                    { IsPrivate: true } => MethodAttributes.Private,
                    { IsFamily: true } => MethodAttributes.Family,
                    { IsFamilyAndAssembly: true } => MethodAttributes.FamANDAssem,
                    { IsFamilyOrAssembly: true } => MethodAttributes.FamORAssem,
                    { IsAssembly: true } => MethodAttributes.Assembly,
                    _ => MethodAttributes.Public,
                };
                if (method.IsStatic) accessMethod = accessMethod | MethodAttributes.Static;
                if (method.DeclaringType.IsInterface) accessMethod = accessMethod | MethodAttributes.Virtual;
                
                var mthdBldr = tb.DefineMethod(method.Name, accessMethod | MethodAttributes.HideBySig, method.ReturnType, method.GetParameters().Select(p => p.ParameterType).ToArray());
                var il = mthdBldr.GetILGenerator();
                il.Emit(OpCodes.Nop);
                il.Emit(OpCodes.Newobj, typeof(NotImplementedException));
                il.Emit(OpCodes.Throw);

                if (method.DeclaringType.IsInterface) tb.DefineMethodOverride(mthdBldr, method);
            }

            Type objectType = tb.CreateType();
            return objectType;
        }
    }
}
