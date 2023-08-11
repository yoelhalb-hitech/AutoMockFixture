using AutoMockFixture.NUnit3;
using Moq;

namespace AutoMockFixture.Moq4.IntegrationTests.AutoMockProxy;

public class DefaultInterfaceForClass_Tests
{
    public interface IWithDefault
    {
        public int TestMethod() => 10;
        public int TestProp { get => 20; set => throw new InvalidOperationException("Test Exception"); }
        public event EventHandler TestEvent { add => throw new InvalidOperationException("Test Exception"); remove => throw new InvalidOperationException("Test Exception"); }
    }

    public interface IWithReimplmentedDefault : IWithDefault
    {
        int IWithDefault.TestMethod() => 20;
        int IWithDefault.TestProp { get => 30; set => throw new ArgumentOutOfRangeException("Test Exception"); }
        event EventHandler IWithDefault.TestEvent { add => throw new ArgumentOutOfRangeException("Test Exception"); remove => throw new ArgumentOutOfRangeException("Test Exception"); }
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
        int IWithDefault.TestMethod() => 40;
        int IWithDefault.TestProp { get => 50; set => throw new AggregateException("Test Exception"); }
        event EventHandler IWithDefault.TestEvent { add => throw new AggregateException("Test Exception"); remove => throw new AggregateException("Test Exception"); }

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
    public void Test_TypeWithDefaultImplementation_DoesNotCallbase_OnNonCallbase_WithDynamicBinding<T>() where T : class, IWithDefault
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
    [TestCase<TypeWithDefault>]
    [TestCase<TypeWithDefaultSub>]
    public void Test_TypeWithDefaultImplementation_DoesNotCallbase_OnNonCallbase_WithStaticBinding<T>() where T : TypeWithDefault, IWithDefault
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
    [TestCase<TypeWithReimplmentedDefault>]
    [TestCase<TypeWithReimplmentedDefaultSub>]
    public void Test_TypeWithDefaultReimplementation_DoesNotCallbase_OnNonCallbase_WithStaticBinding<T>() where T : TypeWithReimplmentedDefault, IWithDefault
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
    [TestCase<TypeWithReimplmentedInClass>]
    [TestCase<TypeWithReimplmentedInClassSub>]
    public void Test_TypeWithDefaultReimplementationInClass_DoesNotCallbase_OnNonCallbase_WithStaticBinding<T>() where T : TypeWithReimplmentedInClass, IWithDefault
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
    [TestCase<TypeWithDefault>]
    [TestCase<TypeWithDefaultSub>]
    public void Test_TypeWithDefaultImplementation_Callsbase_OnCallbase_WithDynamicTyping<T>() where T : class, IWithDefault
    {
        var mock = new AutoMock<TypeWithDefault>() { CallBase = true };

        var obj = mock.Object as IWithDefault;
        obj.TestMethod().Should().Be(10);
        obj.TestProp.Should().Be(20);

        Assert.Throws<InvalidOperationException>(() => obj.TestProp = 70, "Test Exception");
        Assert.Throws<InvalidOperationException>(() => obj.TestEvent += (o, e) => { }, "Test Exception");
        Assert.Throws<InvalidOperationException>(() => obj.TestEvent -= (o, e) => { }, "Test Exception");
    }

    [Test]
    [TestCase<TypeWithDefault>]
    [TestCase<TypeWithDefaultSub>]
    public void Test_TypeWithDefaultImplementation_Callsbase_OnCallbase_WithStaticTyping<T>() where T : TypeWithDefault, IWithDefault
    {
        var mock = new AutoMock<TypeWithDefault>() { CallBase = true };

        var obj = mock.Object as IWithDefault;
        obj.TestMethod().Should().Be(10);
        obj.TestProp.Should().Be(20);

        Assert.Throws<InvalidOperationException>(() => obj.TestProp = 70, "Test Exception");
        Assert.Throws<InvalidOperationException>(() => obj.TestEvent += (o, e) => { }, "Test Exception");
        Assert.Throws<InvalidOperationException>(() => obj.TestEvent -= (o, e) => { }, "Test Exception");
    }

    [Test]
    [TestCase<TypeWithReimplmentedDefault>]
    [TestCase<TypeWithReimplmentedDefaultSub>]
    public void Test_TypeWithReimplemted_Callsbase_OnCallbase_WithDynamicTyping<T>() where T : class, IWithDefault
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
    [TestCase<TypeWithReimplmentedDefault>]
    [TestCase<TypeWithReimplmentedDefaultSub>]
    public void Test_TypeWithReimplemted_Callsbase_OnCallbase_WithStaticTyping<T>() where T : TypeWithReimplmentedDefault, IWithDefault
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
    [TestCase<TypeWithReimplmentedInClass>]
    [TestCase<TypeWithReimplmentedInClassSub>]
    public void Test_TypeWithReimplmentedInClass_Callsbase_OnCallbaseWithDynamicTyping<T>() where T : class, IWithDefault
    {
        var mock = new AutoMock<T>() { CallBase = true };

        var obj = mock.Object as IWithDefault;
        obj.TestMethod().Should().Be(40);
        obj.TestProp.Should().Be(50);

        Assert.Throws<AggregateException>(() => obj.TestProp = 70, "Test Exception");
        Assert.Throws<AggregateException>(() => obj.TestEvent += (o, e) => { }, "Test Exception");
        Assert.Throws<AggregateException>(() => obj.TestEvent -= (o, e) => { }, "Test Exception");
    }

    [Test]
    [TestCase<TypeWithReimplmentedInClass>]
    [TestCase<TypeWithReimplmentedInClassSub>]
    public void Test_TypeWithReimplmentedInClass_Callsbase_OnCallbase_WithStaticTyping<T>() where T : TypeWithReimplmentedInClass, IWithDefault
    {
        var mock = new AutoMock<T>() { CallBase = true };

        var obj = mock.Object as IWithDefault;
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
    public void Test_TypeWithDefaultImplementation_SetsUpCorrectly<T>(bool callbase) where T : class, IWithDefault
    {
        var mock = new AutoMock<T>() { CallBase = callbase };
        mock.As<IWithDefault>().Setup(i => i.TestMethod()).Returns(50);
        mock.As<IWithDefault>().SetupGet(i => i.TestProp).Returns(60);

        int propVal = 0;
        mock.As<IWithDefault>()
            .SetupSet(i => i.TestProp = It.IsAny<int>())
            .Callback<int>(i => propVal = i);

        EventHandler? evt = null;
        if (!callbase) // Moq has a bug that it calls base on .Callback for events (unlike properties and methods)
        {
            mock.As<IWithDefault>()
                .SetupAdd(i => i.TestEvent += It.IsAny<EventHandler>())
                .Callback<EventHandler>(e => evt = e);

            mock.As<IWithDefault>()
                .SetupRemove(i => i.TestEvent -= It.IsAny<EventHandler>())
                .Callback<EventHandler>(e => evt = null);
        }

        var obj = mock.Object as IWithDefault;
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

        mock.As<IWithDefault>().Verify(m => m.TestMethod());
        mock.As<IWithDefault>().VerifyGet(m => m.TestProp);
        mock.As<IWithDefault>().VerifySet(m => m.TestProp = 70);
        if (!callbase)
        {
            mock.As<IWithDefault>().VerifyAdd(m => m.TestEvent += handler);
            mock.As<IWithDefault>().VerifyRemove(m => m.TestEvent -= handler);
        }
    }
}
