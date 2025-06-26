using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Filters;
using BenchmarkDotNet.Jobs;
using WebAssemblySharp.Runtime;
using WebAssemblySharp.Runtime.JIT;
using WebAssemblySharpExampleData;

namespace WebAssemblySharpBenchmark;

//[DotTraceDiagnoser]

[ShortRunJob(RuntimeMoniker.NativeAot10_0)]
[ShortRunJob(RuntimeMoniker.Net10_0)]
[ShortRunJob(RuntimeMoniker.NativeAot90)]
[ShortRunJob(RuntimeMoniker.Net90)]
[MemoryDiagnoser]
[JsonExporter("-custom", indentJson: true, excludeMeasurements: true)]
public class IsPrimeBenchmark {

    [Params(2, 3, 99, 512, 9999)]
    public int N;

    private WebAssemblyModule m_InterpreterModule;
    private WebAssemblyModule m_JitModule;
    
    

    [GlobalSetup]
    public async Task GlobalSetup()
    {
        m_InterpreterModule = await WebAssemblyRuntimeBuilder.CreateSingleModuleRuntime(
            typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.isprime.wasm"));
        await m_InterpreterModule.Call<int, int>("is_prime", 1);
        
        
        if (RuntimeFeature.IsDynamicCodeSupported)
        {
            m_JitModule = await WebAssemblyRuntimeBuilder.CreateSingleModuleRuntime(typeof(WebAssemblyJITExecutor),
                typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.isprime.wasm"));
        
        
            await m_JitModule.Call<int, int>("is_prime", 1);
            
        }
        
    }
    
    [GlobalCleanup]
    public void GlobalCleanup()
    {
        m_InterpreterModule = null;
        m_JitModule = null;
    }
    
    [Benchmark]
    public async Task NativeAsync() {

        await NativeIsPrimeAsync(N);

    }
    
    [Benchmark(Baseline = true)]
    public void NativeSync() {

        NativeIsPrime(N);

    }
    

    [Benchmark]
    public async Task Interpreter() {

        await m_InterpreterModule.Call<int, int>("is_prime", N); 

    }

    [Benchmark]
    [AotFilter]
    public async Task Jit() {

        await m_JitModule.Call<int, int>("is_prime", N);
        
    }
    
    
    private bool NativeIsPrime(int n)
    {
        
        if (n < 2)
            return false;
        if (n == 2)
            return true;
        if (n % 2 == 0)
            return false;

        for (int i = 3; i < n; i += 2)
        {
            if (n % i == 0)
                return false;
        }

        return true;
    }
    
    private Task<bool> NativeIsPrimeAsync(int n)
    {
        
        if (n < 2)
            return Task.FromResult(true);
        if (n == 2)
            return Task.FromResult(true);
        if (n % 2 == 0)
            return Task.FromResult(false);

        for (int i = 3; i < n; i += 2)
        {
            if (n % i == 0)
                return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }

}