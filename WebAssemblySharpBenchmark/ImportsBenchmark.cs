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
public class ImportsBenchmark
{
    [Params(2, 3, 99, 512, 9999)]
    public int N;

    private WebAssemblyModule m_InterpreterAsyncModule;
    private WebAssemblyModule m_InterpreterSyncModule;
    private WebAssemblyModule m_JitAsyncModule;
    private WebAssemblyModule m_JitSyncModule;
    

    [GlobalSetup]
    public async Task GlobalSetup()
    {
        WebAssemblyRuntimeBuilder l_RuntimeBuilder = WebAssemblyRuntimeBuilder.Create();
        await l_RuntimeBuilder.LoadModule("Async",
            typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.imports.wasm"), null,
            (x) =>
            {
                x.ImportMethod("times2", new Func<int, ValueTask<int>>(x => ValueTask.FromResult(x * 2)));
            });
        
        await l_RuntimeBuilder.LoadModule("Sync",
            typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.imports.wasm"), null,
            (x) =>
            {
                x.ImportMethod("times2", new Func<int, int>(x => x * 2));
            });

        WebAssemblyRuntime l_WebAssemblyRuntime = await l_RuntimeBuilder.Build();
        m_InterpreterAsyncModule = l_WebAssemblyRuntime.GetModule("Async");
        m_InterpreterSyncModule = l_WebAssemblyRuntime.GetModule("Sync");
        
        await m_InterpreterAsyncModule.Call<int, int>("twiceplus5", 3);
        await m_InterpreterSyncModule.Call<int, int>("twiceplus5", 3);

        if (WebAssemblyJITExecutor.IsSupported)
        {
            
            l_RuntimeBuilder = WebAssemblyRuntimeBuilder.Create(typeof(WebAssemblyJITExecutor));
            await l_RuntimeBuilder.LoadModule("Async",
                typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.imports.wasm"), null,
                (x) =>
                {
                    x.ImportMethod("times2", new Func<int, ValueTask<int>>(x => ValueTask.FromResult(x * 2)));
                });
        
            await l_RuntimeBuilder.LoadModule("Sync",
                typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.imports.wasm"), null,
                (x) =>
                {
                    x.ImportMethod("times2", new Func<int, int>(x => x * 2));
                });

            
            l_WebAssemblyRuntime = await l_RuntimeBuilder.Build();
            m_JitAsyncModule = l_WebAssemblyRuntime.GetModule("Async");
            m_JitSyncModule = l_WebAssemblyRuntime.GetModule("Sync");
        
            await m_JitAsyncModule.Call<int, int>("twiceplus5", 3);
            await m_JitSyncModule.Call<int, int>("twiceplus5", 3);    
        }
     
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        m_InterpreterSyncModule = null;
        m_InterpreterAsyncModule = null;
    }
    
    [Benchmark(Baseline = true)]
    public async Task InterpreterSync() {

        await m_InterpreterSyncModule.Call<int, int>("twiceplus5", N);

    }

    [Benchmark]
    public async Task InterpreterAsync() {

        await m_InterpreterAsyncModule.DynamicCall("twiceplus5", N); 

    }
    
    [Benchmark]
    [AotFilter]
    public async Task JitSync() {

        await m_JitSyncModule.Call<int, int>("twiceplus5", N);

    }

    [Benchmark]
    [AotFilter]
    public async Task JitAsync() {

        await m_JitAsyncModule.DynamicCall("twiceplus5", N); 

    }
}