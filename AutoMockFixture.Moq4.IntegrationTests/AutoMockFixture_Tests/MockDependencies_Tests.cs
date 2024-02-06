
namespace AutoMockFixture.Moq4.IntegrationTests.AutoMockFixture_Tests;

public class MockDependencies_Tests
{
    public class TestInner{}
    public class TestOuterOuter
    {
        public TestOuterAbstract? TestOuterAbstract { get; set; }
    }
    public abstract class TestOuterAbstract : TestOuter
    {
        protected TestOuterAbstract(TestInner testInner) : base(testInner){}
    }
    public class TestOuter
    {
        public TestOuter(TestInner testInner)
        {
            TestInnerFromCtor = testInner;
        }

        public TestInner TestInnerFromCtor { get; }
        public TestInner? PropWithSet { get; set; }
        public virtual TestInner? ReadOnlyVirtualProp { get; }
        public virtual TestInner? Method() => null;
        public TestInner? Field;
    }

    private void Validate_MockDependencies_CallBase(TestOuter obj)
    {
        obj.Should().NotBeNull();

        obj.TestInnerFromCtor.Should().NotBeNull();
        AutoMockFixture.Moq4.AutoMock.Get(obj.TestInnerFromCtor).Should().NotBeNull();
        AutoMockFixture.Moq4.AutoMock.Get(obj.PropWithSet).Should().NotBeNull();
        AutoMockFixture.Moq4.AutoMock.Get(obj.Field).Should().NotBeNull();

        obj.ReadOnlyVirtualProp.Should().BeNull();
        obj.Method().Should().BeNull();
    }

    private void Validate_MockDependencies_NonCallBase(TestOuter obj)
    {
        obj.Should().NotBeNull();

        obj.TestInnerFromCtor.Should().BeNull(); // Remember that non callBase doesn't call ctor

        AutoMockFixture.Moq4.AutoMock.Get(obj.PropWithSet).Should().NotBeNull();
        AutoMockFixture.Moq4.AutoMock.Get(obj.Field).Should().NotBeNull();

        AutoMockFixture.Moq4.AutoMock.Get(obj.ReadOnlyVirtualProp).Should().NotBeNull();
        AutoMockFixture.Moq4.AutoMock.Get(obj.Method()).Should().NotBeNull();
    }

    [Test]
    public void Test_MockDependencies_When_CreateAutoMockDependecies_AutoMockType_CallBase()
    {
        var result = new AbstractAutoMockFixture().CreateWithAutoMockDependencies<AutoMock<TestOuter>>(true);

        result.Should().NotBeNull();

        var obj = result!.Object;
        Validate_MockDependencies_CallBase(obj);
    }

    [Test]
    public void Test_MockDependencies_When_CreateAutoMockDependecies_AutoMockType_NonCallBase()
    {
        var result = new AbstractAutoMockFixture().CreateWithAutoMockDependencies<AutoMock<TestOuter>>(false);

        result.Should().NotBeNull();

        var obj = result!.Object;
        Validate_MockDependencies_NonCallBase(obj);
    }

    [Test]
    public void Test_MockDependencies_When_CreateAutoMockDependecies_AbstractType_CallBase()
    {
        var result = new AbstractAutoMockFixture().CreateWithAutoMockDependencies<TestOuterOuter>(true);

        result.Should().NotBeNull();

        var obj = result!.TestOuterAbstract;
        obj.Should().NotBeNull();
        AutoMockFixture.Moq4.AutoMock.Get(obj).Should().NotBeNull();

        Validate_MockDependencies_CallBase(obj!);
    }

    [Test]
    public void Test_MockDependencies_When_CreateAutoMockDependecies_AbstractType_NonCallBase()
    {
        var result = new AbstractAutoMockFixture().CreateWithAutoMockDependencies<TestOuterOuter>(false);

        result.Should().NotBeNull();

        var obj = result!.TestOuterAbstract;
        obj.Should().NotBeNull();
        AutoMockFixture.Moq4.AutoMock.Get(obj).Should().NotBeNull();

        Validate_MockDependencies_NonCallBase(obj!);
    }


    [Test]
    public void Test_MockDependencies_When_UnitFixture_CreateAutoMock_AutoMockType_CallBase()
    {
        var result = new UnitFixture().CreateAutoMock<TestOuter>(true);

        result.Should().NotBeNull();
        AutoMockFixture.Moq4.AutoMock.Get(result).Should().NotBeNull();

        Validate_MockDependencies_CallBase(result!);
    }

    [Test]
    public void Test_MockDependencies_When_UnitFixture_CreateAutoMock_AutoMockType_NonCallBase()
    {
        var result = new UnitFixture().CreateAutoMock<TestOuter>(false);

        result.Should().NotBeNull();
        AutoMockFixture.Moq4.AutoMock.Get(result).Should().NotBeNull();

        Validate_MockDependencies_NonCallBase(result!);
    }

    private void Validate_NoMockDependecies_CallBase(TestOuter obj)
    {
        obj.Should().NotBeNull();

        obj.TestInnerFromCtor.Should().NotBeNull();
        Assert.Throws<ArgumentException>(() => AutoMockFixture.Moq4.AutoMock.Get(obj.TestInnerFromCtor));
        (obj.TestInnerFromCtor as IAutoMocked).Should().BeNull();

        obj.PropWithSet.Should().NotBeNull();
        Assert.Throws<ArgumentException>(() => AutoMockFixture.Moq4.AutoMock.Get(obj.PropWithSet));
        (obj.PropWithSet as IAutoMocked).Should().BeNull();

        obj.Field.Should().NotBeNull();
        Assert.Throws<ArgumentException>(() => AutoMockFixture.Moq4.AutoMock.Get(obj.Field));
        (obj.Field as IAutoMocked).Should().BeNull();

        obj.ReadOnlyVirtualProp.Should().BeNull();
        obj.Method().Should().BeNull();
    }

    private void Validate_NoMockDependecies_NonCallBase(TestOuter obj)
    {
        obj.Should().NotBeNull();

        obj.TestInnerFromCtor.Should().BeNull(); // Remember that non callBase doesn't call ctor

        obj.PropWithSet.Should().NotBeNull();
        Assert.Throws<ArgumentException>(() => AutoMockFixture.Moq4.AutoMock.Get(obj.PropWithSet));
        (obj.PropWithSet as IAutoMocked).Should().BeNull();

        obj.Field.Should().NotBeNull();
        Assert.Throws<ArgumentException>(() => AutoMockFixture.Moq4.AutoMock.Get(obj.Field));
        (obj.Field as IAutoMocked).Should().BeNull();

        obj.ReadOnlyVirtualProp.Should().NotBeNull();
        Assert.Throws<ArgumentException>(() => AutoMockFixture.Moq4.AutoMock.Get(obj.ReadOnlyVirtualProp));
        (obj.ReadOnlyVirtualProp as IAutoMocked).Should().BeNull();

        obj.Method().Should().NotBeNull();
        Assert.Throws<ArgumentException>(() => AutoMockFixture.Moq4.AutoMock.Get(obj.Method()));
        (obj.Method() as IAutoMocked).Should().BeNull();
    }

    [Test]
    public void Test_DoesNotMockDependencies_When_CreateNonAutoMock_AutoMockType_CallBase()
    {
        var result = new AbstractAutoMockFixture().CreateNonAutoMock<AutoMock<TestOuter>>(true);

        result.Should().NotBeNull();
        Validate_NoMockDependecies_CallBase(result!.Object);
    }

    [Test]
    public void Test_DoesNotMockDependencies_When_CreateNonAutoMock_AutoMockType_NonCallBase()
    {
        var result = new AbstractAutoMockFixture().CreateNonAutoMock<AutoMock<TestOuter>>(false);

        result.Should().NotBeNull();
        Validate_NoMockDependecies_NonCallBase(result!.Object);
    }

    [Test]
    public void Test_DoesNotMockDependencies_When_CreateNonAutoMock_AbstractType_CallBase()
    {
        var result = new AbstractAutoMockFixture().CreateNonAutoMock<TestOuterOuter>(true);
        result.Should().NotBeNull();

        var obj = result!.TestOuterAbstract;
        obj.Should().NotBeNull();
        AutoMockFixture.Moq4.AutoMock.Get(obj).Should().NotBeNull();

        Validate_NoMockDependecies_CallBase(obj!);
    }

    [Test]
    public void Test_DoesNotMockDependencies_When_CreateNonAutoMock_AbstractType_NonCallBase()
    {
        var result = new AbstractAutoMockFixture().CreateNonAutoMock<TestOuterOuter>(false);
        result.Should().NotBeNull();

        var obj = result!.TestOuterAbstract;
        obj.Should().NotBeNull();
        AutoMockFixture.Moq4.AutoMock.Get(obj).Should().NotBeNull();

        Validate_NoMockDependecies_CallBase(obj!); // Although it is callBase false it won't respect it for the abstract case only for the explicit mock case
    }

    [Test]
    public void Test_DoesNotMockDependencies_When_IntegrationFixtureCreateAutoMock_AutoMockType_CallBase()
    {
        var result = new IntegrationFixture().CreateAutoMock<TestOuter>(true);

        result.Should().NotBeNull();
        AutoMockFixture.Moq4.AutoMock.Get(result).Should().NotBeNull();

        Validate_NoMockDependecies_CallBase(result!);
    }

    [Test]
    public void Test_DoesNotMockDependencies_When_IntegrationFixtureCreateAutoMock_AutoMockType_NonCallBase()
    {
        var result = new IntegrationFixture().CreateAutoMock<TestOuter>(false);

        result.Should().NotBeNull();
        AutoMockFixture.Moq4.AutoMock.Get(result).Should().NotBeNull();

        Validate_NoMockDependecies_NonCallBase(result!);
    }

}
