
using System.Reflection;

namespace AutoMockFixture.Moq4.IntegrationTests.AutoMockFixture_Tests;

internal class ReflectionObjects_Tests
{
    [Test]
    public void Test_Works_WithReflectionObjects()
    {
        var fixture = new AbstractAutoMockFixture();

        Assert.DoesNotThrow(() => fixture.CreateNonAutoMock<Type>());
        Assert.DoesNotThrow(() => fixture.CreateNonAutoMock<TypeInfo>());
        Assert.DoesNotThrow(() => fixture.CreateNonAutoMock<MethodInfo>());
        Assert.DoesNotThrow(() => fixture.CreateNonAutoMock<PropertyInfo>());
        Assert.DoesNotThrow(() => fixture.CreateNonAutoMock<EventInfo>());
        Assert.DoesNotThrow(() => fixture.CreateNonAutoMock<FieldInfo>());
        Assert.DoesNotThrow(() => fixture.CreateNonAutoMock<ParameterInfo>());

        fixture.CreateNonAutoMock<Type>().Should().NotBeNull();
        fixture.CreateNonAutoMock<TypeInfo>().Should().NotBeNull();
        fixture.CreateNonAutoMock<MethodInfo>().Should().NotBeNull();
        fixture.CreateNonAutoMock<PropertyInfo>().Should().NotBeNull();
        fixture.CreateNonAutoMock<EventInfo>().Should().NotBeNull();
        fixture.CreateNonAutoMock<FieldInfo>().Should().NotBeNull();
        fixture.CreateNonAutoMock<ParameterInfo>().Should().NotBeNull();
    }
}
