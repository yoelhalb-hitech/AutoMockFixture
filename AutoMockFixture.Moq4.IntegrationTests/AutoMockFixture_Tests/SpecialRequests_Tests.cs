using AutoMockFixture.NUnit3;
using AutoMockFixture.Extensions;

namespace AutoMockFixture.Tests.AutoMockFixture_Tests;

internal class SpecialRequests_Tests
{
    [Test]
    [TestCase<Action>(true)]
    [TestCase<Action>(false)]
    [TestCase<Action<Action>>(true)]
    [TestCase<Action<Action>>(false)]
    [TestCase<Action<string>>(true)]
    [TestCase<Action<string>>(false)]
    [TestCase<Func<Action>>(true)]
    [TestCase<Func<Action>>(false)]
    [TestCase<Func<Func<Action>>>(true)]
    [TestCase<Func<Func<Action>>>(false)]
    [TestCase<Func<string>>(true)]
    [TestCase<Func<string>>(false)]
    [TestCase<Func<string, int>>(true)]
    [TestCase<Func<string, int>>(false)]
    public void Test_AlwaysNotCallBase_ForDelegates<TDelagate>(bool callBase) where TDelagate : class
    {
        var fixture = new AbstractAutoMockFixture();

        var result = fixture.CreateAutoMock<TDelagate>(callBase: callBase);
        result.Should().BeAssignableTo<TDelagate>();

        AutoMock.IsAutoMock(result).Should().BeTrue();

        var mock = AutoMock.Get(result);
        mock.Should().NotBeNull();
        mock!.CallBase.Should().Be(false);

        fixture.CreateWithAutoMockDependencies<TDelagate>(callBase: callBase).Should().NotBeNull();

        fixture.CreateNonAutoMock<TDelagate>(callBase: callBase).Should().NotBeNull();
    }

    internal class TestClass { }

    [Test]
    [TestCase<Task>(true)]
    [TestCase<Task>(false)]
    [TestCase<Task<TestClass>>(true)]
    [TestCase<Task<TestClass>>(false)]
    [TestCase<Task<Action>>(true)]
    [TestCase<Task<Action>>(false)]
    [TestCase<Task<string>>(true)]
    [TestCase<Task<string>>(false)]
    [TestCase<Task<string[]>>(true)]
    [TestCase<Task<string[]>>(false)]
    [TestCase<Task<List<string>>>(true)]
    [TestCase<Task<List<string>>>(false)]
    [TestCase<Task<Func<Action>>>(true)]
    [TestCase<Task<Func<Action>>>(false)]
    [TestCase<Task<Func<Func<Action>>>>(true)]
    [TestCase<Task<Func<Func<Action>>>>(false)]
    [TestCase<Task<Func<string>>>(true)]
    [TestCase<Task<Func<string>>>(false)]
    [TestCase<Task<Func<string, int>>>(true)]
    [TestCase<Task<Func<string, int>>>(false)]
    public void Test_HandlesCorrectlyTasks<TTask>(bool callBase) where TTask : class
    {
        var fixture = new AbstractAutoMockFixture();

        var ex = Assert.Throws<InvalidOperationException>(() => fixture.CreateAutoMock(typeof(TTask), callBase: callBase));
        ex.Message.Should().Be($"{typeof(TTask).ToGenericTypeString()} cannot be AutoMock");

        fixture.CreateWithAutoMockDependencies<TTask>(callBase: callBase).Should().NotBeNull();

        fixture.CreateNonAutoMock<TTask>(callBase: callBase).Should().NotBeNull();
    }

    [Test]
    [TestCase<ValueTask>(true)]
    [TestCase<ValueTask>(false)]
    [TestCase<ValueTask<TestClass>>(true)]
    [TestCase<ValueTask<TestClass>>(false)]
    [TestCase<ValueTask<Action>>(true)]
    [TestCase<ValueTask<Action>>(false)]
    [TestCase<ValueTask<string>>(true)]
    [TestCase<ValueTask<string>>(false)]
    [TestCase<ValueTask<string[]>>(true)]
    [TestCase<ValueTask<string[]>>(false)]
    [TestCase<ValueTask<List<string>>>(true)]
    [TestCase<ValueTask<List<string>>>(false)]
    [TestCase<ValueTask<Func<Action>>>(true)]
    [TestCase<ValueTask<Func<Action>>>(false)]
    [TestCase<ValueTask<Func<Func<Action>>>>(true)]
    [TestCase<ValueTask<Func<Func<Action>>>>(false)]
    [TestCase<ValueTask<Func<string>>>(true)]
    [TestCase<ValueTask<Func<string>>>(false)]
    [TestCase<ValueTask<Func<string, int>>>(true)]
    [TestCase<ValueTask<Func<string, int>>>(false)]
    public void Test_HandlesCorrectlyValueTasks<TTask>(bool callBase) where TTask : struct
    {
        var fixture = new AbstractAutoMockFixture();

        fixture.CreateWithAutoMockDependencies<TTask>(callBase: callBase).Should().NotBeNull();

        fixture.CreateNonAutoMock<TTask>(callBase: callBase).Should().NotBeNull();
    }
}
