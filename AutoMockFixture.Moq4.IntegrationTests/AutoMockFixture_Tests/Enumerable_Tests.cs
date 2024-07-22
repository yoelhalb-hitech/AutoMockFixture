using Microsoft.VisualBasic;
using NUnit.Framework;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using AutoMockFixture.NUnit3;
using AutoMockT = AutoMockFixture.Moq4.AutoMock;

namespace AutoMockFixture.Moq4.IntegrationTests.AutoMockFixture_Tests;

internal class Enumerable_Tests
{
    public class WithAbstractEnumerableProperty
    {
        public AbstractListWithAddRange<string>? AbstractListPropWithAddRange { get; set; }
        public AbstractListWithAdd<string>? AbstractListPropWithAdd { get; set; }
        public AbstractList<string>? AbstractListProp { get; set; }
    }
    public class WithBuiltInTypes<T> where T : notnull
    {
        public T[]? PropArray { get; set; }
        public T[][]? PropJaggedArray { get; set; }
        public T[,]? Prop2DimArray { get; set; }
        public List<T>? PropList { get; set; }
        public HashSet<T>? PropSet { get; set; }
        public Dictionary<T, T>? PropDict { get; set; }
        public ConcurrentDictionary<T, T>? PropConcurentDict { get; set; }
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
        AutoMockT.Get(result).Should().NotBeNull();

        result = fixture.CreateAutoMock<T>(callBase: true); // So the list should have a value otherwise it will be handled by AutoFixture
        result.Should().NotBeNull();
        AutoMockT.Get(result).Should().NotBeNull();

        result = fixture.CreateWithAutoMockDependencies<T>(callBase: true); // So the list should have a value otherwise it will be handled by AutoFixture
        result.Should().NotBeNull();
        AutoMockT.Get(result).Should().NotBeNull();
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
        Assert.Throws<ArgumentException>(() => AutoMockT.Get(result), "Object instance was not created by AutoMockFixture.Moq.AutoMock. (Parameter 'mocked')");

        result = fixture.CreateAutoMock<T>(callBase: true); // So the list should have a value otherwise it will be handled by AutoFixture
        result.Should().NotBeNull();
        (result as IAutoMock).Should().BeNull();
        Assert.DoesNotThrow(() => AutoMockT.Get(result));
        AutoMockT.Get(result).Should().NotBeNull();

        result = fixture.CreateWithAutoMockDependencies<T>(callBase: true); // So the list should have a value otherwise it will be handled by AutoFixture
        result.Should().NotBeNull();
        (result as IAutoMock).Should().BeNull();
        Assert.Throws<ArgumentException>(() => AutoMockT.Get(result), "Object instance was not created by AutoMockFixture.Moq.AutoMock. (Parameter 'mocked')");
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

        result!.AbstractListProp.Should().BeEmpty();

        result!.AbstractListPropWithAdd.Should().HaveCount(3).And.NotContainNulls();

        result!.AbstractListPropWithAddRange.Should().HaveCount(3).And.NotContainNulls();
    }

    [Test]
    public void Test_HandlesCorrectlyNonAbstractClassProp()
    {
        var fixture = new AbstractAutoMockFixture();

        var result = fixture.CreateNonAutoMock<WithNonAbstractEnumerableProperty>(callBase: true); // So the list should have a value otherwise it will be handled by AutoFixture
        result.Should().NotBeNull();

        result!.NonAbstractListProp.Should().BeEmpty();

        result!.NonAbstractListPropWithAdd.Should().HaveCount(3).And.NotContainNulls();

        result!.NonAbstractListPropWithAddRange.Should().HaveCount(3).And.NotContainNulls();
    }

    [Test]
    public void Test_HandlesCorrectlyNonAbstractClassProp_AutoMockRequest()
    {
        var fixture = new AbstractAutoMockFixture();

        var result = fixture.CreateAutoMock<WithNonAbstractEnumerableProperty>(callBase: true); // So the list should have a value otherwise it will be handled by AutoFixture
        result.Should().NotBeNull();

        result!.NonAbstractListProp.Should().BeEmpty();

        result!.NonAbstractListPropWithAdd.Should().HaveCount(3).And.NotContainNulls();

        result!.NonAbstractListPropWithAddRange.Should().HaveCount(3).And.NotContainNulls();
    }

    [Test]
    [TestCase<string>, TestCase<string[]>, TestCase<List<string>>]
    [TestCase<decimal>, TestCase<decimal[]>, TestCase<List<decimal>>]
    [TestCase<int>, TestCase<int[]>, TestCase<List<int>>]
    public void Test_HandlesCorrectlyBuiltInTypes_AutoMockRequest<T>() where T : notnull
    {
        var fixture = new AbstractAutoMockFixture();

        var result = fixture.CreateAutoMock<WithBuiltInTypes<T>>(callBase: true); // So the list should have a value otherwise it will be handled by AutoFixture
        result.Should().NotBeNull();

        result!.PropArray.Should().HaveCount(3).And.NotContainNulls().And.NotContain(default(T)!);

        result!.PropJaggedArray.Should().HaveCount(3).And.NotContainNulls();
        result.PropJaggedArray!.ToList().ForEach(p => p.Should().HaveCount(3).And.NotContainNulls());

        result!.Prop2DimArray.Should().HaveCount(9).And.NotContainNulls();
        foreach (var item in result!.Prop2DimArray!) item.Should().NotBe(default(T));

        result!.PropList.Should().HaveCount(3).And.NotContainNulls().And.NotContain(default(T)!);

        result!.PropSet.Should().HaveCount(3).And.NotContainNulls().And.NotContain(default(T)!);

        result!.PropDict.Should().HaveCount(3).And.NotContainValue(default(T)!);

        result!.PropConcurentDict.Should().HaveCount(3).And.NotContainValue(default(T)!);
    }

    [Test]
    [TestCase<string>, TestCase<string[]>, TestCase<List<string>>]
    [TestCase<decimal>, TestCase<decimal[]>, TestCase<List<decimal>>]
    [TestCase<int>, TestCase<int[]>, TestCase<List<int>>]
    public void Test_HandlesCorrectlyBuiltInTypes_NonAutoMockRequest<T>() where T : notnull
    {
        var fixture = new AbstractAutoMockFixture();

        var result = fixture.CreateNonAutoMock<WithBuiltInTypes<T>>(callBase: true); // So the list should have a value otherwise it will be handled by AutoFixture
        result.Should().NotBeNull();

        result!.PropArray.Should().HaveCount(3).And.NotContainNulls().And.NotContain(default(T)!);

        result!.PropJaggedArray.Should().HaveCount(3).And.NotContainNulls();
        result.PropJaggedArray!.ToList().ForEach(p => p.Should().HaveCount(3).And.NotContainNulls());

        result!.Prop2DimArray.Should().HaveCount(9).And.NotContainNulls();
        foreach (var item in result!.Prop2DimArray!) item.Should().NotBe(default(T));

        result!.PropList.Should().HaveCount(3).And.NotContainNulls().And.NotContain(default(T)!);

        result!.PropSet.Should().HaveCount(3).And.NotContainNulls().And.NotContain(default(T)!);

        result!.PropDict.Should().HaveCount(3).And.NotContainValue(default(T)!);

        result!.PropConcurentDict.Should().HaveCount(3).And.NotContainValue(default(T)!);

    }

    [Test]
    public void Test_HandlesCorrectly_Callbase_BasedOnStartTracker([Values(true, false)] bool callBase)
    {
        var fixture = new AbstractAutoMockFixture();

        var result = fixture.CreateAutoMock<WithNonAbstractEnumerableProperty>(callBase: callBase);
        result.Should().NotBeNull();
        result!.NonAbstractListProp.Should().NotBeNull();

        var mock = AutoMockT.Get(result!.NonAbstractListProp);
        mock.Should().NotBeNull();

        mock!.CallBase.Should().Be(callBase);
    }
}
