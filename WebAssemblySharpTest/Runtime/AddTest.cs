using System.Threading.Tasks;
using WebAssemblySharp.Runtime;
using WebAssemblySharpExampleData;

namespace WebAssemblySharpTest.Runtime;

[TestClass]
public class AddTest
{
    [TestMethod]
    public async Task ExecuteAddWasmTest()
    {
        WebAssemblyModule l_Module = await WebAssemblyRuntimeBuilder.CreateSingleModuleRuntime(
            typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.add.wasm"));
        
        int l_Result = (int) await l_Module.Call("add", 1, 2);
        Assert.AreEqual(3, l_Result);
    }
}