
using AutoFixture.NUnit3;
using AutoMockFixture.FixtureUtils.Customizations;

namespace AutoMockFixture.NUnit3.Moq4.IntegrationTests;

public class IntegrationAutoDataAttribute_Frozen_Tests
{
    public class Test { }
    public class SubTest : Test { }

    [Test]
    [IntegrationAutoData]
    public void Test_Frozen([Frozen] Test t1, Test t2)
    {
        t1.Should().NotBeNull();
        t1.Should().Be(t2);
    }

    [Test]
    [IntegrationAutoData]
    public void Test_Frozen_AutoMock([Frozen] AutoMock<Test> t1, AutoMock<Test> t2)
    {
        t1.Should().NotBeNull();
        t1.Should().Be(t2);
    }

    [Test]
    [IntegrationAutoData]
    public void Test_Frozen_AutoMockAttribute([Frozen][AutoMock] Test t1, [AutoMock] Test t2)
    {
        t1.Should().NotBeNull();
        Assert.DoesNotThrow(() => AutoMock.Get(t1).Should().NotBeNull());

        t1.Should().Be(t2);
    }

    [Test]
    [IntegrationAutoData<SubclassCustomization<Test, SubTest>>]
    public void Test_Frozen_SubclassCustomization([Frozen] Test t1, Test t2)
    {
        t1.Should().NotBeNull();
        t1.Should().BeAssignableTo<SubTest>();

        t1.Should().Be(t2);
    }

    [Test]
    [IntegrationAutoData<SubclassCustomization<Test, SubTest>>]
    public void Test_Frozen_SubclassCustomization_AutoMockAttribute([Frozen][AutoMock] Test t1, [AutoMock] Test t2)
    {
        t1.Should().NotBeNull();
        t1.Should().BeAssignableTo<SubTest>();

        Assert.DoesNotThrow(() => AutoMock.Get((SubTest)t1).Should().NotBeNull());
        t1.Should().Be(t2);
    }
}
