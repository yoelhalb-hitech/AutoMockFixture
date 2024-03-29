﻿using AutoMockFixture.NUnit3;
using Moq;

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
        int IWithDefaultBase.TestProp { get => 20; set => throw new InvalidOperationException("Test Exception"); }
        event EventHandler IWithDefaultBase.TestEvent { add => throw new InvalidOperationException("Test Exception"); remove => throw new InvalidOperationException("Test Exception"); }
    }

    public interface IWithDefaultSub : IWithDefault { }

    public interface IWithReimplmentedDefault : IWithDefault
    {
        int IWithDefaultBase.TestMethod() => 20;
        int IWithDefaultBase.TestProp { get => 30; set => throw new ArgumentOutOfRangeException("Test Exception"); }
        event EventHandler IWithDefaultBase.TestEvent { add => throw new ArgumentOutOfRangeException("Test Exception"); remove => throw new ArgumentOutOfRangeException("Test Exception"); }
    }

    public interface IWithReimplmentedDefaultSub : IWithReimplmentedDefault { }


    [Test]
    [TestCase<IWithDefault>]
    [TestCase<IWithDefaultSub>]
    [TestCase<IWithReimplmentedDefault>]
    [TestCase<IWithReimplmentedDefaultSub>]
    public void Test_TypeWithDefaultImplementation_DoesNotCallBase_OnNonCallBase<T>() where T : class, IWithDefaultBase
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
    public void Test_TypeWithDefaultImplementation_Callsbase_OnCallBase<T>() where T : class, IWithDefaultBase
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
    [TestCase<IWithReimplmentedDefault>]
    [TestCase<IWithReimplmentedDefaultSub>]
    public void Test_TypeWithReimplemented_Callsbase_OnCallBase<T>() where T : class, IWithDefaultBase
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
    [TestCase<IWithDefault>(true)]
    [TestCase<IWithDefault>(false)]
    [TestCase<IWithDefaultSub>(true)]
    [TestCase<IWithDefaultSub>(false)]
    [TestCase<IWithReimplmentedDefault>(true)]
    [TestCase<IWithReimplmentedDefault>(false)]
    [TestCase<IWithReimplmentedDefaultSub>(true)]
    [TestCase<IWithReimplmentedDefaultSub>(false)]
    public void Test_TypeWithDefaultImplementation_SetsUpCorrectly<T>(bool callBase) where T : class, IWithDefaultBase
    {
        var mock = new AutoMock<T>() { CallBase = callBase };
        mock.As<IWithDefaultBase>().Setup(i => i.TestMethod()).Returns(50);
        mock.As<IWithDefaultBase>().SetupGet(i => i.TestProp).Returns(60);

        int propVal = 0;
        mock.As<IWithDefaultBase>()
            .SetupSet(i => i.TestProp = It.IsAny<int>())
            .Callback<int>(i => propVal = i);

        var obj = mock.Object as IWithDefaultBase;
        obj.TestMethod().Should().Be(50);
        obj.TestProp.Should().Be(60);

        Assert.DoesNotThrow(() => obj.TestProp = 70);
        propVal.Should().Be(70);

        mock.As<IWithDefaultBase>().Verify(m => m.TestMethod());
        mock.As<IWithDefaultBase>().VerifyGet(m => m.TestProp);
        mock.As<IWithDefaultBase>().VerifySet(m => m.TestProp = 70);
    }

    [Test]
    [TestCase<IWithDefault>]
    [TestCase<IWithDefaultSub>]
    [TestCase<IWithReimplmentedDefault>]
    [TestCase<IWithReimplmentedDefaultSub>]
    // Moq has a bug that it calls base on .Callback for events (unlike properties and methods) so we have to test it only on non callBase
    public void Test_TypeWithDefaultImplementation_SetsUpEventsCorrectly_ForNonCallBase<T>() where T : class, IWithDefaultBase
    {
        var mock = new AutoMock<T>() { CallBase = false };

        EventHandler? evt = null;
        mock.As<IWithDefaultBase>()
            .SetupAdd(i => i.TestEvent += It.IsAny<EventHandler>())
            .Callback<EventHandler>(e => evt = e);

        mock.As<IWithDefaultBase>()
            .SetupRemove(i => i.TestEvent -= It.IsAny<EventHandler>())
            .Callback<EventHandler>(e => evt = null);

        var obj = mock.Object as IWithDefaultBase;

        var handler = (EventHandler)((obj, e) => { });
        Assert.DoesNotThrow(() => obj.TestEvent += handler);
        evt.Should().NotBeNull();
        evt.Should().Be(handler);

        Assert.DoesNotThrow(() => obj.TestEvent -= handler);
        evt.Should().BeNull();

        mock.As<IWithDefaultBase>().VerifyAdd(m => m.TestEvent += handler);
        mock.As<IWithDefaultBase>().VerifyRemove(m => m.TestEvent -= handler);
    }
}
