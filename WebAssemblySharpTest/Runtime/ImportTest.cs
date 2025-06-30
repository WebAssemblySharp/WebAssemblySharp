using System;
using System.Threading.Tasks;
using WebAssemblySharp.Runtime;
using WebAssemblySharpExampleData;

namespace WebAssemblySharpTest.Runtime;

[TestClass]
public class ImportTest
{
    [TestMethod]
    [DynamicData(nameof(TestRuntimeProvider.RuntimeTypes), typeof(TestRuntimeProvider))]
    public async Task ExecuteImportRuntimeLevelWasmAsyncTest(Type p_RuntimeType)
    {
        WebAssemblyRuntimeBuilder l_RuntimeBuilder = WebAssemblyRuntimeBuilder.Create(p_RuntimeType);
        await l_RuntimeBuilder.LoadModule("main", typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.imports.wasm"));
        l_RuntimeBuilder.ImportMethod("times2", new Func<int, Task<int>>(x => Task.FromResult(x * 2)));
        WebAssemblyRuntime l_Runtime = await l_RuntimeBuilder.Build();
        WebAssemblyModule l_Module = l_Runtime.GetModule("main");
        int l_Result = (int) await l_Module.DynamicCall("twiceplus5", 3);
        Assert.AreEqual(11, l_Result);
    }
    
    [TestMethod]
    [DynamicData(nameof(TestRuntimeProvider.RuntimeTypes), typeof(TestRuntimeProvider))]
    public async Task ExecuteImportRuntimeLevelWasmSyncTest(Type p_RuntimeType)
    {
        WebAssemblyRuntimeBuilder l_RuntimeBuilder = WebAssemblyRuntimeBuilder.Create(p_RuntimeType);
        await l_RuntimeBuilder.LoadModule("main", typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.imports.wasm"));
        l_RuntimeBuilder.ImportMethod("times2", new Func<int, int>(x => x * 2));
        WebAssemblyRuntime l_Runtime = await l_RuntimeBuilder.Build();
        WebAssemblyModule l_Module = l_Runtime.GetModule("main");
        
        int l_Result = (int) await l_Module.DynamicCall("twiceplus5", 3);
        Assert.AreEqual(11, l_Result);
    }
    
    [TestMethod]
    [DynamicData(nameof(TestRuntimeProvider.RuntimeTypes), typeof(TestRuntimeProvider))]
    public async Task ExecuteImportModuleLevelWasmAsyncTest(Type p_RuntimeType)
    {
        WebAssemblyModule l_Module = await WebAssemblyRuntimeBuilder.CreateSingleModuleRuntime(p_RuntimeType,
            typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.imports.wasm"),
            (x) =>
            {
                x.ImportMethod("times2", new Func<int, Task<int>>(x => Task.FromResult(x * 2)));
            });
        
        int l_Result = (int) await l_Module.DynamicCall("twiceplus5", 3);
        Assert.AreEqual(11, l_Result);
    }
    
    [TestMethod]
    [DynamicData(nameof(TestRuntimeProvider.RuntimeTypes), typeof(TestRuntimeProvider))]
    public async Task ExecuteImportModuleLevelWasmSyncTest(Type p_RuntimeType)
    {
        WebAssemblyModule l_Module = await WebAssemblyRuntimeBuilder.CreateSingleModuleRuntime(p_RuntimeType,
            typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.imports.wasm"),
            (x) =>
            {
                x.ImportMethod("times2", new Func<int, int>(x => x * 2));
            });
        
        int l_Result = (int) await l_Module.DynamicCall("twiceplus5", 3);
        Assert.AreEqual(11, l_Result);
    }
}