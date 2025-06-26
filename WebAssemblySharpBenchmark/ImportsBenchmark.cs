using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using WebAssemblySharp.Runtime;
using WebAssemblySharpExampleData;

namespace WebAssemblySharpBenchmark;

//[DotTraceDiagnoser]
[ShortRunJob(RuntimeMoniker.NativeAot10_0)]
[ShortRunJob(RuntimeMoniker.Net10_0)]
[ShortRunJob(RuntimeMoniker.NativeAot90)]
[ShortRunJob(RuntimeMoniker.Net90)]
[MemoryDiagnoser]
[JsonExporter("-custom", indentJson: true, excludeMeasurements: true)]
public class ImportsBenchmark
{
    [Params(2, 3, 99, 512, 9999)]
    public int N;

    private WebAssemblyModule m_AsyncModule;
    private WebAssemblyModule m_SyncModule;
    

    [GlobalSetup]
    public async Task GlobalSetup()
    {
        WebAssemblyRuntimeBuilder l_RuntimeBuilder = WebAssemblyRuntimeBuilder.Create();
        await l_RuntimeBuilder.LoadModule("Async",
            typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.imports.wasm"), null,
            (x) =>
            {
                x.ImportMethod("times2", new Func<int, Task<int>>(x => Task.FromResult(x * 2)));
            });
        
        await l_RuntimeBuilder.LoadModule("Sync",
            typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.imports.wasm"), null,
            (x) =>
            {
                x.ImportMethod("times2", new Func<int, int>(x => x * 2));
            });

        WebAssemblyRuntime l_WebAssemblyRuntime = await l_RuntimeBuilder.Build();
        m_AsyncModule = l_WebAssemblyRuntime.GetModule("Async");
        m_SyncModule = l_WebAssemblyRuntime.GetModule("Sync");
        
        await m_AsyncModule.Call<int, int>("twiceplus5", 3);
        await m_SyncModule.Call<int, int>("twiceplus5", 3);
     
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        m_SyncModule = null;
        m_AsyncModule = null;
    }
    
    [Benchmark(Baseline = true)]
    public async Task Sync() {

        await m_SyncModule.Call<int, int>("twiceplus5", N);

    }

    [Benchmark]
    public async Task Async() {

        await m_AsyncModule.DynamicCall("twiceplus5", N); 

    }
}