using AutoMockFixture.AutoMockUtils;
using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.SpecialRequests;
using SequelPay.DotNetPowerExtensions.Reflection;
using System.Collections;
using System.Linq;

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

        // TODO... handle the type as par https://github.com/dotnet/csharpstandard/blob/standard-v7/standard/statements.md#1395-the-foreach-statement
        // We can shortcut it possibly if there is only one `Add` method, but if there are more (and not object) then we need to go directly
        // But technically we might assume that the type of any `Add` method (that is not object) will be good enough
        // Just note that before .Net 8 an `.Add(object)` method was valid, see https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/breaking-changes/compiler%20breaking%20changes%20-%20dotnet%208
        var enumerableIface = obj.GetType().GetInterfaces().FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        if(enumerableIface is null && !obj.GetType().IsArray) return;

        var innerType = enumerableIface?.GetInnerTypes().First() ?? obj.GetType().GetElementType()!;
        try
        {
            if (enumerable.GetEnumerator() is not null) // Can happen if not setup correctly
                foreach (var item in enumerable) // Will start enumeration, but we return immediately if there is anything, this way we don't have to figure out how to do `.Any()`...
                    // CAUTION: User .Equals and ==/!= as it might not work correctly for value types
                    if (item is not null && innerType?.GetDefault()?.Equals(item) != true) return; // Only doing if not setup, example if it is from cache
        }
        catch { } // In case there is an issue with the enumerator

        var requestType = typedRequest.Request; // Working with the request so we can get the concrete type to see if it is abstract
        while (AutoMockHelpers.IsAutoMock(requestType)) requestType = AutoMockHelpers.GetMockedType(requestType)!;

        var t = obj.GetType();

        // TODO... handle the new `CollectionBuilderAttribute`
        var singleMethod = t.IsArray ? t.GetMethod("Set") :
            // Going with the request type since if the Add is not implemented in the original type then there is no point in working on the values
            requestType?.GetTypeDetailInfo().MethodDetails
            .Union(requestType.GetTypeDetailInfo().ExplicitMethodDetails)
                .FirstOrDefault(md => (md.Name == "Add" || md.Name == "TryAdd")
                        && !md.ReflectionInfo.IsAbstract && !md.GenericArguments.Any() && !md.ReflectionInfo.IsStatic
                        && md.ArgumentTypes.Length == 1 && md.ArgumentTypes.First() == innerType)?.ReflectionInfo;

        var addRangeMethod = singleMethod is not null ? null :
            requestType?.GetTypeDetailInfo() // Going with the request type since if the Add is not implemented in the original type then there is no point in working on the values
                .MethodDetails.FirstOrDefault(md => md.Name == "AddRange" && !md.ReflectionInfo.IsAbstract
                            && !md.GenericArguments.Any() && !md.ReflectionInfo.IsStatic
                    && md.ArgumentTypes.Length == 1 && md.ArgumentTypes.First().IsAssignableFrom(typeof(IEnumerable<>).MakeGenericType(innerType)))?.ReflectionInfo;
        if (singleMethod is null && addRangeMethod is null) return;

        try
        {
            if (addRangeMethod is not null)
            {
                var inners = GetRepeatedInnerSpecimens(typedRequest, innerType, context).ToArray();
                if (inners.Any(i => i is NoSpecimen || i is OmitSpecimen)) return;

                var cast = typeof(Enumerable).GetMethod(nameof(Enumerable.OfType))!.MakeGenericMethod(innerType);
                var casted = cast.Invoke(typedRequest, new object[] { inners })!;
                addRangeMethod.Invoke(obj, new object[] { casted });
                return;
            }
            HandleSingleMethod(t.IsArray ? t.GetArrayRank() : 1, new int[0]);
        }
        catch { } // Don't throw on it

        void HandleSingleMethod(int depth, int[] index)
        {
            if (depth > 1)
            {
                for (var i = 0; i < Repeat; i++) HandleSingleMethod(depth - 1, (int[])[..index, i]);
                return; // Will be handled at the lowest level
            }

            var inners = GetRepeatedInnerSpecimens(typedRequest, innerType, context).ToArray();
            if (inners.Any(i => i is NoSpecimen || i is OmitSpecimen)) return;

            for (var i = 0; i < Repeat; i++)
            {
                var item = inners[i];

                if (t.IsArray) singleMethod!.Invoke(obj, (object[])[.. index, i, item]);
                else singleMethod!.Invoke(obj, new object[] { item });
            }
        }
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
