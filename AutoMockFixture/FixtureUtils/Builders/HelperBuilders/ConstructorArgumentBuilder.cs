using AutoMockFixture.FixtureUtils.Customizations;
using AutoMockFixture.FixtureUtils.Requests.HelperRequests.NonAutoMock;

namespace AutoMockFixture.FixtureUtils.Builders.HelperBuilders;

internal class ConstructorArgumentBuilder : HelperBuilderBase<ConstructorArgumentRequest>
{
    public ConstructorArgumentBuilder(List<ConstructorArgumentValue> constructorArgumentValues)
    {
        ConstructorArgumentValues = constructorArgumentValues;
    }

    public List<ConstructorArgumentValue> ConstructorArgumentValues { get; }
 
    protected override object? HandleInternal(ConstructorArgumentRequest ctorArgsRequest, ISpecimenContext context)
    {
        var type = GetRequest(ctorArgsRequest);

        // Caution: Cannot just use FirstOrDefault and check for nullability as the custom value itself can be null
        var hasCustomValue = ConstructorArgumentValues.Any(v => IsValidArgumentValue(type, v, ctorArgsRequest.Path));
        if (hasCustomValue)
        {
            var customValue = ConstructorArgumentValues
                                .First(v => IsValidArgumentValue(type, v, ctorArgsRequest.Path))
                                .Value;
            ctorArgsRequest.SetResult(customValue, this);
            return customValue;
        }

        return base.HandleInternal(ctorArgsRequest, context);
    }

    private bool IsValidArgumentValue(Type type, ConstructorArgumentValue argumentValue, string path)
    {
        if (!string.IsNullOrWhiteSpace(argumentValue.Path) && argumentValue.Path != path) return false;

        if (argumentValue.Value is not null) return type.IsInstanceOfType(argumentValue.Value) 
                || (argumentValue.Value is IAutoMock mock && type.IsInstanceOfType(mock.GetMocked()));

        return type.IsNullAllowed();
    }

    protected override Type GetRequest(ConstructorArgumentRequest request) => request.ParameterInfo.ParameterType;

    protected override object GetFullRequest(ConstructorArgumentRequest request) => request.ParameterInfo;
}
