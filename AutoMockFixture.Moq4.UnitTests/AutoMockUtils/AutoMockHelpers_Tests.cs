using AutoMockFixture.Moq4.AutoMockUtils;

namespace AutoMockFixture.Tests.AutoMockUtils;

internal class AutoMockHelpers_Tests
{
    [Test]
    public void Test_GetFromObj_WorksWith_Delegate()
    {
        var mock = new AutoMock<Action>();

        new AutoMockHelpers().GetFromObj(mock.Object).Should().NotBeNull();
    }

    [Test]
    public void Test_ThrowsCorrectly_ForObject()
    {
        var fixture = new AbstractAutoMockFixture();

        var result = fixture.CreateAutoMock(typeof(Action));
        AutoMock.IsAutoMock(result).Should().BeTrue();

        var ex = Assert.Throws<InvalidCastException>(() => AutoMock.Get(result));
        ex.Message.Should().Be("Mock is of type AutoMock<Action> and cannot be casted to AutoMock<object>");
    }
}
