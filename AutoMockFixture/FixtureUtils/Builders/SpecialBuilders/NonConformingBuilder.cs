﻿using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;
using AutoMockFixture.FixtureUtils.Requests.SpecialRequests;
using DotNetPowerExtensions.Reflection;

namespace AutoMockFixture.FixtureUtils.Builders.SpecialBuilders;

internal abstract class NonConformingBuilder : ISpecimenBuilder
{
    public abstract Type[] SupportedTypes { get; }
    public abstract int Repeat { get; }
    public abstract object? CreateResult(Type requestType, object[][] innerResults, IRequestWithType typeRequest, ISpecimenContext context);
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

        var innerResult = GetRepeatedInnerSpecimens(typeRequest, context);

        if (innerResult is NoSpecimen) return innerResult;

        var finalResult = CreateResult(type, (object[][])innerResult, typeRequest, context);
        typeRequest.SetResult(finalResult, this);

        return finalResult;
    }

    protected virtual object GetRepeatedInnerSpecimens(IRequestWithType originalRequest, ISpecimenContext context)
    {
        var resultArray = new object[Repeat][];
        for (int i = 0; i < Repeat; i++)
        {
            var inner = GetInnerSpecimens(originalRequest, i, context);
            if (inner is NoSpecimen) return inner;

            resultArray[i] = (object[])inner;
        }

        return resultArray;
    }

    protected virtual object GetInnerSpecimens(IRequestWithType originalRequest, int index, ISpecimenContext context)
    {
        var innerTypes = originalRequest.Request.GetInnerTypes();

        return BuildInnerSpecimens(originalRequest, innerTypes, index, context);
    }

    protected virtual object BuildInnerSpecimens(IRequestWithType originalRequest, Type[] innerTypes,
                        int index, ISpecimenContext context)
    {
        var result = new List<object>();

        var typeIndex = 0; // To keep track of tuple index
        foreach (var type in innerTypes)
        {
            var newRequest = GetInnerRequest(type, originalRequest, index, typeIndex);

            var specimen = context.Resolve(newRequest);

            if (specimen is NoSpecimen || specimen is OmitSpecimen)
            {
                originalRequest.SetResult(specimen, this);
                return new NoSpecimen(); // Let the system handle it
            }

            result.Add(specimen);

            typeIndex++;
        }

        return result.ToArray();
    }

    protected abstract InnerRequest GetInnerRequest(Type type, IRequestWithType originalRequest, int index, int argIndex);
}
