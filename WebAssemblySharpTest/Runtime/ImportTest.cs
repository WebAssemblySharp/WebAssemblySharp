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
        WebAssemblyModule l_Module = await WebAssemblyRuntimeBuilder.CreateSingleModuleRuntime(
            typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.imports.wasm"),
            (x) =>
            {
                x.ImportMethod("times2", new Func<int, Task<int>>(x => Task.FromResult(x * 2)));
            });
        
        int l_Result = (int) await l_Module.Call("twiceplus5", 3);
        Assert.AreEqual(11, l_Result);
    }
    
    [TestMethod]
    public async Task ExecuteImportWasmSyncTest()
    {
        WebAssemblyModule l_Module = await WebAssemblyRuntimeBuilder.CreateSingleModuleRuntime(
            typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.imports.wasm"),
            (x) =>
            {
                x.ImportMethod("times2", new Func<int, int>(x => x * 2));
            });
        
        int l_Result = (int) await l_Module.Call("twiceplus5", 3);
        Assert.AreEqual(11, l_Result);
    }
}