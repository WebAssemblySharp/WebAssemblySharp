using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Filters;
using BenchmarkDotNet.Jobs;
using WebAssemblySharp.Runtime;
using WebAssemblySharp.Runtime.Interpreter;
using WebAssemblySharp.Runtime.JIT;
using WebAssemblySharp.Runtime.Values;
using WebAssemblySharpExampleData;

namespace WebAssemblySharpBenchmark;

//[DotTraceDiagnoser]
[ShortRunJob(RuntimeMoniker.NativeAot10_0)]
[ShortRunJob(RuntimeMoniker.Net10_0)]
[ShortRunJob(RuntimeMoniker.NativeAot90)]
[ShortRunJob(RuntimeMoniker.Net90)]
[MemoryDiagnoser]
[JsonExporter("-custom", indentJson: true, excludeMeasurements: true)]
public class ItoaBenchmark
{
    [Params(2, 3, 99, 512, 9999)]
    public int N;

    private WebAssemblyModule m_InterpreterModule;
    private WebAssemblyModule m_JitModule;
    

    [GlobalSetup]
    public async Task GlobalSetup()
    {
        m_InterpreterModule = await WebAssemblyRuntimeBuilder.CreateSingleModuleRuntime(typeof(WebAssemblyInterpreterExecutor),
            typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.itoa.wasm"));
        await m_InterpreterModule.CallAndConvert<WebAssemblyUTF8String, int>("itoa", 1);


        if (RuntimeFeature.IsDynamicCodeSupported)
        {
            m_JitModule = await WebAssemblyRuntimeBuilder.CreateSingleModuleRuntime(typeof(WebAssemblyJITExecutor),
                typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.itoa.wasm"));
        
        
            await m_JitModule.CallAndConvert<WebAssemblyUTF8String, int>("itoa", 1);    
        }
        
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        m_InterpreterModule = null;
        m_JitModule = null;
    }
    
    [Benchmark(Baseline = true)]
    public void Native() {

        Convert.ToString(N);

    }

    [Benchmark]
    public async Task Interpreter() {

        await m_InterpreterModule.CallAndConvert<WebAssemblyUTF8String, int>("itoa", N); 

    }
    
    [Benchmark]
    [AotFilter]
    public async Task Jit() {

        await m_JitModule.CallAndConvert<WebAssemblyUTF8String, int>("itoa", N); 

    }
    
}