using AutoMockFixture.FixtureUtils.Customizations;
using AutoMockFixture.NUnit3;
using static AutoMockFixture.Tests.FixtureUtils.Customizations.SubclassCustomization_Tests;

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

    [Test]
    [TestCase<ITestIface<string>, ISubTestIface<string>>]
    public void Test_FailsCorrectly_When_NeverAutoMock<TOriginal, TSubClass>()
        where TOriginal : class where TSubClass : class, TOriginal
    {
        var fixture = new UnitFixture();
        fixture.Customize(new SubclassCustomization<TOriginal, TSubClass>());

        fixture.AutoMockTypeControl.NeverAutoMockTypes.Add(typeof(TOriginal));
        fixture.AutoMockTypeControl.NeverAutoMockTypes.Add(typeof(TSubClass));

        Assert.Catch<AutoFixture.ObjectCreationException>(() => fixture.Create<TOriginal>());
    }

    [Test]
    [TestCase<ITestIface<string>, ISubTestIface<string>>]
    public void Test_DoesNotFail_When_NeverAutoMock_OnlyOriginal<TOriginal, TSubClass>()
        where TOriginal : class where TSubClass : class, TOriginal
    {
        var fixture = new UnitFixture();
        fixture.Customize(new SubclassCustomization<TOriginal, TSubClass>());

        fixture.AutoMockTypeControl.NeverAutoMockTypes.Add(typeof(TOriginal));

        object? result = null;
        Assert.DoesNotThrow(() => result = fixture.Create<TOriginal>());

        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IAutoMocked>();
        result.Should().BeAssignableTo<TSubClass>();

        AutoMock.Get((TSubClass)result!).Should().NotBeNull();
    }

    [Test]
    [TestCase<ITestIface<string>, BaseTestType<string>>]
    [TestCase<ITestIface<string>, SubTestType<string>>]
    [TestCase<ISubTestIface<string>, SubTestType<string>>]
    [TestCase<BaseTestType<string>, SubTestType<string>>]
    [TestCase<ITestIface<string>, ISubTestIface<string>>]
    public void Test_FreezesCorrectly<TOriginal, TSubClass>()
        where TOriginal : class where TSubClass : class, TOriginal
    {
        var fixture = new UnitFixture();
        fixture.Customize(new SubclassOpenGenericCustomization<TOriginal, TSubClass>());

        var result = fixture.Freeze<TOriginal>();
        var result2 = fixture.Create<TOriginal>();

        result.Should().NotBeNull();
        result.Should().BeAssignableTo<TSubClass>();

        result2.Should().NotBeNull();
        result2.Should().BeSameAs(result);
    }

    [Test]
    [TestCase<ITestIface<string>, BaseTestType<string>>]
    [TestCase<ITestIface<string>, SubTestType<string>>]
    [TestCase<ISubTestIface<string>, SubTestType<string>>]
    [TestCase<BaseTestType<string>, SubTestType<string>>]
    public void Test_FreezesCorrectly_When_NeverAutoMock<TOriginal, TSubClass>()
        where TOriginal : class where TSubClass : class, TOriginal
    {
        var fixture = new UnitFixture();
        fixture.Customize(new SubclassCustomization<TOriginal, TSubClass>());

        fixture.AutoMockTypeControl.NeverAutoMockTypes.Add(typeof(TOriginal));
        fixture.AutoMockTypeControl.NeverAutoMockTypes.Add(typeof(TSubClass));

        var result = fixture.Freeze<TOriginal>();
        var result2 = fixture.CreateAutoMock<TOriginal>();

        result.Should().NotBeNull();
        result.Should().BeAssignableTo<TSubClass>();

        result2.Should().NotBeNull();
        result2.Should().BeSameAs(result);
    }

    [Test]
    [TestCase<ITestIface<string>, BaseTestType<string>>]
    [TestCase<ITestIface<string>, SubTestType<string>>]
    [TestCase<ISubTestIface<string>, SubTestType<string>>]
    [TestCase<BaseTestType<string>, SubTestType<string>>]
    [TestCase<ITestIface<string>, ISubTestIface<string>>]
    public void Test_FreezesCorrectly_When_ForceAutoMock<TOriginal, TSubClass>()
        where TOriginal : class where TSubClass : class, TOriginal
    {
        var fixture = new UnitFixture();
        fixture.Customize(new SubclassCustomization<TOriginal, TSubClass>());

        fixture.AutoMockTypeControl.AlwaysAutoMockTypes.Add(typeof(TOriginal));
        fixture.AutoMockTypeControl.AlwaysAutoMockTypes.Add(typeof(TSubClass));

        var result = fixture.Freeze(typeof(TOriginal));
        var result2 = fixture.CreateAutoMock<TOriginal>();
        var result3 = fixture.CreateWithAutoMockDependencies<TOriginal>();

        result.Should().NotBeNull();
        result.Should().BeAssignableTo<TSubClass>();

        result2.Should().NotBeNull();
        result2.Should().BeSameAs(result);

        result3.Should().NotBeNull();
        result3.Should().BeSameAs(result);
    }

    [Test]
    [TestCase<ITestIface<string>, BaseTestType<string>>]
    [TestCase<ITestIface<string>, SubTestType<string>>]
    [TestCase<ISubTestIface<string>, SubTestType<string>>]
    [TestCase<BaseTestType<string>, SubTestType<string>>]
    [TestCase<ITestIface<string>, ISubTestIface<string>>]
    public void Test_FreezesCorrectly_WithDifferent<TOriginal, TSubClass>()
    {
        var fixture = new UnitFixture();
        fixture.Customize(new SubclassOpenGenericCustomization<TOriginal, TSubClass>());

        var result = fixture.Freeze(typeof(TOriginal).GetGenericTypeDefinition().MakeGenericType(typeof(int)));
        var result2 = fixture.Create(typeof(TOriginal).GetGenericTypeDefinition().MakeGenericType(typeof(int)));

        result.Should().NotBeNull();
        result.Should().BeAssignableTo(typeof(TSubClass).GetGenericTypeDefinition().MakeGenericType(typeof(int)));

        result2.Should().NotBeNull();
        result2.Should().BeSameAs(result);
    }

    [Test]
    [TestCase<ITestIface<string>, BaseTestType<string>>]
    [TestCase<ITestIface<string>, SubTestType<string>>]
    [TestCase<ISubTestIface<string>, SubTestType<string>>]
    [TestCase<BaseTestType<string>, SubTestType<string>>]
    [TestCase<ITestIface<string>, ISubTestIface<string>>]
    public void Test_FreezesCorrectly_ForAutoMock<TOriginal, TSubClass>()
        where TOriginal : class where TSubClass : class, TOriginal
    {
        var fixture = new UnitFixture();
        fixture.Customize(new SubclassCustomization<TOriginal, TSubClass>());

        var result = fixture.Freeze(typeof(AutoMock<TOriginal>));
        var result2 = fixture.Create(typeof(AutoMock<TOriginal>));

        result.Should().NotBeNull();
        result.Should().BeAssignableTo<AutoMock<TSubClass>>();

        result2.Should().NotBeNull();
        result2.Should().BeSameAs(result);
    }

    [Test]
    [TestCase<ITestIface<string>, BaseTestType<string>>]
    [TestCase<ITestIface<string>, SubTestType<string>>]
    [TestCase<ISubTestIface<string>, SubTestType<string>>]
    [TestCase<BaseTestType<string>, SubTestType<string>>]
    [TestCase<ITestIface<string>, ISubTestIface<string>>]
    public void Test_FreezesCorrectly_ForAutoMock_WithDifferent<TOriginal, TSubClass>()
        where TOriginal : class where TSubClass : class, TOriginal
    {
        var fixture = new UnitFixture();
        fixture.Customize(new SubclassOpenGenericCustomization<TOriginal, TSubClass>());

        var result = fixture.Freeze(typeof(AutoMock<>).MakeGenericType(typeof(TOriginal).GetGenericTypeDefinition().MakeGenericType(typeof(int))));
        var result2 = fixture.Create(typeof(AutoMock<>).MakeGenericType(typeof(TOriginal).GetGenericTypeDefinition().MakeGenericType(typeof(int))));

        result.Should().NotBeNull();
        result.Should().BeAssignableTo(typeof(AutoMock<>).MakeGenericType(typeof(TSubClass).GetGenericTypeDefinition().MakeGenericType(typeof(int))));

        result2.Should().NotBeNull();
        result2.Should().BeSameAs(result);
    }

    [Test]
    [TestCase<ITestIface<string>, BaseTestType<string>>]
    [TestCase<ITestIface<string>, SubTestType<string>>]
    [TestCase<ISubTestIface<string>, SubTestType<string>>]
    [TestCase<BaseTestType<string>, SubTestType<string>>]
    [TestCase<ITestIface<string>, ISubTestIface<string>>]
    public void Test_ThrowsCorretly_ForAutoMock_WhenAutoMock<TOriginal, TSubClass>()
        where TOriginal : class where TSubClass : class, TOriginal
    {
        var fixture = new UnitFixture();
        fixture.Customize(new SubclassOpenGenericCustomization<TOriginal, TSubClass>());


        Assert.Throws<InvalidCastException>(() => fixture.Freeze<AutoMock<TOriginal>>(),
                                                   $"Requested type AutoMock<{typeof(TOriginal).Name}> has been modified to AutoMock<{typeof(TSubClass).Name}>");
    }
}
