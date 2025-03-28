using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace WebAssemblySharpBenchmark;

public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<IsPrimeBenchmark>();
    }
}
