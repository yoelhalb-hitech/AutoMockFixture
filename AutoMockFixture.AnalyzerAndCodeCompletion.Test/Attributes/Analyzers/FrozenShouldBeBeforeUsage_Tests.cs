using AutoMockFixture.AnalyzerAndCodeCompletion.Attributes.Analyzers;

namespace AutoMockFixture.AnalyzerAndCodeCompletion.Test.Attributes.Analyzers;

internal class FrozenShouldBeBeforeUsage_Tests : AnalyzerVerifierBase<FrozenShouldBeBeforeUsage>
{
    [Test]
    public async Task Test_Warns_WhenLast()
    {
        var code = """
            using AutoFixture.NUnit3;
            public class Test
            {
                public void TestMethod(int i, [[|Frozen|]] int valueTypeParameter)
                {
                }
            }
            """;

        await VerifyAsync(code).ConfigureAwait(false);
    }
}
