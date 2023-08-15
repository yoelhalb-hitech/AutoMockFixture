using AutoMockFixture.FixtureUtils.Customizations;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;
using AutoMockFixture.NUnit3;

namespace AutoMockFixture.Tests.FixtureUtils.Customizations;

internal class SubClassCustomization_Tests
{
    internal interface ITestIface { }
    internal interface ISubTestIface : ITestIface { }
    internal class BaseTestType : ITestIface { }
    internal class SubTestType : BaseTestType, ISubTestIface { }

    [Test]
    [TestCase<ITestIface, BaseTestType>]
    [TestCase<ITestIface, SubTestType>]
    [TestCase<ISubTestIface, SubTestType>]
    [TestCase<BaseTestType, SubTestType>]
    [TestCase<ITestIface, ISubTestIface>]
    public void Test_WorksWithNonAutoMock<TOriginal, TSubClass>()
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.Customize(new SubClassCustomization<TOriginal, TSubClass>());

        var result = fixture.CreateNonAutoMock<TOriginal>();

        result.Should().NotBeNull();
        result.Should().BeAssignableTo<TSubClass>();
    }

    [Test]
    [TestCase<ITestIface, BaseTestType>]
    [TestCase<ITestIface, SubTestType>]
    [TestCase<ISubTestIface, SubTestType>]
    [TestCase<BaseTestType, SubTestType>]
    [TestCase<ITestIface, ISubTestIface>]
    public void Test_WorksWithNonAutoMock_WhenAutoMock<TOriginal, TSubClass>() where TOriginal : class where TSubClass : class
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.Customize(new SubClassCustomization<TOriginal, TSubClass>());

        var result = fixture.CreateNonAutoMock(typeof(AutoMock<TOriginal>));

        result.Should().NotBeNull();
        result.Should().BeAssignableTo<AutoMock<TSubClass>>();
    }

    [Test]
    [TestCase<ITestIface, BaseTestType>]
    [TestCase<ITestIface, SubTestType>]
    [TestCase<ISubTestIface, SubTestType>]
    [TestCase<BaseTestType, SubTestType>]
    [TestCase<ITestIface, ISubTestIface>]
    public void Test_ThrowsCorretly_WithNonAutoMock_WhenAutoMock<TOriginal, TSubClass>() where TOriginal : class where TSubClass : class
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.Customize(new SubClassCustomization<TOriginal, TSubClass>());

        Assert.Throws<InvalidCastException>(() => fixture.CreateNonAutoMock<AutoMock<TOriginal>>(),
                                            $"Requested type AutoMock<{typeof(TOriginal).Name}> has been modified to AutoMock<{typeof(TSubClass).Name}>");
    }

    [Test]
    [TestCase<ITestIface, BaseTestType>]
    [TestCase<ITestIface, SubTestType>]
    [TestCase<ISubTestIface, SubTestType>]
    [TestCase<BaseTestType, SubTestType>]
    [TestCase<ITestIface, ISubTestIface>]
    public void Test_WorksWithAutoMockRequest<TOriginal, TSubClass>() where TOriginal : class where TSubClass : class
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.Customize(new SubClassCustomization<TOriginal, TSubClass>());

        var result = fixture.CreateAutoMock<TOriginal>();

        result.Should().NotBeNull();
        result.Should().BeAssignableTo<TSubClass>();
    }

    [Test]
    [TestCase<ITestIface, BaseTestType>]
    [TestCase<ITestIface, SubTestType>]
    [TestCase<ISubTestIface, SubTestType>]
    [TestCase<BaseTestType, SubTestType>]
    [TestCase<ITestIface, ISubTestIface>]
    public void Test_WorksWithAutoMock_WhenAutoMock<TOriginal, TSubClass>() where TOriginal : class where TSubClass : class
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.Customize(new SubClassCustomization<TOriginal, TSubClass>());

        var result = fixture.CreateAutoMock(typeof(AutoMock<TOriginal>));

        result.Should().NotBeNull();
        result.Should().BeAssignableTo<AutoMock<TSubClass>>();
    }

    [Test]
    [TestCase<ITestIface, BaseTestType>]
    [TestCase<ITestIface, SubTestType>]
    [TestCase<ISubTestIface, SubTestType>]
    [TestCase<BaseTestType, SubTestType>]
    [TestCase<ITestIface, ISubTestIface>]
    public void Test_ThrowsCorretly_WithAutoMock_WhenAutoMock<TOriginal, TSubClass>() where TOriginal : class where TSubClass : class
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.Customize(new SubClassCustomization<TOriginal, TSubClass>());

        Assert.Throws<InvalidCastException>(() => fixture.CreateAutoMock<AutoMock<TOriginal>>(),
                                            $"Requested type AutoMock<{typeof(TOriginal).Name}> has been modified to AutoMock<{typeof(TSubClass).Name}>");
    }

    [Test]
    [TestCase<ITestIface, BaseTestType>]
    [TestCase<ITestIface, SubTestType>]
    [TestCase<ISubTestIface, SubTestType>]
    [TestCase<BaseTestType, SubTestType>]
    [TestCase<ITestIface, ISubTestIface>]
    public void Test_WorksWithAutoMockDependencies<TOriginal, TSubClass>() where TOriginal : class where TSubClass : class
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.Customize(new SubClassCustomization<TOriginal, TSubClass>());

        var result = fixture.CreateWithAutoMockDependencies<TOriginal>();

        result.Should().NotBeNull();
        result.Should().BeAssignableTo<TSubClass>();
    }
        
    [Test]
    [TestCase<ITestIface, BaseTestType>]
    [TestCase<ITestIface, SubTestType>]
    [TestCase<ISubTestIface, SubTestType>]
    [TestCase<BaseTestType, SubTestType>]
    [TestCase<ITestIface, ISubTestIface>]
    public void Test_WorksWithAutoMockDependencies_WhenAutoMock<TOriginal, TSubClass>() where TOriginal : class where TSubClass : class
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.Customize(new SubClassCustomization<TOriginal, TSubClass>());

        var result = fixture.CreateWithAutoMockDependencies(typeof(AutoMock<TOriginal>));

        result.Should().NotBeNull();
        result.Should().BeAssignableTo<AutoMock<TSubClass>>();
    }

    [Test]
    [TestCase<ITestIface, BaseTestType>]
    [TestCase<ITestIface, SubTestType>]
    [TestCase<ISubTestIface, SubTestType>]
    [TestCase<BaseTestType, SubTestType>]
    [TestCase<ITestIface, ISubTestIface>]
    public void Test_ThrowsCorretly_WithAutoMockDependencies_WhenAutoMock<TOriginal, TSubClass>() where TOriginal : class where TSubClass : class
    {
        var fixture = new AbstractAutoMockFixture();
        fixture.Customize(new SubClassCustomization<TOriginal, TSubClass>());

        Assert.Throws<InvalidCastException>(() => fixture.CreateWithAutoMockDependencies<AutoMock<TOriginal>>(),
                                            $"Requested type AutoMock<{typeof(TOriginal).Name}> has been modified to AutoMock<{typeof(TSubClass).Name}>");
    }

}
