extern alias Features;
extern alias Workspaces;

using DotNetPowerExtensions.RoslynExtensions;
using Features::Microsoft.CodeAnalysis.Completion;
using Features::Microsoft.CodeAnalysis.Completion.Providers;
using Features::Microsoft.CodeAnalysis.Features.RQName.Nodes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.Text;
using Roslyn.Utilities;
using SequelPay.DotNetPowerExtensions;
using SequelPay.DotNetPowerExtensions.RoslynExtensions;
using System.Collections.Immutable;
using Workspaces::Microsoft.CodeAnalysis.Host;
using Workspaces::Microsoft.CodeAnalysis.Options;
using Workspaces::Microsoft.CodeAnalysis.Shared.Extensions;

namespace AutoMockFixture.AnalyzerAndCodeCompletion.AutoMockFixtureBase.Features;

[ExportCompletionProvider(nameof(PathCompletionProvider), LanguageNames.CSharp)]
public class PathCompletionProvider : CommonCompletionProvider
{
    public override string Language => LanguageNames.CSharp;

    public override bool ShouldTriggerCompletion(LanguageServices languageServices, SourceText text, int caretPosition, CompletionTrigger trigger, CompletionOptions options, OptionSet passThroughOptions)
    {
        return true;
    }

    public override async Task ProvideCompletionsAsync(CompletionContext context)
    {
        try
        {
            var document = context.Document;
            var position = context.Position;
            var cancellationToken = context.CancellationToken;

            var semanticModel = await document.ReuseExistingSpeculativeModelAsync(position, cancellationToken).ConfigureAwait(false);

            var tree = semanticModel.SyntaxTree;

            if (tree.IsInNonUserCode(position, cancellationToken)) return;

            var token = tree.FindTokenOnLeftOfPosition(position, cancellationToken);
            token = token.GetPreviousTokenIfTouchingWord(position);

            // Note the following will also filter out if there is a more complicated expression such as parenthesis or a combination of expressions such as + or invocation or variable etc.
            var argumentList = token.IsKind(SyntaxKind.CommaToken) ? token.Parent as ArgumentListSyntax : token.IsKind(SyntaxKind.StringLiteralToken) ? token.Parent?.Parent?.Parent as ArgumentListSyntax : null;
            var invocation = argumentList?.Parent as InvocationExpressionSyntax;

            if (invocation?.Expression is not MemberAccessExpressionSyntax m || (!Methods.Contains(m.Name.Identifier.Text) && !AutoMockMethods.Contains(m.Name.Identifier.Text))) return;

            // TODO... should we also support calling it directly as a static method
            var fixtureTypeSymbol = semanticModel.Compilation.GetTypeSymbol(typeof(IAutoMockFixture));
            var extensionsTypeSymbol = semanticModel.Compilation.GetTypeSymbol(typeof(AutoMockFixtureExtensions));

            if (fixtureTypeSymbol is null
                   || semanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol is not IMethodSymbol methodSymbol
                   || methodSymbol.ReceiverType is not INamedTypeSymbol classType // Even though it is an extension method it is consider the `this` as the type
                   || (!classType.IsEqualTo(fixtureTypeSymbol) && !classType.IsEqualTo(extensionsTypeSymbol))
                   || (!Methods.Contains(methodSymbol.Name) && !AutoMockMethods.Contains(methodSymbol.Name))
                   || semanticModel.GetOperation(invocation, cancellationToken) is not IInvocationOperation invocationOperation) return;

            var objArg = invocationOperation.Arguments.Skip(1).First(); // The operation does include the `this` as an argument
            var innerOperation = objArg.Value;
            while (innerOperation is IConversionOperation conversion && conversion?.Operand is not null) innerOperation = conversion.Operand;

            var typeSymbol = innerOperation.Type;
            if(typeSymbol is null) return;

            // Not using .Last() since for .TryGetAutoMock() it is not the last
            var currentValue = invocationOperation.Arguments.Skip(2).First().Value.ConstantValue.Value?.ToString()?.Trim();
            var isAutoMock = AutoMockMethods.Contains(methodSymbol.Name);

            var members = GetMembers(typeSymbol, isAutoMock);
            var candidateStartPath = "";
            var candidates = new List<Info>();
            if (currentValue.HasValue())
            {
                var path = "";
                do
                {
                    candidates.AddRange(members.Where(m => path + m.TrackingPath != currentValue && (path + m.TrackingPath).StartsWith(currentValue)));

                    // TODO we need to handle arrays and tuples and tasks and delegates
                    var current = members.FirstOrDefault(m => path + m.TrackingPath == currentValue || currentValue.StartsWith(path + m.TrackingPath + ".") || currentValue.StartsWith(path + m.TrackingPath + "->"));
                    if (current is null && !candidates.Any()) return;

                    if (candidates.Any()) candidateStartPath = path;
                    if (current is not null)
                    {
                        var newType = current.Symbol switch
                        {
                            IMethodSymbol me => me.ReturnType, //TODO... we need to handle out parameters
                            IParameterSymbol p => p.Type,
                            IPropertySymbol prop => prop.Type,
                            IFieldSymbol f => f.Type,
                            _ => throw new Exception("Should not arrive here"),
                        };

                        members = GetMembers(newType, isAutoMock);
                        path += current.TrackingPath;
                    }
                } while (!candidates.Any() && path != currentValue);
            }

            if (members.Any() || candidates.Any()) context.IsExclusive = true;


            foreach (var member in candidates)
            {
                context.AddItem(SymbolCompletionItem.CreateWithSymbolId(
                            displayText: candidateStartPath + member.TrackingPath,
                            displayTextSuffix: "",
                            insertionText: '"' + candidateStartPath + member.TrackingPath + '+',
                            symbols: ImmutableArray.Create(member.Symbol),
                            contextPosition: position,
                            inlineDescription: member.RequiresEagerLoading ? "Requires Eager Loading or explicit access" : null,
                            rules: CompletionItemRules.Create(enterKeyRule: EnterKeyRule.Never)));
            }

            foreach (var member in members)
            {
                context.AddItem(SymbolCompletionItem.CreateWithSymbolId(
                            displayText: currentValue + member.TrackingPath,
                            displayTextSuffix: "",
                            insertionText: '"' + currentValue + member.TrackingPath + '+',
                            symbols: ImmutableArray.Create(member.Symbol),
                            contextPosition: position,
                            inlineDescription: member.RequiresEagerLoading ? "Requires Eager Loading or explicit access" : null,
                            rules: CompletionItemRules.Create(enterKeyRule: EnterKeyRule.Never)));
            }
        }
        catch (Exception ex)
        {
            Logger.LogInfo(ex.Message);
            return;
        }
    }

    private bool IsSpecial(ITypeSymbol type)
        => type.IsSpecialType() || (type.Name == "String" && type.GetFullName() == "System.String");

    private IEnumerable<Info> GetMembers(ITypeSymbol typeSymbol, bool ignorePrimitive)
    {
        var members = typeSymbol.GetMembers().Where(me => !me.IsStatic && me.DeclaredAccessibility != Accessibility.Private);

        var props = members.OfType<IPropertySymbol>().Where(p => p.GetMethod is not null && (p.SetMethod is not null || p.IsVirtual));

        if (ignorePrimitive)
                props = props.Where(p => !IsSpecial(p.Type));

        foreach (var prop in props)
        {
            yield return new Info { TrackingPath = "." + prop.Name.EscapeIdentifier(), Symbol = prop, RequiresEagerLoading = prop.SetMethod is null };
        }

        var methods = members.OfType<IMethodSymbol>().Where(method => method.IsVirtual
                                                                && (!method.ReturnsVoid || method.Parameters.Any(p => p.RefKind == RefKind.Out))
                                                                && !MethodKindsToSkip.Contains(method.MethodKind)).ToArray();
        if (ignorePrimitive)
                methods = methods.Where(m => !IsSpecial(m.ReturnType) || m.Parameters.Any(p => p.RefKind == RefKind.Out && !IsSpecial(p.Type))).ToArray();

        var groupedMethods = methods.GroupBy(m => m.Name).ToDictionary(g => g.Key, g => g.GroupBy(v => v.Parameters.Length).ToDictionary(x => x.Key, x => x.ToList()));

        var singleMethodGroups = groupedMethods.Where(g => g.Value.Count == 1 && g.Value.First().Value.Count == 1).ToArray();
        foreach (var memberGroup in singleMethodGroups)
        {
            // TODO... handle generic
            yield return new Info { TrackingPath = "." + memberGroup.Key.EscapeIdentifier(), Symbol = memberGroup.Value.First().Value.First(),
                                                                                                                            RequiresEagerLoading = true };
        }

        foreach (var memberGroup in groupedMethods.Except(singleMethodGroups))
        {
            foreach (var overloadSet in memberGroup.Value)
            {
                if (overloadSet.Value.Count == 1)
                {
                    yield return new Info
                    {
                        TrackingPath = $".{memberGroup.Key.EscapeIdentifier()}(`{overloadSet.Key})",
                        Symbol = overloadSet.Value.First(),
                        RequiresEagerLoading = true
                    };

                    continue;
                }
                // TODO... handle arguments and generic

                foreach (var method in overloadSet.Value)
                {
                    yield return new Info
                    {
                        TrackingPath = $".{memberGroup.Key.EscapeIdentifier()}({method.Parameters.Select(p => p.Type.Name).Join(",")})",
                        Symbol = method,
                        RequiresEagerLoading = true
                    };
                }
            }
        }

        var ctor = members.OfType<IMethodSymbol>().Where(me => me.MethodKind == MethodKind.Constructor).OrderBy(me => me.Parameters.Length).FirstOrDefault();
        if (ctor?.Parameters.Length > 0)
        {
            foreach (var p in ctor.Parameters)
            {
                if (ignorePrimitive && IsSpecial(p.Type)) continue;

                yield return new Info
                {
                    TrackingPath = "->" + p.Name.EscapeIdentifier(),
                    Symbol = p,
                    RequiresEagerLoading = false
                };
            }
        }


        foreach (var member in members.OfType<IFieldSymbol>())
        {
            if (ignorePrimitive && IsSpecial(member.Type)) continue;

            yield return new Info
            {
                TrackingPath = "." + member.Name.EscapeIdentifier(),
                Symbol = member,
                RequiresEagerLoading = false
            };
        }
    }

    private class Info
    {
        public string TrackingPath { get; set; }
        public ISymbol Symbol { get; set; }
        public bool RequiresEagerLoading { get; set; }
    }

    private static MethodKind[] MethodKindsToSkip =
    {
        MethodKind.Constructor,
        MethodKind.Destructor,
        MethodKind.PropertyGet,
        MethodKind.PropertySet,
        MethodKind.EventAdd,
        MethodKind.EventRemove,
    };

    private static string[] Methods =
    {
        nameof(AutoMockFixtureExtensions.GetAt),
        nameof(AutoMockFixtureExtensions.GetSingleAt),
    };
    private static string[] AutoMockMethods =
    {
        nameof(AutoMockFixtureExtensions.GetAutoMock),
        nameof(AutoMockFixtureExtensions.TryGetAutoMock),
    };


}
