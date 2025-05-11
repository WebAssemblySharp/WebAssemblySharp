using System;
using System.Threading.Tasks;
using WebAssemblySharp.Runtime;
using WebAssemblySharpExampleData;

namespace WebAssemblySharpTest.Runtime;

[TestClass]
public class AddTest
{
    [TestMethod]
    [DynamicData(nameof(TestRuntimeProvider.RuntimeTypes), typeof(TestRuntimeProvider))]
    public async Task ExecuteAddWasmTest(Type p_RuntimeType)
    {
        WebAssemblyModule l_Module = await WebAssemblyRuntimeBuilder.CreateSingleModuleRuntime(p_RuntimeType,
            typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.add.wasm"));

        int l_Result = (int) await l_Module.Call("add", 1, 2);
        Assert.AreEqual(3, l_Result);   
    }
    
    
}