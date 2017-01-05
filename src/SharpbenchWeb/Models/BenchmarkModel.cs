using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sharpbench;

namespace SharpbenchWeb.Models
{
    public class BenchmarkModel
    {
        public static readonly BenchmarkModel Instance = new BenchmarkModel(); 
        public string SourceCode1 { get; set; }
        public string SourceCode2 { get; set; }
        public BenchmarkResponse BenchmarkResponse { get; set; }

        public BenchmarkModel()
        {
            SourceCode1 = "var x = \"abc\" + \"def\";";
            SourceCode2 = "var x = string.Format(\"{0}{1}\", \"abc\", \"def\");";
        }
    }
}
