using AutoMockFixture.AutoMockUtils;
using AutoMockFixture.FixtureUtils.MethodInvokers;

namespace AutoMockFixture.Tests.FixtureUtils.MethodInvokers;

internal class MethodInvokerWithRecursion_Tests
{
    [Test]
    public void Test_DoesNotAllow_AutoMock()
    {
        var mi = new MethodInvokerWithRecursion(AutoMock.Of<IMethodQuery>(), AutoMock.Of<IAutoMockHelpers>());

        var result = mi.Create(typeof(AutoMock<string>), AutoMock.Of<ISpecimenContext>());

        result.Should().BeAssignableTo<NoSpecimen>();
    }
}
