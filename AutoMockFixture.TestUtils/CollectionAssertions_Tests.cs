using FluentAssertions;
using static System.Environment;

namespace AutoMockFixture.TestUtils;

internal class CollectionAssertions_Tests
{
    class Base { }
    class Base1 { }
    class Base2 { }
    class Sub : Base { }
    class Sub1 : Base1 { }
    class Sub2 : Base2 { }

    [Test]
    public void Test_AllNotBeOfType()
    {
        Base b = new();
        Base1 b1 = new();
        Base2 b2 = new();
        Sub s = new();
        Sub1 s1 = new();
        Sub2 s2 = new();
        new object[] { b1, b2, s1, s2 }.Should().AllNotBeOfType<object, Base>().And.AllNotBeOfType<object, Sub>();

        var func1 = () => new object[] { b, s }.Should().AllNotBeOfType<object, Base>();
        func1.Should().ThrowExactly<AssertionException>().WithMessage($"The following is of type Base: b");

        var func2 = () => new object[] { b, b, b, }.Should().AllNotBeOfType<object, Base>();
        func2.Should().ThrowExactly<AssertionException>().WithMessage($"All entries are of Type Base");

        var func3 = () => new object[] { b, s }.Should().AllNotBeOfType<object, Sub>();
        func3.Should().ThrowExactly<AssertionException>().WithMessage("The following is of type Sub: s");

        var func4 = () => new object[] { b, s, s }.Should().AllNotBeOfType<object, Sub>();
        func4.Should().ThrowExactly<AssertionException>().WithMessage($"The following are of type Sub: {NewLine}\ts{NewLine}\ts");
    }

    [Test]
    public void Test_AllNotBeAssignableTo()
    {
        Base b = new();
        Base1 b1 = new();
        Base2 b2 = new();
        Sub s = new();
        Sub1 s1 = new();
        Sub2 s2 = new();
        new object[] { b1, b2, s1, s2 }.Should().AllNotBeAssignableTo<object, Base>().And.AllNotBeAssignableTo<object, Sub>();

        var func1 = () => new object[] { b, s, b1 }.Should().AllNotBeAssignableTo<object, Base>();
        func1.Should().ThrowExactly<AssertionException>().WithMessage($"The following are assignable to Base: {NewLine}\tb{NewLine}\ts");

        var func2 = () => new object[] { b, s, s, }.Should().AllNotBeAssignableTo<object, Base>();
        func2.Should().ThrowExactly<AssertionException>().WithMessage($"All entries are assignable to Base");

        var func3 = () => new object[] { b, s }.Should().AllNotBeAssignableTo<object, Sub>();
        func3.Should().ThrowExactly<AssertionException>().WithMessage("The following is assignable to Sub: s");

        var func4 = () => new object[] { b, s, s }.Should().AllNotBeAssignableTo<object, Sub>();
        func4.Should().ThrowExactly<AssertionException>().WithMessage($"The following are assignable to Sub: {NewLine}\ts{NewLine}\ts");
    }

    [Test]
    public void Test_AllBeSameAs()
    {
        var a = new object();
        var c = a;
        var b = new object();
        var d = b;
        new object[] { a, c }.Should().AllBeSameAs(a);
        new object[] { b, d }.Should().AllBeSameAs(d);

        var func1 = () => new object[] { a, b }.Should().AllBeSameAs(a);
        func1.Should().ThrowExactly<AssertionException>().WithMessage($"The following is different than provided: b");

        var func2 = () => new object[] { b, b, b, }.Should().AllBeSameAs(a);
        func2.Should().ThrowExactly<AssertionException>().WithMessage($"All entries are different than provided");

        var func3 = () => new object[] { b, c }.Should().AllBeSameAs(a);
        func3.Should().ThrowExactly<AssertionException>().WithMessage("The following is different than provided: b");

        var func4 = () => new object[] { c, b, b }.Should().AllBeSameAs(a);
        func4.Should().ThrowExactly<AssertionException>().WithMessage($"The following are different than provided: {NewLine}\tb{NewLine}\tb");
    }

    [Test]
    public void Test_AllNotBeSameAs()
    {
        var a = new object();
        var c = a;
        var b = new object();
        var d = b;
        new object[] { a, c }.Should().AllNotBeSameAs(b);
        new object[] { b, d }.Should().AllNotBeSameAs(a);

        var func1 = () => new object[] { a, b }.Should().AllNotBeSameAs(a);
        func1.Should().ThrowExactly<AssertionException>().WithMessage($"The following is the same as provided: a");

        var func2 = () => new object[] { b, b, b, }.Should().AllNotBeSameAs(b);
        func2.Should().ThrowExactly<AssertionException>().WithMessage($"All entries are the same as provided");

        var func3 = () => new object[] { b, c }.Should().AllNotBeSameAs(b);
        func3.Should().ThrowExactly<AssertionException>().WithMessage("The following is the same as provided: b");

        var func4 = () => new object[] { c, b, b }.Should().AllNotBeSameAs(b);
        func4.Should().ThrowExactly<AssertionException>().WithMessage($"The following are the same as provided: {NewLine}\tb{NewLine}\tb");
    }

    [Test]
    public void Test_AllNonNull()
    {
        Base b = new();
        Base1 b1 = new();
        Base2 b2 = new();
        Sub s = new();
        Sub1 s1 = new();
        Sub2 s2 = new();
        new object[] { b1, b2, s1, s2 }.Should().AllBeNonNull();

        var func1 = () => new object?[] { (object?)null, s }.Should().AllBeNonNull();
        func1.Should().ThrowExactly<AssertionException>().WithMessage($"The following is null: (object?)null");

        var func2 = () => new object?[] { null, null, null }.Should().AllBeNonNull();
        func2.Should().ThrowExactly<AssertionException>().WithMessage($"All entries are null");

        var func3 = () => new object?[] { b, null }.Should().AllBeNonNull();
        func3.Should().ThrowExactly<AssertionException>().WithMessage("The following is null: null");

        var func4 = () => new object?[] { b, null, (object?)null }.Should().AllBeNonNull();
        func4.Should().ThrowExactly<AssertionException>().WithMessage($"The following are null: {NewLine}\tnull{NewLine}\t(object?)null");
    }

    [Test]
    public void Test_AllNull()
    {
        Base b = new();
        Base1 b1 = new();
        Base2 b2 = new();
        Sub s = new();
        Sub1 s1 = new();
        Sub2 s2 = new();
        new object?[] { null, null, null }.Should().AllBeNull();

        var func1 = () => new object?[] { null, s }.Should().AllBeNull();
        func1.Should().ThrowExactly<AssertionException>().WithMessage($"The following is not null: s");

        var func2 = () => new object?[] { b, b, b }.Should().AllBeNull();
        func2.Should().ThrowExactly<AssertionException>().WithMessage($"All entries are not null");

        var func3 = () => new object?[] { b, null }.Should().AllBeNull();
        func3.Should().ThrowExactly<AssertionException>().WithMessage("The following is not null: b");

        var func4 = () => new object?[] { b, s, null }.Should().AllBeNull();
        func4.Should().ThrowExactly<AssertionException>().WithMessage($"The following are not null: {NewLine}\tb{NewLine}\ts");
    }

    [Test]
    public void Test_AllBeTrue()
    {
        bool a = true;
        new bool[] { true, true, a }.Should().AllBeTrue();

        bool b = false;
        var func1 = () => new bool[] { true, b }.Should().AllBeTrue();
        func1.Should().ThrowExactly<AssertionException>().WithMessage($"The following is false: b");

        var func2 = () => new bool[] { false, false, b, }.Should().AllBeTrue();
        func2.Should().ThrowExactly<AssertionException>().WithMessage($"All entries are false");

        bool c = false;
        var func3 = () => new bool[] { b, true, c }.Should().AllBeTrue();
        func3.Should().ThrowExactly<AssertionException>().WithMessage($"The following are false: {NewLine}\tb{NewLine}\tc");
    }

    [Test]
    public void Test_AllBeFalse()
    {
        bool a = false;
        new bool[] { false, false, a }.Should().AllBeFalse();

        bool b = true;
        var func1 = () => new bool[] { false, b }.Should().AllBeFalse();
        func1.Should().ThrowExactly<AssertionException>().WithMessage($"The following is true: b");

        var func2 = () => new bool[] { true, true, b, }.Should().AllBeFalse();
        func2.Should().ThrowExactly<AssertionException>().WithMessage($"All entries are true");

        bool c = true;
        var func3 = () => new bool[] { b, false, c }.Should().AllBeFalse();
        func3.Should().ThrowExactly<AssertionException>().WithMessage($"The following are true: {NewLine}\tb{NewLine}\tc");
    }
}
