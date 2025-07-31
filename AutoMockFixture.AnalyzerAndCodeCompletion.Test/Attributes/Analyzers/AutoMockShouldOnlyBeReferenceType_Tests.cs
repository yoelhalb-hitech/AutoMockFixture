using AutoMockFixture.AnalyzerAndCodeCompletion.Attributes.Analyzers;

namespace AutoMockFixture.AnalyzerAndCodeCompletion.Test.Attributes.Analyzers;

internal class AutoMockShouldOnlyBeReferenceType_Tests : AnalyzerVerifierBase<AutoMockShouldOnlyBeReferenceType>
{
    [Test]
    public async Task Test_Warns_WhenValueType()
    {
        var code = """
            using AutoMockFixture;
            using System;
            public class Test
            {
                public void TestMethod([[|AutoMock|]] int valueTypeParameter)
                {
                }
            }
            """;

            await VerifyAsync(code).ConfigureAwait(false);
    }

    [Test]
    public async Task Test_DoesNotWarn_WhenReferenceType()
    {
        var code = """
            using AutoMockFixture;
            using System;
            public class Test
            {
                public void TestMethod([AutoMock] object valueTypeParameter)
                {
                }
            }
            """;

        await VerifyAsync(code).ConfigureAwait(false);
    }

    [Test]
    public async Task Test_Warns_WhenGenericMethod()
    {
        var code = """
            using AutoMockFixture;
            using System;
            public class Test
            {
                public void TestMethod<T>([[|AutoMock|]] T valueTypeParameter)
                {
                }
            }
            """;

        await VerifyAsync(code).ConfigureAwait(false);
    }

    [Test]
    public async Task Test_DoesNotWarn_WhenGenericMethod_AndHasClassConstraint()
    {
        var code = """
            using AutoMockFixture;
            using System;
            public class Test
            {
                public void TestMethod<T>([AutoMock] T valueTypeParameter) where T : class
                {
                }
            }
            """;

        await VerifyAsync(code).ConfigureAwait(false);
    }

    [Test]
    public async Task Test_Warns_WhenGenericClass()
    {
        var code = """
            using AutoMockFixture;
            using System;
            public class Test<T>
            {
                public void TestMethod([[|AutoMock|]] T valueTypeParameter)
                {
                }
            }
            """;

        await VerifyAsync(code).ConfigureAwait(false);
    }

    [Test]
    public async Task Test_DoesNotWarn_WhenGenericClass_AndHasClassConstraint()
    {
        var code = """
            using AutoMockFixture;
            using System;
            public class Test<T>  where T : class
            {
                public void TestMethod([AutoMock] T valueTypeParameter)
                {
                }
            }
            """;

        await VerifyAsync(code).ConfigureAwait(false);
    }
}
