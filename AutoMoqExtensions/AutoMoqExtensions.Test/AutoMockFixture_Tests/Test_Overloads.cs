
namespace AutoMoqExtensions.Test.AutoMockFixture_Tests;

internal class Test_Overloads
{
    internal class Overloads
    {
        public virtual Task Overload() { return Task.CompletedTask; }
        public virtual Task Overload(string str) { return Task.CompletedTask; }
        public virtual Task Overload(int i) { return Task.CompletedTask; }            
    }

    [Test]
    [UnitAutoData]
    public void Test_Overloads_DoesNotThrow_WhenAutoMockDependencies()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var o = fixture.CreateWithAutoMockDependencies<Overloads>();
        // Assert
        Assert.DoesNotThrow(() => o.Overload());
        Assert.DoesNotThrow(() => o.Overload());
        Assert.DoesNotThrow(() => o.Overload("Test"));
        Assert.DoesNotThrow(() => o.Overload("Test"));
        Assert.DoesNotThrow(() => o.Overload("Test1"));
        Assert.DoesNotThrow(() => o.Overload(23));
        Assert.DoesNotThrow(() => o.Overload(23));
        Assert.DoesNotThrow(() => o.Overload(24));
    }

    [Test]
    [UnitAutoData]
    public void Test_Overloads_DoesNotThrow_WhenAutoMock()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var o = fixture.CreateAutoMock<Overloads>();
        // Assert
        Assert.DoesNotThrow(() => o.Overload());
        Assert.DoesNotThrow(() => o.Overload());
        Assert.DoesNotThrow(() => o.Overload("Test"));
        Assert.DoesNotThrow(() => o.Overload("Test"));
        Assert.DoesNotThrow(() => o.Overload("Test1"));
        Assert.DoesNotThrow(() => o.Overload(23));
        Assert.DoesNotThrow(() => o.Overload(23));
        Assert.DoesNotThrow(() => o.Overload(24));
    }

    [Test]
    [UnitAutoData]
    public void Test_Overloads_DoesNotThrow_WhenNonAutoMock()
    {
        // Arrange
        var fixture = new AbstractAutoMockFixture();
        // Act
        var o = fixture.CreateNonAutoMock<Overloads>();
        // Assert
        Assert.DoesNotThrow(() => o.Overload());
        Assert.DoesNotThrow(() => o.Overload());
        Assert.DoesNotThrow(() => o.Overload("Test"));
        Assert.DoesNotThrow(() => o.Overload("Test"));
        Assert.DoesNotThrow(() => o.Overload("Test1"));
        Assert.DoesNotThrow(() => o.Overload(23));
        Assert.DoesNotThrow(() => o.Overload(23));
        Assert.DoesNotThrow(() => o.Overload(24));
    }
}
