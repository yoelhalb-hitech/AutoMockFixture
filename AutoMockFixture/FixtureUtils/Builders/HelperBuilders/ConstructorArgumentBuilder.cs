using AutoMockFixture.FixtureUtils.Customizations;
using AutoMockFixture.FixtureUtils.Requests.HelperRequests.NonAutoMock;
using SequelPay.DotNetPowerExtensions.Reflection;

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

        var customArgValue = ConstructorArgumentValues.FirstOrDefault(v => IsValidArgumentValue(type, v, ctorArgsRequest));
        if (customArgValue is not null)
        {
            var customValue = customArgValue.Value;
            ctorArgsRequest.SetResult(customValue, this);
            return customValue;
        }

        return base.HandleInternal(ctorArgsRequest, context);
    }

    private bool IsValidArgumentValue(Type type, ConstructorArgumentValue argumentValue, ConstructorArgumentRequest ctorArgsRequest)
    {
        if(argumentValue.ConstructorType is not null
                    && !argumentValue.ConstructorType.IsAssignableFrom(ctorArgsRequest.DeclaringType)) return false;
        if (!string.IsNullOrWhiteSpace(argumentValue.Path)
                    && argumentValue.Path != ctorArgsRequest.Path) return false;
        if (!string.IsNullOrWhiteSpace(argumentValue.ArgumentName)
                    && argumentValue.ArgumentName != ctorArgsRequest.ParameterInfo.Name) return false;

        if (argumentValue.Value is not null) return type.IsInstanceOfType(argumentValue.Value)
                || (argumentValue.Value is IAutoMock mock && type.IsInstanceOfType(mock.GetMocked()));

        return type.IsNullAllowed();
    }

    protected override Type GetRequest(ConstructorArgumentRequest request) => request.ParameterInfo.ParameterType;

    protected override object GetFullRequest(ConstructorArgumentRequest request) => request.ParameterInfo;
}
