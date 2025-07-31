
using AutoFixture.NUnit3;
using AutoFixture;
using AutoMockFixture.AnalyzerAndCodeCompletion.Attributes.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Moq;
using AutoMockFixture.Moq4;

namespace AutoMockFixture.AnalyzerAndCodeCompletion.Test;

internal class AnalyzerVerifierBase<TAnalyzer>
    : AnalyzerVerifier<TAnalyzer, CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>, DefaultVerifier>
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    // The bottleneck appeard to be the creation of the references so caching them should be fine
    private static MetadataReference[] AdditionalReferences =
    [
        MetadataReference.CreateFromFile(typeof(AutoMockAttribute).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(FrozenAttribute).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(Mock).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(AutoMock).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(Fixture).Assembly.Location)
    ];

    protected static async Task VerifyAsync(string code)
    {
        var test = new CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>
        {
            TestCode = code,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net60,
        };
        test.TestState.AdditionalReferences.AddRange(AdditionalReferences);

        await test.RunAsync().ConfigureAwait(false);
    }
}
