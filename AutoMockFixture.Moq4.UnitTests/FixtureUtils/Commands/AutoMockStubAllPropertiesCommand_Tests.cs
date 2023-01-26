using AutoMockFixture.FixtureUtils.Commands;
using AutoMockFixture.Moq.AutoMockUtils;
using AutoMockFixture.Moq.FixtureUtils.Commands;

namespace AutoMockFixture.Tests.FixtureUtils.Commands;

internal class AutoMockStubAllPropertiesCommand_Tests
{
    public class InnerTest {}

    public class TestClass
    {
        public virtual InnerTest? TestPublic { get; set; }
        internal virtual InnerTest? TestInternal { get; set; }
        protected virtual InnerTest? TestProtected { get; set; }
        public virtual InnerTest? TestImplementationPublic { get => throw new Exception(); set => throw new Exception(); }
        internal virtual InnerTest? TestImplementationInternal { get => throw new Exception(); set => throw new Exception(); }
        protected virtual InnerTest? TestImplementationProtected { get => throw new Exception(); set => throw new Exception(); }
    }

    public abstract class TestAbstractClass
    {
        public virtual InnerTest? TestPublic { get; set; }
        internal virtual InnerTest? TestInternal { get; set; }
        protected virtual InnerTest? TestProtected { get; set; }
        public virtual InnerTest? TestImplementationPublic { get => throw new Exception(); set => throw new Exception(); }
        internal virtual InnerTest? TestImplementationInternal { get => throw new Exception(); set => throw new Exception(); }
        protected virtual InnerTest? TestImplementationProtected { get => throw new Exception(); set => throw new Exception(); }

        public abstract InnerTest? TestPublicAbstract { get; set; }
        internal abstract InnerTest? TestInternalAbstract { get; set; }
        protected abstract InnerTest? TestProtectedAbstract { get; set; }
    }

    public interface TestInterface
    {
        public InnerTest? TestPublic { get; set; }
        internal InnerTest? TestInternal { get; set; }
        protected InnerTest? TestProtected { get; set; }
        public virtual InnerTest? TestImplementationPublic { get => throw new Exception(); set => throw new Exception(); }
        internal virtual InnerTest? TestImplementationInternal { get => throw new Exception(); set => throw new Exception(); }
        protected virtual InnerTest? TestImplementationProtected { get => throw new Exception(); set => throw new Exception(); }

        public abstract InnerTest? TestPublicAbstract { get; set; }
        internal abstract InnerTest? TestInternalAbstract { get; set; }
        protected abstract InnerTest? TestProtectedAbstract { get; set; }
    }

    [Test]
    [TestCase(typeof(AutoMock<TestClass>))]
    [TestCase(typeof(AutoMock<TestAbstractClass>))]
    [TestCase(typeof(AutoMock<TestInterface>))]

    public void Test_SetsUpAllProperties_WhenNotCallBase(Type type)
    {
        var mock = (Activator.CreateInstance(type) as global::Moq.Mock)!;

        var command = new AutoMockStubAllPropertiesCommand(new AutoMockHelpers());

        command.Execute(mock, null!);

        mock.Setups.Any(m => m.ToString()!.EndsWith(".SetupAllProperties()")).Should().BeTrue();
    }

    [Test]
    public void Test_DoesNotSetup_WhenCallBase_AndRegularClass()
    {
        var mock = new AutoMock<TestClass>();
        mock.CallBase = true;

        var command = new AutoMockStubAllPropertiesCommand(new AutoMockHelpers());

        command.Execute(mock, null!);

        mock.Setups.Should().BeEmpty();
    }

    [Test]
    public void Test_SetupsAbstractMembers_WhenCallBase_AndAbstractClass()
    {
        var mock = new AutoMock<TestAbstractClass>();
        mock.CallBase = true;

        var command = new AutoMockStubAllPropertiesCommand(new AutoMockHelpers());

        command.Execute(mock, null!);

        mock.Setups.Count.Should().Be(3);
        mock.Setups.Any(s => s.ToString()!.Contains(nameof(TestAbstractClass.TestPublicAbstract))).Should().BeTrue();
        mock.Setups.Any(s => s.ToString()!.Contains(nameof(TestAbstractClass.TestInternalAbstract))).Should().BeTrue();
        mock.Setups.Any(s => s.ToString()!.Contains("TestProtectedAbstract")).Should().BeTrue();

        // We need a space at the end since TestPublicAbstract also contains TestPublic etc.
        mock.Setups.Any(s => s.ToString()!.Contains(nameof(TestAbstractClass.TestPublic) + " ")).Should().BeFalse();
        mock.Setups.Any(s => s.ToString()!.Contains(nameof(TestAbstractClass.TestInternal) + " ")).Should().BeFalse();
        mock.Setups.Any(s => s.ToString()!.Contains("TestProtected ")).Should().BeFalse();

        mock.Setups.Any(s => s.ToString()!.Contains(nameof(TestAbstractClass.TestImplementationPublic))).Should().BeFalse();
        mock.Setups.Any(s => s.ToString()!.Contains(nameof(TestAbstractClass.TestImplementationInternal))).Should().BeFalse();
        mock.Setups.Any(s => s.ToString()!.Contains("TestImplementationProtected")).Should().BeFalse();
    }

    [Test]
    public void Test_SetupsAbstractMembers_WhenCallBase_AndInterface()
    {
        var mock = new AutoMock<TestInterface>();
        mock.CallBase = true;

        var command = new AutoMockStubAllPropertiesCommand(new AutoMockHelpers());

        command.Execute(mock, null!);

        mock.Setups.Count.Should().Be(6);
        mock.Setups.Any(s => s.ToString()!.Contains(nameof(TestInterface.TestPublicAbstract))).Should().BeTrue();
        mock.Setups.Any(s => s.ToString()!.Contains(nameof(TestInterface.TestInternalAbstract))).Should().BeTrue();
        mock.Setups.Any(s => s.ToString()!.Contains("TestProtectedAbstract")).Should().BeTrue();

        // We need a space at the end since TestPublicAbstract also contains TestPublic etc.
        mock.Setups.Any(s => s.ToString()!.Contains(nameof(TestInterface.TestPublic) + " ")).Should().BeTrue();
        mock.Setups.Any(s => s.ToString()!.Contains(nameof(TestInterface.TestInternal) + " ")).Should().BeTrue();
        mock.Setups.Any(s => s.ToString()!.Contains("TestProtected ")).Should().BeTrue();

        mock.Setups.Any(s => s.ToString()!.Contains(nameof(TestInterface.TestImplementationPublic))).Should().BeFalse();
        mock.Setups.Any(s => s.ToString()!.Contains(nameof(TestInterface.TestImplementationInternal))).Should().BeFalse();
        mock.Setups.Any(s => s.ToString()!.Contains("TestImplementationProtected")).Should().BeFalse();
    }
}
