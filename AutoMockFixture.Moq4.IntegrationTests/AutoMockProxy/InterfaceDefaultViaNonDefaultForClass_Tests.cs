using AutoMockFixture.NUnit3;
using Moq;

namespace AutoMockFixture.Moq4.IntegrationTests.AutoMockProxy;

public class InterfaceDefaultViaNonDefaultForClass_Tests
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
        int IWithDefaultBase.TestProp { get => 20; set => throw new InvalidOperationException("Test Exception"); }
        event EventHandler IWithDefaultBase.TestEvent { add => throw new InvalidOperationException("Test Exception"); remove => throw new InvalidOperationException("Test Exception"); }
    }

    public interface IWithReimplmentedDefault : IWithDefault
    {
        int IWithDefaultBase.TestMethod() => 20;
        int IWithDefaultBase.TestProp { get => 30; set => throw new ArgumentOutOfRangeException("Test Exception"); }
        event EventHandler IWithDefaultBase.TestEvent { add => throw new ArgumentOutOfRangeException("Test Exception"); remove => throw new ArgumentOutOfRangeException("Test Exception"); }
    }

    public class TypeWithDefault : IWithDefault
    {
    }

    public class TypeWithDefaultSub : TypeWithDefault
    {
    }


    public class TypeWithReimplmentedDefault : IWithReimplmentedDefault
    {
    }

    public class TypeWithReimplmentedDefaultSub : TypeWithReimplmentedDefault
    {
    }

    public class TypeWithReimplmentedInClass : IWithDefault
    {
        int IWithDefaultBase.TestMethod() => 40;
        int IWithDefaultBase.TestProp { get => 50; set => throw new AggregateException("Test Exception"); }
        event EventHandler IWithDefaultBase.TestEvent { add => throw new AggregateException("Test Exception"); remove => throw new AggregateException("Test Exception"); }

    }

    public class TypeWithReimplmentedInClassSub : TypeWithReimplmentedInClass
    {
    }

    [Test]
    [TestCase<TypeWithDefault>]
    [TestCase<TypeWithDefaultSub>]
    [TestCase<TypeWithReimplmentedDefault>]
    [TestCase<TypeWithReimplmentedDefaultSub>]
    [TestCase<TypeWithReimplmentedInClass>]
    [TestCase<TypeWithReimplmentedInClassSub>]
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
    [TestCase<TypeWithDefault>]
    [TestCase<TypeWithDefaultSub>]
    public void Test_TypeWithDefaultImplementation_Callsbase_OnCallbase<T>() where T : class, IWithDefaultBase
    {
        var mock = new AutoMock<T>() { CallBase = true };

        var obj = mock.Object as IWithDefaultBase;
        obj.TestMethod().Should().Be(10);
        obj.TestProp.Should().Be(20);

        Assert.Throws<InvalidOperationException>(() => obj.TestProp = 70, "Test Exception");
        Assert.Throws<InvalidOperationException>(() => obj.TestEvent += (o, e) => { }, "Test Exception");
        Assert.Throws<InvalidOperationException>(() => obj.TestEvent -= (o, e) => { }, "Test Exception");
    }

    [Test]
    [TestCase<TypeWithReimplmentedDefault>]
    [TestCase<TypeWithReimplmentedDefaultSub>]
    public void Test_TypeWithReimplemented_Callsbase_OnCallbase<T>() where T : class, IWithDefaultBase
    {
        var mock = new AutoMock<T>() { CallBase = true };

        var obj = mock.Object;
        obj.TestMethod().Should().Be(20);
        obj.TestProp.Should().Be(30);

        Assert.Throws<ArgumentOutOfRangeException>(() => obj.TestProp = 70, "Test Exception");
        Assert.Throws<ArgumentOutOfRangeException>(() => obj.TestEvent += (o, e) => { }, "Test Exception");
        Assert.Throws<ArgumentOutOfRangeException>(() => obj.TestEvent -= (o, e) => { }, "Test Exception");
    }

    [Test]
    [TestCase<TypeWithReimplmentedInClass>]
    [TestCase<TypeWithReimplmentedInClassSub>]
    public void Test_TypeWithReimplmentedInClass_Callsbase_OnCallbase<T>() where T : class, IWithDefaultBase
    {
        var mock = new AutoMock<T>() { CallBase = true };

        var obj = mock.Object;
        obj.TestMethod().Should().Be(40);
        obj.TestProp.Should().Be(50);

        Assert.Throws<AggregateException>(() => obj.TestProp = 70, "Test Exception");
        Assert.Throws<AggregateException>(() => obj.TestEvent += (o, e) => { }, "Test Exception");
        Assert.Throws<AggregateException>(() => obj.TestEvent -= (o, e) => { }, "Test Exception");
    }


    [Test]
    [TestCase<TypeWithDefault>(true)]
    [TestCase<TypeWithDefault>(false)]
    [TestCase<TypeWithDefaultSub>(true)]
    [TestCase<TypeWithDefaultSub>(false)]
    [TestCase<TypeWithReimplmentedDefaultSub>(true)]
    [TestCase<TypeWithReimplmentedDefaultSub>(false)]
    [TestCase<TypeWithReimplmentedDefaultSub>(true)]
    [TestCase<TypeWithReimplmentedDefaultSub>(false)]
    [TestCase<TypeWithReimplmentedInClass>(true)]
    [TestCase<TypeWithReimplmentedInClass>(false)]
    [TestCase<TypeWithReimplmentedInClassSub>(true)]
    [TestCase<TypeWithReimplmentedInClassSub>(false)]
    public void Test_TypeWithDefaultImplementation_SetsUpCorrectly<T>(bool callbase) where T : class, IWithDefaultBase
    {
        var mock = new AutoMock<T>() { CallBase = callbase };
        mock.As<IWithDefaultBase>().Setup(i => i.TestMethod()).Returns(50);
        mock.As<IWithDefaultBase>().SetupGet(i => i.TestProp).Returns(60);

        int propVal = 0;
        mock.As<IWithDefaultBase>()
            .SetupSet(i => i.TestProp = It.IsAny<int>())
            .Callback<int>(i => propVal = i);

        EventHandler? evt = null;
        if (!callbase) // Moq has a bug that it calls base on .Callback for events (unlike properties and methods)
        {
            mock.As<IWithDefaultBase>()
                .SetupAdd(i => i.TestEvent += It.IsAny<EventHandler>())
                .Callback<EventHandler>(e => evt = e);

            mock.As<IWithDefaultBase>()
                .SetupRemove(i => i.TestEvent -= It.IsAny<EventHandler>())
                .Callback<EventHandler>(e => evt = null);
        }

        var obj = mock.Object as IWithDefaultBase;
        obj.TestMethod().Should().Be(50);
        obj.TestProp.Should().Be(60);

        Assert.DoesNotThrow(() => obj.TestProp = 70);
        propVal.Should().Be(70);

        var handler = (EventHandler)((obj, e) => { });
        if (!callbase)
        {
            Assert.DoesNotThrow(() => obj.TestEvent += handler);
            evt.Should().NotBeNull();
            evt.Should().Be(handler);

            Assert.DoesNotThrow(() => obj.TestEvent -= handler);
            evt.Should().BeNull();
        }

        mock.As<IWithDefaultBase>().Verify(m => m.TestMethod());
        mock.As<IWithDefaultBase>().VerifyGet(m => m.TestProp);
        mock.As<IWithDefaultBase>().VerifySet(m => m.TestProp = 70);
        if (!callbase)
        {
            mock.As<IWithDefaultBase>().VerifyAdd(m => m.TestEvent += handler);
            mock.As<IWithDefaultBase>().VerifyRemove(m => m.TestEvent -= handler);
        }
    }
}
