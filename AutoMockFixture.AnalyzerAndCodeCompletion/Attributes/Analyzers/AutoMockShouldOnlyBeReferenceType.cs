extern alias Features;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using SequelPay.DotNetPowerExtensions.RoslynExtensions;
using System.Collections.Immutable;

namespace AutoMockFixture.AnalyzerAndCodeCompletion.Attributes.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AutoMockShouldOnlyBeReferenceType : DiagnosticAnalyzer
{
    public const string DiagnosticId = "AMF0001";
    protected const string Title = "AutoMockShouldOnlyBeReferenceType";
    protected const string Message = "Only use AutoMock attribute on a parameter that is a reference type";

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
                var symbol = compilationContext.Compilation.GetTypeByMetadataName("AutoMockFixture.AutoMockAttribute");
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
         if (!parameterSymbol.Type.IsValueType
                    && (parameterSymbol.Type is not ITypeParameterSymbol typeParameter
                            || typeParameter.HasReferenceTypeConstraint)) return;

        context.ReportDiagnostic(Microsoft.CodeAnalysis.Diagnostic.Create(Diagnostic,
                            attr.ApplicationSyntaxReference?.GetSyntax().GetLocation()));
    }
}
