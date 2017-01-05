using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using NUnit.Framework;

namespace SharpbenchTest
{
    /// <summary>
    /// Only for manual testing purposes and experiments.
    /// </summary>
    [TestFixture]
    public class Playground
    {
        [Test]
        public void ManualTest()
        {
            var code = @"
var x = System.DateTime.Now.Ticks;
return x;";
                    
            var script =  CSharpScript.Create(code, ScriptOptions.Default);
            var compilation = script.GetCompilation();
            //var options = new CSharpCompilationOptions(OutputKind.);
            //var compilation = CSharpCompilation.CreateScriptCompilation("Assembly1", syntaxTree, options: options);
            var diag = compilation.GetDiagnostics();
            var success = compilation.Emit(@"c:\temp\pokus.dll");
        }

        [Test]
        public void ManualTest2()
        {
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(@"c:\temp\pokus.dll");            
            var myType = assembly.GetType("Submission#0");
            var myInstance = Activator.CreateInstance(myType, new object[] {new object[2]});
            var x= myInstance.GetType().GetMethods();
            var mi = myInstance.GetType().GetMethod("<Initialize>");
            var task = (Task<object>)mi.Invoke(myInstance, null);
            var res = task.Result;
            Thread.Sleep(10);
            var res2 = task.Result;
            //var assembly = Assembly.Load(new AssemblyName(@"file://c:/temp/pokus.dll"));
        }
    }
}
