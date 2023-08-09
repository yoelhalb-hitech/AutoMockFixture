using AutoMockFixture.NUnit3;

namespace AutoMockFixture.Moq4.IntegrationTests.AutoMockProxy;

public class InterfaceDefaultViaNonDefaultForInterface_Tests
{
    public interface IWithDefaultBase
    {
        public int TestMethod();
        public int TestProp { get; set; }
        public event EventHandler TestEvent;
    }
    public interface IWithDefault : IWithDefaultBase
    {
        int IWithDefaultBase.TestMethod() => 10;
        int IWithDefaultBase.TestProp { get => 20; set => throw new InvalidOperationException(); }
        event EventHandler IWithDefaultBase.TestEvent { add => throw new InvalidOperationException(); remove => throw new InvalidOperationException(); }
    }

    public interface IWithDefaultSub : IWithDefault { }

    public interface IWithReimplmentedDefault : IWithDefault
    {
        int IWithDefaultBase.TestMethod() => 20;
        int IWithDefaultBase.TestProp { get => 30; set => throw new ArgumentOutOfRangeException(); }
        event EventHandler IWithDefaultBase.TestEvent { add => throw new ArgumentOutOfRangeException(); remove => throw new ArgumentOutOfRangeException(); }
    }

    public interface IWithReimplmentedDefaultSub : IWithReimplmentedDefault { }


    [Test]
    [TestCase<IWithDefault>]
    [TestCase<IWithDefaultSub>]
    [TestCase<IWithReimplmentedDefault>]
    [TestCase<IWithReimplmentedDefaultSub>]
    public void Test_TypeWithDefaultImplementation_DoesNotCallbase_OnNonCallbase<T>() where T : class, IWithDefaultBase
    {
        var mock = new AutoMock<T>() { CallBase = false };

        var obj = mock.Object as IWithDefaultBase;
        obj.TestMethod().Should().Be(0);
        obj.TestProp.Should().Be(0);

        Assert.DoesNotThrow(() => obj.TestProp = 70);
        Assert.DoesNotThrow(() => obj.TestEvent += (o, e) => { });
        Assert.DoesNotThrow(() => obj.TestEvent -= (o, e) => { });
    }

    [Test]
    [TestCase<IWithDefault>]
    [TestCase<IWithDefaultSub>]
    public void Test_TypeWithDefaultImplementation_Callsbase_OnCallbase<T>() where T : class, IWithDefaultBase
    {
        var mock = new AutoMock<T>() { CallBase = true };
        var obj = mock.Object as IWithDefaultBase;

        obj.TestMethod().Should().Be(10);
        obj.TestProp.Should().Be(20);

        Assert.Throws<InvalidOperationException>(() => obj.TestProp = 70);
        Assert.Throws<InvalidOperationException>(() => obj.TestEvent += (o, e) => { });
        Assert.Throws<InvalidOperationException>(() => obj.TestEvent -= (o, e) => { });
    }

    [Test]
    [TestCase<IWithReimplmentedDefault>]
    [TestCase<IWithReimplmentedDefaultSub>]
    public void Test_TypeWithReimplemented_Callsbase_OnCallbase<T>() where T : class, IWithDefaultBase
    {
        var mock = new AutoMock<T>() { CallBase = true };

        var obj = mock.Object;
        obj.TestMethod().Should().Be(20);
        obj.TestProp.Should().Be(30);

        Assert.Throws<ArgumentOutOfRangeException>(() => obj.TestProp = 70);
        Assert.Throws<ArgumentOutOfRangeException>(() => obj.TestEvent += (o, e) => { });
        Assert.Throws<ArgumentOutOfRangeException>(() => obj.TestEvent -= (o, e) => { });
    }


    [Test]
    [TestCase<IWithDefault>(true)]
    [TestCase<IWithDefault>(false)]
    [TestCase<IWithDefaultSub>(true)]
    [TestCase<IWithDefaultSub>(false)]
    [TestCase<IWithReimplmentedDefaultSub>(true)]
    [TestCase<IWithReimplmentedDefaultSub>(false)]
    [TestCase<IWithReimplmentedDefaultSub>(true)]
    [TestCase<IWithReimplmentedDefaultSub>(false)]
    public void Test_TypeWithDefaultImplementation_SetsUpCorrectly<T>(bool callbase) where T : class, IWithDefaultBase
    {
        var mock = new AutoMock<T>() { CallBase = callbase };
        mock.As<IWithDefaultBase>().Setup(i => i.TestMethod()).Returns(50);
        mock.As<IWithDefaultBase>().SetupGet(i => i.TestProp).Returns(60);

        int propVal = 0;
        //mock.As<IWithDefault>().SetupSet(i => propVal = i.TestProp);

        var obj = mock.Object as IWithDefault;
        obj.TestMethod().Should().Be(50);
        //obj.TestProp.Should().Be(60);

        //Assert.DoesNotThrow(() => obj.TestProp = 70);
        //propVal.Should().Be(70);
    }
}
