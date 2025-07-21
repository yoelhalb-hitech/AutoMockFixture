
using AutoFixture.NUnit3;
using AutoMockFixture.FixtureUtils.Customizations;

namespace AutoMockFixture.NUnit3.Moq4.UnitTests;

public class UnitAutoDataAttribute_Frozen_Tests
{
    public class Test { }
    public class SubTest : Test { }
    public interface IFaceTest { }
    public class OuterClass
    {
        public IFaceTest? IFace { get; set; }
        public Test? Class { get; set; }
    }

    [Test]
    [UnitAutoData]
    public void Test_Frozen([Frozen] Test t1, Test t2,
                                    [Frozen] IFaceTest iface1, IFaceTest iface2, OuterClass outer)
    {
        t1.Should().NotBeNull();
        Assert.Throws<ArgumentException>(() => AutoMock.Get(t1));
        t1.Should().Be(t2);

        iface1.Should().NotBeNull();
        iface1.Should().Be(iface2);

        outer.IFace.Should().Be(iface1);

        Assert.DoesNotThrow(() => AutoMock.Get(outer.Class));
        outer.Class.Should().NotBe(t1);
    }

    [Test]
    [UnitAutoData]
    public void Test_ReusesFrozen_WhenFreezeAgain([Frozen] Test t1, [Frozen] Test t2,
                                [Frozen] IFaceTest iface1, [Frozen] IFaceTest iface2, [Frozen] OuterClass outer)
    {
        t1.Should().NotBeNull();
        t1.Should().Be(t2);

        iface1.Should().NotBeNull();
        iface1.Should().Be(iface2);

        outer.IFace.Should().Be(iface1);

        Assert.DoesNotThrow(() => AutoMock.Get(outer.Class));
        outer.Class.Should().NotBe(t1);
    }

    [Test]
    [UnitAutoData]
    public void Test_Frozen_AutoMock([Frozen] AutoMock<Test> t1, AutoMock<Test> t2,
        [Frozen] AutoMock<IFaceTest> iface1, AutoMock<IFaceTest> iface2, AutoMock<OuterClass> outer)
    {
        t1.Should().NotBeNull();
        t1.Should().Be(t2);

        iface1.Should().NotBeNull();
        iface1.Should().Be(iface2);

        outer.GetMocked().Should().NotBeNull();
        outer.GetMocked().IFace.Should().Be(iface1.GetMocked()); // Since this is an abstract interface callbase doesn't matter
        outer.GetMocked().Class.Should().NotBe(t1.GetMocked());
    }

    [Test]
    [UnitAutoData]
    public void Test_Frozen_AutoMockAttribute([Frozen][AutoMock] Test t1, [AutoMock] Test t2,
        [Frozen][AutoMock] IFaceTest iface1, [AutoMock] IFaceTest iface2, [AutoMock] OuterClass outer)
    {
        t1.Should().NotBeNull();
        Assert.DoesNotThrow(() => AutoMock.Get(t1).Should().NotBeNull());

        t1.Should().Be(t2);

        iface1.Should().NotBeNull();
        Assert.DoesNotThrow(() => AutoMock.Get(iface1).Should().NotBeNull());
        iface1.Should().Be(iface2);

        outer.IFace.Should().Be(iface1);
        outer.Class.Should().Be(t1);
    }

    [Test]
    [UnitAutoData]
    public void Test_Frozen_AutoMockAttribute_ForNonAutoMock_SameForIface_ButNotClass(
            [Frozen][AutoMock] Test t1, Test t2,
            [Frozen][AutoMock] IFaceTest iface1, IFaceTest iface2,
            [Frozen][AutoMock] OuterClass outer1, OuterClass outer2)
    {
        t1.Should().NotBeNull();
        Assert.DoesNotThrow(() => AutoMock.Get(t1).Should().NotBeNull());

        t1.Should().NotBe(t2);
        Assert.Throws<ArgumentException>(() => AutoMock.Get(t2));

        iface1.Should().NotBeNull();
        Assert.DoesNotThrow(() => AutoMock.Get(iface1).Should().NotBeNull());
        iface1.Should().Be(iface2); // For Iface we anyway do mock so we should not distinguish unless there is an explicit freeze

        outer1.Should().NotBe(outer2);

        outer1.IFace.Should().Be(iface1);
        outer2.IFace.Should().Be(iface1);

        outer1.Class.Should().Be(t1);
        outer2.Class.Should().Be(t1);
    }

    [Test]
    [UnitAutoData]
    public void Test_Frozen_CallBaseAttribute_ForNonAutoMock_Freezes_EvenNonExplicitlyFrozen(
        [Frozen][CallBase] Test t1, Test t2, Test t3,
        [Frozen][CallBase] IFaceTest iface1, IFaceTest iface2, IFaceTest iface3,
        [Frozen][CallBase] OuterClass outer1, OuterClass outer2, OuterClass outer3)
    {
        t1.Should().NotBeNull();
        t2.Should().NotBeNull();
        t1.Should().NotBe(t2);
        t3.Should().Be(t2);

        iface1.Should().NotBeNull();
        iface2.Should().NotBeNull();

        iface1.Should().NotBe(iface2);
        iface3.Should().Be(iface2);

        outer1.Should().NotBe(outer2);
        outer3.Should().Be(outer2);

        outer1.IFace.Should().Be(iface1);
        outer2.IFace.Should().Be(iface2);
    }

    [Test]
    [UnitAutoData]
    public void Test_Frozen_AutoMockAttribute_ForNonAutoMock_WhenNonAutoMockFreeze_SameForIface_ButNotClass(
        [Frozen][AutoMock] Test t1, [Frozen] Test t2,
        [Frozen][AutoMock] IFaceTest iface1, [Frozen] IFaceTest iface2,
        [Frozen][AutoMock] OuterClass outer1, [Frozen] OuterClass outer2)
    {
        t1.Should().NotBeNull();
        Assert.DoesNotThrow(() => AutoMock.Get(t1).Should().NotBeNull());

        t1.Should().NotBe(t2);
        Assert.Throws<ArgumentException>(() => AutoMock.Get(t2));

        iface1.Should().NotBeNull();
        Assert.DoesNotThrow(() => AutoMock.Get(iface1).Should().NotBeNull());
        iface1.Should().Be(iface2); // For Iface we anyway do mock so we should not distinguish unless there is an explicit freeze

        outer1.Should().NotBe(outer2);

        outer1.IFace.Should().Be(iface1);
        outer2.IFace.Should().Be(iface1);

        outer1.Class.Should().Be(t1);
        outer2.Class.Should().Be(t1);
    }

    // TODO... add all tests for iface
    // TODO... add also the tests for integration

    [Test]
    [UnitAutoData<SubclassCustomization<Test, SubTest>>]
    public void Test_Frozen_SubclassCustomization([Frozen] Test t1, Test t2)
    {
        t1.Should().NotBeNull();
        t1.Should().BeAssignableTo<SubTest>();

        t1.Should().Be(t2);
    }

    [Test]
    [UnitAutoData<SubclassCustomization<Test, SubTest>>]
    public void Test_Frozen_SubclassCustomization_AutoMockAttribute([Frozen][AutoMock] Test t1, [AutoMock] Test t2)
    {
        t1.Should().NotBeNull();
        t1.Should().BeAssignableTo<SubTest>();

        Assert.DoesNotThrow(() => AutoMock.Get((SubTest)t1).Should().NotBeNull());
        t1.Should().Be(t2);
    }

    [Test]
    [UnitAutoData(typeof(Test))]
    public void Test_Frozen_ByUnitAutoDataAttribute(Test t1, Test t2)
    {
        t1.Should().NotBeNull();
        t1.Should().Be(t2);
    }

    [Test]
    [UnitAutoData(typeof(AutoMock<Test>))]
    public void Test_Frozen_AutoMock_ByUnitAutoDataAttribute(AutoMock<Test> t1, AutoMock<Test> t2)
    {
        t1.Should().NotBeNull();
        t1.Should().Be(t2);
    }

    [Test]
    [UnitAutoData(typeof(Test))]
    public void Test_Frozen_AutoMockAttribute_ByUnitAutoDataAttribute([AutoMock] Test t1, [AutoMock] Test t2)
    {
        t1.Should().NotBeNull();
        Assert.DoesNotThrow(() => AutoMock.Get(t1).Should().NotBeNull());

        t1.Should().Be(t2);
    }

    [Test]
    [UnitAutoData<SubclassCustomization<Test, SubTest>>(typeof(Test))]
    public void Test_Frozen_SubclassCustomization_ByUnitAutoDataAttribute(Test t1, Test t2)
    {
        t1.Should().NotBeNull();
        t1.Should().BeAssignableTo<SubTest>();

        t1.Should().Be(t2);
    }

    [Test]
    [UnitAutoData<SubclassCustomization<Test, SubTest>>(typeof(Test))]
    public void Test_Frozen_SubclassCustomization_AutoMockAttribute_ByUnitAutoDataAttribute([AutoMock] Test t1, [AutoMock] Test t2)
    {
        t1.Should().NotBeNull();
        t1.Should().BeAssignableTo<SubTest>();

        Assert.DoesNotThrow(() => AutoMock.Get((SubTest)t1).Should().NotBeNull());
        t1.Should().Be(t2);
    }
}
