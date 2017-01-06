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
        public BenchmarkResponse Benchmark(BenchmarkData benchmarkData)
        {
            var response = new BenchmarkResponse();

            for (var row = 0; row < benchmarkData.Rows; row++)
            {
                for (var column = 0; column < benchmarkData.Columns; column++)
                {
                    var sourceCode = benchmarkData.GetSourceCode(row, column);
                    var buildConfiguration = benchmarkData.GetBuildConfiguration(row);
                    var result = Benchmark(sourceCode, buildConfiguration);
                    response.SetBenchmarkResult(row, column, result);
                }
            }

            return response;
        }

        private BenchmarkResult Benchmark(string sourceCode, BuildConfiguration buildConfiguration)
        {
            var generatedAssemblyFilePath = Path.GetTempFileName();

            var emitResult = CompileAndBuild(generatedAssemblyFilePath, sourceCode, buildConfiguration);
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

        private EmitResult CompileAndBuild(string outputFileName, string sourceCode, BuildConfiguration buildConfiguration)
        {
            var referenceBasePath = $@"c:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\{buildConfiguration.TargetFramework}\";
            var template = File.ReadAllText(@"C:\Workspace\Sharpbench\src\BenchmarkHost\EntryPoint.cs");
            var generatedSourceCode = template.Replace("/*CODE_PLACEHOLDER*/", sourceCode);
            var syntaxTree = CSharpSyntaxTree.ParseText(generatedSourceCode);

            var options = new CSharpCompilationOptions(OutputKind.ConsoleApplication);
            if (buildConfiguration.Optimization == Optimization.Debug) options.WithOptimizationLevel(OptimizationLevel.Debug);
            else if (buildConfiguration.Optimization == Optimization.Release) options.WithOptimizationLevel(OptimizationLevel.Release);
            else throw new ArgumentOutOfRangeException(nameof(buildConfiguration.Optimization));

            if (buildConfiguration.Platform == Platform.X86) options.WithPlatform(Microsoft.CodeAnalysis.Platform.X86);
            else if (buildConfiguration.Platform == Platform.X64) options.WithPlatform(Microsoft.CodeAnalysis.Platform.X64);

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

        private BenchmarkResult LoadResult(string resultsFile)
        {
            var benchmarkResult = BenchmarkResult.Deserialize(resultsFile);
            return benchmarkResult;
        }
    }
}