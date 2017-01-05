namespace Sharpbench
{
    public class BenchmarkResponse
    {
        public bool Success { get;set; }
        public float Source1Ratio { get; set; }
        public float Source2Ratio { get; set; }
        public string Error { get; set; }
    }
}