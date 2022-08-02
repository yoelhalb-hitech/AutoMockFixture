using AutoFixture.Kernel;
using AutoMoqExtensions.AutoMockUtils;
using AutoMoqExtensions.FixtureUtils;
using AutoMoqExtensions.FixtureUtils.Requests;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.MethodInvokers
{
    internal class AutoMockMethodInvoker : ISpecimenBuilder
    {
        
        public AutoMockMethodInvoker(IMethodQuery query)
        {
            this.Query = query ?? throw new ArgumentNullException(nameof(query));
        }

        public IMethodQuery Query { get; }
    
        public object Create(object request, ISpecimenContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (request is not AutoMockDirectRequest mockRequest) return new NoSpecimen();

            var recursionContext = context as RecursionContext;

            if (recursionContext is not null)
            {
                // Not doing this in a special builder, to ensure that no matter what happens we do it on recursion
                if (recursionContext.BuilderCache.ContainsKey(mockRequest.Request))
                    return recursionContext.BuilderCache[mockRequest.Request]; // We are in recursion

                Logger.LogInfo("In ctor arg - creating new");
                Logger.LogInfo("In ctor arg - context cache now contains: " + string.Join(",", recursionContext.BuilderCache.Keys.Select(k => k.FullName)));
            }
            
            var mock = (IAutoMock)Activator.CreateInstance(mockRequest.Request);
            if (mockRequest.MockShouldCallbase != true && mockRequest.StartTracker.MockShouldCallbase != true) return mock;

            var methods = Query.SelectMethods(mockRequest.Request);

            // When `CallBase = false` AutoMock will create us a default ctor
            // However we don't want to do it if there is a default ctor as in this case Automock will oevrride the default ctor
            if (methods.All(m => !m.Parameters.Any())) mock.CallBase = false;
                        
            if (recursionContext is not null)
            {
                recursionContext.BuilderCache[mockRequest.Request] = mock;
            }

            Logger.LogInfo("In autmock ctor arg - type is " + mockRequest.Request.FullName);

            Logger.LogInfo("In ctor arg - creating new");

            try
            {
                if(!methods.Any()) // Just in case...
                {
                    mock.EnsureMocked();

                    return mock;
                }

                foreach (var ci in methods)
                {
                    var paramValues = (from pi in ci.Parameters select ResolveParamater(mockRequest, pi, context)).ToList();

                    if (paramValues.All(IsValueValid))
                    {
                        if (ci is not CustomConstructorMethod cm) throw new Exception("Should not arrive here");

                        cm.Invoke(paramValues.ToArray(), mock.GetMocked());

                        return mock;
                    }                       
                }

                return new NoSpecimen();
            }
            finally
            {
                if(mock is not null && mock is ISetCallBase)
                {
                    ((ISetCallBase)mock).ForceSetCallbase(!mock.GetInnerType().IsDelegate()
                                                && (mockRequest.MockShouldCallbase == true
                                                    || (mockRequest.MockShouldCallbase is null 
                                                            && mockRequest.StartTracker.MockShouldCallbase == true)));
                }

                if (recursionContext is not null) recursionContext.BuilderCache.Remove(mockRequest.Request);
            }
        }

        protected virtual object ResolveParamater(AutoMockDirectRequest mockRequest, ParameterInfo pi, ISpecimenContext context)
        {            
            var argsRequest = mockRequest.NoMockDependencies == true
                    ? new ConstructorArgumentRequest(mockRequest.Request, pi, mockRequest)
                    : new AutoMockConstructorArgumentRequest(mockRequest.Request, pi, mockRequest);

            Logger.LogInfo("\t\t\t\t\t\tBefore args: " + pi.Name);
            var result = context.Resolve(argsRequest);
            Logger.LogInfo("\t\t\t\t\t\tAfter args: " + result.GetType().FullName);

            return result;
        }

        protected virtual bool IsValueValid(object value) => !(value is NoSpecimen) && !(value is OmitSpecimen);
    }
}
