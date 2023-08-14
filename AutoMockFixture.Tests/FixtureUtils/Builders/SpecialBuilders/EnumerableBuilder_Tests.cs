using Microsoft.VisualBasic;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AutoMockFixture.Tests.FixtureUtils.Builders.SpecialBuilders;

internal class EnumerableBuilder_Tests
{
    public class WithAbstractEnumerableProperty
    {
        public AbstractList<string>? AbstractListProp { get; set; }
    }
    public abstract class AbstractList<T> : IEnumerable<T> // Remember that Enumerbale builder will only work for generic types
    {
        List<T> list = new List<T>();
        public void AddRange(IEnumerable<T> en) => list.AddRange(en);

        public IEnumerator GetEnumerator() => list.GetEnumerator();

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => list.GetEnumerator();
    }

    [Test]
    public void Test_HandlesCorrectlyAbstractClass()
    {
        var fixture = new AbstractAutoMockFixture();

        var result = fixture.CreateNonAutoMock<AbstractList<string>>();
        result.Should().NotBeNull();
    }

    [Test]
    public void Test_HandlesCorrectlyAbstractClass_ValueType()
    {
        var fixture = new AbstractAutoMockFixture();

        var result = fixture.CreateNonAutoMock<AbstractList<int>>();
        result.Should().NotBeNull();
    }

    [Test]
    public void Test_HandlesCorrectlyAbstractClassProp()
    {
        var fixture = new AbstractAutoMockFixture();

        var result = fixture.CreateNonAutoMock<WithAbstractEnumerableProperty>();
        result.Should().NotBeNull();
        result.AbstractListProp.Should().NotBeNull();
        result.AbstractListProp.Count().Should().Be(3);
    }
}
