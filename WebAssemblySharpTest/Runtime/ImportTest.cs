using System;
using System.Threading.Tasks;
using WebAssemblySharp.Runtime;
using WebAssemblySharpExampleData;

namespace WebAssemblySharpTest.Runtime;

[TestClass]
public class ImportTest
{
    [TestMethod]
    public async Task ExecuteImportWasmAsyncTest()
    {
        WebAssemblyRuntime l_Runtime = new WebAssemblyRuntime();
        WebAssemblyModuleBuilder l_ModuleBuilder =
            await l_Runtime.LoadModule(
                typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.imports.wasm"));

        l_ModuleBuilder.DefineImport("times2", new Func<int, Task<int>>(x => Task.FromResult(x * 2)));
        WebAssemblyModule l_Module = await l_ModuleBuilder.Build();

        int l_Result = (int) await l_Module.Call("twiceplus5", 3);
        Assert.AreEqual(11, l_Result);
    }
    
    [TestMethod]
    public async Task ExecuteImportWasmSyncTest()
    {
        WebAssemblyRuntime l_Runtime = new WebAssemblyRuntime();
        WebAssemblyModuleBuilder l_ModuleBuilder =
            await l_Runtime.LoadModule(
                typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.imports.wasm"));

        l_ModuleBuilder.DefineImport("times2", new Func<int, int>(x => x * 2));
        WebAssemblyModule l_Module = await l_ModuleBuilder.Build();
        
        int l_Result = (int) await l_Module.Call("twiceplus5", 3);
        Assert.AreEqual(11, l_Result);
    }
}