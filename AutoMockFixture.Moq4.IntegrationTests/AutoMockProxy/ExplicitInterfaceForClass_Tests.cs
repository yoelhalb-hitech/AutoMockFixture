using AutoMockFixture.NUnit3;

namespace AutoMockFixture.Moq4.IntegrationTests.AutoMockProxy;

public class ExplicitInterfaceForClass_Tests
{
    public interface IWithNoDefault
    {
        public int TestMethod();
        public int TestProp { get; set; }
        public event EventHandler TestEvent;
    }

    public class TypeWithExplicit : IWithNoDefault
    {
        int IWithNoDefault.TestMethod() => 10;
        int IWithNoDefault.TestProp { get => 20; set => throw new InvalidOperationException(); }
        event EventHandler IWithNoDefault.TestEvent { add => throw new InvalidOperationException(); remove => throw new InvalidOperationException(); }
    }

    public class TypeWithExplicitSub : TypeWithExplicit
    {
    }

    public class TypeWithReimplmented : TypeWithExplicit, IWithNoDefault
    {
        int IWithNoDefault.TestMethod() => 30;
        int IWithNoDefault.TestProp { get => 40; set => throw new ArgumentOutOfRangeException(); }
        event EventHandler IWithNoDefault.TestEvent { add => throw new ArgumentOutOfRangeException(); remove => throw new ArgumentOutOfRangeException(); }
    }

    public class TypeWithReimplmentedSub : TypeWithReimplmented
    {
    }


    [Test]
    [TestCase<TypeWithExplicit>]
    [TestCase<TypeWithExplicitSub>]
    [TestCase<TypeWithReimplmented>]
    [TestCase<TypeWithReimplmentedSub>]

    public void Test_TypeWithExplicitImplementation_DoesNotCallbase_OnNonCallbase<T>() where T : class, IWithNoDefault
    {
        var mock = new AutoMock<T>() { CallBase = false };

        var obj = mock.Object as IWithNoDefault;
        obj.TestMethod().Should().NotBe(10);
        obj.TestProp.Should().NotBe(20);

        Assert.DoesNotThrow(() => obj.TestProp = 70);
        Assert.DoesNotThrow(() => obj.TestEvent += (o, e) => { });
        Assert.DoesNotThrow(() => obj.TestEvent -= (o, e) => { });
    }

    [Test]
    [TestCase<TypeWithExplicit>]
    [TestCase<TypeWithExplicitSub>]
    public void Test_TypeWithExplicitImplementation_Callsbase_OnCallbase<T>() where T : class, IWithNoDefault
    {
        var mock = new Moq.Mock<T>() { CallBase = true };
        var obj = mock.Object;

        obj.TestMethod().Should().Be(10);
        obj.TestProp.Should().Be(20);

        Assert.Throws<InvalidOperationException>(() => obj.TestProp = 70);
        Assert.Throws<InvalidOperationException>(() => obj.TestEvent += (o, e) => { });
        Assert.Throws<InvalidOperationException>(() => obj.TestEvent -= (o, e) => { });
    }

    [Test]
    [TestCase<TypeWithReimplmented>]
    [TestCase<TypeWithReimplmentedSub>]
    public void Test_TypeWithReimplementation_Callsbase_OnCallbase<T>() where T : class, IWithNoDefault
    {
        var mock = new AutoMock<T>() { CallBase = true };
        var obj = mock.Object;

        obj.TestMethod().Should().Be(30);
        obj.TestProp.Should().Be(40);

        Assert.Throws<ArgumentOutOfRangeException>(() => obj.TestProp = 70);
        Assert.Throws<ArgumentOutOfRangeException>(() => obj.TestEvent += (o, e) => { });
        Assert.Throws<ArgumentOutOfRangeException>(() => obj.TestEvent -= (o, e) => { });
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
    public void Test_TypeWithExplicitImplementation_SetsUpCorrectly<T>(bool callbase) where T : class, IWithNoDefault
    {
        var mock = new AutoMock<TypeWithExplicit>() { CallBase = callbase };
        mock.As<IWithNoDefault>().Setup(i => i.TestMethod()).Returns(50);
        mock.As<IWithNoDefault>().SetupGet(i => i.TestProp).Returns(60);

        //int propVal = 0;
        //mock.As<IWithNoDefault>().SetupSet(i => propVal = i.TestProp);

        var obj = mock.Object as IWithNoDefault;
        obj.TestMethod().Should().Be(50);
        obj.TestProp.Should().Be(60);

        //Assert.DoesNotThrow(() => obj.TestProp = 70);
        //propVal.Should().Be(70);
    }
}
