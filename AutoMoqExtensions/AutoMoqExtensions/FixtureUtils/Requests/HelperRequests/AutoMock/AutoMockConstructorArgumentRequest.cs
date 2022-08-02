using AutoMoqExtensions.AutoMockUtils;
using AutoMoqExtensions.FixtureUtils.Requests.HelperRequests.NonAutoMock;
using System.Reflection;

namespace AutoMoqExtensions.FixtureUtils.Requests.HelperRequests.AutoMock;

internal class AutoMockConstructorArgumentRequest : ConstructorArgumentRequest, IAutoMockRequest
{
    public AutoMockConstructorArgumentRequest(Type declaringType, ParameterInfo parameterInfo, ITracker? tracker)
        : base(declaringType, parameterInfo, tracker)
    {
    }

    public override void SetResult(object? result)
    {
        if (result is not null && AutoMockHelpers.GetFromObj(result) is not null)
        {
            base.SetResult(result);
            return;
        }
        var type = result?.GetType();
        if (type is null || type.IsPrimitive || type == typeof(string) || type.IsEnum ||
                type.GetPublicAndProtectedConstructors().All(c => !c.GetParameters().Any())) // Really we could have only cheked for one as normally take the least, but in case the user will ask for something that does have
        {
            // We want to avoid setting the same path many times which will cause problems
            // This can happen if the code is calling a factory method multiple times from the same method, but passing different arguments to the method
            // So at least not saving things that we know will probably not arrive at an AutoMock anyway...
            SetCompleted();
            return;
        }

        base.SetResult(result);
    }

    public override bool IsRequestEquals(ITracker other)
        => other is AutoMockConstructorArgumentRequest && base.IsRequestEquals(other);
}
