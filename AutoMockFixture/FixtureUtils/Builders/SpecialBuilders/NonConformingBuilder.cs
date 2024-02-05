using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;
using AutoMockFixture.FixtureUtils.Requests.SpecialRequests;
using DotNetPowerExtensions.Reflection;

namespace AutoMockFixture.FixtureUtils.Builders.SpecialBuilders;

internal abstract class NonConformingBuilder : ISpecimenBuilder
{
    public abstract Type[] SupportedTypes { get; }
    public virtual Type[] NotSupportedTypes => new Type[] { };

    public virtual bool NoGenerateInner => false;

    public abstract object? CreateResult(Type requestType, object[] innerResults, IRequestWithType typeRequest, ISpecimenContext context);
    public virtual Type[] GetInnerTypes(Type requestType)
        => requestType.IsArray ? new Type[] { requestType.GetElementType()! } : requestType.GenericTypeArguments;

    public object? Create(object request, ISpecimenContext context)
    {
        if (request is not IRequestWithType typeRequest || typeRequest.Request is not Type type) return new NoSpecimen();
        if(request is AutoMockDirectRequest) return new NoSpecimen();

        var genericDefinitions = type.GetAllGenericDefinitions();

        // generic type defintions are not conisdered assignable
        if (!SupportedTypes.Any(t => t.IsAssignableFrom(type)
                    || ((type.IsGenericType || type.IsArray) && t.IsGenericTypeDefinition && genericDefinitions.Contains(t)))) return new NoSpecimen();
        if(NotSupportedTypes.Any(t => t.IsAssignableFrom(type))) return new NoSpecimen();

        var innerResult = NoGenerateInner ? new object[] { } : GetInnerSpecimens(typeRequest, context);

        if (innerResult.OfType<NoSpecimen>().Any()) return new NoSpecimen();

        var finalResult = CreateResult(type, innerResult, typeRequest, context);
        typeRequest.SetResult(finalResult, this);

        return finalResult;
    }

    protected virtual object[] GetInnerSpecimens(IRequestWithType originalRequest, ISpecimenContext context)
    {
        var innerTypes = originalRequest.Request.GetInnerTypes();

        return BuildInnerSpecimens(originalRequest, innerTypes, context);
    }

    protected virtual object[] BuildInnerSpecimens(IRequestWithType originalRequest, Type[] innerTypes, ISpecimenContext context)
    {
        var result = new List<object>();

        var typeIndex = 0; // To keep track of tuple index
        foreach (var type in innerTypes)
        {
            var newRequest = GetInnerRequest(type, originalRequest, typeIndex);

            var specimen = context.Resolve(newRequest);

            if (specimen is NoSpecimen || specimen is OmitSpecimen)
            {
                originalRequest.SetResult(specimen, this);
                return new[] { new NoSpecimen() }; // Let the system handle it
            }

            result.Add(specimen);

            typeIndex++;
        }

        return result.ToArray();
    }

    protected abstract InnerRequest GetInnerRequest(Type type, IRequestWithType originalRequest, int argIndex);
}
