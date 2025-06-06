﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace WebAssemblySharpBenchmark;

public class Program
{
    public static void Main(string[] args)
    {
        List<Type> l_Types = new List<Type>();
        l_Types.Add(typeof(ImportsBenchmark));
        l_Types.Add(typeof(IsPrimeBenchmark));
        l_Types.Add(typeof(ItoaBenchmark));
        l_Types.Add(typeof(ImportBetweenModulesBenchmark));
        l_Types.Add(typeof(CallingOverHeadBenchmark));
        
        var summary = BenchmarkRunner.Run(l_Types.ToArray(), null, args);
    }
}
