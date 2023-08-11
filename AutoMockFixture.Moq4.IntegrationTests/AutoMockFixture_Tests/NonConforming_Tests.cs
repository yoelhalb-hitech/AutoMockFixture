
namespace AutoMockFixture.Tests.AutoMockFixture_Tests;

internal class NonConforming_Tests
{
    [Test]
    public void Test_Array()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();

        // Act
        // We cannot use CreateAutoMock as it is not valid for arrays
        var obj = fixture.CreateWithAutoMockDependencies<InternalAbstractMethodTestClass[]>();
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<InternalAbstractMethodTestClass[]>();
        obj!.Length.Should().Be(3);
        obj.First().Should().BeAssignableTo<InternalAbstractMethodTestClass>();
        obj.First().Should().NotBeNull();
        obj.First().InternalTest.Should().NotBeNull();
    }

    [Test]
    public void Test_AutoMock_Array()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();

        // Act
        // We cannot use CreateAutoMock as it is not valid for arrays
        var obj = fixture.CreateWithAutoMockDependencies<AutoMock<InternalAbstractMethodTestClass>[]>();
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeOfType<AutoMock<InternalAbstractMethodTestClass>[]>();
        obj!.Length.Should().Be(3);
        obj.First().Should().BeOfType<AutoMock<InternalAbstractMethodTestClass>>();
        obj.First().GetMocked().Should().NotBeNull();
        obj.First().GetMocked().InternalTest.Should().NotBeNull();
    }

    [Test]
    public void Test_NonAutoMock_Tuple()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();

        // Act
        var obj = fixture.CreateNonAutoMock<Tuple<AutoMock<InternalAbstractMethodTestClass>, InternalSimpleTestClass, InternalAbstractSimpleTestClass>>();
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeOfType<Tuple<AutoMock<InternalAbstractMethodTestClass>, InternalSimpleTestClass, InternalAbstractSimpleTestClass>>();
        obj!.Item1.Should().BeOfType<AutoMock<InternalAbstractMethodTestClass>>();
        obj.Item1.GetMocked().Should().NotBeNull();
        obj.Item1.GetMocked().InternalTest.Should().NotBeNull();
        obj.Item2.Should().BeOfType<InternalSimpleTestClass>();
        obj.Item2.Should().NotBeNull();
        obj.Item2.InternalTest.Should().NotBeNull();
    }

    [Test]
    public void Test_NonAutoMock_WithDependencies_Tuple()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();

        // Act
        var obj = fixture.CreateWithAutoMockDependencies<Tuple<AutoMock<InternalAbstractMethodTestClass>, InternalSimpleTestClass, InternalAbstractSimpleTestClass>>();
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeOfType<Tuple<AutoMock<InternalAbstractMethodTestClass>, InternalSimpleTestClass, InternalAbstractSimpleTestClass>>();
        obj!.Item1.Should().BeOfType<AutoMock<InternalAbstractMethodTestClass>>();
        obj.Item1.GetMocked().Should().NotBeNull();
        obj.Item1.GetMocked().InternalTest.Should().NotBeNull();
        obj.Item2.Should().BeOfType<InternalSimpleTestClass>();
        obj.Item2.Should().NotBeNull();
        obj.Item2.InternalTest.Should().NotBeNull();
    }

    [Test]
    public void Test_AutoMock_NonGenericTask()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        Assert.Throws<InvalidOperationException>(() => fixture.CreateAutoMock<Task>());
    }

    [Test]
    public void Test_AutoMock_GenericTask()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        Assert.Throws<InvalidOperationException>(() => fixture.CreateAutoMock<Task<InternalAbstractMethodTestClass>>());
    }

    [Test]
    public void Test_NonAutoMock_Array()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();

        // Act
        var obj = fixture.CreateNonAutoMock<InternalSimpleTestClass[]>();
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeOfType<InternalSimpleTestClass[]>();
        obj!.Length.Should().Be(3);
        obj.First().Should().BeOfType<InternalSimpleTestClass>();
    }

    [Test]
    public void Test_NonAutoMock_WithDependencies_Array()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();

        // Act
        var obj = fixture.CreateWithAutoMockDependencies<InternalSimpleTestClass[]>();
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeOfType<InternalSimpleTestClass[]>();
        obj!.Length.Should().Be(3);
        obj.First().Should().BeOfType<InternalSimpleTestClass>();
    }

    [Test]
    public void Test_NonAutoMock_NonGenericTask()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateNonAutoMock<Task>();
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<Task>();

        obj!.IsCompleted.Should().BeTrue();
    }

    [Test]
    public async Task Test_NonAutoMock_GenericTask()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateNonAutoMock<Task<InternalSimpleTestClass>>();
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeOfType<Task<InternalSimpleTestClass>>();

        obj!.IsCompleted.Should().BeTrue();
        var inner = await obj;
        inner.InternalTest.Should().NotBeNull();
    }

    [Test]
    public void Test_WithDependencies_NonGenericTask()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<Task>();
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<Task>();

        obj!.IsCompleted.Should().BeTrue();
    }

    [Test]
    public async Task Test_WithDependencies_GenericTask()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var obj = fixture.CreateWithAutoMockDependencies<Task<InternalSimpleTestClass>>();
        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeOfType<Task<InternalSimpleTestClass>>();

        obj!.IsCompleted.Should().BeTrue();
        var inner = await obj;
        inner.InternalTest.Should().NotBeNull();
    }
}
