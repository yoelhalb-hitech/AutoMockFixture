using AutoMoqExtensions.AutoMockUtils;
using AutoMoqExtensions.FixtureUtils.Requests;
using AutoMoqExtensions.FixtureUtils.Requests.MainRequests;
using AutoMoqExtensions.FixtureUtils.Specifications;
using System.Collections;
using System.Collections.ObjectModel;

namespace AutoMoqExtensions.FixtureUtils.Builders.SpecialBuilders;

internal abstract class NonConformingBuilder : ISpecimenBuilder
{
    private static InnerTypeSpecification innerSpecification = new InnerTypeSpecification();

    public abstract Type[] SupportedTypes { get; }
    public abstract int Repeat { get; }
    public abstract object CreateResult(Type requestType, object[][] innerResults);
    public virtual Type[] GetInnerTypes(Type requestType)
        => requestType.IsArray ? new [] { requestType.GetElementType() } : requestType.GenericTypeArguments;

    public object? Create(object request, ISpecimenContext context)
    {
        if (request is not IRequestWithType typeRequest
                || (!innerSpecification.IsSatisfiedBy(typeRequest.Request))) return new NoSpecimen();

        var genericDefinitions = typeRequest.Request.GetAllGenericDefinitions();

        // generic type defintions are not conisdered assignable
        if (!SupportedTypes.Any(t => t.IsAssignableFrom(typeRequest.Request) 
                    || (t.IsGenericTypeDefinition && genericDefinitions.Contains(t)))) return new NoSpecimen();
                
        var innerResult = GetRepeatedInnerSpecimens(typeRequest, context);

        if (innerResult is NoSpecimen) return innerResult;

        var finalResult = CreateResult(typeRequest.Request, (object[][])innerResult);
        typeRequest.SetResult(finalResult);

        return finalResult;      
    }

    protected virtual object GetRepeatedInnerSpecimens(IRequestWithType originalRequest, ISpecimenContext context)
    { 
        var resultArray = new object[Repeat][];
        for (int i = 0; i < Repeat; i++)
        {
            var inner = GetInnerSpecimens(originalRequest, context);
            if (inner is NoSpecimen) return inner;

            resultArray[i] = (object[])inner;
        }

        return resultArray;
    }

    protected virtual object GetInnerSpecimens(IRequestWithType originalRequest, ISpecimenContext context)
    {
        var args = originalRequest.Request.GetInnerTypes().ToList();
        var result = new List<object>();

        foreach (var arg in args)
        {
            var specimen = GetInnerSpecimen(arg, originalRequest, context);

            if (specimen is NoSpecimen) return specimen;

            result.Add(specimen);
        }

        return result.ToArray();
    }

    protected virtual object GetInnerSpecimen(Type arg, IRequestWithType originalRequest, ISpecimenContext context)
    {
        object newRequest = originalRequest switch
        {
            { } when AutoMockHelpers.IsAutoMock(arg) => new AutoMockDirectRequest(arg, originalRequest),
            AutoMockDependenciesRequest dependenciesRequest => new AutoMockDependenciesRequest(arg, dependenciesRequest),
            AutoMockRequest autoMockRequest => new AutoMockRequest(arg, autoMockRequest),
            NonAutoMockRequest nonAutoMockRequest => new NonAutoMockRequest(arg, nonAutoMockRequest),
            _ => throw new NotSupportedException(),
        };

        var specimen = context.Resolve(newRequest);

        if (specimen is NoSpecimen || specimen is OmitSpecimen)
        {
            originalRequest.SetResult(specimen);
            return new NoSpecimen(); // Let the system handle it
        }

        return specimen;
    }
}    
