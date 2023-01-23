using AutoMoqExtensions.AutoMockUtils;

namespace AutoMoqExtensions.Test.AutoMockFixture_Tests;

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

        AutoMockHelpers.GetFromObj(mock).Should().NotBeNull();

        Test1? obj = null;
        Assert.DoesNotThrow(() => obj = mock.GetMocked());

        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<Test1>();

        obj!.Test.Should().NotBeNull();
        obj!.Test.Should().BeAssignableTo<Test2>();

        AutoMockHelpers.GetFromObj(obj.Test).Should().NotBeNull();
    }

    [Test]
    public void Test_Can_CreateAutoMock_AutoMock_When_CallBase()
    {
        var fixture = new AbstractAutoMockFixture();
        Test2? obj = null;
        Assert.DoesNotThrow(() => obj = fixture.CreateAutoMock<Test2>());

        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<Test2>();
        AutoMockHelpers.GetFromObj(obj!).Should().NotBeNull();

        obj!.Test.Should().NotBeNull();
        obj!.Test.Should().BeAssignableTo<Test1>();

        AutoMockHelpers.GetFromObj(obj.Test).Should().NotBeNull();
    }

    [Test]
    public void Test_Can_CreateAutoMock_AutoMock_When_NotCallBase()
    {
        var fixture = new AbstractAutoMockFixture();
        Test2? obj = null;
        Assert.DoesNotThrow(() => obj = fixture.CreateAutoMock<Test2>(true));

        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<Test2>();
        AutoMockHelpers.GetFromObj(obj!).Should().NotBeNull();

        obj!.Test.Should().NotBeNull();
        obj!.Test.Should().BeAssignableTo<Test1>();

        AutoMockHelpers.GetFromObj(obj.Test).Should().NotBeNull();
    }

    [Test]
    public void Test_Can_Create_NonAutoMock()
    {
        var fixture = new AbstractAutoMockFixture();
        Test2? obj = null;
        Assert.DoesNotThrow(() => obj = fixture.CreateNonAutoMock<Test2>());

        obj.Should().NotBeNull();
        obj.Should().BeOfType<Test2>();
        AutoMockHelpers.GetFromObj(obj!).Should().BeNull();

        obj!.Test.Should().NotBeNull();
        obj!.Test.Should().BeOfType<Test1>();

        AutoMockHelpers.GetFromObj(obj.Test).Should().BeNull();
    }

    [Test]
    public void Test_Can_Create_NonAutoMock_WithDepdnecies()
    {
        var fixture = new AbstractAutoMockFixture();
        Test2? obj = null;
        Assert.DoesNotThrow(() => obj = fixture.CreateWithAutoMockDependencies<Test2>());

        obj.Should().NotBeNull();
        obj.Should().BeOfType<Test2>();
        AutoMockHelpers.GetFromObj(obj!).Should().BeNull();

        obj!.Test.Should().NotBeNull();
        obj!.Test.Should().BeAssignableTo<Test1>();

        AutoMockHelpers.GetFromObj(obj.Test).Should().NotBeNull(); // Because it is dependency injection
    }
}
