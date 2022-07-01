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
                if (recursionContext.BuilderCache.ContainsKey(mockRequest.Request))
                    return recursionContext.BuilderCache[mockRequest.Request]; // We are in recursion

                Console.WriteLine("In ctor arg - creating new");
                Console.WriteLine("In ctor arg - context cache now contains: " + string.Join(",", recursionContext.BuilderCache.Keys.Select(k => k.FullName)));
            }

            var mock = (IAutoMock)Activator.CreateInstance(mockRequest.Request);
            if (!mockRequest.StartTracker.MockShouldCallbase) return mock;

            if (recursionContext is not null)
            {
                recursionContext.BuilderCache[mockRequest.Request] = mock;
            }            

            Console.WriteLine("In autmock ctor arg - type is " + mockRequest.Request.FullName);

            Console.WriteLine("In ctor arg - creating new");

            try
            {
                foreach (var ci in Query.SelectMethods(mockRequest.Request))
                {
                    var paramValues = (from pi in ci.Parameters select ResolveParamater(mockRequest, pi, context)).ToList();

                    if (paramValues.All(IsValueValid))
                    {
                        if (ci is not AutoMockConstructorMethod cm) throw new Exception("Should not arrive here");

                        cm.Invoke(paramValues.ToArray(), mock.GetMocked());

                        return mock;
                    }                       
                }

                return new NoSpecimen();
            }
            finally
            {
                if(recursionContext is not null) recursionContext.BuilderCache.Remove(mockRequest.Request);
            }
        }

        protected virtual object ResolveParamater(AutoMockDirectRequest mockRequest, ParameterInfo pi, ISpecimenContext context)
        {            
            var argsRequest = new AutoMockConstructorArgumentRequest(mockRequest.Request, pi, mockRequest);

            Console.WriteLine("\t\t\t\t\t\tBefore args: " + pi.Name);
            var result = context.Resolve(argsRequest);
            Console.WriteLine("\t\t\t\t\t\tAfter args: " + result.GetType().FullName);

            return result;
        }

        protected virtual bool IsValueValid(object value) => !(value is NoSpecimen) && !(value is OmitSpecimen);
    }
}
