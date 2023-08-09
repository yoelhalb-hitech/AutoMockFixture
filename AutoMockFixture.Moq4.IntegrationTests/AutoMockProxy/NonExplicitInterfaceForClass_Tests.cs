using AutoMockFixture.NUnit3;

namespace AutoMockFixture.Moq4.IntegrationTests.AutoMockProxy;

internal class NonExplicitInterfaceForClass_Tests
{
    public interface IWithNoDefault
    {
        public int TestMethod();
        public int TestProp { get; set; }
        public event EventHandler TestEvent;
    }

    public class TypeWithNonExplicit : IWithNoDefault
    {
        public int TestMethod() => 10;
        public int TestProp { get => 20; set => throw new InvalidOperationException(); }
        public event EventHandler TestEvent { add => throw new InvalidOperationException(); remove => throw new InvalidOperationException(); }
    }

    public class TypeWithNonExplicitSub : TypeWithNonExplicit
    {
    }

    public class TypeWithReimplmented : TypeWithNonExplicit, IWithNoDefault
    {
        public new int TestMethod() => 30;
        public new int TestProp { get => 40; set => throw new ArgumentOutOfRangeException(); }
        public new event EventHandler TestEvent { add => throw new ArgumentOutOfRangeException(); remove => throw new ArgumentOutOfRangeException(); }
    }

    public class TypeWithReimplmentedSub : TypeWithReimplmented
    {
    }

    [Test]
    [TestCase<TypeWithNonExplicit>(true)]
    [TestCase<TypeWithNonExplicit>(false)]
    [TestCase<TypeWithNonExplicitSub>(true)]
    [TestCase<TypeWithNonExplicitSub>(false)]
    public void Test_TypeWithNonExplicitImplementation_Callsbase_OnDynamicBinding<T>(bool callbase) where T : class, IWithNoDefault
    {
        var mock = new AutoMock<T>() { CallBase = callbase };
        mock.As<IWithNoDefault>(); // Force to reimplement
        var obj = mock.Object;

        obj.TestMethod().Should().Be(10);
        obj.TestProp.Should().Be(20);

        Assert.Throws<InvalidOperationException>(() => obj.TestProp = 70);
        Assert.Throws<InvalidOperationException>(() => obj.TestEvent += (o, e) => { });
        Assert.Throws<InvalidOperationException>(() => obj.TestEvent -= (o, e) => { });
    }

    [Test]
    [TestCase<TypeWithReimplmented>(true)]
    [TestCase<TypeWithReimplmented>(false)]
    [TestCase<TypeWithReimplmentedSub>(true)]
    [TestCase<TypeWithReimplmentedSub>(false)]
    public void Test_TypeWithNonExplicitImplementation_Callsbase_OnDynamicBinding_AndReimplmented<T>(bool callbase) where T : class, IWithNoDefault
    {
        var mock = new AutoMock<T>() { CallBase = callbase };
        mock.As<IWithNoDefault>(); // Force to reimplement
        var obj = mock.Object;

        obj.TestMethod().Should().Be(30);
        obj.TestProp.Should().Be(40);

        Assert.Throws<ArgumentOutOfRangeException>(() => obj.TestProp = 70);
        Assert.Throws<ArgumentOutOfRangeException>(() => obj.TestEvent += (o, e) => { });
        Assert.Throws<ArgumentOutOfRangeException>(() => obj.TestEvent -= (o, e) => { });
    }

    [Test]
    [TestCase<TypeWithNonExplicit>(true)]
    [TestCase<TypeWithNonExplicit>(false)]
    [TestCase<TypeWithNonExplicitSub>(true)]
    [TestCase<TypeWithNonExplicitSub>(false)]
    public void Test_TypeWithNonExplicitImplementation_Callsbase_OnStaticBinding<T>(bool callbase) where T : TypeWithNonExplicit, IWithNoDefault
    {
        var mock = new AutoMock<T>() { CallBase = callbase };
        mock.As<IWithNoDefault>(); // Force to reimplement
        var obj = mock.Object;

        obj.TestMethod().Should().Be(10);
        obj.TestProp.Should().Be(20);

        Assert.Throws<InvalidOperationException>(() => obj.TestProp = 70);
        Assert.Throws<InvalidOperationException>(() => obj.TestEvent += (o, e) => { });
        Assert.Throws<InvalidOperationException>(() => obj.TestEvent -= (o, e) => { });
    }

    [Test]
    [TestCase<TypeWithReimplmented>(true)]
    [TestCase<TypeWithReimplmented>(false)]
    [TestCase<TypeWithReimplmentedSub>(true)]
    [TestCase<TypeWithReimplmentedSub>(false)]
    // NOTE That it won't work correctly if using TypeWithNonExplicit since of the static binding
    public void Test_TypeWithNonExplicitImplementation_Callsbase_OnStaticBinding_AndReimplmented<T>(bool callbase) where T : TypeWithReimplmented, IWithNoDefault
    {
        var mock = new AutoMock<T>() { CallBase = callbase };
        mock.As<IWithNoDefault>(); // Force to reimplement
        var obj = mock.Object;

        obj.TestMethod().Should().Be(30);
        obj.TestProp.Should().Be(40);

        Assert.Throws<ArgumentOutOfRangeException>(() => obj.TestProp = 70);
        Assert.Throws<ArgumentOutOfRangeException>(() => obj.TestEvent += (o, e) => { });
        Assert.Throws<ArgumentOutOfRangeException>(() => obj.TestEvent -= (o, e) => { });
    }
}
