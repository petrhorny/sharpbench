using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sharpbench;
using SharpbenchWeb.Models;

namespace SharpbenchWeb.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(BenchmarkModel.Instance);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
        [HttpPost]
        public IActionResult RunBenchmark(FormData formData)
        {
            var model = BenchmarkModel.Instance;
            for (var row = 0; row < formData.SourceCode.Length; row++)
            {
                for (var column = 0; column < formData.SourceCode[row].Length; column++)
                {
                    var sourceCode = formData.SourceCode[row][column];
                    model.BenchmarkData.SetSourceCode(row, column, sourceCode);
                }
            }
            
            var benchmarkProcessor = new BenchmarkProcessor();            
            var response = benchmarkProcessor.Benchmark(model.BenchmarkData);
            model.BenchmarkResponse = response;
            return View("BenchmarkResult", model);
        }        
        
        public class FormData
        {
            public string [][] SourceCode { get; set; }
        }

        public IActionResult AddRow()
        {
            var model = BenchmarkModel.Instance;
            model.BenchmarkData.AddRow();
            return RedirectToAction("Index");
        }

        public IActionResult AddColumn()
        {
            var model = BenchmarkModel.Instance;
            model.BenchmarkData.AddColumn();
            return RedirectToAction("Index");
        }
    }
}
