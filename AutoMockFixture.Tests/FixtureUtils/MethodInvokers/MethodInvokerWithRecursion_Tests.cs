using AutoMockFixture.AutoMockUtils;
using AutoMockFixture.FixtureUtils.MethodInvokers;
using AutoMockFixture.FixtureUtils.MethodQueries;
using Moq;

namespace AutoMockFixture.Tests.FixtureUtils.MethodInvokers;

internal class MethodInvokerWithRecursion_Tests
{
    [Test]
    public void Test_DoesNotAllow_AutoMock()
    {
        var fixture = new AbstractAutoMockFixture();

        var mi = new MethodInvokerWithRecursion(new CustomModestConstructorQuery(fixture.AutoMockHelpers), fixture.AutoMockHelpers);

        var result = mi.Create(typeof(AutoMock<string>), AutoMock.Of<ISpecimenContext>());

        result.Should().BeAssignableTo<NoSpecimen>();
    }
}
