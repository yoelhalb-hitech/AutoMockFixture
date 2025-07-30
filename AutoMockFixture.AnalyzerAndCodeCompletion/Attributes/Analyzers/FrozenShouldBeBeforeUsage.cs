extern alias Features;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using SequelPay.DotNetPowerExtensions.RoslynExtensions;
using System.Collections.Immutable;

namespace AutoMockFixture.AnalyzerAndCodeCompletion.Attributes.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class FrozenShouldBeBeforeUsage : DiagnosticAnalyzer
{
    public const string DiagnosticId = "AMF0002";
    protected const string Title = "FrozenShouldBeBeforeUsage";
    protected const string Message = "The frozen attribute should be used before usage";

    protected DiagnosticDescriptor Diagnostic = new DiagnosticDescriptor(DiagnosticId, Title, Message, "Language", DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Message + ".");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Diagnostic);

    public override void Initialize(AnalysisContext context)
    {
        try
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(compilationContext =>
            {
                var symbol = compilationContext.Compilation.GetTypeByMetadataName("AutoFixture.NUnit3.FrozenAttribute");
                if (symbol is null) return;

                compilationContext.RegisterSymbolAction(c => AnalyzeParameterUsage(c, symbol),
                                            SymbolKind.Parameter);
            });
        }
        catch { }
    }

    private void AnalyzeParameterUsage(SymbolAnalysisContext context, INamedTypeSymbol attributeSymbol)
    {
        if (context.Symbol is not IParameterSymbol parameterSymbol
            || parameterSymbol.GetAttribute(attributeSymbol) is not AttributeData attr) return;

        if (parameterSymbol.ContainingSymbol is not IMethodSymbol method
                || method.Parameters.IndexOf(parameterSymbol) < method.Parameters.Length - 1) return;

        context.ReportDiagnostic(Microsoft.CodeAnalysis.Diagnostic.Create(Diagnostic,
                            attr.ApplicationSyntaxReference?.GetSyntax().GetLocation()));
    }
}
