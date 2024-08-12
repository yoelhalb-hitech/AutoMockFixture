using AutoMockFixture.Moq4;
using FluentAssertions;
using System.Buffers.Text;
using static System.Environment;

namespace AutoMockFixture.TestUtils;

internal class AutoMockAssertions_Tests
{
    [Test]
    public void Test_BeAutoMock()
    {
        Assert.DoesNotThrow(() => new AutoMock<object>().Should().BeAutoMock());
        Assert.DoesNotThrow(() => new AutoMock<object>().Object.Should().BeAutoMock());

        var func1 = () => new object().Should().BeAutoMock();
        func1.Should().ThrowExactly<AssertionException>().WithMessage($"Expected IsAutoMock to be True, but found False.");
    }

    [Test]
    public void Test_NotBeAutoMock()
    {
        Assert.DoesNotThrow(() => new object().Should().NotBeAutoMock());

        var func1 = () => new AutoMock<object>().Should().NotBeAutoMock();
        func1.Should().ThrowExactly<AssertionException>().WithMessage($"Expected IsAutoMock to be False, but found True.");


        var func2 = () => new AutoMock<object>().Object.Should().NotBeAutoMock();
        func2.Should().ThrowExactly<AssertionException>().WithMessage($"Expected IsAutoMock to be False, but found True.");
    }

    [Test]
    public void Test_AllAutoMock()
    {
        var m = new AutoMock<object>();
        var o = new object();
        new object[] { m, m, m.Object, m.Object }.Should().AllAutoMock();

        var func1 = () => new object[] { m, o }.Should().AllAutoMock();
        func1.Should().ThrowExactly<AssertionException>().WithMessage($"The following is not auto mock: o");

        var func2 = () => new object[] { o, o }.Should().AllAutoMock();
        func2.Should().ThrowExactly<AssertionException>().WithMessage($"All entries are not auto mock");

        var func4 = () => new object?[] { m, o, o }.Should().AllAutoMock();
        func4.Should().ThrowExactly<AssertionException>().WithMessage($"The following are not auto mock: {NewLine}\to{NewLine}\to");
    }

    [Test]
    public void Test_AllNonAutoMock()
    {
        var m = new AutoMock<object>();
        var o = new object();
        new object[] { o, o }.Should().AllNonAutoMock();

        var func1 = () => new object[] { m, o }.Should().AllNonAutoMock();
        func1.Should().ThrowExactly<AssertionException>().WithMessage($"The following is auto mock: m");

        var func2 = () => new object[] { m, m, m.Object, m.Object }.Should().AllNonAutoMock();
        func2.Should().ThrowExactly<AssertionException>().WithMessage($"All entries are auto mock");

        var func4 = () => new object?[] { m, m.Object, o, o }.Should().AllNonAutoMock();
        func4.Should().ThrowExactly<AssertionException>().WithMessage($"The following are auto mock: {NewLine}\tm{NewLine}\tm.Object");
    }
}
