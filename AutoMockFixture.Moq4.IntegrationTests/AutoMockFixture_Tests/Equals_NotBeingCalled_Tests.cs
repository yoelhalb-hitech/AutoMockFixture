using Moq;
using static AutoMockFixture.MethodSetupTypes;

namespace AutoMockFixture.Tests.AutoMockFixture_Tests;

internal class Equals_NotBeingCalled_Tests
{
    [Test]
    public void Test_WorksCorrectly(
        [Values(true, false)] bool callBase,
        [Values(Eager, LazySame, LazyDifferent)] MethodSetupTypes setupType)
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.MethodSetupType = setupType;

        var obj = fixture.CreateAutoMock<WithComplexTestClass>(callBase);
        obj.Should().BeAutoMock();

        AutoMock.Get(obj)!.Verify(o => o.Equals(It.IsAny<object>()), Times.Never());
    }
}
