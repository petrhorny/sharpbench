using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using BenchmarkHost;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace Sharpbench
{
    public class BenchmarkProcessor
    {
        public BenchmarkResponse Benchmark(BenchmarkRequest request)
        {
            var result1 = Benchmark(request.SourceCode1);
            var result2 = Benchmark(request.SourceCode2);

            var response = new BenchmarkResponse();
            response.Success = result1.Success && result2.Success;
            response.Error = result1.Error + result2.Error;

            if (response.Success)
            {
                var maxIterationsPerSecond = Math.Max(result1.IterationsPerSecond, result2.IterationsPerSecond);
                response.Source1Ratio = (float)(result1.IterationsPerSecond / maxIterationsPerSecond);
                response.Source2Ratio = (float)(result2.IterationsPerSecond / maxIterationsPerSecond);
            }

            return response;
        }

        private BenchmarkResult Benchmark(string sourceCode)
        {
            var generatedAssemblyFilePath = Path.GetTempFileName();

            var emitResult = CompileAndBuild(generatedAssemblyFilePath, sourceCode);
            if (!emitResult.Success)
            {
                var result = new BenchmarkResult();
                result.Error = string.Join("\n", emitResult.Diagnostics.Select(d => d.ToString()));
                return result;
            }

            var resultsFile = generatedAssemblyFilePath + ".result";
            var testingTimeInSeconds = 5;
            
            ExecuteBenchmark(generatedAssemblyFilePath, resultsFile, testingTimeInSeconds);

            var benchmarkResult = LoadResult(resultsFile);
            return benchmarkResult;
        }

        private void ExecuteBenchmark(string generatedAssemblyFilePath, string resultsFile, int testingTimeInSeconds)
        {
            var args = $"{testingTimeInSeconds} \"{resultsFile}\"";
            var psi = new ProcessStartInfo(generatedAssemblyFilePath, args);
            psi.UseShellExecute = false;
            psi.LoadUserProfile = false;
            var process = Process.Start(psi);
            process.WaitForExit(testingTimeInSeconds * 1000 * 2);
            if (!process.HasExited) process.Kill();            
        }

        private EmitResult CompileAndBuild(string outputFileName, string sourceCode)
        {
            
            const string referenceBasePath = @"c:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.2\";
            var template = File.ReadAllText(@"C:\Workspace\Sharpbench\src\BenchmarkHost\EntryPoint.cs");
            var generatedSourceCode = template.Replace("/*CODE_PLACEHOLDER*/", sourceCode);
            var syntaxTree = CSharpSyntaxTree.ParseText(generatedSourceCode);
            var options = new CSharpCompilationOptions(OutputKind.ConsoleApplication);
            options.WithOptimizationLevel(OptimizationLevel.Debug);            

            var references = new MetadataReference[]
                                                        {
                                                            MetadataReference.CreateFromFile(Path.Combine(referenceBasePath, "mscorlib.dll")),
                                                            MetadataReference.CreateFromFile(Path.Combine(referenceBasePath, "System.dll")),
                                                            MetadataReference.CreateFromFile(Path.Combine(referenceBasePath, "System.Xml.dll")),
                                                        };
            var compilation = CSharpCompilation.Create("GeneratedAssembly", new[] { syntaxTree }, options: options, references: references);
            var emitResult = compilation.Emit(outputFileName);
            return emitResult;
        }

        private BenchmarkResult LoadResult( string resultsFile)
        {
            var benchmarkResult = BenchmarkResult.Deserialize(resultsFile);
            return benchmarkResult;
        }
    }
}