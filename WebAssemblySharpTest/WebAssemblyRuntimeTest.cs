using System.Threading.Tasks;
using WebAssemblySharp.Runtime;

namespace WebAssemblySharpTest;

[TestClass]
public class WebAssemblyRuntimeTest
{

    [TestMethod]
    public async Task ExecuteAddWasmTest()
    {
        WebAssemblyRuntime l_Runtime = new WebAssemblyRuntime();
        WebAssemblyModule l_Module = await l_Runtime.LoadModule(typeof(WebAssemblyRuntimeTest).Assembly.GetManifestResourceStream("WebAssemblySharpTest.Data.Example.add.wasm"));

        int l_Result = (int)l_Module.Call("add", 1, 2);
        Assert.AreEqual(3, l_Result);

    }
    
    
    [TestMethod]
    public async Task ExecuteIsprimeWasmTest()
    {
        WebAssemblyRuntime l_Runtime = new WebAssemblyRuntime();
        WebAssemblyModule l_Module = await l_Runtime.LoadModule(typeof(WebAssemblyRuntimeTest).Assembly.GetManifestResourceStream("WebAssemblySharpTest.Data.Example.isprime.wasm"));

        int l_Result = (int)l_Module.Call("is_prime", 3);
        Assert.AreEqual(1, l_Result);

        l_Result = (int)l_Module.Call("is_prime", 4);
        Assert.AreEqual(0, l_Result);
        
    }
    
}