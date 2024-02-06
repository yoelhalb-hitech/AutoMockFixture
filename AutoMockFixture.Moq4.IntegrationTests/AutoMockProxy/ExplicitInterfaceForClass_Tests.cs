using AutoMockFixture.NUnit3;
using Moq;

namespace AutoMockFixture.Moq4.IntegrationTests.AutoMockProxy;

public class ExplicitInterfaceForClass_Tests
{
    public interface IWithNoDefault
    {
        public int TestMethod();
        public int TestMethod(int i);
        public int TestProp { get; set; }
        public event EventHandler TestEvent;
    }

    public class TypeWithExplicit : IWithNoDefault
    {
        int IWithNoDefault.TestMethod() => 10;
        int IWithNoDefault.TestMethod(int i) => 15;
        int IWithNoDefault.TestProp { get => 20; set => throw new InvalidOperationException("Test Exception"); }
        event EventHandler IWithNoDefault.TestEvent { add => throw new InvalidOperationException("Test Exception"); remove => throw new InvalidOperationException("Test Exception"); }
    }

    public class TypeWithExplicitSub : TypeWithExplicit
    {
    }

    public class TypeWithReimplmented : TypeWithExplicit, IWithNoDefault
    {
        int IWithNoDefault.TestMethod() => 30;
        int IWithNoDefault.TestMethod(int i) => 35;
        int IWithNoDefault.TestProp { get => 40; set => throw new ArgumentOutOfRangeException("Test Exception"); }
        event EventHandler IWithNoDefault.TestEvent { add => throw new ArgumentOutOfRangeException("Test Exception"); remove => throw new ArgumentOutOfRangeException("Test Exception"); }
    }

    public class TypeWithReimplmentedSub : TypeWithReimplmented
    {
    }


    [Test]
    [TestCase<TypeWithExplicit>]
    [TestCase<TypeWithExplicitSub>]
    [TestCase<TypeWithReimplmented>]
    [TestCase<TypeWithReimplmentedSub>]

    public void Test_TypeWithExplicitImplementation_DoesNotCallBase_OnNonCallBase<T>() where T : class, IWithNoDefault
    {
        var mock = new AutoMock<T>() { CallBase = false };

        var obj = mock.Object as IWithNoDefault;
        obj.TestMethod().Should().NotBe(10);
        obj.TestMethod(0).Should().NotBe(15);
        obj.TestProp.Should().NotBe(20);

        Assert.DoesNotThrow(() => obj.TestProp = 70);
        Assert.DoesNotThrow(() => obj.TestEvent += (o, e) => { });
        Assert.DoesNotThrow(() => obj.TestEvent -= (o, e) => { });
    }

    [Test]
    [TestCase<TypeWithExplicit>]
    [TestCase<TypeWithExplicitSub>]
    public void Test_TypeWithExplicitImplementation_Callsbase_OnCallBase<T>() where T : class, IWithNoDefault
    {
        var mock = new AutoMock<T>() { CallBase = true };
        mock.As<IWithNoDefault>();
        var obj = mock.Object;

        obj.TestMethod().Should().Be(10);
        obj.TestMethod(0).Should().Be(15);
        obj.TestProp.Should().Be(20);

        Assert.Throws<InvalidOperationException>(() => obj.TestProp = 70, "Test Exception");
        Assert.Throws<InvalidOperationException>(() => obj.TestEvent += (o, e) => { }, "Test Exception");
        Assert.Throws<InvalidOperationException>(() => obj.TestEvent -= (o, e) => { }, "Test Exception");
    }

    [Test]
    [TestCase<TypeWithReimplmented>]
    [TestCase<TypeWithReimplmentedSub>]
    public void Test_TypeWithReimplementation_Callsbase_OnCallBase<T>() where T : class, IWithNoDefault
    {
        var mock = new AutoMock<T>() { CallBase = true };
        var obj = mock.Object;

        obj.TestMethod().Should().Be(30);
        obj.TestMethod(0).Should().Be(35);
        obj.TestProp.Should().Be(40);

        Assert.Throws<ArgumentOutOfRangeException>(() => obj.TestProp = 70, "Test Exception");
        Assert.Throws<ArgumentOutOfRangeException>(() => obj.TestEvent += (o, e) => { }, "Test Exception");
        Assert.Throws<ArgumentOutOfRangeException>(() => obj.TestEvent -= (o, e) => { }, "Test Exception");
    }

    [Test]
    [TestCase<TypeWithExplicit>(true)]
    [TestCase<TypeWithExplicit>(false)]
    [TestCase<TypeWithExplicitSub>(true)]
    [TestCase<TypeWithExplicitSub>(false)]
    [TestCase<TypeWithReimplmented>(true)]
    [TestCase<TypeWithReimplmented>(false)]
    [TestCase<TypeWithReimplmentedSub>(true)]
    [TestCase<TypeWithReimplmentedSub>(false)]
    public void Test_TypeWithExplicitImplementation_SetsUpCorrectly<T>(bool callBase) where T : class, IWithNoDefault
    {
        var mock = new AutoMock<T>() { CallBase = callBase };
        mock.As<IWithNoDefault>().Setup(i => i.TestMethod()).Returns(50);
        mock.As<IWithNoDefault>().SetupGet(i => i.TestProp).Returns(60);

        int propVal = 0;
        mock.As<IWithNoDefault>()
            .SetupSet(i => i.TestProp = It.IsAny<int>())
            .Callback<int>(i => propVal = i);

        var obj = mock.Object as IWithNoDefault;
        obj.TestMethod().Should().Be(50);
        obj.TestProp.Should().Be(60);

        Assert.DoesNotThrow(() => obj.TestProp = 70);
        propVal.Should().Be(70);

        mock.As<IWithNoDefault>().Verify(m => m.TestMethod());
        mock.As<IWithNoDefault>().VerifyGet(m => m.TestProp);
        mock.As<IWithNoDefault>().VerifySet(m => m.TestProp = 70);
    }

    [Test]
    [TestCase<TypeWithExplicit>]
    [TestCase<TypeWithExplicitSub>]
    [TestCase<TypeWithReimplmented>]
    [TestCase<TypeWithReimplmentedSub>]
    // Moq has a bug that it calls base on .Callback for events (unlike properties and methods) so we have to test it only on non callBase
    public void Test_TypeWithDefaultImplementation_SetsUpEventsCorrectly_ForNonCallBase<T>() where T : class, IWithNoDefault
    {
        var mock = new AutoMock<T>() { CallBase = false };

        EventHandler? evt = null;
        mock.As<IWithNoDefault>()
            .SetupAdd(i => i.TestEvent += It.IsAny<EventHandler>())
            .Callback<EventHandler>(e => evt = e);

        mock.As<IWithNoDefault>()
            .SetupRemove(i => i.TestEvent -= It.IsAny<EventHandler>())
            .Callback<EventHandler>(e => evt = null);

        var obj = mock.Object as IWithNoDefault;

        var handler = (EventHandler)((obj, e) => { });
        Assert.DoesNotThrow(() => obj.TestEvent += handler);
        evt.Should().NotBeNull();
        evt.Should().Be(handler);

        Assert.DoesNotThrow(() => obj.TestEvent -= handler);
        evt.Should().BeNull();

        mock.As<IWithNoDefault>().VerifyAdd(m => m.TestEvent += handler);
        mock.As<IWithNoDefault>().VerifyRemove(m => m.TestEvent -= handler);
    }
}
