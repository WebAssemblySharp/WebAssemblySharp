using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using WebAssemblySharp.Runtime;
using WebAssemblySharpExampleData;

namespace WebAssemblySharpBenchmark;

[ShortRunJob(RuntimeMoniker.Net10_0)]
[ShortRunJob(RuntimeMoniker.Net90)]
[MemoryDiagnoser]
[JsonExporter("-custom", indentJson: true, excludeMeasurements: true)]
public class ImportBetweenModulesBenchmark
{
    [Params(2, 3, 99, 512, 9999)]
    public int N;
    
    private WebAssemblyModule m_Module;
    
    [GlobalSetup]
    public async Task GlobalSetup()
    {
        WebAssemblyRuntimeBuilder l_RuntimeBuilder = WebAssemblyRuntimeBuilder.Create();
        await l_RuntimeBuilder.LoadModule("mod1", typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.import_mod1.wasm"));
        await l_RuntimeBuilder.LoadModule("mod2", typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.import_mod2.wasm"));
        WebAssemblyRuntime l_Runtime = await l_RuntimeBuilder.Build();

        m_Module = l_Runtime.GetModule("mod2");

        await m_Module.Call<int, int>("twiceplus5", 10);
     
    }

    [Benchmark]
    public async Task Interpreter()
    {
        await m_Module.Call<int, int>("twiceplus5", N);    
    }
    
    [GlobalCleanup]
    public void GlobalCleanup()
    {
        m_Module = null;
    }
    
}