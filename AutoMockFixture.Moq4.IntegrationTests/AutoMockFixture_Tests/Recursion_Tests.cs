
namespace AutoMockFixture.Tests.AutoMockFixture_Tests;

internal class Recursion_Tests
{
    public class Test1
    {
        public Test1(Test2 test){ Test = test; }
        public Test2 Test;
    }

    public class Test2
    {
        public Test2(Test1 test){ Test = test; }
        public Test1 Test;
    }

    [Test]
    public void Test_Can_Create_AutoMock_When_NotCallBase()
    {
        var fixture = new AbstractAutoMockFixture();
        var mock = fixture.CreateAutoMock<AutoMock<Test1>>();

        AutoMock.IsAutoMock(mock).Should().BeTrue();

        Test1? obj = null;
        Assert.DoesNotThrow(() => obj = mock.GetMocked());

        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<Test1>();

        obj!.Test.Should().NotBeNull();
        obj!.Test.Should().BeAssignableTo<Test2>();

        AutoMock.IsAutoMock(obj.Test).Should().BeTrue();
    }

    [Test]
    public void Test_Can_CreateAutoMock_AutoMock_When_CallBase()
    {
        var fixture = new AbstractAutoMockFixture();
        Test2? obj = null;
        Assert.DoesNotThrow(() => obj = fixture.CreateAutoMock<Test2>());

        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<Test2>();
        AutoMock.IsAutoMock(obj!).Should().BeTrue();

        obj!.Test.Should().NotBeNull();
        obj!.Test.Should().BeAssignableTo<Test1>();

        AutoMock.IsAutoMock(obj.Test).Should().BeTrue();
    }

    [Test]
    public void Test_Can_CreateAutoMock_AutoMock_When_NotCallBase()
    {
        var fixture = new AbstractAutoMockFixture();
        Test2? obj = null;
        Assert.DoesNotThrow(() => obj = fixture.CreateAutoMock<Test2>(true));

        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<Test2>();
        AutoMock.IsAutoMock(obj!).Should().BeTrue();

        obj!.Test.Should().NotBeNull();
        obj!.Test.Should().BeAssignableTo<Test1>();

        AutoMock.IsAutoMock(obj.Test).Should().BeTrue();
    }

    [Test]
    public void Test_Can_Create_NonAutoMock()
    {
        var fixture = new AbstractAutoMockFixture();
        Test2? obj = null;
        Assert.DoesNotThrow(() => obj = fixture.CreateNonAutoMock<Test2>());

        obj.Should().NotBeNull();
        obj.Should().BeOfType<Test2>();
        AutoMock.IsAutoMock(obj!).Should().BeFalse();

        obj!.Test.Should().NotBeNull();
        obj!.Test.Should().BeOfType<Test1>();

        AutoMock.IsAutoMock(obj.Test).Should().BeFalse();
    }

    [Test]
    public void Test_Can_Create_NonAutoMock_WithDepdnecies()
    {
        var fixture = new AbstractAutoMockFixture();
        Test2? obj = null;
        Assert.DoesNotThrow(() => obj = fixture.CreateWithAutoMockDependencies<Test2>());

        obj.Should().NotBeNull();
        obj.Should().BeOfType<Test2>();
        AutoMock.IsAutoMock(obj!).Should().BeFalse();

        obj!.Test.Should().NotBeNull();
        obj!.Test.Should().BeAssignableTo<Test1>();

        AutoMock.IsAutoMock(obj.Test).Should().BeTrue(); // Because it is dependency injection
    }
}
