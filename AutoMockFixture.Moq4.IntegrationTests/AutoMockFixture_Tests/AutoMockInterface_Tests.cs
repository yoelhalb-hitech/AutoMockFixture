
namespace AutoMockFixture.Tests.AutoMockFixture_Tests;

file interface ITest
{
    string? TestReadOnly { get; }
    string? TestReadWrite { get; set; }
    string? TestMethod();
}
file class TestNoExplicit : ITest
{
    public virtual string? TestReadOnly { get; }
    public virtual string? TestReadWrite { get; set; }
    public virtual string? TestMethod() => "";
}
file class SubNoExplicit : TestNoExplicit { }

file class TestOnlyExplicit : ITest
{
    string? ITest.TestReadOnly => "Works";
    string? ITest.TestReadWrite { get => "Works"; set => _ = value; }
    string? ITest.TestMethod() => "Works";
}
file class SubOnlyExplicit : TestOnlyExplicit { }


file class TestBoth : ITest
{
    public virtual string? TestReadOnly => "Works1";
    public virtual string? TestReadWrite { get => "Works1"; set => _ = value; }
    public virtual string? TestMethod() => "Works1";
    string? ITest.TestReadOnly => "Works2";
    string? ITest.TestReadWrite { get => "Works2"; set => _ = value; }
    string? ITest.TestMethod() => "Works2";
}
file class SubBoth : TestBoth { }

file interface ITestWithDefault : ITest
{
    string? ITest.TestReadOnly => "DefaultTest";
    string? ITest.TestReadWrite { get => "DefaultTest"; set => _ = value; }
    string? ITest.TestMethod() => "DefaultTestMethod";
}

file class TestWithDefault : ITestWithDefault { }

internal class AutoMockInterface_Tests
{
    [Test]
    public void Test_ITestWithDefault_NonCallbase()
    {
        var fixture = new AbstractAutoMockFixture();
        var obj = fixture.CreateAutoMock<ITestWithDefault>();

        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<ITest>();

        (obj as ITest)!.TestReadOnly.Should().NotBeNullOrWhiteSpace(); // Should be set by the fixture
        (obj as ITest)!.TestReadOnly.Should().NotBe("DefaultTest");
        (obj as ITest)!.TestReadWrite.Should().NotBeNullOrWhiteSpace(); // Should be set by the fixture
        (obj as ITest)!.TestReadWrite.Should().NotBe("DefaultTest");
        (obj as ITest).TestMethod().Should().NotBeNullOrWhiteSpace();  // Should be set by the fixture
        (obj as ITest).TestMethod().Should().NotBe("DefaultTestMethod");
    }

    [Test]
    public void Test_ITestWithDefault_WithCallbase()
    {
        var fixture = new AbstractAutoMockFixture();
        var obj = fixture.CreateAutoMock<ITestWithDefault>(callBase: true);

        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<ITest>();

        (obj as ITestWithDefault)!.TestReadOnly.Should().Be("DefaultTest");
        (obj as ITestWithDefault)!.TestReadWrite.Should().Be("DefaultTest");
        (obj as ITestWithDefault).TestMethod().Should().Be("DefaultTestMethod");
    }

    [Test]
    public void Test_ITestWithDefault_WithCallbase_AsCall()
    {
        var fixture = new AbstractAutoMockFixture();
        var obj = fixture.CreateAutoMock<ITestWithDefault>(callBase: false);

        obj.Should().NotBeNull();
        AutoMock.Get(obj!).Should().NotBeNull();
        AutoMock.Get(obj)!.Setup(m => m.TestReadOnly).CallBase();
        AutoMock.Get(obj)!.Setup(m => m.TestReadWrite).CallBase();
        AutoMock.Get(obj)!.Setup(m => m.TestMethod()).CallBase();

        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<ITest>();

        (obj as ITestWithDefault)!.TestReadOnly.Should().Be("DefaultTest");
        (obj as ITestWithDefault)!.TestReadWrite.Should().Be("DefaultTest");
        (obj as ITestWithDefault).TestMethod().Should().Be("DefaultTestMethod");
    }

    [Test]
    public void Test_ITestWithDefault_WithCallbase_AsInterface_AsCall()
    {
        var fixture = new AbstractAutoMockFixture();
        var obj = fixture.CreateAutoMock<ITestWithDefault>(callBase: false);

        obj.Should().NotBeNull();
        AutoMock.Get(obj!).Should().NotBeNull();
        AutoMock.Get(obj)!.As<ITest>().Setup(m => m.TestReadOnly).CallBase();
        AutoMock.Get(obj)!.As<ITest>().Setup(m => m.TestReadWrite).CallBase();
        AutoMock.Get(obj)!.As<ITest>().Setup(m => m.TestMethod()).CallBase();

        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<ITest>();

        (obj as ITestWithDefault)!.TestReadOnly.Should().Be("DefaultTest");
        (obj as ITestWithDefault)!.TestReadWrite.Should().Be("DefaultTest");
        (obj as ITestWithDefault).TestMethod().Should().Be("DefaultTestMethod");
    }

    [Test]
    public void Test_TestWithDefault_NonCallbase()
    {
        var fixture = new AbstractAutoMockFixture();
        var obj = fixture.CreateAutoMock<TestWithDefault>();

        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<ITest>();

        (obj as ITest)!.TestReadOnly.Should().NotBeNullOrWhiteSpace(); // Should be set by the fixture
        (obj as ITest)!.TestReadOnly.Should().NotBe("DefaultTest");
        (obj as ITest)!.TestReadWrite.Should().NotBeNullOrWhiteSpace(); // Should be set by the fixture
        (obj as ITest)!.TestReadWrite.Should().NotBe("DefaultTest");
        (obj as ITest).TestMethod().Should().NotBeNullOrWhiteSpace(); // Should be set by the fixture
        (obj as ITest).TestMethod().Should().NotBe("DefaultTestMethod");
    }

    [Test]
    public void Test_TestWithDefault_WithCallbase()
    {
        var fixture = new AbstractAutoMockFixture();

        var obj = fixture.CreateAutoMock<TestWithDefault>(callBase: true) ;

        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<ITest>();

        (obj as ITest)!.TestReadOnly.Should().Be("DefaultTest");
        (obj as ITest)!.TestReadWrite.Should().Be("DefaultTest");
        (obj as ITest).TestMethod().Should().Be("DefaultTestMethod");
    }

    [Test]
    public void Test_ITest()
    {
        var fixture = new AbstractAutoMockFixture();

        var obj = fixture.CreateAutoMock<ITest>();

        obj.Should().NotBeNull();
        obj!.TestReadOnly.Should().NotBeNull();  // Should be set by the fixture
        obj!.TestReadWrite.Should().NotBeNull();  // Should be set by the fixture
        (obj as ITest).TestReadOnly.Should().NotBeNullOrWhiteSpace();  // Should be set by the fixture
        (obj as ITest).TestReadWrite.Should().NotBeNullOrWhiteSpace();  // Should be set by the fixture
        (obj as ITest).TestMethod().Should().NotBeNullOrWhiteSpace();  // Should be set by the fixture
    }

    [Test]
    public void Test_ITest_WithCallbase()
    {
        var fixture = new AbstractAutoMockFixture();

        var obj = fixture.CreateAutoMock<ITest>(callBase: true);

        obj.Should().NotBeNull();
        obj!.TestReadOnly.Should().NotBeNull();
        obj!.TestReadWrite.Should().NotBeNull();
        (obj as ITest).TestReadOnly.Should().NotBeNullOrWhiteSpace();  // Should be set by the fixture
        (obj as ITest).TestReadWrite.Should().NotBeNullOrWhiteSpace();  // Should be set by the fixture
        (obj as ITest).TestMethod().Should().NotBeNullOrWhiteSpace();  // Should be set by the fixture
    }

    [Test]
    public void Test_TestNoExplicit()
    {
        var fixture = new AbstractAutoMockFixture();
        var obj = fixture.CreateAutoMock<TestNoExplicit>();

        obj.Should().NotBeNull();
        obj!.TestReadOnly.Should().NotBeNull();
        obj!.TestReadWrite.Should().NotBeNull();
        (obj as ITest).TestReadOnly.Should().NotBeNullOrWhiteSpace();
        (obj as ITest).TestReadWrite.Should().NotBeNullOrWhiteSpace();
        (obj as ITest).TestMethod().Should().NotBeNullOrWhiteSpace();
    }

    [Test]
    public void Test_SubNoExplicit()
    {
        var fixture = new AbstractAutoMockFixture();

        var obj = fixture.CreateAutoMock<SubNoExplicit>();

        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<ITest>();

        obj!.TestReadOnly.Should().NotBeNull();
        obj!.TestReadWrite.Should().NotBeNull();
        obj!.TestMethod().Should().NotBeNull();
        (obj as ITest).TestReadOnly.Should().NotBeNullOrWhiteSpace();
        (obj as ITest).TestReadWrite.Should().NotBeNullOrWhiteSpace();
        (obj as ITest).TestMethod().Should().NotBeNullOrWhiteSpace();
    }

    [Test]
    public void Test_TestOnlyExplicit_Callbase()
    {
        var fixture = new AbstractAutoMockFixture();
        var obj = fixture.CreateAutoMock<TestOnlyExplicit>(callBase: true);

        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<ITest>();
        (obj as ITest)!.TestReadOnly.Should().Be("Works");
        (obj as ITest)!.TestReadWrite.Should().Be("Works");
        (obj as ITest).TestMethod().Should().Be("Works");
    }

    [Test]
    public void Test_TestOnlyExplicit_NonCallbase()
    {
        var fixture = new AbstractAutoMockFixture();
        var obj = fixture.CreateAutoMock<TestOnlyExplicit>(callBase: false);

        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<ITest>();
        (obj as ITest)!.TestReadOnly.Should().NotBeNullOrEmpty(); // Should be set by the fixture
        (obj as ITest)!.TestReadWrite.Should().NotBeNullOrEmpty(); // Should be set by the fixture
        (obj as ITest).TestMethod().Should().NotBeNullOrEmpty(); // Should be set by the fixture

        (obj as ITest)!.TestReadOnly.Should().NotBe("Works");
        (obj as ITest)!.TestReadWrite.Should().NotBe("Works");
        (obj as ITest).TestMethod().Should().NotBe("Works");
    }

    [Test]
    public void Test_SubOnlyExplicit_Callbase()
    {
        var fixture = new AbstractAutoMockFixture();
        var obj = fixture.CreateAutoMock<SubOnlyExplicit>(callBase: true);

        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<ITest>();
        (obj as ITest)!.TestReadOnly.Should().Be("Works");
        (obj as ITest)!.TestReadWrite.Should().Be("Works");
        (obj as ITest).TestMethod().Should().Be("Works");
    }

    [Test]
    public void Test_SubOnlyExplicit_NonCallbase()
    {
        var fixture = new AbstractAutoMockFixture();
        var obj = fixture.CreateAutoMock<SubOnlyExplicit>(callBase: false);

        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<ITest>();
        (obj as ITest)!.TestReadOnly.Should().NotBeNullOrEmpty();  // Should be set by the fixture
        (obj as ITest)!.TestReadOnly.Should().NotBe("Works1");
        (obj as ITest)!.TestReadOnly.Should().NotBe("Works2");
        (obj as ITest)!.TestReadWrite.Should().NotBeNullOrEmpty();  // Should be set by the fixture
        (obj as ITest)!.TestReadWrite.Should().NotBe("Works1");
        (obj as ITest)!.TestReadWrite.Should().NotBe("Works2");
        (obj as ITest).TestMethod().Should().NotBeNullOrEmpty();  // Should be set by the fixture
        (obj as ITest)!.TestMethod().Should().NotBe("Works1");
        (obj as ITest)!.TestMethod().Should().NotBe("Works2");
    }

    [Test]
    public void Test_TestBoth_Callbase()
    {
        var fixture = new AbstractAutoMockFixture();

        var obj = fixture.CreateAutoMock<TestBoth>(callBase: true);

        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<ITest>();

        obj!.TestReadOnly.Should().Be("Works1");
        obj!.TestReadWrite.Should().Be("Works1");
        obj!.TestMethod().Should().Be("Works1");
        (obj as ITest)!.TestReadOnly.Should().Be("Works2");
        (obj as ITest)!.TestReadWrite.Should().Be("Works2");
        (obj as ITest).TestMethod().Should().Be("Works2");
    }

    [Test]
    public void Test_TestBoth_NonCallbase()
    {
        var fixture = new AbstractAutoMockFixture();
        var obj = fixture.CreateAutoMock<TestBoth>(callBase: false);

        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<ITest>();

        obj!.TestReadOnly.Should().NotBe("Works1");
        obj!.TestReadWrite.Should().NotBe("Works1");
        obj!.TestMethod().Should().NotBe("Works1");
        (obj as ITest)!.TestReadOnly.Should().NotBe("Works2");
        (obj as ITest)!.TestReadWrite.Should().NotBe("Works2");
        (obj as ITest).TestMethod().Should().NotBe("Works2");

        obj!.TestReadOnly.Should().NotBeNullOrWhiteSpace();  // Should be set by the fixture
        obj!.TestReadWrite.Should().NotBeNullOrWhiteSpace();  // Should be set by the fixture
        obj.TestMethod().Should().NotBeNullOrWhiteSpace();  // Should be set by the fixture
        (obj as ITest)!.TestReadOnly.Should().NotBeNullOrWhiteSpace();  // Should be set by the fixture
        (obj as ITest)!.TestReadWrite.Should().NotBeNullOrWhiteSpace();  // Should be set by the fixture
        (obj as ITest).TestMethod().Should().NotBeNullOrWhiteSpace();  // Should be set by the fixture
    }

    [Test]
    public void Test_SubBoth_Callbase()
    {
        var fixture = new AbstractAutoMockFixture();

        var obj = fixture.CreateAutoMock<SubBoth>(callBase: true);

        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<ITest>();

        obj!.TestReadOnly.Should().Be("Works1");
        obj!.TestReadWrite.Should().Be("Works1");
        obj!.TestMethod().Should().Be("Works1");
        (obj as ITest)!.TestReadOnly.Should().Be("Works2");
        (obj as ITest)!.TestReadWrite.Should().Be("Works2");
        (obj as ITest).TestMethod().Should().Be("Works2");
    }

    [Test]
    public void Test_SubBoth_NonCallbase()
    {
        var fixture = new AbstractAutoMockFixture();
        var obj = fixture.CreateAutoMock<SubBoth>(callBase: false);

        obj.Should().NotBeNull();
        obj.Should().BeAssignableTo<ITest>();

        obj!.TestReadOnly.Should().NotBe("Works1");
        obj!.TestReadWrite.Should().NotBe("Works1");
        obj!.TestMethod().Should().NotBe("Works1");
        (obj as ITest)!.TestReadOnly.Should().NotBe("Works2");
        (obj as ITest)!.TestReadWrite.Should().NotBe("Works2");
        (obj as ITest).TestMethod().Should().NotBe("Works2");

        obj!.TestReadOnly.Should().NotBeNullOrWhiteSpace();  // Should be set by the fixture
        obj!.TestReadWrite.Should().NotBeNullOrWhiteSpace();  // Should be set by the fixture
        obj.TestMethod().Should().NotBeNullOrWhiteSpace();  // Should be set by the fixture
        (obj as ITest)!.TestReadOnly.Should().NotBeNullOrWhiteSpace();  // Should be set by the fixture
        (obj as ITest)!.TestReadWrite.Should().NotBeNullOrWhiteSpace();  // Should be set by the fixture
        (obj as ITest).TestMethod().Should().NotBeNullOrWhiteSpace();  // Should be set by the fixture
    }
}
