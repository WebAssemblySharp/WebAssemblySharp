using System;
using System.Threading.Tasks;
using WebAssemblySharp.Runtime;
using WebAssemblySharpExampleData;

namespace WebAssemblySharpTest.Runtime;

[TestClass]
public class ImportTest
{
    [TestMethod]
    public async Task ExecuteImportRuntimeLevelWasmAsyncTest()
    {
        WebAssemblyRuntimeBuilder l_RuntimeBuilder = WebAssemblyRuntimeBuilder.Create();
        await l_RuntimeBuilder.LoadModule("main", typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.imports.wasm"));
        l_RuntimeBuilder.ImportMethod("times2", new Func<int, Task<int>>(x => Task.FromResult(x * 2)));
        WebAssemblyRuntime l_Runtime = await l_RuntimeBuilder.Build();
        WebAssemblyModule l_Module = l_Runtime.GetModule("main");
        int l_Result = (int) await l_Module.DynamicCall("twiceplus5", 3);
        Assert.AreEqual(11, l_Result);
    }
    
    [TestMethod]
    public async Task ExecuteImportRuntimeLevelWasmSyncTest()
    {
        WebAssemblyRuntimeBuilder l_RuntimeBuilder = WebAssemblyRuntimeBuilder.Create();
        await l_RuntimeBuilder.LoadModule("main", typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.imports.wasm"));
        l_RuntimeBuilder.ImportMethod("times2", new Func<int, int>(x => x * 2));
        WebAssemblyRuntime l_Runtime = await l_RuntimeBuilder.Build();
        WebAssemblyModule l_Module = l_Runtime.GetModule("main");
        
        int l_Result = (int) await l_Module.DynamicCall("twiceplus5", 3);
        Assert.AreEqual(11, l_Result);
    }
    
    [TestMethod]
    public async Task ExecuteImportModuleLevelWasmAsyncTest()
    {
        WebAssemblyModule l_Module = await WebAssemblyRuntimeBuilder.CreateSingleModuleRuntime(
            typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.imports.wasm"),
            (x) =>
            {
                x.ImportMethod("times2", new Func<int, Task<int>>(x => Task.FromResult(x * 2)));
            });
        
        int l_Result = (int) await l_Module.DynamicCall("twiceplus5", 3);
        Assert.AreEqual(11, l_Result);
    }
    
    [TestMethod]
    public async Task ExecuteImportModuleLevelWasmSyncTest()
    {
        WebAssemblyModule l_Module = await WebAssemblyRuntimeBuilder.CreateSingleModuleRuntime(
            typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.imports.wasm"),
            (x) =>
            {
                x.ImportMethod("times2", new Func<int, int>(x => x * 2));
            });
        
        int l_Result = (int) await l_Module.DynamicCall("twiceplus5", 3);
        Assert.AreEqual(11, l_Result);
    }
}