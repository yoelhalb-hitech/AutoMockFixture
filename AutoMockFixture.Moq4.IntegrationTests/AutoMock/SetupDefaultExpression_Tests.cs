using Moq;
using System.Linq.Expressions;

namespace AutoMockFixture.Moq4.IntegrationTests.AutoMock;

internal class SetupDefaultExpression_Tests
{
    public class Test
    {
        public virtual string TestString(string? str) => "";
        public virtual string TestNullableInt(int? i) => "";
        public virtual string TestInt(int i) => "";
        public virtual string TestNullableBool(bool? b) => "";
        public virtual string TestBool(bool b) => "";
        public virtual string TestNullableDateTime(DateTime? d) => "";
        public virtual string TestDateTime(DateTime d) => "";
    }

    [Test]
    [TestCaseSource(nameof(SetupStringActions))]
    public void Test_SetupDefaultExpression_WhenString_IsLikeIsAny(Action<AutoMock<Test>> action)
    {
        var autoMock = new AutoMock<Test>();
        action(autoMock);

        var obj = autoMock.Object;
        obj.TestString("TestArg").Should().Be("Works");
        obj.TestString(null).Should().Be("Works");
        obj.TestString(default).Should().Be("Works");
        obj.TestString("").Should().Be("Works");

        autoMock.Setup(x => x.TestString(ItIs.DefaultValue<string>())).Returns("ItIs.DefaultValue() Works");

        obj.TestString("TestArg").Should().Be("Works");
        obj.TestString(null).Should().Be("ItIs.DefaultValue() Works");
        obj.TestString(default).Should().Be("ItIs.DefaultValue() Works");
        obj.TestString("").Should().Be("Works");
    }

    [Test]
    [TestCaseSource(nameof(SetupIntActions))]
    public void Test_SetupDefaultExpression_WhenInt_IsLikeIsAny(Action<AutoMock<Test>> action)
    {
        var autoMock = new AutoMock<Test>();
        action(autoMock);

        var obj = autoMock.Object;
        obj.TestInt(20).Should().Be("Works");
        obj.TestInt(default).Should().Be("Works");
        obj.TestInt(0).Should().Be("Works");

        autoMock.Setup(x => x.TestInt(ItIs.DefaultValue<int>())).Returns("ItIs.DefaultValue() Works");
        obj.TestInt(20).Should().Be("Works");
        obj.TestInt(default).Should().Be("ItIs.DefaultValue() Works");
        obj.TestInt(0).Should().Be("ItIs.DefaultValue() Works");
    }

    [Test]
    [TestCaseSource(nameof(SetupNullableIntActions))]
    public void Test_SetupDefaultExpression_WhenNullableInt_IsLikeIsAny(Action<AutoMock<Test>> action)
    {
        var autoMock = new AutoMock<Test>();
        action(autoMock);

        var obj = autoMock.Object;
        obj.TestNullableInt(20).Should().Be("Works");
        obj.TestNullableInt(null).Should().Be("Works");
        obj.TestNullableInt(default).Should().Be("Works");
        obj.TestNullableInt(default(int?)).Should().Be("Works");
        obj.TestNullableInt(default(int)).Should().Be("Works");
        obj.TestNullableInt(0).Should().Be("Works");

        autoMock.Setup(x => x.TestNullableInt(ItIs.DefaultValue<int?>())).Returns("ItIs.DefaultValue<int?>() Works");
        obj.TestNullableInt(20).Should().Be("Works");
        obj.TestNullableInt(null).Should().Be("ItIs.DefaultValue<int?>() Works");
        obj.TestNullableInt(default).Should().Be("ItIs.DefaultValue<int?>() Works");
        obj.TestNullableInt(default(int?)).Should().Be("ItIs.DefaultValue<int?>() Works");
        obj.TestNullableInt(default(int)).Should().Be("Works");
        obj.TestNullableInt(0).Should().Be("Works");

        autoMock.Setup(x => x.TestNullableInt(ItIs.DefaultValue<int>())).Returns("ItIs.DefaultValue<int>() Works");
        obj.TestNullableInt(default(int)).Should().Be("ItIs.DefaultValue<int>() Works");
        obj.TestNullableInt(0).Should().Be("ItIs.DefaultValue<int>() Works");
    }

    [Test]
    [TestCaseSource(nameof(SetupBoolActions))]
    public void Test_SetupDefaultExpression_WhenBool_IsLikeIsAny(Action<AutoMock<Test>> action)
    {
        var autoMock = new AutoMock<Test>();
        action(autoMock);

        var obj = autoMock.Object;
        obj.TestBool(true).Should().Be("Works");
        obj.TestBool(false).Should().Be("Works");
        obj.TestBool(default).Should().Be("Works");

        autoMock.Setup(a => a.TestBool(ItIs.False()), "ItIs.False() Works");
        obj.TestBool(true).Should().Be("Works");
        obj.TestBool(false).Should().Be("ItIs.False() Works");
        obj.TestBool(default).Should().Be("ItIs.False() Works");

        autoMock.Setup(a => a.TestBool(ItIs.True()), "ItIs.True() Works");
        obj.TestBool(true).Should().Be("ItIs.True() Works");
        obj.TestBool(false).Should().Be("ItIs.False() Works");
        obj.TestBool(default).Should().Be("ItIs.False() Works");
    }

    [Test]
    [TestCaseSource(nameof(SetupNullableBoolActions))]
    public void Test_SetupDefaultExpression_WhenNullableBool_IsLikeIsAny(Action<AutoMock<Test>> action)
    {
        var autoMock = new AutoMock<Test>();
        action(autoMock);

        var obj = autoMock.Object;
        obj.TestNullableBool(true).Should().Be("Works");
        obj.TestNullableBool(false).Should().Be("Works");
        obj.TestNullableBool(null).Should().Be("Works");
        obj.TestNullableBool(default).Should().Be("Works");
        obj.TestNullableBool(default(bool)).Should().Be("Works");
        obj.TestNullableBool(default(bool?)).Should().Be("Works");

        autoMock.Setup(a => a.TestNullableBool(ItIs.False()), "ItIs.False() Works");
        obj.TestNullableBool(true).Should().Be("Works");
        obj.TestNullableBool(false).Should().Be("ItIs.False() Works");
        obj.TestNullableBool(default).Should().Be("Works");
        obj.TestNullableBool(null).Should().Be("Works");

        autoMock.Setup(a => a.TestNullableBool(ItIs.True()), "ItIs.True() Works");
        obj.TestNullableBool(true).Should().Be("ItIs.True() Works");
        obj.TestNullableBool(false).Should().Be("ItIs.False() Works");
        obj.TestNullableBool(default).Should().Be("Works");
        obj.TestNullableBool(null).Should().Be("Works");

        autoMock.Setup(a => a.TestNullableBool(ItIs.DefaultValue<bool?>()), "ItIs.DefaultValue() Works");
        obj.TestNullableBool(true).Should().Be("ItIs.True() Works");
        obj.TestNullableBool(false).Should().Be("ItIs.False() Works");
        obj.TestNullableBool(default).Should().Be("ItIs.DefaultValue() Works");
        obj.TestNullableBool(null).Should().Be("ItIs.DefaultValue() Works");
    }

    [Test]
    [TestCaseSource(nameof(SetupDateTimeActions))]
    public void Test_SetupDefaultExpression_WhenDateTime_IsLikeIsAny(Action<AutoMock<Test>> action)
    {
        var autoMock = new AutoMock<Test>();
        action(autoMock);

        var obj = autoMock.Object;
        obj.TestDateTime(DateTime.Now).Should().Be("Works");
        obj.TestDateTime(DateTime.Today).Should().Be("Works");
        obj.TestDateTime(DateTime.MinValue).Should().Be("Works");
        obj.TestDateTime(DateTime.MaxValue).Should().Be("Works");
        obj.TestDateTime(default).Should().Be("Works");
    }

    [Test]
    [TestCaseSource(nameof(SetupNullableDateTimeActions))]
    public void Test_SetupDefaultExpression_WhenNullableDateTime_IsLikeIsAny(Action<AutoMock<Test>> action)
    {
        var autoMock = new AutoMock<Test>();
        action(autoMock);

        var obj = autoMock.Object;
        obj.TestNullableDateTime(DateTime.Now).Should().Be("Works");
        obj.TestNullableDateTime(DateTime.Today).Should().Be("Works");
        obj.TestNullableDateTime(DateTime.MinValue).Should().Be("Works");
        obj.TestNullableDateTime(DateTime.MaxValue).Should().Be("Works");
        obj.TestNullableDateTime(null).Should().Be("Works");
        obj.TestNullableDateTime(null).Should().Be("Works");
        obj.TestNullableDateTime(default(DateTime)).Should().Be("Works");
        obj.TestNullableDateTime(default(DateTime?)).Should().Be("Works");
    }

    public static Action<AutoMock<Test>>[] SetupStringActions =
{
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => a.TestString(default), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => a.TestString(default)).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => a.TestString(default(string)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => a.TestString(default(string))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => a.TestString(default)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => a.TestString(default))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => a.TestString(default(string))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => a.TestString(default(string)))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Expression<Func<Test,string>>)(a => a.TestString(default)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Expression<Func<Test,string>>)(a => a.TestString(default))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Expression<Func<Test,string>>)(a => a.TestString(default(string))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Expression<Func<Test, string>>)(a => a.TestString(default(string)))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test, string>>)(a => a.TestString(default))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test, string>>)(a => a.TestString(default)))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test, string>>)(a => a.TestString(default(string)))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test, string>>)(a => a.TestString(default(string))))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (a.TestString(default)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (a.TestString(default))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (a.TestString(default(string))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (a.TestString(default(string)))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (a.TestString(default))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (a.TestString(default)))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (a.TestString(default(string)))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (a.TestString(default(string))))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => a.TestString(default), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => a.TestString(default)).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => a.TestString(default(string)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => a.TestString(default(string))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => a.TestString(default)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => a.TestString(default))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => a.TestString(default(string))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => a.TestString(default(string)))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (a.TestString(default)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (a.TestString(default))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (a.TestString(default(string))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (a.TestString(default(string)))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (a.TestString(default))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (a.TestString(default)))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (a.TestString(default(string)))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (a.TestString(default(string))))).Returns("Works")),

        // Moq has an issue setting up with a convert method
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (string)a.TestString(default), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (string)a.TestString(default)).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (string)a.TestString(default(string)), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (string)a.TestString(default(string))).Returns("Works")),

        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (string)a.TestString(default)), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (string)a.TestString(default))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (string)a.TestString(default(string))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (string)a.TestString(default(string)))).Returns("Works")),

        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (string)(a.TestString(default)), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (string)(a.TestString(default))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (string)(a.TestString(default(string))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (string)(a.TestString(default(string)))).Returns("Works")),

        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => ((string)a.TestString(default)), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => ((string)a.TestString(default))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => ((string)a.TestString(default(string))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => ((string)a.TestString(default(string)))).Returns("Works")),

        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => ((string)(a.TestString(default))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => ((string)(a.TestString(default)))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => ((string)(a.TestString(default(string)))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => ((string)(a.TestString(default(string))))).Returns("Works")),

        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (string)(a.TestString(default))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (string)(a.TestString(default)))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (string)(a.TestString(default(string)))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (string)(a.TestString(default(string))))).Returns("Works")),

        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => ((string)a.TestString(default))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => ((string)a.TestString(default)))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => ((string)a.TestString(default(string)))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => ((string)a.TestString(default(string))))).Returns("Works")),

        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => ((string)(a.TestString(default)))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => ((string)(a.TestString(default))))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => ((string)(a.TestString(default(string))))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => ((string)(a.TestString(default(string)))))).Returns("Works")),

        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (string)a.TestString(default), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (string)a.TestString(default)).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (string)a.TestString(default(string)), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (string)a.TestString(default(string))).Returns("Works")),

        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (string)a.TestString(default)), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (string)a.TestString(default))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (string)a.TestString(default(string))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (string)a.TestString(default(string)))).Returns("Works")),

        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (string)(a.TestString(default)), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (string)(a.TestString(default))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (string)(a.TestString(default(string))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (string)(a.TestString(default(string)))).Returns("Works")),

        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => ((string)a.TestString(default)), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => ((string)a.TestString(default))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => ((string)a.TestString(default(string))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => ((string)a.TestString(default(string)))).Returns("Works")),

        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => ((string)(a.TestString(default))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => ((string)(a.TestString(default)))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => ((string)(a.TestString(default(string)))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => ((string)(a.TestString(default(string))))).Returns("Works")),

        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (string)(a.TestString(default))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (string)(a.TestString(default)))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (string)(a.TestString(default(string)))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (string)(a.TestString(default(string))))).Returns("Works")),

        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => ((string)a.TestString(default))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => ((string)a.TestString(default)))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => ((string)a.TestString(default(string)))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => ((string)a.TestString(default(string))))).Returns("Works")),

        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => ((string)(a.TestString(default)))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => ((string)(a.TestString(default))))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => ((string)(a.TestString(default(string))))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => ((string)(a.TestString(default(string)))))).Returns("Works")),

        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)((Test a) => ((string)(a.TestString(default))))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)((Test a) => ((string)(a.TestString(default)))))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)((Test a) => ((string)(a.TestString(default(string)))))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test, string>>)((Test a) => ((string)(a.TestString(default(string))))))).Returns("Works")),
    };

    public static Action<AutoMock<Test>>[] SetupNullableIntActions =
    {
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => a.TestNullableInt(default), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => a.TestNullableInt(default)).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => a.TestNullableInt(default(int)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => a.TestNullableInt(default(int))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => a.TestNullableInt(default(int?)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => a.TestNullableInt(default(int?))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => a.TestNullableInt(default)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => a.TestNullableInt(default))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => a.TestNullableInt(default(int))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => a.TestNullableInt(default(int)))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => a.TestNullableInt(default(int?))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => a.TestNullableInt(default(int?)))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Expression<Func<Test,string>>)(a => a.TestNullableInt(default)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Expression<Func<Test,string>>)(a => a.TestNullableInt(default))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Expression<Func<Test,string>>)(a => a.TestNullableInt(default(int))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Expression<Func<Test,string>>)(a => a.TestNullableInt(default(int)))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Expression<Func<Test,string>>)(a => a.TestNullableInt(default(int?))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Expression<Func<Test,string>>)(a => a.TestNullableInt(default(int?)))).Returns("Works")),


        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)(a => a.TestNullableInt(default))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)(a => a.TestNullableInt(default)))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)(a => a.TestNullableInt(default(int)))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)(a => a.TestNullableInt(default(int))))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)(a => a.TestNullableInt(default(int?)))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)(a => a.TestNullableInt(default(int?))))).Returns("Works")),


        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (a.TestNullableInt(default)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (a.TestNullableInt(default))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (a.TestNullableInt(default(int))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (a.TestNullableInt(default(int)))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (a.TestNullableInt(default(int?))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (a.TestNullableInt(default(int?)))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (a.TestNullableInt(default))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (a.TestNullableInt(default)))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (a.TestNullableInt(default(int)))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (a.TestNullableInt(default(int))))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (a.TestNullableInt(default(int?)))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (a.TestNullableInt(default(int?))))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => a.TestNullableInt(default), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => a.TestNullableInt(default)).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => a.TestNullableInt(default(int)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => a.TestNullableInt(default(int))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => a.TestNullableInt(default(int?)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => a.TestNullableInt(default(int?))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => a.TestNullableInt(default)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => a.TestNullableInt(default))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => a.TestNullableInt(default(int))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => a.TestNullableInt(default(int)))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => a.TestNullableInt(default(int?))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => a.TestNullableInt(default(int?)))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (a.TestNullableInt(default)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (a.TestNullableInt(default))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (a.TestNullableInt(default(int))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (a.TestNullableInt(default(int)))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (a.TestNullableInt(default(int?))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (a.TestNullableInt(default(int?)))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (a.TestNullableInt(default))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (a.TestNullableInt(default)))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (a.TestNullableInt(default(int)))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (a.TestNullableInt(default(int))))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (a.TestNullableInt(default(int?)))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (a.TestNullableInt(default(int?))))).Returns("Works")),

        // Moq has an issue setting up with a convert method
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (string)a.TestNullableInt(default), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (string)a.TestNullableInt(default)).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (string)a.TestNullableInt(default(int)), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (string)a.TestNullableInt(default(int))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (string)a.TestNullableInt(default(int?)), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (string)a.TestNullableInt(default(int?))).Returns("Works")),

        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (string)a.TestNullableInt(default)), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (string)a.TestNullableInt(default))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (string)a.TestNullableInt(default(int))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (string)a.TestNullableInt(default(int)))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (string)a.TestNullableInt(default(int?))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (string)a.TestNullableInt(default(int?)))).Returns("Works")),

        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (string)(a.TestNullableInt(default)), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (string)(a.TestNullableInt(default))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (string)(a.TestNullableInt(default(int))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (string)(a.TestNullableInt(default(int)))).Returns("Works")),

        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => ((string)a.TestNullableInt(default)), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => ((string)a.TestNullableInt(default))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => ((string)a.TestNullableInt(default(int))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => ((string)a.TestNullableInt(default(int)))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => ((string)a.TestNullableInt(default(int?))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => ((string)a.TestNullableInt(default(int?)))).Returns("Works")),

        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => ((string)(a.TestNullableInt(default))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => ((string)(a.TestNullableInt(default)))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => ((string)(a.TestNullableInt(default(int)))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => ((string)(a.TestNullableInt(default(int))))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => ((string)(a.TestNullableInt(default(int?)))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => ((string)(a.TestNullableInt(default(int?))))).Returns("Works")),

        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (string)(a.TestNullableInt(default))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (string)(a.TestNullableInt(default)))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (string)(a.TestNullableInt(default(int)))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (string)(a.TestNullableInt(default(int))))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (string)(a.TestNullableInt(default(int?)))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (string)(a.TestNullableInt(default(int?))))).Returns("Works")),

        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => ((string)a.TestNullableInt(default))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => ((string)a.TestNullableInt(default)))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => ((string)a.TestNullableInt(default(int)))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => ((string)a.TestNullableInt(default(int))))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => ((string)a.TestNullableInt(default(int?)))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => ((string)a.TestNullableInt(default(int?))))).Returns("Works")),

        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => ((string)(a.TestNullableInt(default)))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => ((string)(a.TestNullableInt(default))))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => ((string)(a.TestNullableInt(default(int))))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => ((string)(a.TestNullableInt(default(int)))))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => ((string)(a.TestNullableInt(default(int?))))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => ((string)(a.TestNullableInt(default(int?)))))).Returns("Works")),

        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (string)a.TestNullableInt(default), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (string)a.TestNullableInt(default)).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (string)a.TestNullableInt(default(int)), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (string)a.TestNullableInt(default(int))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (string)a.TestNullableInt(default(int?)), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (string)a.TestNullableInt(default(int?))).Returns("Works")),


        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (string)a.TestNullableInt(default)), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (string)a.TestNullableInt(default))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (string)a.TestNullableInt(default(int))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (string)a.TestNullableInt(default(int)))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (string)a.TestNullableInt(default(int?))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (string)a.TestNullableInt(default(int?)))).Returns("Works")),


        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (string)(a.TestNullableInt(default)), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (string)(a.TestNullableInt(default))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (string)(a.TestNullableInt(default(int))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (string)(a.TestNullableInt(default(int)))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (string)(a.TestNullableInt(default(int?))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (string)(a.TestNullableInt(default(int?)))).Returns("Works")),


        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => ((string)a.TestNullableInt(default)), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => ((string)a.TestNullableInt(default))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => ((string)a.TestNullableInt(default(int))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => ((string)a.TestNullableInt(default(int)))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => ((string)a.TestNullableInt(default(int?))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => ((string)a.TestNullableInt(default(int?)))).Returns("Works")),


        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => ((string)(a.TestNullableInt(default))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => ((string)(a.TestNullableInt(default)))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => ((string)(a.TestNullableInt(default(int)))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => ((string)(a.TestNullableInt(default(int))))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => ((string)(a.TestNullableInt(default(int?)))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => ((string)(a.TestNullableInt(default(int?))))).Returns("Works")),


        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (string)(a.TestNullableInt(default))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (string)(a.TestNullableInt(default)))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (string)(a.TestNullableInt(default(int)))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (string)(a.TestNullableInt(default(int))))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (string)(a.TestNullableInt(default(int?)))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (string)(a.TestNullableInt(default(int?))))).Returns("Works")),


        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => ((string)a.TestNullableInt(default))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => ((string)a.TestNullableInt(default)))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => ((string)a.TestNullableInt(default(int)))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => ((string)a.TestNullableInt(default(int))))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => ((string)a.TestNullableInt(default(int?)))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => ((string)a.TestNullableInt(default(int?))))).Returns("Works")),


        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => ((string)(a.TestNullableInt(default)))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => ((string)(a.TestNullableInt(default))))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => ((string)(a.TestNullableInt(default(int))))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => ((string)(a.TestNullableInt(default(int)))))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => ((string)(a.TestNullableInt(default(int?))))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => ((string)(a.TestNullableInt(default(int?)))))).Returns("Works")),


        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)((Test a) => ((string)(a.TestNullableInt(default))))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)((Test a) => ((string)(a.TestNullableInt(default)))))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)((Test a) => ((string)(a.TestNullableInt(default(int)))))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)((Test a) => ((string)(a.TestInt(default(int))))))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)((Test a) => ((string)(a.TestNullableInt(default(int?)))))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)((Test a) => ((string)(a.TestInt(default(int?))))))).Returns("Works")),
    };

    public static Action<AutoMock<Test>>[] SetupIntActions =
    {
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => a.TestInt(default), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => a.TestInt(default)).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => a.TestInt(default(int)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => a.TestInt(default(int))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => a.TestInt(default)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => a.TestInt(default))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => a.TestInt(default(int))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => a.TestInt(default(int)))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Expression<Func<Test,string>>)(a => a.TestInt(default)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Expression<Func<Test,string>>)(a => a.TestInt(default))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Expression<Func<Test,string>>)(a => a.TestInt(default(int))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Expression<Func<Test,string>>)(a => a.TestInt(default(int)))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)(a => a.TestInt(default))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)(a => a.TestInt(default)))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)(a => a.TestInt(default(int)))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)(a => a.TestInt(default(int))))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (a.TestInt(default)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (a.TestInt(default))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (a.TestInt(default(int))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (a.TestInt(default(int)))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (a.TestInt(default))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (a.TestInt(default)))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (a.TestInt(default(int)))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (a.TestInt(default(int))))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => a.TestInt(default), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => a.TestInt(default)).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => a.TestInt(default(int)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => a.TestInt(default(int))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => a.TestInt(default)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => a.TestInt(default))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => a.TestInt(default(int))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => a.TestInt(default(int)))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (a.TestInt(default)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (a.TestInt(default))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (a.TestInt(default(int))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (a.TestInt(default(int)))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (a.TestInt(default))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (a.TestInt(default)))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (a.TestInt(default(int)))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (a.TestInt(default(int))))).Returns("Works")),

        // Moq has an issue setting up with a convert method
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (string)a.TestInt(default), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (string)a.TestInt(default)).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (string)a.TestInt(default(int)), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (string)a.TestInt(default(int))).Returns("Works")),

        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (string)a.TestInt(default)), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (string)a.TestInt(default))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (string)a.TestInt(default(int))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (string)a.TestInt(default(int)))).Returns("Works")),

        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (string)(a.TestInt(default)), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (string)(a.TestInt(default))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (string)(a.TestInt(default(int))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (string)(a.TestInt(default(int)))).Returns("Works")),

        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => ((string)a.TestInt(default)), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => ((string)a.TestInt(default))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => ((string)a.TestInt(default(int))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => ((string)a.TestInt(default(int)))).Returns("Works")),

        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => ((string)(a.TestInt(default))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => ((string)(a.TestInt(default)))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => ((string)(a.TestInt(default(int)))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => ((string)(a.TestInt(default(int))))).Returns("Works")),

        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (string)(a.TestInt(default))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (string)(a.TestInt(default)))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (string)(a.TestInt(default(int)))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (string)(a.TestInt(default(int))))).Returns("Works")),

        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => ((string)a.TestInt(default))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => ((string)a.TestInt(default)))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => ((string)a.TestInt(default(int)))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => ((string)a.TestInt(default(int))))).Returns("Works")),

        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => ((string)(a.TestInt(default)))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => ((string)(a.TestInt(default))))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => ((string)(a.TestInt(default(int))))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => ((string)(a.TestInt(default(int)))))).Returns("Works")),

        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (string)a.TestInt(default), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (string)a.TestInt(default)).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (string)a.TestInt(default(int)), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (string)a.TestInt(default(int))).Returns("Works")),

        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (string)a.TestInt(default)), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (string)a.TestInt(default))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (string)a.TestInt(default(int))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (string)a.TestInt(default(int)))).Returns("Works")),

        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (string)(a.TestInt(default)), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (string)(a.TestInt(default))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (string)(a.TestInt(default(int))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (string)(a.TestInt(default(int)))).Returns("Works")),

        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => ((string)a.TestInt(default)), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => ((string)a.TestInt(default))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => ((string)a.TestInt(default(int))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => ((string)a.TestInt(default(int)))).Returns("Works")),

        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => ((string)(a.TestInt(default))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => ((string)(a.TestInt(default)))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => ((string)(a.TestInt(default(int)))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => ((string)(a.TestInt(default(int))))).Returns("Works")),

        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (string)(a.TestInt(default))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (string)(a.TestInt(default)))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (string)(a.TestInt(default(int)))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (string)(a.TestInt(default(int))))).Returns("Works")),

        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => ((string)a.TestInt(default))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => ((string)a.TestInt(default)))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => ((string)a.TestInt(default(int)))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => ((string)a.TestInt(default(int))))).Returns("Works")),

        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => ((string)(a.TestInt(default)))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => ((string)(a.TestInt(default))))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => ((string)(a.TestInt(default(int))))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => ((string)(a.TestInt(default(int)))))).Returns("Works")),

        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)((Test a) => ((string)(a.TestInt(default))))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)((Test a) => ((string)(a.TestInt(default)))))).Returns("Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)((Test a) => ((string)(a.TestInt(default(int)))))), "Works")),
        //(Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)((Test a) => ((string)(a.TestInt(default(int))))))).Returns("Works")),
    };

    public static Action<AutoMock<Test>>[] SetupBoolActions =
    {
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => a.TestBool(default), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => a.TestBool(default)).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => a.TestBool(default(bool)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => a.TestBool(default(bool))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => a.TestBool(default)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => a.TestBool(default))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => a.TestBool(default(bool))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => a.TestBool(default(bool)))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Expression<Func<Test,string>>)(a => a.TestBool(default)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Expression<Func<Test,string>>)(a => a.TestBool(default))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Expression<Func<Test,string>>)(a => a.TestBool(default(bool))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Expression<Func<Test,string>>)(a => a.TestBool(default(bool)))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)(a => a.TestBool(default))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)(a => a.TestBool(default)))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)(a => a.TestBool(default(bool)))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)(a => a.TestBool(default(bool))))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (a.TestBool(default)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (a.TestBool(default))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (a.TestBool(default(bool))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (a.TestBool(default(bool)))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (a.TestBool(default))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (a.TestBool(default)))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (a.TestBool(default(bool)))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (a.TestBool(default(bool))))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => a.TestBool(default), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => a.TestBool(default)).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => a.TestBool(default(bool)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => a.TestBool(default(bool))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => a.TestBool(default)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => a.TestBool(default))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => a.TestBool(default(bool))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => a.TestBool(default(bool)))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (a.TestBool(default)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (a.TestBool(default))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (a.TestBool(default(bool))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (a.TestBool(default(bool)))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (a.TestBool(default))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (a.TestBool(default)))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (a.TestBool(default(bool)))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (a.TestBool(default(bool))))).Returns("Works")),
    };

    public static Action<AutoMock<Test>>[] SetupNullableBoolActions =
{
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => a.TestNullableBool(default), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => a.TestNullableBool(default)).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => a.TestNullableBool(default(bool)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => a.TestNullableBool(default(bool))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => a.TestNullableBool(default(bool?)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => a.TestNullableBool(default(bool?))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => a.TestNullableBool(default)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => a.TestNullableBool(default))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => a.TestNullableBool(default(bool))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => a.TestNullableBool(default(bool)))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => a.TestNullableBool(default(bool?))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => a.TestNullableBool(default(bool?)))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Expression<Func<Test,string>>)(a => a.TestNullableBool(default)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Expression<Func<Test,string>>)(a => a.TestNullableBool(default))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Expression<Func<Test,string>>)(a => a.TestNullableBool(default(bool))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Expression<Func<Test,string>>)(a => a.TestNullableBool(default(bool)))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Expression<Func<Test,string>>)(a => a.TestNullableBool(default(bool?))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Expression<Func<Test,string>>)(a => a.TestNullableBool(default(bool?)))).Returns("Works")),


        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)(a => a.TestNullableBool(default))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)(a => a.TestNullableBool(default)))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)(a => a.TestNullableBool(default(bool)))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)(a => a.TestNullableBool(default(bool))))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)(a => a.TestNullableBool(default(bool?)))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)(a => a.TestNullableBool(default(bool?))))).Returns("Works")),


        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (a.TestNullableBool(default)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (a.TestNullableBool(default))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (a.TestNullableBool(default(bool))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (a.TestNullableBool(default(bool)))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (a.TestNullableBool(default(bool?))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (a.TestNullableBool(default(bool?)))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (a.TestNullableBool(default))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (a.TestNullableBool(default)))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (a.TestNullableBool(default(bool)))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (a.TestNullableBool(default(bool))))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (a.TestNullableBool(default(bool?)))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (a.TestNullableBool(default(bool?))))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => a.TestNullableBool(default), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => a.TestNullableBool(default)).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => a.TestNullableBool(default(bool)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => a.TestNullableBool(default(bool))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => a.TestNullableBool(default(bool?)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => a.TestNullableBool(default(bool?))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => a.TestNullableBool(default)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => a.TestNullableBool(default))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => a.TestNullableBool(default(bool))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => a.TestNullableBool(default(bool)))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => a.TestNullableBool(default(bool?))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => a.TestNullableBool(default(bool?)))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (a.TestNullableBool(default)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (a.TestNullableBool(default))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (a.TestNullableBool(default(bool))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (a.TestNullableBool(default(bool)))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (a.TestNullableBool(default(bool?))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (a.TestNullableBool(default(bool?)))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (a.TestNullableBool(default))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (a.TestNullableBool(default)))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (a.TestNullableBool(default(bool)))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (a.TestNullableBool(default(bool))))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (a.TestNullableBool(default(bool?)))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (a.TestNullableBool(default(bool?))))).Returns("Works")),
    };

    public static Action<AutoMock<Test>>[] SetupDateTimeActions =
{
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => a.TestDateTime(default), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => a.TestDateTime(default)).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => a.TestDateTime(default(DateTime)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => a.TestDateTime(default(DateTime))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => a.TestDateTime(default)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => a.TestDateTime(default))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => a.TestDateTime(default(DateTime))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => a.TestDateTime(default(DateTime)))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Expression<Func<Test,string>>)(a => a.TestDateTime(default)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Expression<Func<Test,string>>)(a => a.TestDateTime(default))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Expression<Func<Test,string>>)(a => a.TestDateTime(default(DateTime))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Expression<Func<Test,string>>)(a => a.TestDateTime(default(DateTime)))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)(a => a.TestDateTime(default))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)(a => a.TestDateTime(default)))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)(a => a.TestDateTime(default(DateTime)))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)(a => a.TestDateTime(default(DateTime))))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (a.TestDateTime(default)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (a.TestDateTime(default))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (a.TestDateTime(default(DateTime))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (a.TestDateTime(default(DateTime)))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (a.TestDateTime(default))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (a.TestDateTime(default)))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (a.TestDateTime(default(DateTime)))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (a.TestDateTime(default(DateTime))))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => a.TestDateTime(default), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => a.TestDateTime(default)).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => a.TestDateTime(default(DateTime)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => a.TestDateTime(default(DateTime))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => a.TestDateTime(default)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => a.TestDateTime(default))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => a.TestDateTime(default(DateTime))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => a.TestDateTime(default(DateTime)))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (a.TestDateTime(default)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (a.TestDateTime(default))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (a.TestDateTime(default(DateTime))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (a.TestDateTime(default(DateTime)))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (a.TestDateTime(default))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (a.TestDateTime(default)))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (a.TestDateTime(default(DateTime)))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (a.TestDateTime(default(DateTime))))).Returns("Works")),
    };

    public static Action<AutoMock<Test>>[] SetupNullableDateTimeActions =
{
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => a.TestNullableDateTime(default), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => a.TestNullableDateTime(default)).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => a.TestNullableDateTime(default(DateTime)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => a.TestNullableDateTime(default(DateTime))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => a.TestNullableDateTime(default(DateTime?)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => a.TestNullableDateTime(default(DateTime?))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => a.TestNullableDateTime(default)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => a.TestNullableDateTime(default))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => a.TestNullableDateTime(default(DateTime))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => a.TestNullableDateTime(default(DateTime)))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => a.TestNullableDateTime(default(DateTime?))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => a.TestNullableDateTime(default(DateTime?)))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Expression<Func<Test,string>>)(a => a.TestNullableDateTime(default)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Expression<Func<Test,string>>)(a => a.TestNullableDateTime(default))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Expression<Func<Test,string>>)(a => a.TestNullableDateTime(default(DateTime))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Expression<Func<Test,string>>)(a => a.TestNullableDateTime(default(DateTime)))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Expression<Func<Test,string>>)(a => a.TestNullableDateTime(default(DateTime?))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Expression<Func<Test,string>>)(a => a.TestNullableDateTime(default(DateTime?)))).Returns("Works")),


        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)(a => a.TestNullableDateTime(default))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)(a => a.TestNullableDateTime(default)))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)(a => a.TestNullableDateTime(default(DateTime)))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)(a => a.TestNullableDateTime(default(DateTime))))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)(a => a.TestNullableDateTime(default(DateTime?)))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Expression<Func<Test,string>>)(a => a.TestNullableDateTime(default(DateTime?))))).Returns("Works")),


        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (a.TestNullableDateTime(default)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (a.TestNullableDateTime(default))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (a.TestNullableDateTime(default(DateTime))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (a.TestNullableDateTime(default(DateTime)))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (a.TestNullableDateTime(default(DateTime?))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(a => (a.TestNullableDateTime(default(DateTime?)))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (a.TestNullableDateTime(default))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (a.TestNullableDateTime(default)))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (a.TestNullableDateTime(default(DateTime)))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (a.TestNullableDateTime(default(DateTime))))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (a.TestNullableDateTime(default(DateTime?)))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((a => (a.TestNullableDateTime(default(DateTime?))))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => a.TestNullableDateTime(default), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => a.TestNullableDateTime(default)).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => a.TestNullableDateTime(default(DateTime)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => a.TestNullableDateTime(default(DateTime))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => a.TestNullableDateTime(default(DateTime?)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => a.TestNullableDateTime(default(DateTime?))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => a.TestNullableDateTime(default)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => a.TestNullableDateTime(default))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => a.TestNullableDateTime(default(DateTime))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => a.TestNullableDateTime(default(DateTime)))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => a.TestNullableDateTime(default(DateTime?))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => a.TestNullableDateTime(default(DateTime?)))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (a.TestNullableDateTime(default)), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (a.TestNullableDateTime(default))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (a.TestNullableDateTime(default(DateTime))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (a.TestNullableDateTime(default(DateTime)))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (a.TestNullableDateTime(default(DateTime?))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup((Test a) => (a.TestNullableDateTime(default(DateTime?)))).Returns("Works")),

        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (a.TestNullableDateTime(default))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (a.TestNullableDateTime(default)))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (a.TestNullableDateTime(default(DateTime)))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (a.TestNullableDateTime(default(DateTime))))).Returns("Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (a.TestNullableDateTime(default(DateTime?)))), "Works")),
        (Action<AutoMock<Test>>)(autoMock => autoMock.Setup(((Test a) => (a.TestNullableDateTime(default(DateTime?))))).Returns("Works")),
    };
}
