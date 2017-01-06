using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkHost;

namespace Sharpbench
{
    public class BenchmarkResponse
    {
        private BenchmarkResult[,] _results = new BenchmarkResult[0, 0];

        private IEnumerable<BenchmarkResult> Results => _results.Cast<BenchmarkResult>();

        public bool Success => Results.All(i => i.Success);

        public string GetErrors()
        {
            return string.Join("\n", Results.Where(r => !r.Success).Select(r => r.Error));
        }

        public void SetBenchmarkResult(int row, int column, BenchmarkResult result)
        {
            _results = Utils.AdjustArray(_results, row, column);
            _results[row, column] = result;
        }

        public double GetRatio(int row, int column, Func<BenchmarkResult, double> valueFunc)
        {
            var maxValuePerIteration = Results.Max(r => r.GetValuePerIteration(valueFunc));
            var currentValuePerIteration = _results[row, column].GetValuePerIteration(valueFunc);
            return currentValuePerIteration / maxValuePerIteration;
        }

        
    }
}