using AutoMockFixture.AutoMockUtils;

namespace AutoMockFixture.Tests.AutoMockUtils;

internal class AutoMockHelpers_Tests
{
    [Test]
    public void Test_GetFromObj_WorksWith_Delegate()
    {
        var mock = new AutoMock<Action>();

        AutoMockHelpers.GetFromObj(mock.Object).Should().NotBeNull();
    }
}
