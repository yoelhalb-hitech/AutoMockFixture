using AutoMockFixture.NUnit3;

namespace AutoMockFixture.Moq4.IntegrationTests.AutoMockProxy;

public class DefaultInterfaceForInterface_Tests
{
    public interface IWithDefault
    {
        public int TestMethod() => 10;
        public int TestProp { get => 20; set => throw new InvalidOperationException("Test Exception"); }
        public event EventHandler TestEvent { add => throw new InvalidOperationException("Test Exception"); remove => throw new InvalidOperationException("Test Exception"); }
    }

    public interface IWithDefaultSub : IWithDefault { }

    public interface IWithReimplmentedDefault : IWithDefault
    {
        int IWithDefault.TestMethod() => 20;
        int IWithDefault.TestProp { get => 30; set => throw new ArgumentOutOfRangeException("Test Exception"); }
        event EventHandler IWithDefault.TestEvent { add => throw new ArgumentOutOfRangeException("Test Exception"); remove => throw new ArgumentOutOfRangeException("Test Exception"); }
    }

    public interface IWithReimplmentedDefaultSub : IWithReimplmentedDefault { }

    [Test]
    [TestCase<IWithDefault>]
    [TestCase<IWithDefaultSub>]
    [TestCase<IWithReimplmentedDefault>]
    [TestCase<IWithReimplmentedDefaultSub>]
    public void Test_TypeWithDefaultImplementation_DoesNotCallbase_OnNonCallbase<T>() where T : class, IWithDefault
    {
        var mock = new AutoMock<T>() { CallBase = false };
        var obj = mock.Object as IWithDefault;

        obj.TestMethod().Should().Be(0);
        obj.TestProp.Should().Be(0);

        Assert.DoesNotThrow(() => obj.TestProp = 70);
        Assert.DoesNotThrow(() => obj.TestEvent += (o, e) => { });
        Assert.DoesNotThrow(() => obj.TestEvent -= (o, e) => { });
    }

    [Test]
    [TestCase<IWithDefault>]
    [TestCase<IWithDefaultSub>]
    public void Test_TypeWithDefaultImplementation_Callsbase_OnCallbase<T>() where T : class, IWithDefault
    {
        var mock = new AutoMock<T>() { CallBase = true };

        var obj = mock.Object as IWithDefault;
        obj.TestMethod().Should().Be(10);
        obj.TestProp.Should().Be(20);

        Assert.Throws<InvalidOperationException>(() => obj.TestProp = 70, "Test Exception");
        Assert.Throws<InvalidOperationException>(() => obj.TestEvent += (o, e) => { }, "Test Exception");
        Assert.Throws<InvalidOperationException>(() => obj.TestEvent -= (o, e) => { }, "Test Exception");
    }

    [Test]
    [TestCase<IWithReimplmentedDefault>]
    [TestCase<IWithReimplmentedDefaultSub>]
    public void Test_TypeWithReimplemted_Callsbase_OnCallbase<T>() where T : class, IWithDefault
    {
        var mock = new AutoMock<T>() { CallBase = true };

        var obj = mock.Object as IWithDefault;
        obj.TestMethod().Should().Be(20);
        obj.TestProp.Should().Be(30);

        Assert.Throws<ArgumentOutOfRangeException>(() => obj.TestProp = 70, "Test Exception");
        Assert.Throws<ArgumentOutOfRangeException>(() => obj.TestEvent += (o, e) => { }, "Test Exception");
        Assert.Throws<ArgumentOutOfRangeException>(() => obj.TestEvent -= (o, e) => { }, "Test Exception");
    }


    [Test]
    [TestCase<IWithDefault>(true)]
    [TestCase<IWithDefault>(false)]
    [TestCase<IWithDefaultSub>(true)]
    [TestCase<IWithDefaultSub>(false)]
    [TestCase<IWithReimplmentedDefault>(true)]
    [TestCase<IWithReimplmentedDefault>(false)]
    [TestCase<IWithReimplmentedDefaultSub>(true)]
    [TestCase<IWithReimplmentedDefaultSub>(false)]
    public void Test_TypeWithDefaultImplementation_SetsUpCorrectly<T>(bool callbase) where T : class, IWithDefault
    {
        var mock = new AutoMock<T>() { CallBase = callbase };
        mock.As<IWithDefault>().Setup(i => i.TestMethod()).Returns(50);
        mock.As<IWithDefault>().SetupGet(i => i.TestProp).Returns(60);

        int propVal = 0;
        //mock.As<IWithDefault>().SetupSet(i => propVal = i.TestProp); // TODO...
        //mock.As<IWithDefault>().SetupAdd(i => propVal = i.TestProp); // TODO...
        //mock.As<IWithDefault>().SetupARemove(i => propVal = i.TestProp); // TODO...

        var obj = mock.Object as IWithDefault;
        obj.TestMethod().Should().Be(50);
        obj.TestProp.Should().Be(60);

        //Assert.DoesNotThrow(() => obj.TestProp = 70);
        //propVal.Should().Be(70);
    }
}
