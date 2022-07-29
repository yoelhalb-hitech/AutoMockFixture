using AutoFixture.Kernel;
using AutoMoqExtensions.AutoMockUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.MethodInvokers
{
    public class MethodInvokerWithRecursion : ISpecimenBuilder
    {
        public MethodInvokerWithRecursion(IMethodQuery query)
        {
            this.Query = query ?? throw new ArgumentNullException(nameof(query));
        }

        public IMethodQuery Query { get; }

        public object Create(object request, ISpecimenContext context)
        {
            if (context is null) throw new ArgumentNullException(nameof(context));

            var requestedType = GetRequestedType(request);
            if(requestedType is null || requestedType.IsInterface 
                    || requestedType.IsAbstract || requestedType.IsGenericTypeDefinition) return new NoSpecimen();

            var methods = Query.SelectMethods(requestedType);

            object? unformattedObject = null;
            var recursionContext = context as RecursionContext;

            if (recursionContext is not null && methods.Any(m => m is CustomConstructorMethod))
            {
                // Not doing this in a special builder, to ensure that no matter what happens we do it on recursion
                if (recursionContext.BuilderCache.ContainsKey(requestedType)) 
                    return recursionContext.BuilderCache[requestedType]; // We are in recursion

                methods = methods.Where(m => m is CustomConstructorMethod);

                Logger.LogInfo("In ctor arg - creating new");
                Logger.LogInfo("In ctor arg - context cache now contains: " + string.Join(",", recursionContext.BuilderCache.Keys.Select(k => k.FullName)));

                unformattedObject = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(requestedType);
                recursionContext.BuilderCache[requestedType] = unformattedObject;
            }

            try
            {           
                foreach (var ci in methods)
                {
                    var paramValues = (from pi in ci.Parameters select ResolveParamater(request, pi, context)).ToList();

                    if (paramValues.All(IsValueValid))
                    {
                        if(unformattedObject is null) return ci.Invoke(paramValues.ToArray());

                        if (ci is not CustomConstructorMethod cm) throw new Exception("Should not arrive here");

                        cm.Invoke(paramValues.ToArray(), unformattedObject);

                        return unformattedObject;
                    }
                }

                return new NoSpecimen();
            }
            finally
            {
                if (recursionContext is not null) recursionContext.BuilderCache.Remove(requestedType);
            }
        }

        protected virtual Type? GetRequestedType(object request) => request as Type;
        protected virtual object ResolveParamater(object request, ParameterInfo pi, ISpecimenContext context) 
            => context.Resolve(pi.ParameterType);

        protected virtual bool IsValueValid(object value) => !(value is NoSpecimen) && !(value is OmitSpecimen);
    }
}
