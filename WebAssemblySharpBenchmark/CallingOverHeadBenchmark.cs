using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using WebAssemblySharp.Attributes;
using WebAssemblySharp.Runtime;
using WebAssemblySharp.Runtime.Interpreter;
using WebAssemblySharp.Runtime.JIT;
using WebAssemblySharpExampleData;

namespace WebAssemblySharpBenchmark;

//[DotTraceDiagnoser]
[ShortRunJob(RuntimeMoniker.Net90)]
[MemoryDiagnoser]
[JsonExporter("-custom", indentJson: true, excludeMeasurements: true)]
public class CallingOverHeadBenchmark
{
    private WebAssemblyModule m_InterpreterModule;
    private WebAssemblyModule m_JitModule;
    private IAdd m_InterpreterModuleAdd;
    private IAdd m_JitModuleAdd;
    private IAddLateBinding m_InterpreterModuleLateBinding;
    private IAddLateBinding m_JitModuleLateBinding;

    [GlobalSetup]
    public async Task GlobalSetup()
    {
        m_InterpreterModule = await WebAssemblyRuntimeBuilder.CreateSingleModuleRuntime(typeof(WebAssemblyInterpreterExecutor), typeof(IAdd));
        m_JitModule = await WebAssemblyRuntimeBuilder.CreateSingleModuleRuntime(typeof(WebAssemblyJITExecutor), typeof(IAdd));

        m_InterpreterModuleAdd = m_InterpreterModule.AsInterface<IAdd>();
        m_JitModuleAdd = m_JitModule.AsInterface<IAdd>();
        m_InterpreterModuleLateBinding = m_InterpreterModule.AsInterface<IAddLateBinding>();
        m_JitModuleLateBinding = m_JitModule.AsInterface<IAddLateBinding>();

        await m_InterpreterModule.Call<int, int, int>("add", 1, 1);
        await m_JitModule.Call<int, int, int>("add", 1, 1);
        await m_InterpreterModuleAdd.add(1, 1);
        await m_JitModuleAdd.add(1, 1);
        await m_InterpreterModuleLateBinding.add(1, 1);
        await m_JitModuleLateBinding.add(1, 1);
        
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        m_InterpreterModule = null;
        m_JitModule = null;
        m_InterpreterModuleAdd = null;
        m_JitModuleAdd = null;
        m_InterpreterModuleLateBinding = null;
        m_JitModuleLateBinding = null;
    }


    [Benchmark(Baseline = true)]
    public async Task Native()
    {
        await Add(1, 1);
    }

    [Benchmark]
    public async Task InterpreterModule()
    {
        await m_InterpreterModule.Call<int, int, int>("add", 1, 1);
    }

    [Benchmark]
    public async Task InterpreterInterface()
    {
        await m_InterpreterModuleAdd.add(1, 1);
    }
    
    [Benchmark]
    public async Task InterpreterInterfaceLateBinding()
    {
        await m_InterpreterModuleLateBinding.add(1, 1);
    }

    [Benchmark]
    public async Task JitModule()
    {
        await m_JitModule.Call<int, int, int>("add", 1, 1);
    }

    [Benchmark]
    public async Task JitInterface()
    {
        await m_JitModuleAdd.add(1, 1);
    }
    
    [Benchmark]
    public async Task JitInterfaceLateBinding()
    {
        await m_JitModuleLateBinding.add(1, 1);
    }

    private ValueTask<int> Add(int p_I, int p_I1)
    {
        return ValueTask.FromResult(p_I + p_I1);
    }


    [WebAssemblyModuleManifestResource("WebAssemblySharpExampleData.Programms.add.wasm", typeof(WebAssemblyExamples))]
    [WebAssemblyModuleDefinition("add")]
    public interface IAdd
    {
        ValueTask<int> add(int a, int b);
    }
    
    
    public interface IAddLateBinding
    {
        ValueTask<int> add(int a, int b);
    }
}