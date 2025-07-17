using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Filters;
using BenchmarkDotNet.Jobs;
using WebAssemblySharp.Runtime;
using WebAssemblySharp.Runtime.JIT;
using WebAssemblySharpExampleData;

namespace WebAssemblySharpBenchmark;

[ShortRunJob(RuntimeMoniker.NativeAot10_0)]
[ShortRunJob(RuntimeMoniker.Net10_0)]
[ShortRunJob(RuntimeMoniker.NativeAot90)]
[ShortRunJob(RuntimeMoniker.Net90)]
[MemoryDiagnoser]
[JsonExporter("-custom", indentJson: true, excludeMeasurements: true)]
public class ImportBetweenModulesBenchmark
{
    [Params(2, 3, 99, 512, 9999)]
    public int N;
    
    private WebAssemblyModule m_InterpreterModule;
    private WebAssemblyModule m_JitModule;
    
    [GlobalSetup]
    public async Task GlobalSetup()
    {
        WebAssemblyRuntimeBuilder l_RuntimeBuilder = WebAssemblyRuntimeBuilder.Create();
        await l_RuntimeBuilder.LoadModule("mod1", typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.import_mod1.wasm"));
        await l_RuntimeBuilder.LoadModule("mod2", typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.import_mod2.wasm"));
        WebAssemblyRuntime l_Runtime = await l_RuntimeBuilder.Build();

        m_InterpreterModule = l_Runtime.GetModule("mod2");
        await m_InterpreterModule.Call<int, int>("twiceplus5", 10);

        if (WebAssemblyJITExecutor.IsSupported)
        {
            l_RuntimeBuilder = WebAssemblyRuntimeBuilder.Create(typeof(WebAssemblyJITExecutor));
            await l_RuntimeBuilder.LoadModule("mod1", typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.import_mod1.wasm"));
            await l_RuntimeBuilder.LoadModule("mod2", typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.import_mod2.wasm"));
            l_Runtime = await l_RuntimeBuilder.Build();

            m_JitModule = l_Runtime.GetModule("mod2");
            await m_JitModule.Call<int, int>("twiceplus5", 10);    
        }
        
    }

    [Benchmark]
    public async Task Interpreter()
    {
        await m_InterpreterModule.Call<int, int>("twiceplus5", N);    
    }
    
    [Benchmark]
    [AotFilter]
    public async Task Jit()
    {
        await m_JitModule.Call<int, int>("twiceplus5", N);    
    }
    
    [GlobalCleanup]
    public void GlobalCleanup()
    {
        m_InterpreterModule = null;
        m_JitModule = null;
    }
    
}