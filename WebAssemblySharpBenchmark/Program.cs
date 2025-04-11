using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace WebAssemblySharpBenchmark;

public class Program
{
    public static void Main(string[] args)
    {
        List<Type> types = new List<Type>();
        types.Add(typeof(ImportsBenchmark));
        types.Add(typeof(IsPrimeBenchmark));
        
        var summary = BenchmarkRunner.Run(types.ToArray(), null, args);
    }
}
