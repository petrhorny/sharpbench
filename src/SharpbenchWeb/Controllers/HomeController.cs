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

        public IActionResult RunBenchmark(string sourceCode1, string sourceCode2)
        {
            var model = BenchmarkModel.Instance;
            model.SourceCode1 = sourceCode1;
            model.SourceCode2 = sourceCode2;
            var benchmarkProcessor = new BenchmarkProcessor();
            var request = new BenchmarkRequest();
            request.SourceCode1 = sourceCode1;
            request.SourceCode2 = sourceCode2;
            var response = benchmarkProcessor.Benchmark(request);
            model.BenchmarkResponse = response;

            return View("BenchmarkResult", model);
        }        
        
    }
}
