using AutoMoqExtensions.AutoMockUtils;
using AutoMoqExtensions.FixtureUtils.Requests;
using System.Collections;
using System.Collections.ObjectModel;

namespace AutoMoqExtensions.FixtureUtils.Builders.SpecialBuilders;

internal abstract class NonConformingBuilder : ISpecimenBuilder
{
    public abstract Type[] SupportedTypes { get; }
    public abstract int Repeat { get; }
    public abstract object CreateResult(Type requestType, object[][] innerResults);
    public virtual Type[] GetInnerTypes(Type requestType)
        => requestType.IsArray ? new [] { requestType.GetElementType() } : requestType.GenericTypeArguments;

    public object? Create(object request, ISpecimenContext context)
    {
        if (request is not IRequestWithType typeRequest 
                || (!typeRequest.Request.IsGenericType && !typeRequest.Request.IsArray)) return new NoSpecimen();

        var genericDefinitions = typeRequest.Request.GetInterfaces()
                .Where(i => i.IsGenericType)
                .Select(i => i.GetGenericTypeDefinition())
                .Distinct().ToList();

        if (typeRequest.Request.IsClass)
        {
            var t = typeRequest.Request;
            do
            {
                if (t.IsGenericType) genericDefinitions.Add(t.GetGenericTypeDefinition());
                t = t.BaseType;
            } while (t is not null);

        }
        // generic type defintions are not conisdered assignable
        if (!SupportedTypes.Any(t => t.IsAssignableFrom(typeRequest.Request) || t.IsGenericTypeDefinition && genericDefinitions.Contains(t))) return new NoSpecimen();

        var args = GetInnerTypes(typeRequest.Request).ToList();
        var resultArray = new object[Repeat][];
        for (int i = 0; i < Repeat; i++)
        {
            resultArray[i] = new object[args.Count];
            foreach (var arg in args)
            {
                object newRequest = typeRequest switch
                {
                    { } when AutoMockHelpers.IsAutoMock(arg) => new AutoMockDirectRequest(arg, typeRequest),
                    AutoMockDependenciesRequest dependenciesRequest => new AutoMockDependenciesRequest(arg, dependenciesRequest),
                    AutoMockRequest autoMockRequest => new AutoMockRequest(arg, autoMockRequest),
                    NonAutoMockRequest nonAutoMockRequest => new NonAutoMockRequest(arg, nonAutoMockRequest),
                    _ => throw new NotSupportedException(),
                };
                var specimen = context.Resolve(newRequest);

                if (specimen is NoSpecimen || specimen is OmitSpecimen)
                {
                    typeRequest.SetResult(specimen);
                    return new NoSpecimen(); // Let the system handle it
                }

                resultArray[i][args.IndexOf(arg)] = specimen;
            }
        }
        var finalResult = CreateResult(typeRequest.Request, resultArray);
        typeRequest.SetResult(finalResult);

        return finalResult;      
    }
}    
