using AutoMockFixture.AutoMockUtils;

namespace AutoMockFixture.Tests.AutoMockFixture_Tests;

internal class AutoMockRelay_Tests
{
    [Test]
    public void Test_AutoMockRelay_NotMessingUp_BugRepro()
    {
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateNonAutoMock<InternalSimpleTestClass>();

        obj.Should().NotBeNull();
        obj.InternalTest.Should().NotBeNullOrWhiteSpace();

        obj.InternalTest.Should().StartWith(nameof(InternalSimpleTestClass.InternalTest));

        Assert.DoesNotThrow(() => Guid.Parse(obj.InternalTest!.Replace(nameof(InternalSimpleTestClass.InternalTest), "")));
    }

    internal class TestRelayByAbstractProperty
    {
        public InternalAbstractMethodTestClass? TestProp { get; set; }
    }

    // The main object has a direct test for abstract and interfaces, so we have to test the relay by a property
    [Test]
    public void Test_NonAutoMock_Abstract_ViaRelay()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateNonAutoMock<TestRelayByAbstractProperty>();
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<TestRelayByAbstractProperty>();

        obj.TestProp.Should().NotBeNull();
        var mock = AutoMockHelpers.GetAutoMock(obj.TestProp);
        mock.Should().NotBeNull();

        obj.TestProp!.InternalTest.Should().NotBeNull();
    }
}
