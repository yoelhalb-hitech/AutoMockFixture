using AutoMockFixture.AnalyzerAndCodeCompletion.AutoMockFixtureBase.Features;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
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
               new AnalyzerFileReference(Path.Combine(Path.GetDirectoryName(analyzerPath)!, "AutoMockFixture.dll"), loader),
               new AnalyzerFileReference(Path.Combine(Path.GetDirectoryName(analyzerPath)!, "AutoMockFixture.Moq4.dll"), loader)
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

    private static async Task<CompletionList> GetCompletionList(string call, string arg, string objectArg, string trailing = "")
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
        		var result = {{call}}{{objectArg}},"{{arg}}"{{trailing}});
        	}
        }
        """;

        var document = GetInitializedDocument(code);

        var position = code.LastIndexOf(objectArg + ",", StringComparison.Ordinal) + (objectArg + ",").Length;

        var text = await document.GetTextAsync().ConfigureAwait(false);
        var insertionTrigger = CompletionTrigger.CreateInsertionTrigger(text[position]);

        var completionService = CompletionService.GetService(document)!;
        return await completionService.GetCompletionsAsync(document, position, insertionTrigger).ConfigureAwait(false);
    }

    [Test]
    [TestCase("fixture.GetAt(", "new TestClass{}")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetAt(fixture,", "new TestClass{}")]
    [TestCase("fixture.GetSingleAt(", "new TestClass{}")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetSingleAt(fixture,", "new TestClass{}")]
    [TestCase("fixture.GetAt(", "new AutoMock<TestClass>()")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetAt(fixture,", "new AutoMock<TestClass>()")]
    [TestCase("fixture.GetSingleAt(", "new AutoMock<TestClass>()")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetSingleAt(fixture,", "new AutoMock<TestClass>()")]
    public async Task Test(string call, string objectArg)
    {
        var results = await GetCompletionList(call, "", objectArg).ConfigureAwait(false);

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
    [TestCase("fixture.GetAutoMock(", "new TestClass{}", "")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetAutoMock(fixture,", "new TestClass{}", "")]
    [TestCase("fixture.TryGetAutoMock(", "new TestClass{}", ", out _")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.TryGetAutoMock(fixture,", "new TestClass{}", ", out _")]
    [TestCase("fixture.GetAutoMock(", "new AutoMock<TestClass>()", "")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetAutoMock(fixture,", "new AutoMock<TestClass>()", "")]
    [TestCase("fixture.TryGetAutoMock(", "new AutoMock<TestClass>()", ", out _")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.TryGetAutoMock(fixture,", "new AutoMock<TestClass>()", ", out _")]
    public async Task Test_AutoMock(string call, string objectArg, string trailing)
    {
        var results = await GetCompletionList(call, "", objectArg, trailing).ConfigureAwait(false);

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
    [TestCase("fixture.GetAt(", "new TestClass{}")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetAt(fixture,", "new TestClass{}")]
    [TestCase("fixture.GetSingleAt(", "new TestClass{}")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetSingleAt(fixture,", "new TestClass{}")]
    [TestCase("fixture.GetAt(", "new AutoMock<TestClass>()")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetAt(fixture,", "new AutoMock<TestClass>()")]
    [TestCase("fixture.GetSingleAt(", "new AutoMock<TestClass>()")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetSingleAt(fixture,", "new AutoMock<TestClass>()")]
    public async Task TestInner(string call, string objectArg)
    {
        var results = await GetCompletionList(call, ".TestProp", objectArg).ConfigureAwait(false);

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
    [TestCase("fixture.GetAutoMock(", "new TestClass{}", "")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetAutoMock(fixture,", "new TestClass{}", "")]
    [TestCase("fixture.TryGetAutoMock(", "new TestClass{}", ", out _")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.TryGetAutoMock(fixture,", "new TestClass{}", ", out _")]
    [TestCase("fixture.GetAutoMock(", "new AutoMock<TestClass>()", "")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetAutoMock(fixture,", "new AutoMock<TestClass>()", "")]
    [TestCase("fixture.TryGetAutoMock(", "new AutoMock<TestClass>()", ", out _")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.TryGetAutoMock(fixture,", "new AutoMock<TestClass>()", ", out _")]
    public async Task Test_AutoMock_Inner(string call, string objectArg, string trailing)
    {
        var results = await GetCompletionList(call, ".TestProp", objectArg, trailing).ConfigureAwait(false);

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
    [TestCase("fixture.GetAt(", "new TestClass{}")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetAt(fixture,", "new TestClass{}")]
    [TestCase("fixture.GetSingleAt(", "new TestClass{}")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetSingleAt(fixture,", "new TestClass{}")]
    [TestCase("fixture.GetAt(", "new AutoMock<TestClass>()")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetAt(fixture,", "new AutoMock<TestClass>()")]
    [TestCase("fixture.GetSingleAt(", "new AutoMock<TestClass>()")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetSingleAt(fixture,", "new AutoMock<TestClass>()")]
    public async Task TestInnerInner(string call, string objectArg)
    {
        var results = await GetCompletionList(call, ".TestProp.TestProp", objectArg).ConfigureAwait(false);
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
    [TestCase("fixture.GetAutoMock(", "new TestClass{}", "")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetAutoMock(fixture,", "new TestClass{}", "")]
    [TestCase("fixture.TryGetAutoMock(", "new TestClass{}", ", out _")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.TryGetAutoMock(fixture,", "new TestClass{}", ", out _")]
    [TestCase("fixture.GetAutoMock(", "new AutoMock<TestClass>()", "")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetAutoMock(fixture,", "new AutoMock<TestClass>()", "")]
    [TestCase("fixture.TryGetAutoMock(", "new AutoMock<TestClass>()", ", out _")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.TryGetAutoMock(fixture,", "new AutoMock<TestClass>()", ", out _")]
    public async Task Test_AutoMock_InnerInner(string call, string objectArg, string trailing)
    {
        var results = await GetCompletionList(call, ".TestProp.TestProp", objectArg, trailing).ConfigureAwait(false);

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
