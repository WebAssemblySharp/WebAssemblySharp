using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.dotTrace;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using WebAssemblySharp.Runtime;
using WebAssemblySharpExampleData;

namespace WebAssemblySharpBenchmark;

//[DotTraceDiagnoser]
[ShortRunJob(RuntimeMoniker.Net90)]
[MemoryDiagnoser]
[JsonExporter("-custom", indentJson: true, excludeMeasurements: true)]
public class IsPrimeBenchmark {

    [Params(2, 3, 99, 512, 9999)]
    public int N;

    private WebAssemblyModule m_Module;
    

    [GlobalSetup]
    public async Task GlobalSetup()
    {
        WebAssemblyRuntime l_Runtime = new WebAssemblyRuntime();
        m_Module =
            await (await l_Runtime.LoadModule(
                typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.isprime.wasm"))).Build();

        await m_Module.Call("is_prime", 1);     
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        m_Module = null;
    }
    
    [Benchmark(Baseline = true)]
    public void Native() {

        NativeIsPrime(N);

    }

    [Benchmark]
    public async Task Interpreter() {

        await m_Module.Call("is_prime", N); 

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

}