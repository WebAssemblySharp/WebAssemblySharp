using BenchmarkDotNet.Attributes;
using WebAssemblySharp.Runtime;
using WebAssemblySharpExampleData;

namespace WebAssemblySharpBenchmark;

//[DotTraceDiagnoser]
[ShortRunJob]
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
        WebAssemblyRuntime l_Runtime = new WebAssemblyRuntime();

        WebAssemblyModuleBuilder l_ModuleBuilder = await l_Runtime.LoadModule(
            typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.imports.wasm"));
        
        l_ModuleBuilder.DefineImport("times2", new Func<int, Task<int>>(x => Task.FromResult(x * 2)));
        m_AsyncModule = l_ModuleBuilder.Build();
        await m_AsyncModule.Call("twiceplus5", 3);
        
        l_ModuleBuilder = await l_Runtime.LoadModule(
            typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.imports.wasm"));
        
        l_ModuleBuilder.DefineImport("times2", new Func<int, int>(x => x * 2));
        m_SyncModule = l_ModuleBuilder.Build();
        await m_SyncModule.Call("twiceplus5", 3);
     
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        m_SyncModule = null;
        m_AsyncModule = null;
    }
    
    [Benchmark(Baseline = true)]
    public async Task Sync() {

        await m_SyncModule.Call("twiceplus5", N);

    }

    [Benchmark]
    public async Task Async() {

        await m_AsyncModule.Call("twiceplus5", N); 

    }
}