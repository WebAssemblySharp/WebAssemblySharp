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
        WebAssemblyRuntime l_Runtime = new WebAssemblyRuntime();
        WebAssemblyModule l_Module =
            await (await l_Runtime.LoadModule(
                typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.add.wasm"))).Build();

        int l_Result = (int) await l_Module.Call("add", 1, 2);
        Assert.AreEqual(3, l_Result);
    }
}