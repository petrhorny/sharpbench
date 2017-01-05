using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace BenchmarkHost
{
    public class EntryPoint
    {
        public static void Main(params string[] args)
        {
            Console.WriteLine("Benchmarking entry point");
            try
            {
                if (args.Length < 2) throw new Exception("To few arguments");
                var durationSeconds = int.Parse(args[0]);                
                var outputFileName = args[1];
                Console.WriteLine($"Duration: {durationSeconds} seconds");

                var benchmarkResult = Benchmark(durationSeconds);

                WriteResultsToFile(benchmarkResult, outputFileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static BenchmarkResult Benchmark(int durationSeconds)
        {
            try
            {
                Console.WriteLine("Guessing number of required iterations...");
                var iterations = GuessIterations(TimeSpan.FromSeconds(durationSeconds));

                Console.WriteLine("BENCHMARKING STARTED");

                var benchmarkResult = Run(iterations);

                Console.WriteLine("done");

                return benchmarkResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                var result = new BenchmarkResult();
                result.Error = ex.ToString();
                return result;
            }
        }

        private static void WriteResultsToFile(BenchmarkResult result, string outputFileName)
        {
            if (String.IsNullOrEmpty(outputFileName)) return;
            Console.WriteLine($"Writing results to file {outputFileName}");
            result.Serialize(outputFileName);
        }

        private static object RunMany(uint __iterations)
        {
            object result = null;
            for (uint iteration = 1; iteration <= __iterations; iteration++)
            {
                /*CODE_PLACEHOLDER*/
            }
            return result;
        }

        private static BenchmarkResult Run(uint iterations)
        {
            Console.Write($"Running {iterations} iterations...");
            var watch = BenchmarkWatch.StartNew();
            RunMany(iterations);
            var result = watch.Stop(iterations);
            Console.WriteLine("done, " + result);
            return result;
        }

        private static uint GuessIterations(TimeSpan desiredExecutionTime)
        {
            // one dummy run
            Run(1);
            uint iterations = 1;
            var result = Run(iterations);
            while (result.ElapsedTime.TotalMilliseconds < 100)
            {
                iterations *= 10;
                result = Run(iterations);
            }
            var guessedIterations = (desiredExecutionTime.TotalMilliseconds / result.ElapsedTime.TotalMilliseconds) * iterations;
            return (uint)guessedIterations;
        }

        private class BenchmarkWatch
        {
            private Stopwatch _stopWatch;
            private Process _currentProcess;
            private BenchmarkResult _benchmarkResult;

            private BenchmarkWatch()
            {
            }

            public void Restart()
            {
                _benchmarkResult = new BenchmarkResult();
                _currentProcess = Process.GetCurrentProcess();
                GC.Collect();
                _benchmarkResult.GcCollectionCount0 = GC.CollectionCount(0);
                _benchmarkResult.GcCollectionCount1 = GC.CollectionCount(1);
                _benchmarkResult.GcCollectionCount2 = GC.CollectionCount(2);
                _stopWatch = Stopwatch.StartNew();
                _benchmarkResult.UserProcessorTime = _currentProcess.UserProcessorTime;
            }

            public static BenchmarkWatch StartNew()
            {
                var watch = new BenchmarkWatch();
                watch.Restart();
                return watch;
            }

            public BenchmarkResult Stop(uint iterations)
            {
                if (_benchmarkResult == null) throw new InvalidOperationException("You can't call Stop without calling Restart.");
                _benchmarkResult.UserProcessorTime = _currentProcess.UserProcessorTime - _benchmarkResult.UserProcessorTime;
                _stopWatch.Stop();
                if (iterations == 0) throw new ArgumentOutOfRangeException(nameof(iterations));
                _benchmarkResult.Iterations = iterations;
                _benchmarkResult.ElapsedTime = _stopWatch.Elapsed;
                _benchmarkResult.GcCollectionCount0 = GC.CollectionCount(0) - _benchmarkResult.GcCollectionCount0;
                _benchmarkResult.GcCollectionCount1 = GC.CollectionCount(1) - _benchmarkResult.GcCollectionCount1;
                _benchmarkResult.GcCollectionCount2 = GC.CollectionCount(2) - _benchmarkResult.GcCollectionCount2;
                var result = _benchmarkResult;
                _benchmarkResult = null;
                return result;
            }
        }
    }

    public class BenchmarkResult
    {
        [XmlIgnore]
        public TimeSpan ElapsedTime { get; set; }

        [XmlElement("ElapsedTime")]
        public long ElapsedTimeTicks
        {
            get { return ElapsedTime.Ticks; }
            set { ElapsedTime = new TimeSpan(value); }
        }

        [XmlIgnore]
        public TimeSpan UserProcessorTime { get; set; }

        [XmlElement("UserProcessorTime")]
        public long UserProcessorTimeTicks
        {
            get { return UserProcessorTime.Ticks; }
            set { UserProcessorTime = new TimeSpan(value); }
        }

        public uint Iterations { get; set; }
        public int GcCollectionCount0 { get; set; }
        public int GcCollectionCount1 { get; set; }
        public int GcCollectionCount2 { get; set; }
        public string Error { get; set; }
        public bool Success => string.IsNullOrEmpty(Error);
        public double IterationsPerSecond => Iterations / ElapsedTime.TotalSeconds;

        public void Serialize(string fileName)
        {
            using (var stream = File.Create(fileName))
            {
                var serializer = new XmlSerializer(typeof(BenchmarkResult));
                serializer.Serialize(stream, this);
            }
        }

        public static BenchmarkResult Deserialize(string fileName)
        {
            using (var stream = File.OpenRead(fileName))
            {
                var serializer = new XmlSerializer(typeof(BenchmarkResult));
                var result = (BenchmarkResult)serializer.Deserialize(stream);
                return result;
            }
        }

        public override string ToString()
        {
            return $"iterations: {Iterations}, total time: {ElapsedTime}, total CPU time: {UserProcessorTime}, GC: {GcCollectionCount0},{GcCollectionCount1},{GcCollectionCount2} ";
        }
    }
}
