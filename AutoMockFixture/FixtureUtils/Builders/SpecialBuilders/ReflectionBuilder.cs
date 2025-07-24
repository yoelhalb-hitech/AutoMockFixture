using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;
using AutoMockFixture.FixtureUtils.Requests.SpecialRequests;

namespace AutoMockFixture.FixtureUtils.Builders.SpecialBuilders;

internal class ReflectionBuilder : ISpecimenBuilder
{
    class StubType
    {
        public void StubMethod(int stubParamter) { }
        public int StubProperty { get; set; }
        public int StubField;
        public event EventHandler StubEvent;
    }


    public object Create(object request, ISpecimenContext context)
    {
        if (request is not IRequestWithType typeRequest || typeRequest.Request is not Type type) return new NoSpecimen();
        if (request is AutoMockDirectRequest) return new NoSpecimen();

        if(type != typeof(Type) && type.Namespace != "System.Reflection") return new NoSpecimen();

        return type switch
        {
            Type t when t == typeof(Type) => typeof(StubType),
            Type t when t == typeof(TypeInfo) => typeof(StubType).GetTypeInfo(),
            Type t when t == typeof(MethodInfo) || t == typeof(MethodBase)
                    => typeof(StubType).GetMethod("StubMethod")!,
            Type t when t == typeof(PropertyInfo) => typeof(StubType).GetProperty("StubProperty")!,
            Type t when t == typeof(FieldInfo) => typeof(StubType).GetField("StubField")!,
            Type t when t == typeof(EventInfo) => typeof(StubType).GetEvent("StubEvent")!,
            Type t when t == typeof(ParameterInfo) => typeof(StubType).GetMethod("StubMethod")!.GetParameters().First(),
            _ => new NoSpecimen(),
        };
    }
}
