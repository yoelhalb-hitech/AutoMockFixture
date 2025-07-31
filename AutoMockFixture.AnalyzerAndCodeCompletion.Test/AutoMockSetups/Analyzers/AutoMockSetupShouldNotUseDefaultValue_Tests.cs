using AutoMockFixture.AnalyzerAndCodeCompletion.AutoMockSetups.Analyzers;

namespace AutoMockFixture.AnalyzerAndCodeCompletion.Test.AutoMockSetups.Analyzers;

internal class AutoMockSetupShouldNotUseDefaultValue_Tests
    : AnalyzerVerifierBase<AutoMockSetupShouldNotUseDefaultValue>
{
    [Test]
    public async Task Test_Works_WithReferenceTypes()
    {
        var code = """
            using AutoMockFixture.Moq4;
            using Moq;
            public class Test
            {
                public virtual string TestString(string? str) => "";
            }
            public class Testing
            {
                public void TestMethod()
                {
                    var autoMock = new AutoMock<Test>();

                    autoMock.Setup(a => a.TestString([|null|]), "Works");
                    autoMock.Setup(a => a.TestString([|null|])).Returns("Works");
                    autoMock.Setup(a => a.TestString([|(string)null|]), "Works");
                    autoMock.Setup(a => a.TestString([|(string)null|])).Returns("Works");
                    autoMock.Setup(a => a.TestString([|null as string|]), "Works");
                    autoMock.Setup(a => a.TestString([|null as string|])).Returns("Works");

                    autoMock.Setup(a => a.TestString([|(null)|]), "Works");
                    autoMock.Setup(a => a.TestString([|(null)|])).Returns("Works");
                    autoMock.Setup(a => a.TestString([|(string)(null)|]), "Works");
                    autoMock.Setup(a => a.TestString([|(string)(null)|])).Returns("Works");
                    autoMock.Setup(a => a.TestString([|((string)null)|]), "Works");
                    autoMock.Setup(a => a.TestString([|((string)null)|])).Returns("Works");
                    autoMock.Setup(a => a.TestString([|((string)(null))|]), "Works");
                    autoMock.Setup(a => a.TestString([|((string)(null))|])).Returns("Works");
                    autoMock.Setup(a => a.TestString([|(null as string)|]), "Works");
                    autoMock.Setup(a => a.TestString([|(null as string)|])).Returns("Works");
                    autoMock.Setup(a => a.TestString([|((null) as string)|]), "Works");
                    autoMock.Setup(a => a.TestString([|((null) as string)|])).Returns("Works");

                    autoMock.Setup(a => (a.TestString([|null|])), "Works");
                    autoMock.Setup(a => (a.TestString([|null|]))).Returns("Works");
                    autoMock.Setup(a => (a.TestString([|(string)null|])), "Works");
                    autoMock.Setup(a => (a.TestString([|(string)null|]))).Returns("Works");
                    autoMock.Setup(a => (a.TestString([|null as string|])), "Works");
                    autoMock.Setup(a => (a.TestString([|null as string|]))).Returns("Works");

                    autoMock.Setup((a => a.TestString([|null|])), "Works");
                    autoMock.Setup((a => a.TestString([|null|]))).Returns("Works");
                    autoMock.Setup((a => a.TestString([|(string)null|])), "Works");
                    autoMock.Setup((a => a.TestString([|(string)null|]))).Returns("Works");
                    autoMock.Setup((a => a.TestString([|null as string|])), "Works");
                    autoMock.Setup((a => a.TestString([|null as string|]))).Returns("Works");

                    autoMock.Setup((Test a) => a.TestString([|null|]), "Works");
                    autoMock.Setup((Test a) => a.TestString([|null|])).Returns("Works");
                    autoMock.Setup((Test a) => a.TestString([|(string)null|]), "Works");
                    autoMock.Setup((Test a) => a.TestString([|(string)null|])).Returns("Works");
                    autoMock.Setup((Test a) => a.TestString([|null as string|]), "Works");
                    autoMock.Setup((Test a) => a.TestString([|null as string|])).Returns("Works");

                    autoMock.Setup(((Test a) => a.TestString([|null|])), "Works");
                    autoMock.Setup(((Test a) => a.TestString([|null|]))).Returns("Works");
                    autoMock.Setup(((Test a) => a.TestString([|(string)null|])), "Works");
                    autoMock.Setup(((Test a) => a.TestString([|(string)null|]))).Returns("Works");
                    autoMock.Setup(((Test a) => a.TestString([|null as string|])), "Works");
                    autoMock.Setup(((Test a) => a.TestString([|null as string|]))).Returns("Works");

                    autoMock.Setup(((Test a) => a.TestString([|(null)|])), "Works");
                    autoMock.Setup(((Test a) => a.TestString([|(null)|]))).Returns("Works");
                    autoMock.Setup(((Test a) => a.TestString([|((string)null)|])), "Works");
                    autoMock.Setup(((Test a) => a.TestString([|((string)null)|]))).Returns("Works");
                    autoMock.Setup(((Test a) => a.TestString([|(null as string)|])), "Works");
                    autoMock.Setup(((Test a) => a.TestString([|(null as string)|]))).Returns("Works");

                    autoMock.Setup(a => a.TestString(default), "Works");
                    autoMock.Setup(a => a.TestString(default)).Returns("Works");
                    autoMock.Setup(a => a.TestString(default(string)), "Works");
                    autoMock.Setup(a => a.TestString(default(string))).Returns("Works");
                    autoMock.Setup(a => a.TestString(It.IsAny<string>()), "Works");
                    autoMock.Setup(a => a.TestString(It.IsAny<string>())).Returns("Works");

                    autoMock.Setup(a => a.TestString(It.Is<string>(s => s == null)), "Works");
                    autoMock.Setup(a => a.TestString(It.Is<string>(s => s == null))).Returns("Works");
                    ((Moq.Mock<Test>)autoMock).Setup(a => a.TestString(null)).Returns("Works");
                    ((Moq.Mock<Test>)autoMock).Setup(a => a.TestString((string)null)).Returns("Works");
                }
            }
            """;

        await VerifyAsync(code).ConfigureAwait(false);
    }

    [Test]
    public async Task Test_Works_WithValueTypes()
    {
        var code = """
            using AutoMockFixture.Moq4;
            using Moq;
            public class Test
            {
                public virtual string TestBool(bool b) => "";
                public virtual string TestInt(int i) => "";
                public virtual string TestUint(uint ui) => "";
                public virtual string TestDecimal(decimal d) => "";
                public virtual string TestFloat(float f) => "";
            }
            public class Testing
            {
                public void TestMethod()
                {
                    var autoMock = new AutoMock<Test>();
                    autoMock.Setup(a => a.TestInt(default), "Works");
                    autoMock.Setup(a => a.TestInt(default)).Returns("Works");
                    autoMock.Setup(a => a.TestInt(default(int)), "Works");
                    autoMock.Setup(a => a.TestInt(default(int))).Returns("Works");
                    autoMock.Setup(a => a.TestInt(It.IsAny<int>()), "Works");
                    autoMock.Setup(a => a.TestInt(It.IsAny<int>())).Returns("Works");

                    autoMock.Setup(a => a.TestInt([|0|]), "Works");
                    autoMock.Setup(a => a.TestInt([|0|])).Returns("Works");
                    autoMock.Setup(a => a.TestInt([|(int)0|]), "Works");
                    autoMock.Setup(a => a.TestInt([|(int)0|])).Returns("Works");

                    autoMock.Setup(a => a.TestBool([|false|]), "Works");
                    autoMock.Setup(a => a.TestBool([|false|])).Returns("Works");
                    autoMock.Setup(a => a.TestBool([|(bool)false|]), "Works");
                    autoMock.Setup(a => a.TestBool([|(bool)false|])).Returns("Works");

                    autoMock.Setup(a => a.TestDecimal([|0m|]), "Works");
                    autoMock.Setup(a => a.TestDecimal([|0m|])).Returns("Works");
                    autoMock.Setup(a => a.TestDecimal([|(decimal)0m|]), "Works");
                    autoMock.Setup(a => a.TestDecimal([|(decimal)0m|])).Returns("Works");

                    autoMock.Setup(a => a.TestDecimal([|0.0m|]), "Works");
                    autoMock.Setup(a => a.TestDecimal([|0.0m|])).Returns("Works");
                    autoMock.Setup(a => a.TestDecimal([|(decimal)0.0m|]), "Works");
                    autoMock.Setup(a => a.TestDecimal([|(decimal)0.0m|])).Returns("Works");

                    autoMock.Setup(a => a.TestFloat([|0f|]), "Works");
                    autoMock.Setup(a => a.TestFloat([|0f|])).Returns("Works");
                    autoMock.Setup(a => a.TestFloat([|(float)0f|]), "Works");
                    autoMock.Setup(a => a.TestFloat([|(float)0f|])).Returns("Works");

                    autoMock.Setup(a => a.TestFloat([|0.0f|]), "Works");
                    autoMock.Setup(a => a.TestFloat([|0000.000f|]), "Works");
                    autoMock.Setup(a => a.TestFloat([|0.0f|])).Returns("Works");
                    autoMock.Setup(a => a.TestFloat([|(float)0.0f|]), "Works");
                    autoMock.Setup(a => a.TestFloat([|(float)0.0f|])).Returns("Works");

                    autoMock.Setup(a => a.TestUint([|0u|]), "Works");
                    autoMock.Setup(a => a.TestUint([|0u|])).Returns("Works");
                    autoMock.Setup(a => a.TestUint([|(uint)0u|]), "Works");
                    autoMock.Setup(a => a.TestUint([|(uint)0u|])).Returns("Works");

                    autoMock.Setup(a => a.TestInt([|(0)|]), "Works");
                    autoMock.Setup(a => a.TestInt([|(0)|])).Returns("Works");
                    autoMock.Setup(a => a.TestInt([|(int)(0)|]), "Works");
                    autoMock.Setup(a => a.TestInt([|(int)(0)|])).Returns("Works");
                    autoMock.Setup(a => a.TestInt([|((int)0)|]), "Works");
                    autoMock.Setup(a => a.TestInt([|((int)0)|])).Returns("Works");
                    autoMock.Setup(a => a.TestInt([|((int)(0))|]), "Works");
                    autoMock.Setup(a => a.TestInt([|((int)(0))|])).Returns("Works");

                    autoMock.Setup(a => a.TestInt(It.Is<int>(i => i == 0)), "Works");
                    autoMock.Setup(a => a.TestInt(It.Is<int>(i => i == 0))).Returns("Works");
                    ((Moq.Mock<Test>)autoMock).Setup(a => a.TestInt(0)).Returns("Works");
                    ((Moq.Mock<Test>)autoMock).Setup(a => a.TestInt((int)0)).Returns("Works");
                }
            }
            """;

        await VerifyAsync(code).ConfigureAwait(false);
    }

    [Test]
    public async Task Test_Works_WithNullableValueTypes()
    {
        var code = """
            using AutoMockFixture.Moq4;
            using Moq;
            public class Test
            {
                public virtual string TestNullableInt(int? i) => "";
                public virtual string TestNullableDecimal(decimal? d) => "";
                public virtual string TestNullableBool(bool? b) => "";
            }
            public class Testing
            {
                public void TestMethod()
                {
                    var autoMock = new AutoMock<Test>();
                    autoMock.Setup(a => a.TestNullableInt(default), "Works");
                    autoMock.Setup(a => a.TestNullableInt(default)).Returns("Works");
                    autoMock.Setup(a => a.TestNullableInt(default(int)), "Works");
                    autoMock.Setup(a => a.TestNullableInt(default(int))).Returns("Works");
                    autoMock.Setup(a => a.TestNullableInt(default(int?)), "Works");
                    autoMock.Setup(a => a.TestNullableInt(default(int?))).Returns("Works");
                    autoMock.Setup(a => a.TestNullableInt(It.IsAny<int>()), "Works");
                    autoMock.Setup(a => a.TestNullableInt(It.IsAny<int>())).Returns("Works");
                    autoMock.Setup(a => a.TestNullableInt(It.IsAny<int?>()), "Works");
                    autoMock.Setup(a => a.TestNullableInt(It.IsAny<int?>())).Returns("Works");

                    autoMock.Setup(a => a.TestNullableInt([|0|]), "Works");
                    autoMock.Setup(a => a.TestNullableInt([|0|])).Returns("Works");
                    autoMock.Setup(a => a.TestNullableInt([|(int)0|]), "Works");
                    autoMock.Setup(a => a.TestNullableInt([|(int)0|])).Returns("Works");
                    autoMock.Setup(a => a.TestNullableInt([|(int?)0|]), "Works");
                    autoMock.Setup(a => a.TestNullableInt([|(int?)0|])).Returns("Works");

                    autoMock.Setup(a => a.TestNullableInt([|null|]), "Works");
                    autoMock.Setup(a => a.TestNullableInt([|null|])).Returns("Works");
                    autoMock.Setup(a => a.TestNullableInt([|(int?)null|]), "Works");
                    autoMock.Setup(a => a.TestNullableInt([|(int?)null|])).Returns("Works");

                    autoMock.Setup(a => a.TestNullableDecimal([|0|]), "Works");
                    autoMock.Setup(a => a.TestNullableDecimal([|0|])).Returns("Works");
                    autoMock.Setup(a => a.TestNullableDecimal([|(decimal)0|]), "Works");
                    autoMock.Setup(a => a.TestNullableDecimal([|(decimal)0|])).Returns("Works");
                    autoMock.Setup(a => a.TestNullableDecimal([|(decimal?)0|]), "Works");
                    autoMock.Setup(a => a.TestNullableDecimal([|(decimal?)0|])).Returns("Works");

                    autoMock.Setup(a => a.TestNullableDecimal([|null|]), "Works");
                    autoMock.Setup(a => a.TestNullableDecimal([|null|])).Returns("Works");
                    autoMock.Setup(a => a.TestNullableDecimal([|(decimal?)null|]), "Works");
                    autoMock.Setup(a => a.TestNullableDecimal([|(decimal?)null|])).Returns("Works");


                    autoMock.Setup(a => a.TestNullableBool([|false|]), "Works");
                    autoMock.Setup(a => a.TestNullableBool([|false|])).Returns("Works");
                    autoMock.Setup(a => a.TestNullableBool([|(bool)false|]), "Works");
                    autoMock.Setup(a => a.TestNullableBool([|(bool)false|])).Returns("Works");
                    autoMock.Setup(a => a.TestNullableBool([|(bool?)false|]), "Works");
                    autoMock.Setup(a => a.TestNullableBool([|(bool?)false|])).Returns("Works");

                    autoMock.Setup(a => a.TestNullableBool([|null|]), "Works");
                    autoMock.Setup(a => a.TestNullableBool([|null|])).Returns("Works");
                    autoMock.Setup(a => a.TestNullableBool([|(bool?)null|]), "Works");
                    autoMock.Setup(a => a.TestNullableBool([|(bool?)null|])).Returns("Works");

                    autoMock.Setup(a => a.TestNullableInt(It.Is<int>(i => i == 0)), "Works");
                    autoMock.Setup(a => a.TestNullableInt(It.Is<int>(i => i == 0))).Returns("Works");
                    autoMock.Setup(a => a.TestNullableInt(It.Is<int?>(i => i == 0)), "Works");
                    autoMock.Setup(a => a.TestNullableInt(It.Is<int?>(i => i == 0))).Returns("Works");
                    autoMock.Setup(a => a.TestNullableInt(It.Is<int?>(i => i == null)), "Works");
                    autoMock.Setup(a => a.TestNullableInt(It.Is<int?>(i => i == null))).Returns("Works");
                    ((Moq.Mock<Test>)autoMock).Setup(a => a.TestNullableInt(0)).Returns("Works");
                    ((Moq.Mock<Test>)autoMock).Setup(a => a.TestNullableInt((int)0)).Returns("Works");
                    ((Moq.Mock<Test>)autoMock).Setup(a => a.TestNullableInt((int)0)).Returns("Works");
                    ((Moq.Mock<Test>)autoMock).Setup(a => a.TestNullableInt((int?)0)).Returns("Works");
                    ((Moq.Mock<Test>)autoMock).Setup(a => a.TestNullableInt(null)).Returns("Works");
                    ((Moq.Mock<Test>)autoMock).Setup(a => a.TestNullableInt((int?)null)).Returns("Works");
                }
            }
            """;

        await VerifyAsync(code).ConfigureAwait(false);
    }

    [Test]
    public async Task Test_Works_WithSpecialValueTypes()
    {
        var code = """
            using AutoMockFixture.Moq4;
            using Moq;
            using System;
            public class Test
            {
                public virtual string TestDateTime(DateTime d) => "";
                public virtual string TestDateOnly(DateOnly d) => "";
                public virtual string TestTimeOnly(TimeOnly t) => "";
                public virtual string TestTimeSpan(TimeSpan t) => "";
            }
            public class Testing
            {
                public void TestMethod()
                {
                    var autoMock = new AutoMock<Test>();
                    autoMock.Setup(a => a.TestDateTime(default), "Works");
                    autoMock.Setup(a => a.TestDateTime(default)).Returns("Works");
                    autoMock.Setup(a => a.TestDateTime(default(DateTime)), "Works");
                    autoMock.Setup(a => a.TestDateTime(default(DateTime))).Returns("Works");
                    autoMock.Setup(a => a.TestDateTime(It.IsAny<DateTime>()), "Works");
                    autoMock.Setup(a => a.TestDateTime(It.IsAny<DateTime>())).Returns("Works");

                    autoMock.Setup(a => a.TestDateTime([|DateTime.MinValue|]), "Works");
                    autoMock.Setup(a => a.TestDateTime([|DateTime.MinValue|])).Returns("Works");
                    autoMock.Setup(a => a.TestDateTime([|(DateTime)DateTime.MinValue|]), "Works");
                    autoMock.Setup(a => a.TestDateTime([|(DateTime)DateTime.MinValue|])).Returns("Works");

                    autoMock.Setup(a => a.TestDateTime([|System.DateTime.MinValue|]), "Works");
                    autoMock.Setup(a => a.TestDateTime([|System.DateTime.MinValue|])).Returns("Works");
                    autoMock.Setup(a => a.TestDateTime([|(System.DateTime)System.DateTime.MinValue|]), "Works");
                    autoMock.Setup(a => a.TestDateTime([|(System.DateTime)System.DateTime.MinValue|])).Returns("Works");

                    autoMock.Setup(a => a.TestDateOnly([|DateOnly.MinValue|]), "Works");
                    autoMock.Setup(a => a.TestDateOnly([|DateOnly.MinValue|])).Returns("Works");
                    autoMock.Setup(a => a.TestDateOnly([|(DateOnly)DateOnly.MinValue|]), "Works");
                    autoMock.Setup(a => a.TestDateOnly([|(DateOnly)DateOnly.MinValue|])).Returns("Works");

                    autoMock.Setup(a => a.TestDateOnly([|System.DateOnly.MinValue|]), "Works");
                    autoMock.Setup(a => a.TestDateOnly([|System.DateOnly.MinValue|])).Returns("Works");
                    autoMock.Setup(a => a.TestDateOnly([|(DateOnly)System.DateOnly.MinValue|]), "Works");
                    autoMock.Setup(a => a.TestDateOnly([|(System.DateOnly)System.DateOnly.MinValue|])).Returns("Works");

                    autoMock.Setup(a => a.TestTimeOnly([|TimeOnly.MinValue|]), "Works");
                    autoMock.Setup(a => a.TestTimeOnly([|TimeOnly.MinValue|])).Returns("Works");
                    autoMock.Setup(a => a.TestTimeOnly([|(TimeOnly)TimeOnly.MinValue|]), "Works");
                    autoMock.Setup(a => a.TestTimeOnly([|(TimeOnly)TimeOnly.MinValue|])).Returns("Works");

                    autoMock.Setup(a => a.TestTimeOnly([|System.TimeOnly.MinValue|]), "Works");
                    autoMock.Setup(a => a.TestTimeOnly([|System.TimeOnly.MinValue|])).Returns("Works");
                    autoMock.Setup(a => a.TestTimeOnly([|(TimeOnly)System.TimeOnly.MinValue|]), "Works");
                    autoMock.Setup(a => a.TestTimeOnly([|(System.TimeOnly)System.TimeOnly.MinValue|])).Returns("Works");

                    autoMock.Setup(a => a.TestTimeSpan([|TimeSpan.Zero|]), "Works");
                    autoMock.Setup(a => a.TestTimeSpan([|TimeSpan.Zero|])).Returns("Works");
                    autoMock.Setup(a => a.TestTimeSpan([|(TimeSpan)TimeSpan.Zero|]), "Works");
                    autoMock.Setup(a => a.TestTimeSpan([|(TimeSpan)TimeSpan.Zero|])).Returns("Works");

                    autoMock.Setup(a => a.TestTimeSpan([|System.TimeSpan.Zero|]), "Works");
                    autoMock.Setup(a => a.TestTimeSpan([|System.TimeSpan.Zero|])).Returns("Works");
                    autoMock.Setup(a => a.TestTimeSpan([|(System.TimeSpan)System.TimeSpan.Zero|]), "Works");
                    autoMock.Setup(a => a.TestTimeSpan([|(System.TimeSpan)System.TimeSpan.Zero|])).Returns("Works");

                    autoMock.Setup(a => a.TestDateTime(It.Is<DateTime>(d => d == DateTime.MinValue)), "Works");
                    autoMock.Setup(a => a.TestDateTime(It.Is<DateTime>(d => d == DateTime.MinValue))).Returns("Works");
                    ((Moq.Mock<Test>)autoMock).Setup(a => a.TestDateTime(DateTime.MinValue)).Returns("Works");
                    ((Moq.Mock<Test>)autoMock).Setup(a => a.TestDateTime((DateTime)DateTime.MinValue)).Returns("Works");
                }
            }
            """;

        await VerifyAsync(code).ConfigureAwait(false);
    }

    [Test]
    public async Task Test_Works_WithNullableSpecialValueTypes()
    {
        var code = """
            using AutoMockFixture.Moq4;
            using Moq;
            using System;
            public class Test
            {
                public virtual string TestNullableDateTime(DateTime? d) => "";
                public virtual string TestNullableDateOnly(DateOnly? d) => "";
                public virtual string TestNullableTimeOnly(TimeOnly? t) => "";
                public virtual string TestNullableTimeSpan(TimeSpan? t) => "";
            }
            public class Testing
            {
                public void TestMethod()
                {
                    var autoMock = new AutoMock<Test>();

                    autoMock.Setup(a => a.TestNullableDateTime(default), "Works");
                    autoMock.Setup(a => a.TestNullableDateTime(default)).Returns("Works");
                    autoMock.Setup(a => a.TestNullableDateTime(default(DateTime)), "Works");
                    autoMock.Setup(a => a.TestNullableDateTime(default(DateTime))).Returns("Works");
                    autoMock.Setup(a => a.TestNullableDateTime(default(DateTime?)), "Works");
                    autoMock.Setup(a => a.TestNullableDateTime(default(DateTime?))).Returns("Works");
                    autoMock.Setup(a => a.TestNullableDateTime(It.IsAny<DateTime>()), "Works");
                    autoMock.Setup(a => a.TestNullableDateTime(It.IsAny<DateTime>())).Returns("Works");
                    autoMock.Setup(a => a.TestNullableDateTime(It.IsAny<DateTime?>()), "Works");
                    autoMock.Setup(a => a.TestNullableDateTime(It.IsAny<DateTime?>())).Returns("Works");


                    autoMock.Setup(a => a.TestNullableDateTime([|DateTime.MinValue|]), "Works");
                    autoMock.Setup(a => a.TestNullableDateTime([|DateTime.MinValue|])).Returns("Works");
                    autoMock.Setup(a => a.TestNullableDateTime([|(DateTime)DateTime.MinValue|]), "Works");
                    autoMock.Setup(a => a.TestNullableDateTime([|(DateTime)DateTime.MinValue|])).Returns("Works");
                    autoMock.Setup(a => a.TestNullableDateTime([|(DateTime?)DateTime.MinValue|]), "Works");
                    autoMock.Setup(a => a.TestNullableDateTime([|(DateTime?)DateTime.MinValue|])).Returns("Works");

                    autoMock.Setup(a => a.TestNullableDateTime([|null|]), "Works");
                    autoMock.Setup(a => a.TestNullableDateTime([|null|])).Returns("Works");
                    autoMock.Setup(a => a.TestNullableDateTime([|(System.DateTime?)null|]), "Works");
                    autoMock.Setup(a => a.TestNullableDateTime([|(System.DateTime?)null|])).Returns("Works");

                    autoMock.Setup(a => a.TestNullableDateOnly([|DateOnly.MinValue|]), "Works");
                    autoMock.Setup(a => a.TestNullableDateOnly([|DateOnly.MinValue|])).Returns("Works");
                    autoMock.Setup(a => a.TestNullableDateOnly([|(DateOnly)DateOnly.MinValue|]), "Works");
                    autoMock.Setup(a => a.TestNullableDateOnly([|(DateOnly)DateOnly.MinValue|])).Returns("Works");
                    autoMock.Setup(a => a.TestNullableDateOnly([|(DateOnly?)DateOnly.MinValue|]), "Works");
                    autoMock.Setup(a => a.TestNullableDateOnly([|(DateOnly?)DateOnly.MinValue|])).Returns("Works");

                    autoMock.Setup(a => a.TestNullableDateOnly([|null|]), "Works");
                    autoMock.Setup(a => a.TestNullableDateOnly([|null|])).Returns("Works");
                    autoMock.Setup(a => a.TestNullableDateOnly([|(DateOnly?)null|]), "Works");
                    autoMock.Setup(a => a.TestNullableDateOnly([|(System.DateOnly?)null|])).Returns("Works");

                    autoMock.Setup(a => a.TestNullableTimeOnly([|TimeOnly.MinValue|]), "Works");
                    autoMock.Setup(a => a.TestNullableTimeOnly([|TimeOnly.MinValue|])).Returns("Works");
                    autoMock.Setup(a => a.TestNullableTimeOnly([|(TimeOnly)TimeOnly.MinValue|]), "Works");
                    autoMock.Setup(a => a.TestNullableTimeOnly([|(TimeOnly)TimeOnly.MinValue|])).Returns("Works");
                    autoMock.Setup(a => a.TestNullableTimeOnly([|(TimeOnly?)TimeOnly.MinValue|]), "Works");
                    autoMock.Setup(a => a.TestNullableTimeOnly([|(TimeOnly?)TimeOnly.MinValue|])).Returns("Works");

                    autoMock.Setup(a => a.TestNullableTimeOnly([|null|]), "Works");
                    autoMock.Setup(a => a.TestNullableTimeOnly([|null|])).Returns("Works");
                    autoMock.Setup(a => a.TestNullableTimeOnly([|(TimeOnly?)null|]), "Works");
                    autoMock.Setup(a => a.TestNullableTimeOnly([|(System.TimeOnly?)null|])).Returns("Works");

                    autoMock.Setup(a => a.TestNullableTimeSpan([|TimeSpan.Zero|]), "Works");
                    autoMock.Setup(a => a.TestNullableTimeSpan([|TimeSpan.Zero|])).Returns("Works");
                    autoMock.Setup(a => a.TestNullableTimeSpan([|(TimeSpan)TimeSpan.Zero|]), "Works");
                    autoMock.Setup(a => a.TestNullableTimeSpan([|(TimeSpan)TimeSpan.Zero|])).Returns("Works");
                    autoMock.Setup(a => a.TestNullableTimeSpan([|(TimeSpan?)TimeSpan.Zero|]), "Works");
                    autoMock.Setup(a => a.TestNullableTimeSpan([|(TimeSpan?)TimeSpan.Zero|])).Returns("Works");


                    autoMock.Setup(a => a.TestNullableTimeSpan([|null|]), "Works");
                    autoMock.Setup(a => a.TestNullableTimeSpan([|null|])).Returns("Works");
                    autoMock.Setup(a => a.TestNullableTimeSpan([|(System.TimeSpan?)null|]), "Works");
                    autoMock.Setup(a => a.TestNullableTimeSpan([|(System.TimeSpan?)null|])).Returns("Works");

                    autoMock.Setup(a => a.TestNullableDateTime(It.Is<DateTime>(d => d == DateTime.MinValue)), "Works");
                    autoMock.Setup(a => a.TestNullableDateTime(It.Is<DateTime>(d => d == DateTime.MinValue))).Returns("Works");
                    autoMock.Setup(a => a.TestNullableDateTime(It.Is<DateTime>(d => d == (DateTime?)null)), "Works");
                    autoMock.Setup(a => a.TestNullableDateTime(It.Is<DateTime>(d => d == (DateTime?)null))).Returns("Works");
                    ((Moq.Mock<Test>)autoMock).Setup(a => a.TestNullableDateTime(DateTime.MinValue)).Returns("Works");
                    ((Moq.Mock<Test>)autoMock).Setup(a => a.TestNullableDateTime((DateTime)DateTime.MinValue)).Returns("Works");
                    ((Moq.Mock<Test>)autoMock).Setup(a => a.TestNullableDateTime(null)).Returns("Works");
                    ((Moq.Mock<Test>)autoMock).Setup(a => a.TestNullableDateTime((DateTime?)null)).Returns("Works");
                 }
            }
            """;

        await VerifyAsync(code).ConfigureAwait(false);
    }

}
