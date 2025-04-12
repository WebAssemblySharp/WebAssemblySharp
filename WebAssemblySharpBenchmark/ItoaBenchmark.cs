using BenchmarkDotNet.Attributes;
using WebAssemblySharp.Runtime;
using WebAssemblySharpExampleData;

namespace WebAssemblySharpBenchmark;

//[DotTraceDiagnoser]
[ShortRunJob]
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
        WebAssemblyRuntime l_Runtime = new WebAssemblyRuntime();
        m_Module =
            await (await l_Runtime.LoadModule(
                typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.itoa.wasm"))).Build();

        await m_Module.Call("itoa", 1);     
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

        await m_Module.Call("itoa", N); 

    }
    
}