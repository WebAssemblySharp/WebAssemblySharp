using System;
using System.Threading.Tasks;
using WebAssemblySharp.Runtime;
using WebAssemblySharpExampleData;

namespace WebAssemblySharpTest.Runtime;

[TestClass]
public class LoopsTest
{
    //[TestMethod]
    public async Task ExecuteAddAllTest()
    {
        WebAssemblyModule l_Module = await WebAssemblyRuntimeBuilder.CreateSingleModuleRuntime(
            typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.loops.wasm"),
            (x) => { x.ImportMethod("log_i32", new Action<int>((x) => { throw new Exception("log_i32 not implemented"); })); });


        int l_Sum = await l_Module.Call<int>("add_all", 4, 1);
        Assert.AreEqual(4, l_Sum);
    }
}