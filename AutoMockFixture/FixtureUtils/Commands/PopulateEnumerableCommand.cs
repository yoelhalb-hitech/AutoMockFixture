using AutoMockFixture.AutoMockUtils;
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

        var obj = (specimen as IAutoMock)?.GetMocked() ?? specimen;
        if (obj is not IEnumerable enumerable || obj is string) return;

        var enumerableIface = obj.GetType().GetInterfaces().FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        if(enumerableIface is null) return;

        var innerType = enumerableIface.GetInnerTypes().First();
        foreach (var item in enumerable) // Will start enumeration, but we return immediately if there is anything, this way we don't have to figure out how to do `.Any()`...
            if (item is not null && innerType?.GetDefault() != item) return; // Only doing if not setup, example if it is from cache


        var requestType = typedRequest.Request; // Working with the request so we can get the concrete type to see if it is abstract
        while (AutoMockHelpers.IsAutoMock(requestType)) requestType = AutoMockHelpers.GetMockedType(requestType)!;

        var t = obj.GetType();

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

        var inners = GetRepeatedInnerSpecimens(typedRequest, innerType, context).ToArray();
        if(inners.Any(i => i is NoSpecimen || i is OmitSpecimen)) return;

        try
        {
            if (addRangeMethod is not null)
            {
                var cast = typeof(Enumerable).GetMethod(nameof(Enumerable.OfType)).MakeGenericMethod(innerType);
                var casted = cast.Invoke(typedRequest, new object[] { inners });
                addRangeMethod.Invoke(obj, new object[] { casted });
                return;
            }

            for (var i = 0; i < Repeat; i++)
            {
                var item = inners[i];

                if (t.IsArray) singleMethod!.Invoke(obj, new object[] { i, item });
                else singleMethod!.Invoke(obj, new object[] { item });
            }
        }
        catch { } // Don't throw on it
    }

    protected virtual IEnumerable<object> GetRepeatedInnerSpecimens(IRequestWithType originalRequest, Type innerType, ISpecimenContext context)
    {
        for (int i = 0; i < Repeat; i++)
        {
            var newRequest = new ListItemRequest(innerType, originalRequest, i);
            var inner = context.Resolve(newRequest);

            yield return inner; // Return first even on nospeciman otherwise we won't detect it
            if (inner is NoSpecimen || inner is OmitSpecimen) yield break; // optimization
        }
    }
}
