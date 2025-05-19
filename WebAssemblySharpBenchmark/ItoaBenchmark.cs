using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using WebAssemblySharp.Runtime;
using WebAssemblySharp.Runtime.Values;
using WebAssemblySharpExampleData;

namespace WebAssemblySharpBenchmark;

//[DotTraceDiagnoser]
[ShortRunJob(RuntimeMoniker.Net90)]
[MemoryDiagnoser]
[JsonExporter("-custom", indentJson: true, excludeMeasurements: true)]
public class ItoaBenchmark
{
    [Params(2, 3, 99, 512, 9999)]
    public int N;

    private WebAssemblyModule m_Module;
    

    [GlobalSetup]
    public async Task GlobalSetup()
    {
        m_Module = await WebAssemblyRuntimeBuilder.CreateSingleModuleRuntime(
            typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.itoa.wasm"));
        
        await m_Module.Call<WebAssemblyUTF8String, int>("itoa", 1);     
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        m_Module = null;
    }
    
    [Benchmark(Baseline = true)]
    public void Native() {

        Convert.ToString(N);

    }

    [Benchmark]
    public async Task Interpreter() {

        await m_Module.Call<WebAssemblyUTF8String, int>("itoa", N); 

    }
    
}