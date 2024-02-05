using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.SpecialRequests;
using DotNetPowerExtensions.Reflection;
using System.Collections;

namespace AutoMockFixture.FixtureUtils.Commands;

internal class PopulateEnumerableCommand : ISpecimenCommand
{
    public PopulateEnumerableCommand(IAutoMockHelpers autoMockHelpers, IAutoMockFixture fixture, int repeat)
    {
        AutoMockHelpers = autoMockHelpers;
        Fixture = fixture;
        Repeat = repeat;
    }

    public IAutoMockHelpers AutoMockHelpers { get; }
    public IAutoMockFixture Fixture { get; }
    public int Repeat { get; }

    public void Execute(object specimen, ISpecimenContext context)
    {
        if(!Fixture.ProcessingTrackerDict.TryGetValue(specimen, out var existingTracker)
                || existingTracker is not IRequestWithType typedRequest) return;

        if (specimen is not IEnumerable enumerable) return;

        var requestType = typedRequest.Request;
        while (AutoMockHelpers.IsAutoMock(requestType)) requestType = AutoMockHelpers.GetMockedType(requestType);

        var innerType = typedRequest.Request.GetInnerTypes().First();
        foreach (var item in enumerable) if (item is not null && innerType?.GetDefault() != item) return; // Only doing if not setup, example if it is from cache

        var t = specimen.GetType();

        var singleMethod = t.IsArray ? t.GetMethod("Set") :
            requestType?.GetTypeDetailInfo() // Going with the request type since if the Add is not implemented in the original type then there is no point in working on the values
                .MethodDetails.FirstOrDefault(md => md.Name == "Add" && !md.ReflectionInfo.IsAbstract
                                    && !md.GenericArguments.Any() && !md.ReflectionInfo.IsStatic
                                && md.ArgumentTypes.Length == 1 && md.ArgumentTypes.First() == innerType)?.ReflectionInfo;

        var addRangeMethod = singleMethod is not null ? null :
            requestType?.GetTypeDetailInfo() // Going with the request type since if the Add is not implemented in the original type then there is no point in working on the values
                .MethodDetails.FirstOrDefault(md => md.Name == "AddRange" && !md.ReflectionInfo.IsAbstract
                            && !md.GenericArguments.Any() && !md.ReflectionInfo.IsStatic
                    && md.ArgumentTypes.Length == 1 && md.ArgumentTypes.First().IsAssignableFrom(typeof(IEnumerable<>).MakeGenericType(innerType)))?.ReflectionInfo;
        if (singleMethod is null && addRangeMethod is null) return;

        var inners = GetRepeatedInnerSpecimens(typedRequest, context).ToArray();
        if(inners.Any(i => i is NoSpecimen || i is OmitSpecimen)) return;

        if(addRangeMethod is not null)
        {
            var cast = typeof(Enumerable).GetMethod(nameof(Enumerable.OfType)).MakeGenericMethod(innerType);
            var casted = cast.Invoke(typedRequest, new object[] { inners });
            addRangeMethod.Invoke(specimen, new object[] { casted });
            return;
        }

        for (var i = 0; i < Repeat; i++)
        {
            var item = inners[i];

            if(t.IsArray) singleMethod!.Invoke(specimen, new object[] { i, item });
            else singleMethod!.Invoke(specimen, new object[] { item });
        }
    }

    protected virtual IEnumerable<object> GetRepeatedInnerSpecimens(IRequestWithType originalRequest, ISpecimenContext context)
    {
        var innerType = originalRequest.Request.GetInnerTypes().First();

        for (int i = 0; i < Repeat; i++)
        {
            var newRequest = new ListItemRequest(innerType, originalRequest, i);
            var inner = context.Resolve(newRequest);

            yield return inner; // Return first even on nospeciman otherwise we won't detect it
            if (inner is NoSpecimen || inner is OmitSpecimen) yield break; // optimization
        }
    }
}
