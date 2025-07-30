using AutoFixture.NUnit3;
using AutoMockFixture.AnalyzerAndCodeCompletion.Attributes.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace AutoMockFixture.AnalyzerAndCodeCompletion.Test.Attributes.Analyzers;

internal class FrozenShouldBeBeforeUsage_Tests
    : AnalyzerVerifier<FrozenShouldBeBeforeUsage, CSharpAnalyzerTest<FrozenShouldBeBeforeUsage, DefaultVerifier>, DefaultVerifier>
{
    [Test]
    public async Task Test_Warns_WhenLast()
    {
        var code = """
            using AutoFixture.NUnit3;
            using System;
            public class Test
            {
                public void TestMethod(int i, [[|Frozen|]] int valueTypeParameter)
                {
                }
            }
            """;

        await VerifyAsync(code).ConfigureAwait(false);
    }


    private static async Task VerifyAsync(string code)
    {
        var test = new CSharpAnalyzerTest<FrozenShouldBeBeforeUsage, DefaultVerifier>
        {
            TestCode = code,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net60,
        };
        test.TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(typeof(FrozenAttribute).Assembly.Location));
        //await VerifyAnalyzerAsync(code);
        await test.RunAsync().ConfigureAwait(false);
    }
}
