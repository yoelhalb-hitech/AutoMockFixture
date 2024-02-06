using Microsoft.VisualBasic;
using NUnit.Framework;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using AutoMockFixture.NUnit3;

namespace AutoMockFixture.Tests.FixtureUtils.Builders.SpecialBuilders;

internal class EnumerableBuilder_Tests
{
    public class WithAbstractEnumerableProperty
    {
        public AbstractListWithAddRange<string>? AbstractListPropWithAddRange { get; set; }
        public AbstractListWithAdd<string>? AbstractListPropWithAdd { get; set; }
        public AbstractList<string>? AbstractListProp { get; set; }
    }
    public class WithBuiltInTypes
    {
        public string[]? PropArray { get; set; }
        public List<string>? PropList { get; set; }
        public HashSet<string>? PropSet { get; set; }
        public Dictionary<string, string>? PropDict { get; set; }
        public ConcurrentDictionary<string, string>? PropConcurentDict { get; set; }
    }
    public abstract class AbstractList<T> : IEnumerable<T> // Remember that Enumerbale builder will only work for generic types
    {
        protected List<T> list = new List<T>();

        public IEnumerator GetEnumerator() => list.GetEnumerator();

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => list.GetEnumerator();
    }
    public abstract class AbstractListWithAddRange<T> : AbstractList<T>
    {
        public void AddRange(IEnumerable<T> en) => list.AddRange(en);
    }
    public abstract class AbstractListWithAdd<T> : AbstractList<T>
    {
        public void Add(T en) => list.Add(en);
    }

    public class WithNonAbstractEnumerableProperty
    {
        public NonAbstractList<string>? NonAbstractListProp { get; set; }
        public NonAbstractListWithAdd<string>? NonAbstractListPropWithAdd { get; set; }
        public NonAbstractListWithAddRange<string>? NonAbstractListPropWithAddRange { get; set; }
    }
    public class NonAbstractList<T> : AbstractList<T> { }
    public class NonAbstractListWithAdd<T> : AbstractListWithAdd<T> { }
    public class NonAbstractListWithAddRange<T> : AbstractListWithAddRange<T> { }

    [Test]
    [TestCase<AbstractList<string>>()]
    [TestCase<AbstractListWithAdd<string>>]
    [TestCase<AbstractListWithAddRange<string>>]
    [TestCase<AbstractList<int>>]
    [TestCase<AbstractListWithAdd<int>>]
    [TestCase<AbstractListWithAddRange<int>>]
    public void Test_CreatesAutoMock_NonAutoMockType_ForAbstract<T>() where T : class
    {
        var fixture = new AbstractAutoMockFixture();

        var result = fixture.CreateNonAutoMock<T>(callBase: true); // So the list should have a value otherwise it will be handled by AutoFixture
        result.Should().NotBeNull();
        AutoMock.Get(result).Should().NotBeNull();

        result = fixture.CreateAutoMock<T>(callBase: true); // So the list should have a value otherwise it will be handled by AutoFixture
        result.Should().NotBeNull();
        AutoMock.Get(result).Should().NotBeNull();

        result = fixture.CreateWithAutoMockDependencies<T>(callBase: true); // So the list should have a value otherwise it will be handled by AutoFixture
        result.Should().NotBeNull();
        AutoMock.Get(result).Should().NotBeNull();
    }

    [Test]
    [TestCase<AutoMock<AbstractList<string>>>]
    [TestCase<AutoMock<AbstractListWithAdd<string>>>]
    [TestCase<AutoMock<AbstractListWithAddRange<string>>>]
    [TestCase<AutoMock<AbstractList<int>>>]
    [TestCase<AutoMock<AbstractListWithAdd<int>>>]
    [TestCase<AutoMock<AbstractListWithAddRange<int>>>]
    public void Test_CreatesAutoMock_ForAbstract_AutoMockType<T>() where T : class
    {
        var fixture = new AbstractAutoMockFixture();

        var result = fixture.CreateNonAutoMock<T>(callBase: true); // So the list should have a value otherwise it will be handled by AutoFixture
        result.Should().NotBeNull();
        (result as IAutoMock).Should().NotBeNull();
        (result as IAutoMock)!.GetMocked().Should().NotBeNull();

        result = fixture.CreateAutoMock<T>(callBase: true); // So the list should have a value otherwise it will be handled by AutoFixture
        result.Should().NotBeNull();
        (result as IAutoMock).Should().NotBeNull();
        (result as IAutoMock)!.GetMocked().Should().NotBeNull();

        result = fixture.CreateWithAutoMockDependencies<T>(callBase: true); // So the list should have a value otherwise it will be handled by AutoFixture
        result.Should().NotBeNull();
        (result as IAutoMock).Should().NotBeNull();
        (result as IAutoMock)!.GetMocked().Should().NotBeNull();
    }

    [Test]
    [TestCase<NonAbstractList<string>>]
    [TestCase<NonAbstractListWithAdd<string>>]
    [TestCase<NonAbstractListWithAddRange<string>>]
    [TestCase<NonAbstractList<int>>]
    [TestCase<NonAbstractListWithAdd<int>>]
    [TestCase<NonAbstractListWithAddRange<int>>]
    public void Test_WorksCorrectly_ForNonAbstract<T>() where T : class
    {
        var fixture = new AbstractAutoMockFixture();

        var result = fixture.CreateNonAutoMock<T>(callBase: true); // So the list should have a value otherwise it will be handled by AutoFixture
        result.Should().NotBeNull();
        (result as IAutoMock).Should().BeNull();
        Assert.Throws<ArgumentException>(() => AutoMock.Get(result), "Object instance was not created by AutoMockFixture.Moq.AutoMock. (Parameter 'mocked')");

        result = fixture.CreateAutoMock<T>(callBase: true); // So the list should have a value otherwise it will be handled by AutoFixture
        result.Should().NotBeNull();
        (result as IAutoMock).Should().BeNull();
        Assert.DoesNotThrow(() => AutoMock.Get(result));
        AutoMock.Get(result).Should().NotBeNull();

        result = fixture.CreateWithAutoMockDependencies<T>(callBase: true); // So the list should have a value otherwise it will be handled by AutoFixture
        result.Should().NotBeNull();
        (result as IAutoMock).Should().BeNull();
        Assert.Throws<ArgumentException>(() => AutoMock.Get(result), "Object instance was not created by AutoMockFixture.Moq.AutoMock. (Parameter 'mocked')");
    }

    [Test]
    [TestCase<AutoMock<NonAbstractList<string>>>]
    [TestCase<AutoMock<NonAbstractListWithAdd<string>>>]
    [TestCase<AutoMock<NonAbstractListWithAddRange<string>>>]
    [TestCase<AutoMock<NonAbstractList<int>>>]
    [TestCase<AutoMock<NonAbstractListWithAdd<int>>>]
    [TestCase<AutoMock<NonAbstractListWithAddRange<int>>>]
    public void Test_Throws_ForNonAbstractAutoMockType<T>() where T : class
    {
        var fixture = new AbstractAutoMockFixture();

        var result = fixture.CreateAutoMock<T>(callBase: true); // So the list should have a value otherwise it will be handled by AutoFixture
        result.Should().NotBeNull();
        (result as IAutoMock).Should().NotBeNull();
        (result as IAutoMock)!.GetMocked().Should().NotBeNull();

        result = fixture.CreateNonAutoMock<T>(callBase: true); // So the list should have a value otherwise it will be handled by AutoFixture
        result.Should().NotBeNull();
        (result as IAutoMock).Should().NotBeNull();
        (result as IAutoMock)!.GetMocked().Should().NotBeNull();

        result = fixture.CreateWithAutoMockDependencies<T>(callBase: true); // So the list should have a value otherwise it will be handled by AutoFixture
        result.Should().NotBeNull();
        (result as IAutoMock).Should().NotBeNull();
        (result as IAutoMock)!.GetMocked().Should().NotBeNull();
    }

    [Test]
    public void Test_HandlesCorrectlyAbstractClassProp()
    {
        var fixture = new AbstractAutoMockFixture();

        var result = fixture.CreateNonAutoMock<WithAbstractEnumerableProperty>(callBase: true); // So the list should have a value otherwise it will be handled by AutoFixture
        result.Should().NotBeNull();

        result!.AbstractListProp.Should().NotBeNull();
        result.AbstractListProp!.Count().Should().Be(0);

        result!.AbstractListPropWithAdd.Should().NotBeNull();
        result.AbstractListPropWithAdd!.Count().Should().Be(3);
        result.AbstractListPropWithAdd!.Should().NotContainNulls();

        result!.AbstractListPropWithAddRange.Should().NotBeNull();
        result.AbstractListPropWithAddRange!.Count().Should().Be(3);
        result.AbstractListPropWithAddRange!.Should().NotContainNulls();
    }

    [Test]
    public void Test_HandlesCorrectlyNonAbstractClassProp()
    {
        var fixture = new AbstractAutoMockFixture();

        var result = fixture.CreateNonAutoMock<WithNonAbstractEnumerableProperty>(callBase: true); // So the list should have a value otherwise it will be handled by AutoFixture
        result.Should().NotBeNull();

        result!.NonAbstractListProp.Should().NotBeNull();
        result.NonAbstractListProp!.Count().Should().Be(0);

        result!.NonAbstractListPropWithAdd.Should().NotBeNull();
        result.NonAbstractListPropWithAdd!.Count().Should().Be(3);
        result.NonAbstractListPropWithAdd!.Should().NotContainNulls();

        result!.NonAbstractListPropWithAddRange.Should().NotBeNull();
        result.NonAbstractListPropWithAddRange!.Count().Should().Be(3);
        result.NonAbstractListPropWithAddRange!.Should().NotContainNulls();
    }

    [Test]
    public void Test_HandlesCorrectlyNonAbstractClassProp_AutoMockRequest()
    {
        var fixture = new AbstractAutoMockFixture();

        var result = fixture.CreateAutoMock<WithNonAbstractEnumerableProperty>(callBase: true); // So the list should have a value otherwise it will be handled by AutoFixture
        result.Should().NotBeNull();

        result!.NonAbstractListProp.Should().NotBeNull();
        result.NonAbstractListProp!.Count().Should().Be(0);

        result!.NonAbstractListPropWithAdd.Should().NotBeNull();
        result.NonAbstractListPropWithAdd!.Count().Should().Be(3);
        result.NonAbstractListPropWithAdd!.Should().NotContainNulls();

        result!.NonAbstractListPropWithAddRange.Should().NotBeNull();
        result.NonAbstractListPropWithAddRange!.Count().Should().Be(3);
        result.NonAbstractListPropWithAddRange!.Should().NotContainNulls();
    }

    [Test]
    public void Test_HandlesCorrectlyBuiltInTypes_AutoMockRequest()
    {
        var fixture = new AbstractAutoMockFixture();

        var result = fixture.CreateAutoMock<WithBuiltInTypes>(callBase: true); // So the list should have a value otherwise it will be handled by AutoFixture
        result.Should().NotBeNull();

        result!.PropArray.Should().NotBeNull();
        result.PropArray!.Count().Should().Be(3);
        result.PropArray!.Should().NotContainNulls();

        result!.PropList.Should().NotBeNull();
        result.PropList!.Count().Should().Be(3);
        result.PropList!.Should().NotContainNulls();

        result!.PropSet.Should().NotBeNull();
        result.PropSet!.Count().Should().Be(3);
        result.PropSet!.Should().NotContainNulls();

        result!.PropDict.Should().NotBeNull();
        result.PropDict!.Count().Should().Be(3);

        result!.PropConcurentDict.Should().NotBeNull();
        result.PropConcurentDict!.Count().Should().Be(0);

    }
}
