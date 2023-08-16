using AutoMockFixture.FixtureUtils.Customizations;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;
using AutoMockFixture.NUnit3;

namespace AutoMockFixture.Tests.FixtureUtils.Customizations;

internal class SubclassOpenGenericCustomization_Tests
{
    internal interface ITestIface<T> { }
    internal interface ISubTestIface<T> : ITestIface<T> { }
    internal class BaseTestType<T> : ITestIface<T> { }
    internal class SubTestType<T> : BaseTestType<T>, ISubTestIface<T> { }

    [Test]
    [TestCase<ITestIface<string>, BaseTestType<string>>]
    [TestCase<ITestIface<string>, SubTestType<string>>]
    [TestCase<ISubTestIface<string>, SubTestType<string>>]
    [TestCase<BaseTestType<string>, SubTestType<string>>]
    [TestCase<ITestIface<string>, ISubTestIface<string>>]
    public void Test_WorksWithNonAutoMock<TOriginal, TSubClass>()
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.Customize(new SubclassOpenGenericCustomization<TOriginal, TSubClass>());

        var result = fixture.CreateNonAutoMock<TOriginal>();

        result.Should().NotBeNull();
        result.Should().BeAssignableTo<TSubClass>();
    }

    [Test]
    [TestCase<ITestIface<string>, BaseTestType<string>>]
    [TestCase<ITestIface<string>, SubTestType<string>>]
    [TestCase<ISubTestIface<string>, SubTestType<string>>]
    [TestCase<BaseTestType<string>, SubTestType<string>>]
    [TestCase<ITestIface<string>, ISubTestIface<string>>]
    public void Test_WorksWithNonAutoMock_WithDifferent<TOriginal, TSubClass>()
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.Customize(new SubclassOpenGenericCustomization<TOriginal, TSubClass>());

        var result = fixture.CreateNonAutoMock(typeof(TOriginal).GetGenericTypeDefinition().MakeGenericType(typeof(int)));

        result.Should().NotBeNull();
        result.Should().BeAssignableTo(typeof(TSubClass).GetGenericTypeDefinition().MakeGenericType(typeof(int)));
    }

    [Test]
    [TestCase<ITestIface<string>, BaseTestType<string>>]
    [TestCase<ITestIface<string>, SubTestType<string>>]
    [TestCase<ISubTestIface<string>, SubTestType<string>>]
    [TestCase<BaseTestType<string>, SubTestType<string>>]
    [TestCase<ITestIface<string>, ISubTestIface<string>>]
    public void Test_WorksWithNonAutoMock_WhenAutoMock<TOriginal, TSubClass>() where TOriginal : class where TSubClass : class
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.Customize(new SubclassOpenGenericCustomization<TOriginal, TSubClass>());

        var result = fixture.CreateNonAutoMock(typeof(AutoMock<TOriginal>));

        result.Should().NotBeNull();
        result.Should().BeAssignableTo<AutoMock<TSubClass>>();
    }

    [Test]
    [TestCase<ITestIface<string>, BaseTestType<string>>]
    [TestCase<ITestIface<string>, SubTestType<string>>]
    [TestCase<ISubTestIface<string>, SubTestType<string>>]
    [TestCase<BaseTestType<string>, SubTestType<string>>]
    [TestCase<ITestIface<string>, ISubTestIface<string>>]
    public void Test_WorksWithNonAutoMock_WhenAutoMock_WithDifferent<TOriginal, TSubClass>() where TOriginal : class where TSubClass : class
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.Customize(new SubclassOpenGenericCustomization<TOriginal, TSubClass>());

        var result = fixture.CreateNonAutoMock(typeof(AutoMock<>).MakeGenericType(typeof(TOriginal).GetGenericTypeDefinition().MakeGenericType(typeof(int))));

        result.Should().NotBeNull();
        result.Should().BeAssignableTo(typeof(AutoMock<>).MakeGenericType(typeof(TSubClass).GetGenericTypeDefinition().MakeGenericType(typeof(int))));
    }

    [Test]
    [TestCase<ITestIface<string>, BaseTestType<string>>]
    [TestCase<ITestIface<string>, SubTestType<string>>]
    [TestCase<ISubTestIface<string>, SubTestType<string>>]
    [TestCase<BaseTestType<string>, SubTestType<string>>]
    [TestCase<ITestIface<string>, ISubTestIface<string>>]
    public void Test_ThrowsCorretly_WithNonAutoMock_WhenAutoMock<TOriginal, TSubClass>() where TOriginal : class where TSubClass : class
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.Customize(new SubclassOpenGenericCustomization<TOriginal, TSubClass>());

        Assert.Throws<InvalidCastException>(() => fixture.CreateNonAutoMock<AutoMock<TOriginal>>(),
                                            $"Requested type AutoMock<{typeof(TOriginal).Name}> has been modified to AutoMock<{typeof(TSubClass).Name}>");
    }

    [Test]
    [TestCase<ITestIface<string>, BaseTestType<string>>]
    [TestCase<ITestIface<string>, SubTestType<string>>]
    [TestCase<ISubTestIface<string>, SubTestType<string>>]
    [TestCase<BaseTestType<string>, SubTestType<string>>]
    [TestCase<ITestIface<string>, ISubTestIface<string>>]
    public void Test_WorksWithAutoMockRequest<TOriginal, TSubClass>() where TOriginal : class where TSubClass : class
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.Customize(new SubclassOpenGenericCustomization<TOriginal, TSubClass>());

        var result = fixture.CreateAutoMock<TOriginal>();

        result.Should().NotBeNull();
        result.Should().BeAssignableTo<TSubClass>();
    }

    [Test]
    [TestCase<ITestIface<string>, BaseTestType<string>>]
    [TestCase<ITestIface<string>, SubTestType<string>>]
    [TestCase<ISubTestIface<string>, SubTestType<string>>]
    [TestCase<BaseTestType<string>, SubTestType<string>>]
    [TestCase<ITestIface<string>, ISubTestIface<string>>]
    public void Test_WorksWithAutoMockRequest_WithDifferent<TOriginal, TSubClass>()
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.Customize(new SubclassOpenGenericCustomization<TOriginal, TSubClass>());

        var result = fixture.CreateAutoMock(typeof(TOriginal).GetGenericTypeDefinition().MakeGenericType(typeof(int)));

        result.Should().NotBeNull();
        result.Should().BeAssignableTo(typeof(TSubClass).GetGenericTypeDefinition().MakeGenericType(typeof(int)));
    }

    [Test]
    [TestCase<ITestIface<string>, BaseTestType<string>>]
    [TestCase<ITestIface<string>, SubTestType<string>>]
    [TestCase<ISubTestIface<string>, SubTestType<string>>]
    [TestCase<BaseTestType<string>, SubTestType<string>>]
    [TestCase<ITestIface<string>, ISubTestIface<string>>]
    public void Test_WorksWithAutoMock_WhenAutoMock<TOriginal, TSubClass>() where TOriginal : class where TSubClass : class
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.Customize(new SubclassOpenGenericCustomization<TOriginal, TSubClass>());

        var result = fixture.CreateAutoMock(typeof(AutoMock<TOriginal>));

        result.Should().NotBeNull();
        result.Should().BeAssignableTo<AutoMock<TSubClass>>();
    }

    [Test]
    [TestCase<ITestIface<string>, BaseTestType<string>>]
    [TestCase<ITestIface<string>, SubTestType<string>>]
    [TestCase<ISubTestIface<string>, SubTestType<string>>]
    [TestCase<BaseTestType<string>, SubTestType<string>>]
    [TestCase<ITestIface<string>, ISubTestIface<string>>]
    public void Test_WorksWithAutoMock_WhenAutoMock_WithDifferent<TOriginal, TSubClass>() where TOriginal : class where TSubClass : class
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.Customize(new SubclassOpenGenericCustomization<TOriginal, TSubClass>());

        var result = fixture.CreateAutoMock(typeof(AutoMock<>).MakeGenericType(typeof(TOriginal).GetGenericTypeDefinition().MakeGenericType(typeof(int))));

        result.Should().NotBeNull();
        result.Should().BeAssignableTo(typeof(AutoMock<>).MakeGenericType(typeof(TSubClass).GetGenericTypeDefinition().MakeGenericType(typeof(int))));
    }

    [Test]
    [TestCase<ITestIface<string>, BaseTestType<string>>]
    [TestCase<ITestIface<string>, SubTestType<string>>]
    [TestCase<ISubTestIface<string>, SubTestType<string>>]
    [TestCase<BaseTestType<string>, SubTestType<string>>]
    [TestCase<ITestIface<string>, ISubTestIface<string>>]
    public void Test_ThrowsCorretly_WithAutoMock_WhenAutoMock<TOriginal, TSubClass>() where TOriginal : class where TSubClass : class
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.Customize(new SubclassOpenGenericCustomization<TOriginal, TSubClass>());

        Assert.Throws<InvalidCastException>(() => fixture.CreateAutoMock<AutoMock<TOriginal>>(),
                                            $"Requested type AutoMock<{typeof(TOriginal).Name}> has been modified to AutoMock<{typeof(TSubClass).Name}>");
    }

    [Test]
    [TestCase<ITestIface<string>, BaseTestType<string>>]
    [TestCase<ITestIface<string>, SubTestType<string>>]
    [TestCase<ISubTestIface<string>, SubTestType<string>>]
    [TestCase<BaseTestType<string>, SubTestType<string>>]
    [TestCase<ITestIface<string>, ISubTestIface<string>>]
    public void Test_WorksWithAutoMockDependencies<TOriginal, TSubClass>() where TOriginal : class where TSubClass : class
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.Customize(new SubclassOpenGenericCustomization<TOriginal, TSubClass>());

        var result = fixture.CreateWithAutoMockDependencies<TOriginal>();

        result.Should().NotBeNull();
        result.Should().BeAssignableTo<TSubClass>();
    }

    [Test]
    [TestCase<ITestIface<string>, BaseTestType<string>>]
    [TestCase<ITestIface<string>, SubTestType<string>>]
    [TestCase<ISubTestIface<string>, SubTestType<string>>]
    [TestCase<BaseTestType<string>, SubTestType<string>>]
    [TestCase<ITestIface<string>, ISubTestIface<string>>]
    public void Test_WorksWithAutoMockDependencies_WithDifferent<TOriginal, TSubClass>()
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.Customize(new SubclassOpenGenericCustomization<TOriginal, TSubClass>());

        var result = fixture.CreateWithAutoMockDependencies(typeof(TOriginal).GetGenericTypeDefinition().MakeGenericType(typeof(int)));

        result.Should().NotBeNull();
        result.Should().BeAssignableTo(typeof(TSubClass).GetGenericTypeDefinition().MakeGenericType(typeof(int)));
    }

    [Test]
    [TestCase<ITestIface<string>, BaseTestType<string>>]
    [TestCase<ITestIface<string>, SubTestType<string>>]
    [TestCase<ISubTestIface<string>, SubTestType<string>>]
    [TestCase<BaseTestType<string>, SubTestType<string>>]
    [TestCase<ITestIface<string>, ISubTestIface<string>>]
    public void Test_WorksWithAutoMockDependencies_WhenAutoMock<TOriginal, TSubClass>() where TOriginal : class where TSubClass : class
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.Customize(new SubclassOpenGenericCustomization<TOriginal, TSubClass>());

        var result = fixture.CreateWithAutoMockDependencies(typeof(AutoMock<TOriginal>));

        result.Should().NotBeNull();
        result.Should().BeAssignableTo<AutoMock<TSubClass>>();
    }

    [Test]
    [TestCase<ITestIface<string>, BaseTestType<string>>]
    [TestCase<ITestIface<string>, SubTestType<string>>]
    [TestCase<ISubTestIface<string>, SubTestType<string>>]
    [TestCase<BaseTestType<string>, SubTestType<string>>]
    [TestCase<ITestIface<string>, ISubTestIface<string>>]
    public void Test_WorksWithAutoMockDependencies_WhenAutoMock_WithDifferent<TOriginal, TSubClass>() where TOriginal : class where TSubClass : class
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.Customize(new SubclassOpenGenericCustomization<TOriginal, TSubClass>());

        var result = fixture.CreateWithAutoMockDependencies(typeof(AutoMock<>).MakeGenericType(typeof(TOriginal).GetGenericTypeDefinition().MakeGenericType(typeof(int))));

        result.Should().NotBeNull();
        result.Should().BeAssignableTo(typeof(AutoMock<>).MakeGenericType(typeof(TSubClass).GetGenericTypeDefinition().MakeGenericType(typeof(int))));
    }

    [Test]
    [TestCase<ITestIface<string>, BaseTestType<string>>]
    [TestCase<ITestIface<string>, SubTestType<string>>]
    [TestCase<ISubTestIface<string>, SubTestType<string>>]
    [TestCase<BaseTestType<string>, SubTestType<string>>]
    [TestCase<ITestIface<string>, ISubTestIface<string>>]
    public void Test_ThrowsCorretly_WithAutoMockDependencies_WhenAutoMock<TOriginal, TSubClass>() where TOriginal : class where TSubClass : class
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.Customize(new SubclassOpenGenericCustomization<TOriginal, TSubClass>());

        Assert.Throws<InvalidCastException>(() => fixture.CreateWithAutoMockDependencies<AutoMock<TOriginal>>(),
                                            $"Requested type AutoMock<{typeof(TOriginal).Name}> has been modified to AutoMock<{typeof(TSubClass).Name}>");
    }

}
