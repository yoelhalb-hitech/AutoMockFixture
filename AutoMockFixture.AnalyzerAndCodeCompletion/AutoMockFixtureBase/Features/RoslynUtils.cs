extern alias Workspaces;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Workspaces::Microsoft.CodeAnalysis.Shared.Extensions;

namespace AutoMockFixture.AnalyzerAndCodeCompletion.AutoMockFixtureBase.Features;

internal class RoslynUtils
{
    public IEnumerable<(IInvocationOperation, int)> GetRoslynInfos(InvocationExpressionSyntax invocation, SemanticModel semanticModel, int position, CancellationToken cancellationToken = default)
    {
        var methodSymbols = GetMethodSymbols(invocation, semanticModel, position, cancellationToken);

        foreach (var methodSymbol in methodSymbols)
        {
            var invocationOperation = semanticModel.GetOperation(invocation, cancellationToken) as IInvocationOperation;
            if (invocationOperation is not null)
            {
                yield return (invocationOperation, 0);
                continue;
            }

            var speculativeInvocation = GetSpeculativeInvocation(invocation, methodSymbol);
            if(speculativeInvocation is null) continue;

            var expr = SyntaxFactory.ExpressionStatement(speculativeInvocation);

            if (!semanticModel.TryGetSpeculativeSemanticModel(position, expr, out var speculative)) continue;
            var speculativeOperation = speculative.GetOperation(expr, cancellationToken)?.ChildOperations.FirstOrDefault() as IInvocationOperation;

            if(speculativeOperation is not null) yield return (speculativeOperation, invocation.SpanStart);
        }
    }

    public InvocationExpressionSyntax? GetSpeculativeInvocation(InvocationExpressionSyntax invocation, IMethodSymbol methodSymbol)
    {

        if (methodSymbol.GetParameters().Length == invocation.ArgumentList.Arguments.Count && !invocation.ArgumentList.Arguments.Any(a => a.IsMissing)) return null;

        var fixedArguments = methodSymbol.GetParameters().Select((p, i) =>
        {
            if(invocation.ArgumentList.Arguments.Count > i && !invocation.ArgumentList.Arguments[i].IsMissing) return invocation.ArgumentList.Arguments[i];

            //*********************************** TODO...
            // We might make it more general and put it in DotNetPowerExtensions.RoslynExtensions, but then we will also have to handle 1) all types and 2) the case of ref
            // For in parameters for all types we can just do a null casted to the specific type (or to a nullable of it for non nullable type)
            // For out we can just do `out _` and we don't need a type
            // But for ref we have to add an extra statement of variable decleration of the correct type (or nullable of it) and initilaize with null, not ewe have to make sure there is no name clash...
            // Also note that for type names we have to use the C# type name, maybe better the full name (since there is no guarantee that the `using` statement for the namespace has been set)
            //***********************************

            if (p.Type.SpecialType == SpecialType.System_String) return SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression,
                                                                                                                            SyntaxFactory.ParseToken("\"\"")));
            if (p.RefKind == RefKind.Out) return SyntaxFactory.Argument(null, SyntaxFactory.Token(SyntaxKind.OutKeyword),
                    SyntaxFactory.DeclarationExpression(SyntaxFactory.IdentifierName(p.Type.Name),
                                SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Token(SyntaxKind.IdentifierToken))));
            return null;
        });
        if (fixedArguments.Contains(null)) return null;

        return invocation.WithArgumentList(SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(fixedArguments.OfType<ArgumentSyntax>())));
    }

    public IEnumerable<IMethodSymbol> GetMethodSymbols(InvocationExpressionSyntax invocation, SemanticModel semanticModel, int position, CancellationToken cancellationToken = default)
    {
        var methodSymbol = semanticModel.GetSymbolInfo(invocation, cancellationToken).Symbol as IMethodSymbol;
        if (methodSymbol is not null) return new[] { methodSymbol };

        return semanticModel.GetSpeculativeSymbolInfo(position, invocation, SpeculativeBindingOption.BindAsExpression)
                                                    .CandidateSymbols
                                                    .OfType<IMethodSymbol>();
    }

}
