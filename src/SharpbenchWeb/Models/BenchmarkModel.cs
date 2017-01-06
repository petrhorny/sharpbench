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
        public BenchmarkData BenchmarkData { get; private set; }
        public BenchmarkResponse BenchmarkResponse { get; set; }

        public BenchmarkModel()
        {
            BenchmarkData = new BenchmarkData();
            BenchmarkData.SetSourceCode(0, 0, "var x = \"abc\" + \"def\";");
            BenchmarkData.SetSourceCode(0, 1, "var x = string.Format(\"{0}{1}\", \"abc\", \"def\");");            
        }
    }
}
