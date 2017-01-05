using NUnit.Framework;
using Sharpbench;

namespace SharpbenchTest
{
    [TestFixture]
    public class BenchmarkProcessorTest
    {
        [Test]
        public void Test()
        {
            var processor = new BenchmarkProcessor();
            var request = new BenchmarkRequest();
            request.SourceCode1 = "System.Threading.Thread.Sleep(10);";
            request.SourceCode2 = "System.Threading.Thread.Sleep(100);";
            var response = processor.Benchmark(request);
            Assert.IsTrue(response.Success);
            Assert.IsTrue(response.Source1Ratio > 5*response.Source2Ratio);
        }
    }
}
