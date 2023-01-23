using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.Test.AutoMockFixture_Tests;

file interface ITest
{
    string? Test { get; }
    string? TestMethod();
}
file class TestNoExplicit : ITest
{
    public virtual string? Test { get; }
    public virtual string? TestMethod() => "";
}
file class SubNoExplicit : TestNoExplicit { }

file class TestOnlyExplicit : ITest
{
    string? ITest.Test { get; }
    string? ITest.TestMethod() => "";
}
file class SubOnlyExplicit : TestOnlyExplicit { }


file class TestBoth : ITest
{
    public virtual string? Test { get; }
    public virtual string? TestMethod() => "";
    string? ITest.Test { get; }
    string? ITest.TestMethod() => "";
}
file class SubBoth : TestBoth { }


internal class AutoMockInterface_Tests
{
    //  TODO... We need to handle default interface implementations when Callbase is false
    //  The issue is that Moq currently doesn't have  way to do it
    //
    //  
    //  To handle the default interface issue when it wasn't implemented at all we probably need:
    //  1) Get the method and interface name by using the GetInterface or GetInterfaceMap
    //  2) Add the method to explicit implementation
    //      A) (either by faking the type object as we do with the ctor for non callbase)
    //      B) or by emitting a subclass
    //      C) even possible that we can add the interface directly to the new mock either via Moq or via Castle
    // Note also that for default interface implementation we can have a private setter either in the interface OR in the implementation
    //      We need to test both situations
    // We also need to make sure that the SetupProperty actually works and that it works on both callbase and non callbase correctly
    // We may decide to temporary not support it, but we need to throw an error then
    // TODO... finish adding tests
    [Test]
    public void Test_TestNoExplicit()
    {
        var fixture = new AbstractAutoMockFixture();

        var obj = fixture.CreateAutoMock<TestNoExplicit>();

        obj.Should().NotBeNull();
        obj!.Test.Should().NotBeNull();
        (obj as ITest).Test.Should().NotBeNullOrWhiteSpace();
        (obj as ITest).TestMethod().Should().NotBeNullOrWhiteSpace();
    }

    [Test]
    public void Test_SubNoExplicit()
    {
        var fixture = new AbstractAutoMockFixture();

        var obj = fixture.CreateAutoMock<SubNoExplicit>();

        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<ITest>();

        obj!.Test.Should().NotBeNull();
        obj!.TestMethod().Should().NotBeNull();
        (obj as ITest).Test.Should().NotBeNullOrWhiteSpace();
        (obj as ITest).TestMethod().Should().NotBeNullOrWhiteSpace();
    }

    [Test]
    public void Test_TestOnlyExplicit()
    {
        var fixture = new AbstractAutoMockFixture();

        var obj = fixture.CreateAutoMock<TestOnlyExplicit>();

        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<ITest>();
        (obj as ITest)!.Test.Should().NotBeNullOrWhiteSpace();
        (obj as ITest).TestMethod().Should().NotBeNullOrWhiteSpace();
    }

    [Test]
    public void Test_SubOnlyExplicit()
    {
        var fixture = new AbstractAutoMockFixture();

        var obj = fixture.CreateAutoMock<SubOnlyExplicit>();

        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<ITest>();

        (obj as ITest)!.Test.Should().NotBeNullOrWhiteSpace();
        (obj as ITest).TestMethod().Should().NotBeNullOrWhiteSpace();
    }

    [Test]
    public void Test_TestBoth()
    {
        var fixture = new AbstractAutoMockFixture();

        var obj = fixture.CreateAutoMock<TestBoth>();

        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<ITest>();

        obj!.Test.Should().NotBeNull();
        obj.TestMethod().Should().NotBeNullOrWhiteSpace();
        (obj as ITest)!.Test.Should().NotBeNullOrWhiteSpace();
        (obj as ITest).TestMethod().Should().NotBeNullOrWhiteSpace();
    }

    [Test]
    public void Test_SubBoth()
    {
        var fixture = new AbstractAutoMockFixture();

        var obj = fixture.CreateAutoMock<SubBoth>();

        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<ITest>();

        obj!.Test.Should().NotBeNull();
        obj.TestMethod().Should().NotBeNullOrWhiteSpace();
        (obj as ITest)!.Test.Should().NotBeNullOrWhiteSpace();
        (obj as ITest).TestMethod().Should().NotBeNullOrWhiteSpace();
    }
}
