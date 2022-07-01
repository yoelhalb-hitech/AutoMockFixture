using AutoFixture.Kernel;
using AutoMoqExtensions.FixtureUtils.Requests;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.MethodInvokers
{
    internal class DependencyInjectionMethodinvoker : MethodInvokerWithRecursion
    {
        public DependencyInjectionMethodinvoker(IMethodQuery query) : base(query)
        {
        }

        protected override Type? GetRequestedType(object request) => (request as AutoMockDependenciesRequest)?.Request;

        protected override object ResolveParamater(object request,  ParameterInfo pi, ISpecimenContext context)
        {
            if (request is not AutoMockDependenciesRequest dependencyRequest) return base.ResolveParamater(request, pi, context);

            var argsRequest = new AutoMockConstructorArgumentRequest(dependencyRequest.Request, pi, dependencyRequest);

            Console.WriteLine("\t\t\t\t\t\tBefore args: " + pi.Name);
            var result = context.Resolve(argsRequest);
            Console.WriteLine("\t\t\t\t\t\tAfter args: " + result.GetType().FullName);

            return result;
        }
    }
}
