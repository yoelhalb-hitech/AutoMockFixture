extern alias Features;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Diagnostics;

namespace AutoMockFixture.AnalyzerAndCodeCompletion.AutoMockSetups.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AutoMockSetupShouldNotUseDefaultValue : DiagnosticAnalyzer
{
    public const string DiagnosticId = "AMF0003";
    protected const string Title = "AutoMockSetupShouldNotUseDefaultValue";
    protected const string Message = "Do not use default values (such as null or 0) for parameters in `.Setup()`, for matching all values use the `default` keyword, while for matching only the default value use `Is.DefaultValue<T>()` (or in case of bool you can use `Is.False()`)";

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
                var symbol = compilationContext.Compilation.GetTypeByMetadataName("AutoMockFixture.Moq4.AutoMock`1");
                if (symbol is null) return;

                compilationContext.RegisterSyntaxNodeAction(c => AnalyzeParameterUsage(c, symbol),
                            SyntaxKind.Argument);

                compilationContext.RegisterOperationAction(c => { Debugger.Break(); }, OperationKind.Invocation);
            });
        }
        catch { }
    }



    private void AnalyzeParameterUsage(SyntaxNodeAnalysisContext context, INamedTypeSymbol autoMockSymbol)
    {

        //compilationContext.RegisterSyntaxNodeAction(c => AnalyzeParameterUsage(c, symbol),
        //                    SyntaxKind.NullLiteralExpression,
        //                    SyntaxKind.NumericLiteralExpression,
        //                    SyntaxKind.FalseLiteralExpression,
        //                    SyntaxKind.SimpleMemberAccessExpression);

        if (context.Node is not ArgumentSyntax arg || !IsInvalidValue(arg.Expression)) return;

        if (arg.Ancestors().OfType<InvocationExpressionSyntax>().FirstOrDefault() is not InvocationExpressionSyntax invocation
                || invocation.Ancestors().OfType<InvocationExpressionSyntax>().FirstOrDefault() is not InvocationExpressionSyntax firstInvocation
                || context.SemanticModel.GetSymbolInfo(firstInvocation).Symbol is not IMethodSymbol method
                || method.Name != "Setup"
                || method.ReceiverType is not INamedTypeSymbol namedType
                || !SymbolEqualityComparer.Default.Equals(namedType.ConstructedFrom, autoMockSymbol)) return;

            context.ReportDiagnostic(Microsoft.CodeAnalysis.Diagnostic.Create(Diagnostic,
                                arg.GetLocation()));
    }

    private bool IsInvalidValue(ExpressionSyntax expr)
        => expr switch
        {
            ParenthesizedExpressionSyntax parenthesized => IsInvalidValue(UnwrapParenthesized(parenthesized)),
            CastExpressionSyntax cast => IsInvalidValue(cast.Expression),
            BinaryExpressionSyntax binary when binary.Kind() is SyntaxKind.AsExpression => IsInvalidValue(binary.Left),
            LiteralExpressionSyntax literal when literal.Kind() is SyntaxKind.NullLiteralExpression
                || literal.Kind() is SyntaxKind.FalseLiteralExpression
                || (literal.Kind() is SyntaxKind.NumericLiteralExpression && Convert.ToDecimal(literal.Token.Value!) == 0m) => true,
            // TODO... for MinValue and Zero we should check the symbol if it is the correct one
            MemberAccessExpressionSyntax memberAccess when Names.Contains(memberAccess.ToString()) => true,
            _ => false,
        };

    private static SyntaxKind[] SupportedLiteralKinds = new[]
    {
        SyntaxKind.NullLiteralExpression,
        SyntaxKind.NumericLiteralExpression,
        SyntaxKind.FalseLiteralExpression
    };

    private string[] Names =
    [
        "MinValue",
        "Zero",
        "DateTime.MinValue",
        "DateOnly.MinValue",
        "TimeOnly.MinValue",
        "TimeSpan.Zero",
        "System.DateTime.MinValue",
        "System.DateOnly.MinValue",
        "System.TimeOnly.MinValue",
        "System.TimeSpan.Zero"
    ];

    private ExpressionSyntax UnwrapParenthesized(ParenthesizedExpressionSyntax parenthesized)
            => parenthesized.Expression switch
            {
                ParenthesizedExpressionSyntax innerParenthesized => UnwrapParenthesized(innerParenthesized),
                _ => parenthesized.Expression
            };
}
