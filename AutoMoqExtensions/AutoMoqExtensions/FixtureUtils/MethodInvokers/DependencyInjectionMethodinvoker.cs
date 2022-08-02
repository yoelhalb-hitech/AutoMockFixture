using AutoFixture.Kernel;
using AutoMoqExtensions.FixtureUtils.Requests.HelperRequests.AutoMock;
using AutoMoqExtensions.FixtureUtils.Requests.MainRequests;
using System.Reflection;

namespace AutoMoqExtensions.FixtureUtils.MethodInvokers;

internal class DependencyInjectionMethodInvoker : MethodInvokerWithRecursion
{
    public DependencyInjectionMethodInvoker(IMethodQuery query) : base(query)
    {
    }

    protected override Type? GetRequestedType(object request) => (request as AutoMockDependenciesRequest)?.Request;

    protected override object ResolveParamater(object request, Type declaringType,
                                                        ParameterInfo pi, ISpecimenContext context)
    {
        if (request is not AutoMockDependenciesRequest dependencyRequest) // TODO... why did it arrive here??
                return base.ResolveParamater(request, declaringType, pi, context);

        var argsRequest = new AutoMockConstructorArgumentRequest(declaringType, pi, dependencyRequest);

        Logger.LogInfo("\t\t\t\t\t\tBefore args: " + pi.Name);
        var result = context.Resolve(argsRequest);
        Logger.LogInfo("\t\t\t\t\t\tAfter args: " + result.GetType().FullName);

        return result;
    }
}
