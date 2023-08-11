
using NUnit.Framework.Internal;

namespace AutoMockFixture.Tests.Extensions;

internal class TypeExtensions_Tests
{
    internal class TestClass
    {
        public void TestMethod() { }
        public void TestMethod(int i) { }
        public void TestMethod<T>(int i) { }
        public int TestMethod1() => 0;
        private string TestMethod2() => "Test";
        internal int TestProp1 => 10;
        private string TestProp2 { set => throw new Exception(value); }
        protected string? TestProp3 { get; set; }
        internal event EventHandler TestEvent;
    }

    [Test]
    public void Test_GetAllMethods()
    {
        var result = typeof(TestClass).GetAllMethods();

        result.Count().Should().Be(11);

        var names = result.Select(r => r.Name).ToArray();
        names.Where(n => n == nameof(TestClass.TestMethod)).Count().Should().Be(3);
        names.Where(n => n == nameof(TestClass.TestMethod1)).Count().Should().Be(1);
        names.Where(n => n == "TestMethod2").Count().Should().Be(1);
        names.Where(n => n == "get_TestProp1").Count().Should().Be(1);
        names.Where(n => n == "set_TestProp2").Count().Should().Be(1);
        names.Where(n => n == "get_TestProp3").Count().Should().Be(1);
        names.Where(n => n == "set_TestProp3").Count().Should().Be(1);
        names.Where(n => n == "add_" + nameof(TestClass.TestEvent)).Count().Should().Be(1);
        names.Where(n => n == "remove_" + nameof(TestClass.TestEvent)).Count().Should().Be(1);
    }

    [Test]
    [TestCase(typeof(List<string>), ExpectedResult = "List<string>")]
    [TestCase(typeof(string[]), ExpectedResult = "string[]")]
    [TestCase(typeof(List<List<string>>), ExpectedResult = "List<List<string>>")]
    [TestCase(typeof(List<string[]>), ExpectedResult = "List<string[]>")]
    [TestCase(typeof(int?), ExpectedResult = "int?")]
    [TestCase(typeof((int?, string, IEnumerable<char>, IntPtr, List<IList<decimal[]>>)), ExpectedResult = "(int?,string,IEnumerable<char>,nint,List<IList<decimal[]>>)")]
    public string Test_ToGenericTypeString(Type type)
        => type.ToGenericTypeString();
}
