using System.Threading.Tasks;
using WebAssemblySharp.Runtime;

namespace WebAssemblySharpTest;

[TestClass]
public class WebAssemblyRuntimeTest
{

    [DataTestMethod]
    [DataRow("WebAssemblySharpTest.Data.Example.add.wasm", "WebAssemblySharpTest.Data.Example.addReaderResult.txt")]
    //[DataRow("WebAssemblySharpTest.Data.Example.isprime.wasm", "WebAssemblySharpTest.Data.Example.addReaderResult.txt")]
    public async Task ExecuteTest(string p_SourcePath, string p_ExpectedResultPath)
    {
        WebAssemblyRuntime l_Runtime = new WebAssemblyRuntime();
        WebAssemblyModule l_Module = await l_Runtime.LoadModule(typeof(WebAssemblyRuntimeTest).Assembly.GetManifestResourceStream(p_SourcePath));

        int l_Result = (int)l_Module.Call("add", 1, 2);
        Assert.AreEqual(3, l_Result);

    }
    
}