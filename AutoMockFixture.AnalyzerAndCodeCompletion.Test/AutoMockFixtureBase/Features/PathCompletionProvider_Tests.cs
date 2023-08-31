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
               new AnalyzerFileReference(typeof(SequelPay.DotNetPowerExtensions.MustInitializeAttribute).Assembly.Location, loader),
               new AnalyzerFileReference(typeof(SequelPay.DotNetPowerExtensions.RoslynExtensions.SymbolExtensions).Assembly.Location, loader),
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
        using System.Threading.Tasks;
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
            public virtual TestClass MethodWithDifferentArgs(out TestClass i){}
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
    [TestCase("fixture.GetAt(", "new Task<TestClass>()")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetAt(fixture,", "new Task<TestClass>()")]
    [TestCase("fixture.GetSingleAt(", "new Task<TestClass>()")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetSingleAt(fixture,", "new Task<TestClass>()")]
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
    [TestCase("fixture.GetAt(", "new TestClass[]")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetAt(fixture,", "new TestClass[]")]
    [TestCase("fixture.GetSingleAt(", "new TestClass[]")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetSingleAt(fixture,", "new TestClass[]")]
    [TestCase("fixture.GetAt(", "new AutoMock<TestClass>[]")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetAt(fixture,", "new AutoMock<TestClass>[]")]
    [TestCase("fixture.GetSingleAt(", "new AutoMock<TestClass>[]")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetSingleAt(fixture,", "new AutoMock<TestClass>[]")]
    [TestCase("fixture.GetAt(", "new Task<TestClass>[]")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetAt(fixture,", "new Task<TestClass>[]")]
    [TestCase("fixture.GetSingleAt(", "new Task<TestClass>[]")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetSingleAt(fixture,", "new Task<TestClass>[]")]
    public async Task TestArray(string call, string objectArg)
    {
        var results = await GetCompletionList(call, "", objectArg).ConfigureAwait(false);

        results.ItemsList.Should().NotBeNullOrEmpty();

        var expected = new[]
        {
            "[0]",
            "[1]",
            "[2]",
        };
        results.ItemsList.Count.Should().Be(expected.Length);
        results.ItemsList.Select(i => i.DisplayText).Should().BeEquivalentTo(expected);
    }

    [Test]
    [TestCase("fixture.GetAt(", "(new TestClass{},3)")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetAt(fixture,", "(new TestClass{},3)")]
    [TestCase("fixture.GetSingleAt(", "(new TestClass{},3)")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetSingleAt(fixture,", "(new TestClass{},3)")]
    [TestCase("fixture.GetAt(", "(new AutoMock<TestClass>(),3)")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetAt(fixture,", "(new AutoMock<TestClass>(),3)")]
    [TestCase("fixture.GetSingleAt(", "(new AutoMock<TestClass>(),3)")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetSingleAt(fixture,", "(new AutoMock<TestClass>(),3)")]
    [TestCase("fixture.GetAt(", "(new Task<TestClass>(),3)")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetAt(fixture,", "(new Task<TestClass>(),3)")]
    [TestCase("fixture.GetSingleAt(", "(new Task<TestClass>(),3)")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetSingleAt(fixture,", "(new Task<TestClass>(),3)")]
    public async Task TestTuple(string call, string objectArg)
    {
        var results = await GetCompletionList(call, "", objectArg).ConfigureAwait(false);

        results.ItemsList.Should().NotBeNullOrEmpty();

        var expected = new[]
        {
            "()",
            "(,)",
        };
        results.ItemsList.Count.Should().Be(expected.Length);
        results.ItemsList.Select(i => i.DisplayText).Should().BeEquivalentTo(expected);
    }

    [Test]
    [TestCase("fixture.GetAt(", "new TestClass[]")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetAt(fixture,", "new TestClass[]")]
    [TestCase("fixture.GetSingleAt(", "new TestClass[]")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetSingleAt(fixture,", "new TestClass[]")]
    [TestCase("fixture.GetAt(", "new AutoMock<TestClass>[]")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetAt(fixture,", "new AutoMock<TestClass>[]")]
    [TestCase("fixture.GetSingleAt(", "new AutoMock<TestClass>[]")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetSingleAt(fixture,", "new AutoMock<TestClass>[]")]
    [TestCase("fixture.GetAt(", "new Task<TestClass>[]")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetAt(fixture,", "new Task<TestClass>[]")]
    [TestCase("fixture.GetSingleAt(", "new Task<TestClass>[]")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetSingleAt(fixture,", "new Task<TestClass>[]")]
    public async Task TestArrayInner(string call, string objectArg)
    {
        var results = await GetCompletionList(call, "[0]", objectArg).ConfigureAwait(false);

        results.ItemsList.Should().NotBeNullOrEmpty();

        var expected = new[]
        {
            "[0]->firstArg",
            "[0]->secondArg",
            "[0].TestProp",
            "[0].TestPropGetVirtual",
            "[0].TestNonVoidVirtualMethod",
            "[0].MethodWithDifferentArgs(`1)",
            "[0].MethodWithDifferentArgs(`2)",
            "[0].MethodWithSameArgs(Int32,String)",
            "[0].MethodWithSameArgs(String,Int32)",
            "[0].TestField",
        };
        results.ItemsList.Count.Should().Be(expected.Length);
        results.ItemsList.Select(i => i.DisplayText).Should().BeEquivalentTo(expected);
    }

    [Test]
    [TestCase("fixture.GetAt(", "(new TestClass{},3)")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetAt(fixture,", "(new TestClass{},3)")]
    [TestCase("fixture.GetSingleAt(", "(new TestClass{},3)")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetSingleAt(fixture,", "(new TestClass{},3)")]
    [TestCase("fixture.GetAt(", "(new AutoMock<TestClass>(),3)")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetAt(fixture,", "(new AutoMock<TestClass>(),3)")]
    [TestCase("fixture.GetSingleAt(", "(new AutoMock<TestClass>(),3)")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetSingleAt(fixture,", "(new AutoMock<TestClass>(),3)")]
    [TestCase("fixture.GetAt(", "(new Task<TestClass>(),3)")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetAt(fixture,", "(new Task<TestClass>(),3)")]
    [TestCase("fixture.GetSingleAt(", "(new Task<TestClass>(),3)")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetSingleAt(fixture,", "(new Task<TestClass>(),3)")]
    public async Task TestTupleInner(string call, string objectArg)
    {
        var results = await GetCompletionList(call, "()", objectArg).ConfigureAwait(false);

        results.ItemsList.Should().NotBeNullOrEmpty();

        var expected = new[]
        {
            "()->firstArg",
            "()->secondArg",
            "().TestProp",
            "().TestPropGetVirtual",
            "().TestNonVoidVirtualMethod",
            "().MethodWithDifferentArgs(`1)",
            "().MethodWithDifferentArgs(`2)",
            "().MethodWithSameArgs(Int32,String)",
            "().MethodWithSameArgs(String,Int32)",
            "().TestField",
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
    [TestCase("fixture.GetAutoMock(", "new Task<TestClass>()", "")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetAutoMock(fixture,", "new Task<TestClass>()", "")]
    [TestCase("fixture.TryGetAutoMock(", "new Task<TestClass>()", ", out _")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.TryGetAutoMock(fixture,", "new Task<TestClass>()", ", out _")]
    public async Task Test_AutoMock(string call, string objectArg, string trailing)
    {
        var results = await GetCompletionList(call, "", objectArg, trailing).ConfigureAwait(false);

        results.ItemsList.Should().NotBeNullOrEmpty();

        var expected = new[]
        {
            ".TestProp",
            ".TestNonVoidVirtualMethod",
            ".MethodWithDifferentArgs",
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
    [TestCase("fixture.GetAt(", "new Task<TestClass>()")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetAt(fixture,", "new Task<TestClass>()")]
    [TestCase("fixture.GetSingleAt(", "new Task<TestClass>()")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetSingleAt(fixture,", "new Task<TestClass>()")]
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
    [TestCase("fixture.GetAt(", "new TestClass{}")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetAt(fixture,", "new TestClass{}")]
    [TestCase("fixture.GetSingleAt(", "new TestClass{}")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetSingleAt(fixture,", "new TestClass{}")]
    [TestCase("fixture.GetAt(", "new AutoMock<TestClass>()")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetAt(fixture,", "new AutoMock<TestClass>()")]
    [TestCase("fixture.GetSingleAt(", "new AutoMock<TestClass>()")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetSingleAt(fixture,", "new AutoMock<TestClass>()")]
    [TestCase("fixture.GetAt(", "new Task<TestClass>()")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetAt(fixture,", "new Task<TestClass>()")]
    [TestCase("fixture.GetSingleAt(", "new Task<TestClass>()")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetSingleAt(fixture,", "new Task<TestClass>()")]
    public async Task TestInnerMethodWithOutArgs(string call, string objectArg)
    {
        var results = await GetCompletionList(call, ".MethodWithDifferentArgs(`1)", objectArg).ConfigureAwait(false);

        results.ItemsList.Should().NotBeNullOrEmpty();

        var expected = new[]
        {
            ".MethodWithDifferentArgs(`1)->i",
            ".MethodWithDifferentArgs(`1)->firstArg",
            ".MethodWithDifferentArgs(`1)->secondArg",
            ".MethodWithDifferentArgs(`1).TestProp",
            ".MethodWithDifferentArgs(`1).TestPropGetVirtual",
            ".MethodWithDifferentArgs(`1).TestNonVoidVirtualMethod",
            ".MethodWithDifferentArgs(`1).MethodWithDifferentArgs(`1)",
            ".MethodWithDifferentArgs(`1).MethodWithDifferentArgs(`2)",
            ".MethodWithDifferentArgs(`1).MethodWithSameArgs(Int32,String)",
            ".MethodWithDifferentArgs(`1).MethodWithSameArgs(String,Int32)",
            ".MethodWithDifferentArgs(`1).TestField",
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
    [TestCase("fixture.GetAt(", "new Task<TestClass>()")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetAt(fixture,", "new Task<TestClass>()")]
    [TestCase("fixture.GetSingleAt(", "new Task<TestClass>()")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetSingleAt(fixture,", "new Task<TestClass>()")]
    public async Task TestInnerMethodWithOutArgsHandled(string call, string objectArg)
    {
        var results = await GetCompletionList(call, ".MethodWithDifferentArgs(`1)->i", objectArg).ConfigureAwait(false);

        results.ItemsList.Should().NotBeNullOrEmpty();

        var expected = new[]
        {
            ".MethodWithDifferentArgs(`1)->i->firstArg",
            ".MethodWithDifferentArgs(`1)->i->secondArg",
            ".MethodWithDifferentArgs(`1)->i.TestProp",
            ".MethodWithDifferentArgs(`1)->i.TestPropGetVirtual",
            ".MethodWithDifferentArgs(`1)->i.TestNonVoidVirtualMethod",
            ".MethodWithDifferentArgs(`1)->i.MethodWithDifferentArgs(`1)",
            ".MethodWithDifferentArgs(`1)->i.MethodWithDifferentArgs(`2)",
            ".MethodWithDifferentArgs(`1)->i.MethodWithSameArgs(Int32,String)",
            ".MethodWithDifferentArgs(`1)->i.MethodWithSameArgs(String,Int32)",
            ".MethodWithDifferentArgs(`1)->i.TestField",
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
    [TestCase("fixture.GetAutoMock(", "new Task<TestClass>()", "")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetAutoMock(fixture,", "new Task<TestClass>()", "")]
    [TestCase("fixture.TryGetAutoMock(", "new Task<TestClass>()", ", out _")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.TryGetAutoMock(fixture,", "new Task<TestClass>()", ", out _")]
    public async Task Test_AutoMock_Inner(string call, string objectArg, string trailing)
    {
        var results = await GetCompletionList(call, ".TestProp", objectArg, trailing).ConfigureAwait(false);

        results.ItemsList.Should().NotBeNullOrEmpty();

        var expected = new[]
        {
            ".TestProp.TestProp",
            ".TestProp.TestNonVoidVirtualMethod",
            ".TestProp.MethodWithDifferentArgs",
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
    [TestCase("fixture.GetAt(", "new Task<TestClass>()")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetAt(fixture,", "new Task<TestClass>()")]
    [TestCase("fixture.GetSingleAt(", "new Task<TestClass>()")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetSingleAt(fixture,", "new Task<TestClass>()")]
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
    [TestCase("fixture.GetAutoMock(", "new Task<TestClass>()", "")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.GetAutoMock(fixture,", "new Task<TestClass>()", "")]
    [TestCase("fixture.TryGetAutoMock(", "new Task<TestClass>()", ", out _")]
    [TestCase("AutoMockFixture.AutoMockFixtureExtensions.TryGetAutoMock(fixture,", "new Task<TestClass>()", ", out _")]
    public async Task Test_AutoMock_InnerInner(string call, string objectArg, string trailing)
    {
        var results = await GetCompletionList(call, ".TestProp.TestProp", objectArg, trailing).ConfigureAwait(false);

        results.ItemsList.Should().NotBeNullOrEmpty();

        var expected = new[]
        {
            ".TestProp.TestProp.TestProp",
            ".TestProp.TestProp.TestNonVoidVirtualMethod",
            ".TestProp.TestProp.MethodWithDifferentArgs",
            ".TestProp.TestProp.TestField",
        };
        results.ItemsList.Count.Should().Be(expected.Length);
        results.ItemsList.Select(i => i.DisplayText).Should().BeEquivalentTo(expected);
    }
}
