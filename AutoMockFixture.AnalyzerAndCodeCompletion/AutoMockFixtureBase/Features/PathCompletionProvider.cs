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
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
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

            if (tree.IsInNonUserCode(position, cancellationToken) && !tree.IsEntirelyWithinStringOrCharLiteral(position, cancellationToken)) return;

            var token = tree.FindTokenOnLeftOfPosition(position, cancellationToken);
            token = token.GetPreviousTokenIfTouchingWord(position);

            // Note the following will also filter out if there is a more complicated expression such as parenthesis or a combination of expressions such as + or invocation or variable etc.
            var argumentList = token.IsKind(SyntaxKind.CommaToken) ? token.Parent as ArgumentListSyntax : token.IsKind(SyntaxKind.StringLiteralToken) ? token.Parent?.Parent?.Parent as ArgumentListSyntax : null;
            var invocation = argumentList?.Parent as InvocationExpressionSyntax;

            if (invocation?.Expression is not MemberAccessExpressionSyntax m || (!Methods.Contains(m.Name.Identifier.Text) && !AutoMockMethods.Contains(m.Name.Identifier.Text))) return;

            // CAUTION: do not reference directly the assmeblies of the follwoing types as it creeats issues at runtime because it needs the dependencies
            var fixtureTypeSymbol = semanticModel.Compilation.GetTypeSymbol("AutoMockFixture.IAutoMockFixture", "AutoMockFixture");
            var extensionsTypeSymbol = semanticModel.Compilation.GetTypeSymbol("AutoMockFixture.AutoMockFixtureExtensions", "AutoMockFixture");

            if (fixtureTypeSymbol is null && extensionsTypeSymbol is null) return;

            var roslynInfo = new RoslynUtils().GetRoslynInfos(invocation, semanticModel, position, cancellationToken);
            var (invocationOperation, startPosition) = roslynInfo.FirstOrDefault(i => IsValidMethod(i.Item1.TargetMethod, fixtureTypeSymbol, extensionsTypeSymbol));
            if (invocationOperation is null) return;

            var currentArgument = invocationOperation.Arguments.Skip(2).FirstOrDefault();
            if(startPosition + currentArgument.Syntax?.Span.Start < token.SpanStart) return; // We are already by the next arg

            var objArg = invocationOperation.Arguments.Skip(1).First(); // The operation does include the `this` as an argument
            if (startPosition + objArg.Syntax?.Span.Start > token.SpanStart) return; // We are not by the correct arg

            var innerOperation = objArg.Value;
            while (innerOperation is IConversionOperation conversion && conversion?.Operand is not null) innerOperation = conversion.Operand;

            var typeSymbol = innerOperation.Type;
            if (typeSymbol is null) return;

            // Not using .Last() since for .TryGetAutoMock() it is not the last
            var currentValue = currentArgument?.Value.ConstantValue.Value?.ToString()?.Trim();
            var isAutoMock = AutoMockMethods.Contains(invocationOperation.TargetMethod.Name);

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
                            ITypeSymbol t => t,
                            _ => throw new Exception("Should not arrive here"),
                        };

                        var newMembers = new List<Info>();
                        if(current.Symbol is IMethodSymbol method)
                        {
                            newMembers.AddRange(method.Parameters.Where(p => p.RefKind == RefKind.Out)
                                                    .Select(p => new Info { TrackingPath = "->" + p.Name, Symbol = p, RequiresEagerLoading = true }));
                        }

                        newMembers.AddRange(GetMembers(newType, isAutoMock));

                        members = newMembers;
                        path += current.TrackingPath;
                    }
                } while (!candidates.Any() && path != currentValue);
            }

            if (members.Any() || candidates.Any()) context.IsExclusive = true;

            var completions = candidates.Select(c => new Info { TrackingPath = candidateStartPath + c.TrackingPath, Symbol = c.Symbol, RequiresEagerLoading = c.RequiresEagerLoading })
                                .Concat(members.Select(m => new Info { TrackingPath = currentValue + m.TrackingPath, Symbol = m.Symbol, RequiresEagerLoading = m.RequiresEagerLoading }));

            if (completions.Any()) context.IsExclusive = true;

            var isStaticInvocation = (invocationOperation.Syntax as InvocationExpressionSyntax)!.ArgumentList.Arguments.Count == invocationOperation.Arguments.Length;
            var argIndex = isStaticInvocation ? 3 : 2;
            var hasArgument = invocation.ArgumentList.Arguments.Count >= argIndex  && !invocation.ArgumentList.Arguments[argIndex - 1].IsMissing; // TODO... it might still be a `null` expression
            var quotes = !hasArgument ? "\"" : ""; // If it's a string it will put it inside the quotes so no need for extra
            foreach (var completion in completions)
            {

                context.AddItem(SymbolCompletionItem.CreateWithSymbolId(
                            displayText: quotes + completion.TrackingPath + quotes,
                            displayTextSuffix: "",
                            insertionText: null,
                            symbols: ImmutableArray.Create(completion.Symbol),
                            contextPosition: token.IsKind(SyntaxKind.StringLiteralToken) ? token.SpanStart : position,
                            inlineDescription: completion.RequiresEagerLoading ? "Requires Eager Loading or explicit access" : null,
                            rules: CompletionItemRules.Create(enterKeyRule: EnterKeyRule.Never)));
            }
        }
        catch (Exception ex)
        {
           Console.WriteLine(ex.Message);
           return;
        }
    }

    private bool IsValidMethod(IMethodSymbol methodSymbol, ITypeSymbol? fixtureTypeSymbol, ITypeSymbol? extensionsTypeSymbol)
        => methodSymbol.ReceiverType is INamedTypeSymbol classType // Even though it is an extension method it is consider the `this` as the type
                   && ((fixtureTypeSymbol is not null && classType.IsEqualTo(fixtureTypeSymbol))
                                || (extensionsTypeSymbol is not null && classType.IsEqualTo(extensionsTypeSymbol)))
                   && (Methods.Contains(methodSymbol.Name) || AutoMockMethods.Contains(methodSymbol.Name));

    private bool IsSpecial(ITypeSymbol type)
        => type.IsSpecialType() || (type.Name == "String" && type.GetFullName() == "System.String");

    private IEnumerable<Info> GetMembers(ITypeSymbol typeSymbol, bool ignorePrimitive)
    {
        // TODO... add tests
        if (typeSymbol.GetTypeArguments().Any()
            && ((typeSymbol.MetadataName == "AutoMock`1" && typeSymbol.ContainingAssembly.Name == "AutoMockFixture.Moq4")
                || (typeSymbol.MetadataName == "Task`1" && typeSymbol.GetFullName() == "System.Threading.Tasks.Task`1")))
        {
            var inner = typeSymbol.GetTypeArguments().First();
            foreach (var m in GetMembers(inner, ignorePrimitive)) yield return m;
            yield break;
        }

        if (typeSymbol.GetTypeArguments().Any() && typeSymbol.Name == "ValueTuple" && typeSymbol.ContainingNamespace.Name == "System")
        {
            var args = typeSymbol.GetTypeArguments();
            foreach (var arg in args) yield return new Info { TrackingPath = $"({"".PadLeft(args.IndexOf(arg), ',')})", Symbol = arg, RequiresEagerLoading = false };
            yield break;
        }

        if (typeSymbol.IsArrayType())
        {
            var arraySymbol = typeSymbol as IArrayTypeSymbol;
            if (arraySymbol is null) yield break;

            for (int i = 0; i < 3; i++)
            {
                yield return new Info { TrackingPath = $"[{i}]", Symbol = arraySymbol.ElementType, RequiresEagerLoading = false };
            }
            yield break;
        }

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

        Func<IMethodSymbol, string> methodGeneric = m => m switch
            {
                { IsGenericMethod: false } => "",
                { OriginalDefinition: var original } when !original.IsEqualTo(m) => $"<`{m.TypeArguments.Length}>",
                _ => $"<{m.TypeArguments.Select(t => t.Name).Join(",")}>"
            };
        foreach (var memberGroup in singleMethodGroups)
        {
            var method = memberGroup.Value.First().Value.First() as IMethodSymbol;
            if (method is null) continue;
            yield return new Info
            {
                TrackingPath = "." + methodGeneric(method) + memberGroup.Key.EscapeIdentifier(),
                Symbol = method,
                RequiresEagerLoading = true
            };
        }

        foreach (var memberGroup in groupedMethods.Except(singleMethodGroups))
        {
            foreach (var overloadSet in memberGroup.Value)
            {
                if (overloadSet.Value.Count == 1)
                {
                    var method = overloadSet.Value.First() as IMethodSymbol;
                    if (method is null) continue;

                    yield return new Info
                    {
                        TrackingPath = $".{methodGeneric(method) + memberGroup.Key.EscapeIdentifier()}(`{overloadSet.Key})",
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
                        TrackingPath = $".{methodGeneric(method) + memberGroup.Key.EscapeIdentifier()}({method.Parameters.Select(p => p.Type.Name).Join(",")})",
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
                    TrackingPath = "..ctor->" + p.Name.EscapeIdentifier(),
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
        [MustInitialize] public string TrackingPath { get; set; }
        [MustInitialize] public ISymbol Symbol { get; set; }
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
        "GetAt",
        "GetSingleAt",
    };
    private static string[] AutoMockMethods =
    {
        "GetAutoMock",
        "TryGetAutoMock",
    };


}
