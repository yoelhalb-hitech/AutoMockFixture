using AutoMockFixture.AnalyzerAndCodeCompletion.AutoMockFixtureBase.Features;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using System.Reflection;

namespace AutoMockFixture.AnalyzerAndCodeCompletion.Test.AutoMockFixtureBase.Features;

internal class PathCompletionProvider_Tests
{

    private static AdhocWorkspace GetWorkspace()
    {
        var host = MefHostServices.Create(MefHostServices.DefaultAssemblies);
        var workspace = new AdhocWorkspace(host);

        var loader = workspace.Services.GetRequiredService<IAnalyzerService>().GetLoader();
        var analyzerPath = typeof(PathCompletionProvider).Assembly.Location!;

        var projectInfo = ProjectInfo.Create(ProjectId.CreateNewId(), VersionStamp.Create(), "MyProject", "MyProject", LanguageNames.CSharp).
           WithMetadataReferences(new[]
           {
               MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
               MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location), "System.Collections.dll")),
               MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location),
               MetadataReference.CreateFromFile(Assembly.Load("netstandard").Location),
               MetadataReference.CreateFromFile(typeof(AutoMockFixture.AutoMockFixtureExtensions).Assembly.Location),
               MetadataReference.CreateFromFile(typeof(AutoMockFixture.Moq4.UnitFixture).Assembly.Location),
               MetadataReference.CreateFromFile(typeof(AutoMockFixture.Moq4.UnitFixture).BaseType.BaseType.BaseType.Assembly.Location)
           })
           .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
           .WithAnalyzerReferences(new AnalyzerReference[]
           {
               new AnalyzerFileReference(analyzerPath, loader),
               new AnalyzerFileReference(Path.Combine(Path.GetDirectoryName(analyzerPath)!, "DotNetPowerExtensions.RoslynExtensions.dll"), loader),
               new AnalyzerFileReference(Path.Combine(Path.GetDirectoryName(analyzerPath)!, "AutoMockFixture.dll"), loader)
           });

        var project = workspace.CurrentSolution.AddProject(projectInfo);
        workspace.TryApplyChanges(project);

        return workspace;
    }

    public static Document GetInitializedDocument(string code)
    {
        using var workspace = GetWorkspace();

        var document = workspace.AddDocument(workspace.CurrentSolution.Projects.First().Id, "MyFile.cs", SourceText.From(code));

        var completionService = CompletionService.GetService(document)!;

        // We need to do it once first to register it
        _ = completionService.GetCompletionsAsync(document, 0, CompletionTrigger.CreateInsertionTrigger(' ')).Result;

        return document;
    }

    private static async Task<CompletionList> GetCompletionList(string call, string arg, string trailing = "")
    {
        var code = $$"""
        using AutoMockFixture;
        using AutoMockFixture.Moq4;
        public class TestClass
        {
            public TestClass(int firstArg, string secondArg){}
            public TestClass(int firstArg, string secondArg, double thirdArg){}
            public TestClass TestProp { get; set; }
            public virtual string TestPropGetVirtual { get; }
            public virtual string TestPropSetVirtual { set; }
            public string TestPropGetNonVirtual { get; }
            public TestClass TestField;
            public virtual TestClass TestNonVoidVirtualMethod(){}
            public virtual int MethodWithDifferentArgs(int i){}
            public virtual int MethodWithDifferentArgs(int i, string s){}
            public virtual int MethodWithSameArgs(int i, string s){}
            public virtual int MethodWithSameArgs(string s, int i){}
            public int TestNonVoidNonVirtualMethod(){}
            public virtual void TestVoidVirtualMethod(){}
        	public static void Test()
        	{
                var fixture = new UnitFixture();
        		var result = {{call}}new TestClass{},"{{arg}}"{{trailing}});
        	}
        }
        """;

        var document = GetInitializedDocument(code);

        var position = code.LastIndexOf("new TestClass{},", StringComparison.Ordinal) + "new TestClass{},".Length;

        var text = await document.GetTextAsync().ConfigureAwait(false);
        var insertionTrigger = CompletionTrigger.CreateInsertionTrigger(text[position]);

        var completionService = CompletionService.GetService(document)!;
        return await completionService.GetCompletionsAsync(document, position, insertionTrigger).ConfigureAwait(false);
    }

    [Test]
    [TestCase("fixture.GetAt(")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetAt(fixture,")]
    [TestCase("fixture.GetSingleAt(")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetSingleAt(fixture,")]
    public async Task Test(string call)
    {
        var results = await GetCompletionList(call, "").ConfigureAwait(false);

        results.ItemsList.Should().NotBeNullOrEmpty();

        var expected = new[]
        {
            "->firstArg",
            "->secondArg",
            ".TestProp",
            ".TestPropGetVirtual",
            ".TestNonVoidVirtualMethod",
            ".MethodWithDifferentArgs(`1)",
            ".MethodWithDifferentArgs(`2)",
            ".MethodWithSameArgs(Int32,String)",
            ".MethodWithSameArgs(String,Int32)",
            ".TestField",
        };
        results.ItemsList.Count.Should().Be(expected.Length);
        results.ItemsList.Select(i => i.DisplayText).Should().BeEquivalentTo(expected);
    }

    [Test]
    [TestCase("fixture.GetAutoMock(", "")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetAutoMock(fixture,", "")]
    [TestCase("fixture.TryGetAutoMock(", ", out _")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.TryGetAutoMock(fixture,", ", out _")]
    public async Task Test_AutoMock(string call, string trailing)
    {
        var results = await GetCompletionList(call, "", trailing).ConfigureAwait(false);

        results.ItemsList.Should().NotBeNullOrEmpty();

        var expected = new[]
        {
            ".TestProp",
            ".TestNonVoidVirtualMethod",
            ".TestField",
        };
        results.ItemsList.Count.Should().Be(expected.Length);
        results.ItemsList.Select(i => i.DisplayText).Should().BeEquivalentTo(expected);
    }

    [Test]
    [TestCase("fixture.GetAt(")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetAt(fixture,")]
    [TestCase("fixture.GetSingleAt(")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetSingleAt(fixture,")]
    public async Task TestInner(string call)
    {
        var results = await GetCompletionList(call, ".TestProp").ConfigureAwait(false);

        results.ItemsList.Should().NotBeNullOrEmpty();

        var expected = new[]
        {
            ".TestPropGetVirtual", // Possible completion
            ".TestProp->firstArg",
            ".TestProp->secondArg",
            ".TestProp.TestProp",
            ".TestProp.TestPropGetVirtual",
            ".TestProp.TestNonVoidVirtualMethod",
            ".TestProp.MethodWithDifferentArgs(`1)",
            ".TestProp.MethodWithDifferentArgs(`2)",
            ".TestProp.MethodWithSameArgs(Int32,String)",
            ".TestProp.MethodWithSameArgs(String,Int32)",
            ".TestProp.TestField",
        };
        results.ItemsList.Count.Should().Be(expected.Length);
        results.ItemsList.Select(i => i.DisplayText).Should().BeEquivalentTo(expected);
    }

    [Test]
    [TestCase("fixture.GetAutoMock(", "")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetAutoMock(fixture,", "")]
    [TestCase("fixture.TryGetAutoMock(", ", out _")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.TryGetAutoMock(fixture,", ", out _")]
    public async Task Test_AutoMock_Inner(string call, string trailing)
    {
        var results = await GetCompletionList(call, ".TestProp", trailing).ConfigureAwait(false);

        results.ItemsList.Should().NotBeNullOrEmpty();

        var expected = new[]
        {
            ".TestProp.TestProp",
            ".TestProp.TestNonVoidVirtualMethod",
            ".TestProp.TestField",
        };
        results.ItemsList.Count.Should().Be(expected.Length);
        results.ItemsList.Select(i => i.DisplayText).Should().BeEquivalentTo(expected);
    }

    [Test]
    [TestCase("fixture.GetAt(")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetAt(fixture,")]
    [TestCase("fixture.GetSingleAt(")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetSingleAt(fixture,")]
    public async Task TestInnerInner(string call)
    {
        var results = await GetCompletionList(call, ".TestProp.TestProp").ConfigureAwait(false);
        results.ItemsList.Should().NotBeNullOrEmpty();

        var expected = new[]
        {
            ".TestProp.TestPropGetVirtual", // Possible completion
            ".TestProp.TestProp->firstArg",
            ".TestProp.TestProp->secondArg",
            ".TestProp.TestProp.TestProp",
            ".TestProp.TestProp.TestPropGetVirtual",
            ".TestProp.TestProp.TestNonVoidVirtualMethod",
            ".TestProp.TestProp.MethodWithDifferentArgs(`1)",
            ".TestProp.TestProp.MethodWithDifferentArgs(`2)",
            ".TestProp.TestProp.MethodWithSameArgs(Int32,String)",
            ".TestProp.TestProp.MethodWithSameArgs(String,Int32)",
            ".TestProp.TestProp.TestField",
        };
        results.ItemsList.Count.Should().Be(expected.Length);
        results.ItemsList.Select(i => i.DisplayText).Should().BeEquivalentTo(expected);
    }

    [Test]
    [TestCase("fixture.GetAutoMock(", "")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetAutoMock(fixture,", "")]
    [TestCase("fixture.TryGetAutoMock(", ", out _")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.TryGetAutoMock(fixture,", ", out _")]
    public async Task Test_AutoMock_InnerInner(string call, string trailing)
    {
        var results = await GetCompletionList(call, ".TestProp.TestProp", trailing).ConfigureAwait(false);

        results.ItemsList.Should().NotBeNullOrEmpty();

        var expected = new[]
        {
            ".TestProp.TestProp.TestProp",
            ".TestProp.TestProp.TestNonVoidVirtualMethod",
            ".TestProp.TestProp.TestField",
        };
        results.ItemsList.Count.Should().Be(expected.Length);
        results.ItemsList.Select(i => i.DisplayText).Should().BeEquivalentTo(expected);
    }
}
