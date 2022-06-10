using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace AutoMoqSourceGenerator
{
    class Info
    {
        public string Namespace { get; set; }
        public string Name { get; set; }
        public Info BaseClass { get; set; }
        public Dictionary<Type, string> Properties { get; set; }
    }

    [Generator]
    internal class GenerateRequestProcessors : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            // Find the main method
            var mainMethod = context.Compilation.GetEntryPoint(context.CancellationToken);

            var outRequest = new Info
            {
                Namespace = "AutoMoqExtensions.FixtureUtils.Requests",
                Name = "OutParameterRequest",
                Properties = new Dictionary<Type, string>
                {
                    [typeof(Type)] = "DeclaringType",
                    [typeof(MethodInfo)] = "MethodInfo",
                    [typeof(ParameterInfo)] = "ParameterInfo",
                    [typeof(Type)] = "ParameterType",
                }
            };
            var infos = new[]
            {
                outRequest,
                new Info
                {
                    Namespace = "AutoMoqExtensions.FixtureUtils.Requests",
                    Name = "AutoMockOutParameterRequest",
                    BaseClass = outRequest,
                    Properties = new Dictionary<Type, string>{}

                },
            };


            // Build up the source code
            foreach (var info in infos)
            {
                //var typeName = mainMethod.ContainingType.Name;
                var source = info.BaseClass is null ? GetBaseRequest(info) : GetSubRequest(info);
                // Add the source code to the compilation
                context.AddSource($"{info.Namespace}.{info.Name}.g.cs", source);
            }

           
        }

        Func<string, string> fieldName = s => (s[0] + "").ToLower() + s.Substring(1);

        private string GetBaseRequest(Info info)
        {
            return $@" // Auto-generated code
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace {info.Namespace};

internal partial class {info.Name} : IEquatable<{info.Name}>
{{
    public {info.Name}({string.Join(", ", info.Properties.Select(p => $"{p.Key.Name} {fieldName(p.Value)}"))})
    {{
        {string.Join("        ", info.Properties.Select(p => $"{p.Value} = {fieldName(p.Value)};\r\n"))}
    }}

    {string.Join("    ", info.Properties.Select(p => $"public {p.Key.Name} {p.Value} {{ get; }}\r\n"))}
    
    public override bool Equals(object obj)
        => obj is {info.Name} other && this.Equals(other);

    public override int GetHashCode() => HashCode.Combine({string.Join(", ", info.Properties.Select(p => p.Value))});

    public virtual bool Equals({info.Name} other)
        => other is not null &&
            {string.Join("\r\n            && ", info.Properties.Select(p => $"other.{p.Value} == {p.Value}"))};       
}}
";
        }

        private string GetSubRequest(Info info)
        {
            return $@" // Auto-generated code
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace {info.Namespace};

internal partial class {info.Name} : {info.BaseClass.Name}, IEquatable<{info.Name}>
{{
    public {info.Name}({string.Join(", ", info.Properties.Select(p => $"{p.Key.Name} {fieldName(p.Value)}"))})
        : base({string.Join(", ", info.BaseClass.Properties.Select(p => $"{fieldName(p.Value)}"))})
    {{
        {string.Join("        ", info.Properties.Select(p => $"{p.Value} = {fieldName(p.Value)};\r\n"))}
    }}

    {string.Join("    ", info.Properties.Select(p => $"public {p.Key.Name} {p.Value} {{ get; }}\r\n"))}
    
    public override bool Equals({info.BaseClass.Name} obj)
        => obj is {info.Name} other && this.Equals(other);

    public override int GetHashCode() => HashCode.Combine({string.Join(", ", info.Properties.Select(p => p.Value))});

    public virtual bool Equals({info.Name} other)
        => base.Equals(({info.BaseClass.Name})other){(!info.Properties.Any() ? ";" : "")} 
            {string.Join("\r\n            && ", info.Properties.Select(p => $"other.{p.Value} == {p.Value}"))}{(info.Properties.Any() ? "" : ";")}       
}}
";
        }
        public void Initialize(GeneratorInitializationContext context)
        {            
        }
    }
}
